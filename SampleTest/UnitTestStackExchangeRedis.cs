using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;


namespace SampleTest
{
    [TestClass]
    public class UnitTestStackExchangeRedis {
        protected ConnectionMultiplexer redis;
        protected IDatabase db;

        [TestInitialize]
        public void SetUp() {
            redis = ConnectionMultiplexer.Connect("127.0.0.1");
            db = redis.GetDatabase();
        }

        [TestMethod]
        public void Test1AccessScalarValue() {
            var keyName = "hello";
            string stringValue = "world";

            db.StringSet(keyName, stringValue);
            string result = db.StringGet(stringValue);

            Assert.AreEqual(stringValue, result);
        }

        [TestMethod]
        public void Test2AccessHashSet() {
            var keyName = "Setting";
            var hashKeyLang = "Lang";
            var hashValueLang = "En";

            db.HashSet(keyName, hashKeyLang, hashValueLang);
            var resultLang = db.HashGet(keyName, hashKeyLang);

            Assert.AreEqual(resultLang.HasValue, true);
            Assert.AreEqual((string)resultLang, hashValueLang);
        }

        [TestMethod]
        public void Test3AccessSortedSet() {
            var keyName = "OnlineUsers";
            var userIds = GenerateRandomUserIds();

            foreach (var userId in userIds) db.SortedSetAdd(keyName, userId, userId);

            var result = db.SortedSetScan(keyName);
            var lhs = 0;

            foreach (var item in result) {
                Assert.AreEqual(lhs <= (int)item.Element, true);
                lhs = (int)item.Element;
            }
        }

        protected List<int> GenerateRandomUserIds() {
            var random = new Random();

            var userIds = new List<int>();

            for (int i = 0; i < 20; i++) userIds.Add(random.Next(100));

            return userIds;
        }

        [TestMethod]
        public void Test4AccessSet() {
            var keyName = "RecentUsers";
            var userIds = GenerateRandomUserIds();

            foreach (var userId in userIds) db.SetAdd(keyName, userId);

            foreach(var userId in userIds)
            {
                Assert.AreEqual(db.SetContains(keyName, userId), true);
            }
        }

        [TestMethod]
        public void Test5Transaction() {
            var keyName = "Setting";

            var transaction = db.CreateTransaction();
            db.HashSet(keyName, "Lang", "En");
            db.HashSet(keyName, "CharSet", "UTF8mb4");

            Assert.AreEqual(transaction.Execute(), true);
        }
    }
}
