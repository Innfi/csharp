using MySql.Data.MySqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SampleTest
{
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
    }
}
