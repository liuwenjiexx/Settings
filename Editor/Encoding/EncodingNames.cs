using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace Unity.Text.Editor
{
    /// <summary>
    /// 编码名称
    /// </summary>
    public class EncodingNames
    {

        public const string UTF8 = "utf8";
        public const string UTF8WithBOM = "utf8bom";
        public const string GB2312 = "gb2312";
        public const string UTF16LE = "utf8le";
        public const string UTF16BE = "utf16be";
        public const string UTF32BE = "utf32be";
         
        public const string ASCII = "ascii";
        //WesternEuropean
        public const string WindowsWestern = "windows1252";
        public const string IsoWestern = "iso28591";

        private static Dictionary<string, string> displayNames;
        private static Dictionary<string, Encoding> cachedEncodings;

        public static string[] allEncodingNames;

        public static string[] AllEncodingNames
        {
            get
            {
                if (allEncodingNames == null)
                {
                    allEncodingNames = new string[]
                    {
                        UTF8,
                        UTF8WithBOM,
                        UTF16LE,
                        UTF16BE,
                        UTF32BE,
                        GB2312,
                        ASCII,
                        WindowsWestern,
                        IsoWestern,
                    };
                    Array.Sort(allEncodingNames);
                }
                return allEncodingNames;
            }
        }

        public static string GetDisplayName(string encodingName)
        {
            if (displayNames == null)
            {
                displayNames = new();
                displayNames[UTF8] = "UTF-8";
                displayNames[UTF8WithBOM] = "UTF-8 With BOM";
                displayNames[UTF16LE] = "UTF-16 LE";
                displayNames[UTF16BE] = "UTF-16 BE";
                displayNames[UTF32BE] = "UTF-32 BE";
                displayNames[GB2312] = "Simplified Chinese (GB 2312)";
                displayNames[ASCII] = "ASCII";
                displayNames[WindowsWestern] = "Western (Windows 1252)";
                displayNames[IsoWestern] = "Western (ISO 28591)";
            }

            if (displayNames.TryGetValue(encodingName, out var name))
                return name;
            return null;
        }

        public static Encoding GetEncoding(string encodingName)
        {
            Encoding encoding;

            if (cachedEncodings == null)
            {
                cachedEncodings = new Dictionary<string, Encoding>();
            }
            if (cachedEncodings.TryGetValue(encodingName, out encoding))
                return encoding;
            switch (encodingName)
            {
                case UTF8:
                    encoding = new UTF8Encoding(false); break;
                case UTF8WithBOM:
                    encoding = Encoding.UTF8; break;
                case GB2312:
                    encoding = Encoding.GetEncoding(936); break;
                case UTF16LE:
                    encoding = new UnicodeEncoding(false, false); break;
                case UTF16BE:
                    encoding = new UnicodeEncoding(true, false); break;
                case UTF32BE:
                    encoding = new UTF32Encoding(false, false); break;
                case ASCII:
                    encoding = Encoding.ASCII; break;
                case WindowsWestern:
                    encoding = Encoding.GetEncoding(1252); break;
                case IsoWestern:
                    encoding = Encoding.GetEncoding(28591); break;
            }
            
            if (encoding != null)
            {
                cachedEncodings[encodingName] = encoding;
            }
            return encoding;
        }
    }
}