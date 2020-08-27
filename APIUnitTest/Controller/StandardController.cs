
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CLBS.Controllers;
using CLBS.Models;
using CLBS;

namespace APIUnitTest.Controller
{
    /// <summary>
    /// Summary description for StandardController
    /// </summary>
    /// 
    [TestClass]
    public class StandardController
    {
        //private STANDARDData Standard()
        //{
        //    var testdata = new STANDARDData()
        //    {
        //        SurfaceFile = "def",
        //        SlopeX = 0.5,
        //        SlopeY = 0.6,
        //        constantX = 0.0,
        //        constantY = 0.1,
        //        Base = 1.0,
        //        Addition = 2.0,
        //        Material = "oma",
        //        Design = "-",
        //        ID = "00001"
        //    };
        //    return testdata;
        //}
        //[TestMethod]
        //public async Task getAllSTDdata()
        //{
        //    var addData = Standard();
        //    var controller = new STANDARDDatasController();
        //    var result = await controller.GetSTANDARDData("abc") as OkNegotiatedContentResult<STANDARDData>; ;
        //    try
        //    {
        //        Assert.IsNotNull(result);
        //        Assert.AreEqual("abc", result.Content.SurfaceFile);
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
            

        //}

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            //
            // TODO: Add test logic here
            //
        }
    }
}
