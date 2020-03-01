using System.Collections.Generic;
using System.Linq;
using System.Net;
using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.Sqlite;


namespace SampleTest
{
    public class EntityUserContext : DbContext {
        public EntityUserContext() : base("TestDBContext") { }

        public DbSet<EntityUser> EntityUsers { get; set; }
    }

    [Table ("EntityUser")]
    public class EntityUser {
        public int UserId { get; set; }
        public int Rank { get; set; }
        public string Nickname { get; set; }
    }

    [TestClass]
    public class UnitTestEntityFramework {
        [TestMethod]
        public void Test0InitEntityModel() {
            var entityContext = new EntityUserContext();
        }

        [TestMethod]
        public void Test1AccessDatabaseWithModel() {
            var entityContext = new EntityUserContext();
            entityContext.Database.Create();

            var db = entityContext.Database;
            var dbExists = db.Exists();

            entityContext.EntityUsers.Add(new EntityUser {
                UserId = 1234,
                Nickname = "innfi",
                Rank = 5,
            });

            var result = entityContext.SaveChanges();

            Assert.AreEqual(result, 0);
        }
    }
}
