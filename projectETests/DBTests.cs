using Microsoft.VisualStudio.TestTools.UnitTesting;
using projectE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projectE.Tests
{
    [TestClass()]
    public class DBTests
    {
        DB db = new DB();
        Parser parser = new Parser();
        bool Bool = false;

        [TestMethod()]
        public void AddMovieTest()
        {
            
            try
            {
                db.AddMovie("a", 2019, "a", "a", "a", "a", "a", "a", "a", "a", "a", Convert.ToBoolean(Bool), Convert.ToBoolean(Bool), "a");
            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
        }
    }
}