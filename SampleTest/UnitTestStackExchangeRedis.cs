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
        [TestMethod]
        public void Test0InitRedisConnector() {
            var redis = ConnectionMultiplexer.Connect("127.0.0.1");

            Assert.AreEqual(redis.IsConnected, true);
        }

        [TestMethod]
        public void Test1AccessScalarValue() {
            var redis = ConnectionMultiplexer.Connect("127.0.0.1");
            var db = redis.GetDatabase();

            var keyName = "hello";
            string stringValue = "world";

            db.StringSet(keyName, stringValue);
            string result = db.StringGet(stringValue);

            Assert.AreEqual(stringValue, result);
        }

        [TestMethod]
        public void Test2AccessHashSet() {

        }
    }
}
