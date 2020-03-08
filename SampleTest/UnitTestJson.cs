using System;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SampleTest
{
    public class DummyUserData {
        public string UserName;
        public int UserId;
        public string Region;
    }

    [TestClass]
    public class UnitTestJson {
        [TestMethod]
        public void Test0CallJson() {
            var userData = new DummyUserData {
                UserName = "innfi",
                UserId = 1,
                Region = "ap-northeast-2"
            };

            var serialized = JsonConvert.SerializeObject(userData, Formatting.Indented);

            var jsonText = ToDummyJsonText();

            Assert.AreEqual(serialized, jsonText);
        }

        protected string ToDummyJsonText() {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter();

            using (var writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartObject();

                writer.WritePropertyName("UserName");
                writer.WriteValue("Innfi");
                writer.WritePropertyName("UserId");
                writer.WriteValue(1);
                writer.WritePropertyName("Region");
                writer.WriteValue("ap-northeast-2");

                writer.WriteEnd();
                writer.WriteEndObject();
            }

            return sb.ToString();
        }
    }
}
