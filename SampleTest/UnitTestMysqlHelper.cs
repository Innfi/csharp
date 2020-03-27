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
        [TestMethod]
        public void Test0CallMysqlHelper() {

            var mysqlAddr = "127.0.0.1";
            var uid = "innfi";
            var password = "test_password";
            var database = "test_db";
            var connectionData = new MySqlConnection {
                ConnectionString = $"server={mysqlAddr}; uid={uid}; pwd={password}; " +
                $"database={database}; Allow User Variables=True"
            };

            var result = MySqlHelper.ExecuteReader(connectionData, "select count(*) from players; ");

            Assert.AreEqual(result.HasRows, true);
        }

        [TestMethod]
        public void Test1Select() {
            var mysqlAddr = "127.0.0.1";
            var uid = "innfi";
            var password = "test_password";
            var database = "test_db";
            var connectionData = new MySqlConnection {
                ConnectionString = $"server={mysqlAddr}; uid={uid}; pwd={password}; " +
                $"database={database}; Allow User Variables=True"
            };
            var query = "select * from players where UserId=1;";
            var obj = new TestClass();

            using (var reader = MySqlHelper.ExecuteReader(connectionData, query)) {
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
    }
}
