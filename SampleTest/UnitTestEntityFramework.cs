using System.Collections.Generic;
using System.Linq;
using System.Net;
using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SampleTest
{
    public class EntityUserContext : DbContext {
        public EntityUserContext() : base() { }

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
        public void Test1AccessDatabaseWithModel() { }
    }
}
