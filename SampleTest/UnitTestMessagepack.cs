using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MessagePack;


namespace SampleTest
{
    [MessagePackObject]
    public class DummyData {
        [Key(0)] public int UserId;
        [Key(1)] public string Nickname;
        [Key(2)] public string Region;
    }

    [TestClass]
    public class UnitTestMessagepack {
        [TestMethod]
        public void Test0MessagePackSerialize() {
            var data = new DummyData {
                UserId = 1,
                Nickname = "innfi",
                Region = "ap-northeast-2a"
            };

            var bytes = MessagePackSerializer.Serialize(data);

            var resultData = MessagePackSerializer.Deserialize<DummyData>(bytes);

            Assert.AreEqual(data.UserId, resultData.UserId);
            Assert.AreEqual(data.Nickname, resultData.Nickname);
            Assert.AreEqual(data.Region, resultData.Region);
        }
    }
}
