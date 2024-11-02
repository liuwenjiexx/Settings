using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SettingsManagement.Tests
{
    public class PlatformSettingsTests : SettingsTestsBase
    {

        static Setting<string> platformString = new(Settings, "platform.string", "abc123", SettingsScope.EditorUser);


        [Test]
        public void String()
        {
            Assert.AreEqual("abc123", platformString.GetValue(PlatformNames.Default, null));
            Assert.AreEqual("abc123", platformString.GetValue(PlatformNames.Standalone, null));

            platformString.SetValue(PlatformNames.Standalone, null, "abc", true);
            Assert.AreEqual("abc", platformString.GetValue(PlatformNames.Standalone, null));

            Assert.AreEqual("abc123", platformString.GetValue(PlatformNames.Default, null));

        }



    }
}