using System;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SampleTest
{
    public class UserData {
        public string UserName;
        public int UserId;
        public string Region;
    }

    [TestClass]
    public class UnitTestJson {
        [TestMethod]
        public void Test1JsonSerialize() {
            var userData = new UserData
            {
                UserName = "Innfi",
                UserId = 1,
                Region = "ap-northeast-2"
            };

            var serialized = JsonConvert.SerializeObject(userData, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<UserData>(serialized);

            Assert.AreEqual(userData.UserName, deserialized.UserName);
            Assert.AreEqual(userData.UserId, deserialized.UserId);
            Assert.AreEqual(userData.Region, deserialized.Region);
        }


        [TestMethod]
        public void Test2JsonStringWriter() {
            var userData = new UserData {
                UserName = "Innfi",
                UserId = 1,
                Region = "ap-northeast-2"
            };

            var serialized = JsonConvert.SerializeObject(userData, Formatting.Indented);
            var jsonText = ToDummyJsonText();

            Assert.AreEqual(serialized, jsonText);
        }

        protected string ToDummyJsonText() {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

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
                
                writer.WriteEndObject();
            }

            return sb.ToString();
        }
    }
}
