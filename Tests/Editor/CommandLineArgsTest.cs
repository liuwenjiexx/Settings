using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.SettingsManagement.Tests
{
    public class CommandLineArgsTest
    {
        [TearDown]
        public void TearDown()
        {
            TestData.staticStringValue = null;
            TestData.staticIntValue = 0;
        }

        [Test]
        public void NullableType()
        {
            int? n = 1;
            Assert.IsTrue(n is int?);
            string? ss = "AA";

            SortedDictionary<int, int> dic1 = new SortedDictionary<int, int>();
            dic1.Add(1, 1);
            dic1.Add(3, 3);
            dic1.Add(2, 2);

            foreach (var it in dic1)
            {
                Debug.Log(it.Key + ", " + it.Value);
            }


            Dictionary<int, int> dic2 = new Dictionary<int, int>();
            dic2.Add(1, 1);
            dic2.Add(3, 3);
            dic2.Add(2, 2);

            foreach (var it in dic2)
            {
                Debug.Log(it.Key + ", " + it.Value);
            }
        }





        [Test]
        public void Parse()
        {
            TestParse("abc 123", new string[] { "abc", "123" });
            TestParse("abc\"def\"", new string[] { "abcdef" });
            TestParse("abc'def'", new string[] { "abcdef" });
            TestParse("abc \"123\"", new string[] { "abc", "123" });
            TestParse("abc '123'", new string[] { "abc", "123" });
            TestParse("abc \"123", new string[] { "abc", "\"123" });
            TestParse("sftp -c 'mkdir dir,cd dir;'", new string[] { "sftp", "-c", "mkdir dir,cd dir;" });
        }


        static void TestParse(string commandLine, string[] assert)
        {
            var args = CommandArguments.Parse(commandLine);
            if (args.Count != assert.Length)
                throw new Exception($"Parse '{commandLine}': {string.Join(",", args.List)}");
            for (int i = 0; i < args.Count; i++)
            {
                if (assert[i] != args[i])
                    throw new Exception($"Parse '{commandLine}': {string.Join(",", args.List)}");
            }
        }


        [Test]
        public void Fill_Instance()
        {
            CommandArguments args = CommandArguments.Parse("-string abc -int 123 -float 1.23 -bool true -toggle -enum string -nested-string NestedAbc");

            Assert.IsNull(TestData.staticStringValue);

            TestData data = new TestData();

            args.Fill(data);

            Assert.AreEqual("abc", data.stringValue);
            Assert.AreEqual(123, data.intValue);
            Assert.IsTrue(data.boolValue);
            Assert.AreEqual(1.23f, data.floatValue);
            Assert.AreEqual(TypeCode.String, data.enumValue);

            Assert.AreEqual("NestedAbc", data.nestedData.nestedString);

            Assert.IsNull(TestData.staticStringValue);
        }

        [Test]
        public void Fill_Static()
        {
            CommandArguments args = CommandArguments.Parse("-string abc -int 123");

            args.Fill(typeof(TestData));

            Assert.AreEqual("abc", TestData.staticStringValue);
            Assert.AreEqual(123, TestData.staticIntValue);

        }

        [Test]
        public void Value_Bool()
        {
            CommandArguments args = CommandArguments.Parse("-bool true");

            TestData data = new TestData();
            args.Fill(data);
            Assert.IsTrue(data.boolValue);

            args = CommandArguments.Parse("-bool false");
            data = new TestData();
            args.Fill(data);
            Assert.IsFalse(data.boolValue);

            args = CommandArguments.Parse("-bool 0");
            data = new TestData();
            args.Fill(data);
            Assert.IsFalse(data.boolValue);

            args = CommandArguments.Parse("-bool 1");
            data = new TestData();
            args.Fill(data);
            Assert.IsTrue(data.boolValue);


            args = CommandArguments.Parse("-bool '1'");
            data = new TestData();
            args.Fill(data);
            Assert.IsTrue(data.boolValue);

            args = CommandArguments.Parse("-bool ' 1 '");
            data = new TestData();
            args.Fill(data);
            Assert.IsTrue(data.boolValue);

            Assert.Throws<Exception>(() =>
            {
                CommandArguments args = CommandArguments.Parse("-bool a");

                TestData data = new TestData();
                args.Fill(data);
            });
        }

        [Test]
        public void Nullable_Bool()
        {
            CommandArguments args = CommandArguments.Parse("-bool 1");

            TestData data = new TestData();
            args.Fill(data);

            Assert.IsTrue(data.nullableBool);
        }


        [Test]
        public void Convert_Error()
        {
            Assert.Throws<Exception>(() =>
            {
                CommandArguments args = CommandArguments.Parse("-int a");

                TestData data = new TestData();
                args.Fill(data);
            });
            Assert.Throws<Exception>(() =>
            {
                CommandArguments args = CommandArguments.Parse("-float a");

                TestData data = new TestData();
                args.Fill(data);
            });

        }


        [Test]
        public void MultiArgName()
        {
            CommandArguments args = CommandArguments.Parse("-multi-arg-name 1");

            TestData data = new TestData();
            args.Fill(data);
            Assert.AreEqual(1, data.multiArgName);

            args = CommandArguments.Parse("-multi-arg-name2 2");
            data = new TestData();
            args.Fill(data);
            Assert.AreEqual(2, data.multiArgName);
        }

        private class TestData
        {
            [CommandArgument("-string")]
            public string stringValue;

            [CommandArgument("-string")]
            public static string staticStringValue;


            [CommandArgument("-string")]
            public static readonly string readonlyStringValue;

            [CommandArgument("-int")]
            public int intValue;
            [CommandArgument("-int")]
            public static int staticIntValue;

            [CommandArgument("-bool")]
            public bool boolValue;

            [CommandArgument("-float")]
            public float floatValue;


            [CommandArgument("-multi-arg-name")]
            [CommandArgument("-multi-arg-name2")]
            public int multiArgName;

            [CommandArgument("-toggle", Toggle = true)]
            public bool toggle;

            [CommandArgument("-enum")]
            public TypeCode enumValue;

            [CommandArgument("-bool")]
            public NullableValue<bool> nullableBool;

            public NestedTestData nestedData = new NestedTestData();

        }

        [CommandArgument]
        class NestedTestData
        {
            [CommandArgument("-nested-string")]
            public string nestedString;

        }

        private class TestEnvVariable
        {
            [EnvironmentVariable("TEST_STRING")]
            public string stringValue;

            [EnvironmentVariable("TEST_BOOL")]
            public bool boolValue;

            [EnvironmentVariable("TEST_INT")]
            public int intValue;
        }

        [Test]
        public void FillEnv()
        {
            // SettingsUtility.AddEnvironmentVariable("TEST_STRING","abc");
            //SettingsUtility.AddEnvironmentVariable("TEST_BOOL", "1");
            //SettingsUtility.AddEnvironmentVariable("TEST_INT", "123");
            Environment.SetEnvironmentVariable("TEST_STRING", "abc");
            Environment.SetEnvironmentVariable("TEST_BOOL", "1");
            Environment.SetEnvironmentVariable("TEST_INT", "123");

            TestEnvVariable data = new TestEnvVariable();

            SettingsUtility.FillEnvironmentVariables(data);

            Assert.AreEqual("abc", data.stringValue);
            Assert.AreEqual(123, data.intValue);
            Assert.IsTrue(data.boolValue);

        }



    }
}