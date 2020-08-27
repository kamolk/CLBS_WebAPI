
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

  
    [TestClass]
    public class CLBSControllerTest
    {
        private CLBSData AddCBLSdata()
        {
            var testdata = new CLBSData()
            {
                OPC = "000235",
                SurfaceID = "abc",
                CreateBy = "00002",
                CreateDate = DateTime.Today
            };
            return testdata;
        }


        [TestMethod]
        public async void addAllCLBS()
        {
            var addData = AddCBLSdata();
            var controller = new CLBSDatasController();
            var result = await controller.PutCLBSData("000235",addData);

            Assert.IsNotNull(result);
            Assert.AreEqual(123, result);

        }

        

    }
    
}
