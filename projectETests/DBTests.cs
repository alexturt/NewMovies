using Microsoft.VisualStudio.TestTools.UnitTesting;
using projectE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projectE.Tests
{
    [TestClass()]
    public class DBTests
    {
        [TestMethod()]
        public void GetRecommendsTest()
        {
            DB db = new DB();
            var v = db.GetRecommends();
            Assert.IsInstanceOfType(v,typeof(DataTable));
        }
        [TestMethod()]
        public void GetRecommendsTest2()
        {
            DB db = new DB();
            var v = db.GetRecommends();
            Assert.IsInstanceOfType(v, typeof(int));
        }
    }
}