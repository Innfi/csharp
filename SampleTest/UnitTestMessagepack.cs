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

    [MessagePackObject]
    public class UserLoginhistory {
        [Key(0)] public int HistoryId;
        [Key(1)] public List<ElementClass> UserDatas;
        [Key(2)] public DateTime Created;
    }

    [MessagePackObject]
    public class ElementClass
    {
        [Key(0)] public int UserId;
        [Key(1)] public string GreetingsMessage;
        [Key(2)] public DateTime LastLoggedIn;
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
            var history = new UserLoginhistory {
                HistoryId = 1324,
                UserDatas = GenerateDummyElements(),
                Created = DateTime.UtcNow
            };

            var bytes = MessagePackSerializer.Serialize(history);

            var result = MessagePackSerializer.Deserialize<UserLoginhistory>(bytes);

            Assert.AreEqual(SameObject(history, result), true);
        }

        protected List<ElementClass> GenerateDummyElements() {
            var elements = new List<ElementClass>();
            var random = new Random();
            for (int i = 0; i < 10; i++) {
                var hour = random.Next(12);
                var second = random.Next(60);

                elements.Add(new ElementClass {
                    UserId = i + 1,
                    GreetingsMessage = $"test{i}",
                    LastLoggedIn = DateTime.UtcNow.AddDays(-7).AddHours(hour).AddSeconds(second)
                });
            }

            return elements;
        }

        protected bool SameObject(UserLoginhistory lhs, UserLoginhistory rhs) {
            if (lhs.HistoryId != rhs.HistoryId) return false;
            if (lhs.UserDatas.Count != rhs.UserDatas.Count) return false;
            for (int i = 0; i < lhs.UserDatas.Count; i++) {
                if (lhs.UserDatas[i].UserId != rhs.UserDatas[i].UserId) return false;
                if (lhs.UserDatas[i].GreetingsMessage != rhs.UserDatas[i].GreetingsMessage) return false;
                if (lhs.UserDatas[i].LastLoggedIn != rhs.UserDatas[i].LastLoggedIn) return false;
            }

            if (lhs.Created != rhs.Created) return false;

            return true;
        }
    }
}
