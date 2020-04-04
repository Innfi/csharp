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
    }
}
