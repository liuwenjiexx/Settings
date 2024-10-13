using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Unity.Text.Editor
{
    class FileEncodingPostprocessor : AssetPostprocessor
    {
        static Regex packageCacheRegex;

        void OnPreprocessAsset()
        {
            if (!(FileEncodingUserSettings.GlobalEnabled && FileEncodingUserSettings.Enabled))
                return;

            bool isText = false;

            switch (assetImporter.GetType().Name)
            {
                case "TextScriptImporter":
                case nameof(MonoImporter):
                    isText = true;
                    break;
            }

            if (!isText)
            {
                return;
            }

            string filePath = assetImporter.assetPath;
            if (string.IsNullOrEmpty(filePath))
                return;

            if (!File.Exists(filePath))
                return;

            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists) return;

            //排除 Library/PackageCache 文件
            if (packageCacheRegex == null)
            {
                packageCacheRegex = new Regex("Packages/(?<package>[^/]+)(?<path>.*)");
            }
            var m = packageCacheRegex.Match(filePath);
            if (m.Success)
            {
                string pattern = $"{m.Groups["package"].Value}@*";
                if (Directory.GetDirectories(Path.GetFullPath("Library/PackageCache"), pattern).Any())
                    return;
            }

            foreach (var rule in FileEncodingUserSettings.GlobalRules)
            {
                if (string.IsNullOrEmpty(rule.EncodingName))
                    continue;

                if (!rule.IsMatchPath(filePath))
                    continue;
                var encoding = rule.Encoding;
                if (encoding == null)
                    continue;

                if (ConvEncoding(filePath, encoding, rule.TestEncoding, out var srcEncoding))
                {
                    Debug.LogWarning($"File Encoding Changed: [{GetEncodingString(srcEncoding)}] => [{GetEncodingString(encoding)}]\n" +
                        $"{filePath}");
                }
                break;
            }

            //var encoding = GetEncoding(filePath);

            //if (encoding != Encoding.UTF8)
            //{
            //    string cotnent = File.ReadAllText(filePath, encoding);
            //    File.WriteAllText(filePath, cotnent, new UTF8Encoding(false));
            //}


        }

        string GetEncodingString(Encoding encoding)
        {
            string name = $"{encoding.EncodingName}, CodePaage={encoding.CodePage}";
            if (encoding == EncodingNames.GetEncoding(EncodingNames.UTF8WithBOM))
            {
                name += ", BOM";
            }
            return name;
        }

        static byte[] ReadAllBytes(FileStream fs)
        {
            byte[] bytes;
            int total = (int)(fs.Length - fs.Position);
            bytes = new byte[total];
            for (int i = 0; i < total;)
            {
                int readCount = fs.Read(bytes, i, total - i);
                if (readCount > 0)
                {
                    i += readCount;
                }
            }
            return bytes;
        }

        public static bool ConvEncoding(string filePath, Encoding encoding, Encoding testEncoding, out Encoding srcEncoding)
        {
            srcEncoding = null;
            byte[] bytes = null;


            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                if (fs.Length == 0)
                    return false;

                srcEncoding = TryGetEncoding(fs);
                if (srcEncoding == encoding)
                    return false;

                if (srcEncoding == null)
                {
                    fs.Position = 0;
                    bytes = ReadAllBytes(fs);
                    srcEncoding = DetectCodePage(bytes);
                    if (srcEncoding == encoding)
                        return false;
                }

                if (srcEncoding == null)
                    return false;

                if (bytes == null)
                {
                    fs.Position = 0;
                    bytes = ReadAllBytes(fs);
                }

                //try
                //{
                //    var a = testEncoding.GetCharCount(bytes);
                //    return false;
                //}
                //catch (Exception)
                //{
                //}


            }

            string text = srcEncoding.GetString(bytes);
            text = File.ReadAllText(filePath, srcEncoding);
            File.WriteAllText(filePath, text, encoding);
            return true;
        }

        public static Encoding TryGetEncoding(FileStream fs)
        {
            // Read the BOM
            var bom = new byte[4];

            fs.Read(bom, 0, 4);
            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0) return Encoding.UTF32; //UTF-32LE
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return EncodingNames.GetEncoding(EncodingNames.UTF32BE);  //UTF-32BE
            return null;
        }

        public static Encoding GetEncoding(string filename)
        {
            Encoding encoding;
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                encoding = TryGetEncoding(fs);
                if (encoding != null) return encoding;
            }
            var bytes = File.ReadAllBytes(filename);
            return DetectCodePage(bytes);
        }


        private static Encoding TestCodePage(Encoding testCode, byte[] byteArray)
        {
            try
            {
                var encoding = Encoding.GetEncoding(testCode.CodePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
                var a = encoding.GetCharCount(byteArray);
                return testCode;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static Encoding DetectCodePage(byte[] contents)
        {
            if (contents == null || contents.Length == 0)
            {
                return null;
            }

            //foreach (var encodingName in EncodingNames.AllEncodingNames)
            //{
            //    var encoding = EncodingNames.GetEncoding(encodingName);
            //    encoding = TestCodePage(encoding, contents);
            //    if (encoding != null)
            //        return encoding;
            //}
            //return null;
            return TestCodePage(EncodingNames.GetEncoding(EncodingNames.UTF8), contents) ??
                TestCodePage(Encoding.UTF8, contents)
                   ?? TestCodePage(EncodingNames.GetEncoding(EncodingNames.GB2312), contents) // GB2312
                   ?? TestCodePage(EncodingNames.GetEncoding(EncodingNames.WindowsWestern), contents) // Western European
                   ?? TestCodePage(EncodingNames.GetEncoding(EncodingNames.IsoWestern), contents) // ISO Western European
                   ?? TestCodePage(Encoding.Unicode, contents)
                   ?? TestCodePage(Encoding.BigEndianUnicode, contents)
                   ?? TestCodePage(Encoding.ASCII, contents)
                   ?? TestCodePage(Encoding.Default, contents); // Default
        }
    }


}