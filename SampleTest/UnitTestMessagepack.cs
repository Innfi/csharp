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

    [MessagePackObject]
    public class CompoundObject {
        [Key(0)] public int UserId;
        [Key(1)] public List<int> Friends;
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

        [TestMethod]
        public void Test1SerializeContainers() {
            var testInstance = new CompoundObject {
                UserId = 2,
                Friends = new List<int>() { 10, 3, 4, 5, 1 }
            };

            var bytes = MessagePackSerializer.Serialize(testInstance);
            var result = MessagePackSerializer.Deserialize<CompoundObject>(bytes);

            Assert.AreEqual(testInstance.UserId, result.UserId);
            for (int i = 0; i < testInstance.Friends.Count; i++) {
                Assert.AreEqual(testInstance.Friends[i], result.Friends[i]);
            }
        }

        [TestMethod]
        public void TestSerializeNestedClassInstance() {

        }
    }
}
