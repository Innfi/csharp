using System;
using MySql.Data.MySqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SampleTest
{
    public class TestClass {
        public int UserId;
        public string Nickname;
        public DateTime Created;
    }

    [TestClass]
    public class UnitTestMysqlHelper {
        public MySqlConnection ConnectionData;

        [TestInitialize]
        public void SetUp() {
            var mysqlAddr = "127.0.0.1";
            var uid = "innfi";
            var password = "test_password";
            var database = "test_db";
            ConnectionData = new MySqlConnection
            {
                ConnectionString = $"server={mysqlAddr}; uid={uid}; pwd={password}; " +
                $"database={database}; Allow User Variables=True"
            };
        }

        [TestMethod]
        public void Test0CallMysqlHelper() {

            var result = MySqlHelper.ExecuteReader(ConnectionData, "select count(*) from players; ");

            Assert.AreEqual(result.HasRows, true);
        }

        [TestMethod]
        public void Test1Select() {
            var query = "select * from players where UserId=1;";
            var obj = new TestClass();

            using (var reader = MySqlHelper.ExecuteReader(ConnectionData, query)) {
                while (reader.Read()) {
                    if (!reader.HasRows) continue;

                    obj.UserId = reader.GetInt32("UserId");
                    obj.Nickname = reader.GetString("Nickname");
                    obj.Created = reader.GetDateTime("Created");
                }
            }

            Assert.AreEqual(obj.UserId, 1);
            Assert.AreEqual(obj.Nickname, "innfi");
        }

        [TestMethod]
        public void Test2Insert() {
            var testData = DummyTestClass();

            var query = "insert into players (UserId, Nickname, Created) " +
                "values(@uid, @nickname, @created); ";
            var mysqlParams = new MySqlParameter[] {
                new MySqlParameter("uid", testData.UserId),
                new MySqlParameter("nickname", testData.Nickname),
                new MySqlParameter("created", testData.Created)
            };

            var result = MySqlHelper.ExecuteNonQuery(ConnectionData, query, mysqlParams);

            Assert.AreEqual(result > 0, true);
        }

        protected TestClass DummyTestClass() {
            var random = new Random();

            return new TestClass {
                UserId = random.Next(),
                Nickname = $"name{random.Next()}",
                Created = DateTime.UtcNow
            };
        }
    }
}
