using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using Microsoft.AspNet.Identity;
using System.Xml;
using CLBS.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using log4net;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using HttpPutAttribute = System.Web.Http.HttpPutAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using System.Reflection;
using System.Web.Http.Cors;
using System.Web.UI.WebControls;
//using FileContentResult = System.Web.Mvc.FileContentResult;
//using File = System.Web.Mvc.FileContentResult;
using CLBS.Domain;
using System.Net.Http.Headers;
using EntityState = System.Data.Entity.EntityState;
using CLBS.Infrastructure;
using Ionic.Zip;
using System.IO.Compression;
using ZipFile = Ionic.Zip.ZipFile;
using ActionResult = System.Web.Mvc.ActionResult;
using HttpDeleteAttribute = System.Web.Mvc.HttpDeleteAttribute;
using AuthorizeAttribute = System.Web.Mvc.AuthorizeAttribute;
using System.Web.Mvc;
using AllowAnonymousAttribute = System.Web.Mvc.AllowAnonymousAttribute;
using System.Security.Claims;
using System.Threading;
using System.Security.Principal;
using System.Globalization;

//using Microsoft.AspNetCore.Mvc.JsonOptions.SerializerOptions;

namespace CLBS.Controllers
{
    //  [EnableCors(origins: "http://localhost:5000", headers: "*", methods: "*")]
    public class CLBSDatasController : ApiController
    {

           private PRISMCOLLECTION db = new PRISMCOLLECTION();
     


        private static readonly ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        // protected internal virtual FilePathResult File((byte[] fileContents, string contentType, string fileDownloadName);
        private PrismColsController Prism = new PrismColsController();
        private STANDARDDatasController sTD = new STANDARDDatasController();
        

        [HttpGet]
        [Authorize]
        [Route("api/AuthenticateUser")]
        public HttpResponseMessage AuthenticateUser()
        {
            Thread.CurrentPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            ClaimsPrincipal principal = new ClaimsPrincipal(Thread.CurrentPrincipal);
            var name = Thread.CurrentPrincipal.Identity.Name;
            // Save Total Payment Stream Payments (Async)
            //Task.Run(() =>
            //{
            //    HttpContext.Current.User = new ClaimsPrincipal(
            //        new ClaimsPrincipal(
            //            new ClaimsIdentity(
            //                new \[\] { new Claim("[http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name](http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name)", [principal.Identity.Name](https://principal.Identity.Name)) }, "auth")));

            //Thread.CurrentPrincipal = principal;

            //  SaveTotalPaymentStream(opportunityTermId);
            //}
            //);
            if (User != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    status = (int)HttpStatusCode.OK,
                    isAuthenticated = true,
                    isLibraryAdmin = User.IsInRole(@"domain\AdminGroup"),
                    username = User.Identity.Name.Substring(User.Identity.Name.LastIndexOf(@"\") + 1)
                });
            }
            else
            {
                //This code never execute as we have used Authorize attribute on action method  
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    status = (int)HttpStatusCode.BadRequest,
                    isAuthenticated = false,
                    isLibraryAdmin = false,
                    username = ""
                });

            }
        }


        // GET: api/CLBSDatas
        [HttpGet]
        [Produces("application/json")]
        public CLBSList_Result GetCLBSDatas()
        {
            CLBSList_Result dataResult = new CLBSList_Result();
            try
            {
                var clbs = from b in db.CLBSDatas
                           join c in db.STANDARDDatas on b.SurfaceID equals c.SurfaceID
                           //where b.SurfaceID = b.SurfaceID.StartsWith("_old");
                           select new vCLBSData
                           {
                               OPC = b.OPC,
                               SurfaceID = c.SurfaceID,
                               Eye = b.Eye,
                               SlopeX = c.SlopeX,
                               SlopeY = c.SlopeY,
                               Base = c.Base,
                               Addition = c.Addition,
                               Material = c.Material,
                               Design = c.Design,
                               CreateBy = b.CreateBy,
                               CreateDate = b.CreateDate
                           };

                //var s = data.Where(x => x.SurfaceID.StartsWith("_")).toList();
                if (clbs != null)
                {
                    //var data = clbs.Where(x => !x.SurfaceID.StartsWith("_old"));
                    //var newdata = clbs.All(x => !x.SurfaceID.StartsWith("_old"));
                    //foreach(var i in newdata)
                    //{

                    //}
                    dataResult.DataResult.AddRange(clbs);
                    dataResult.StatusCode = "200";
                }
                else
                {
                    dataResult.StatusCode = "404";
                    dataResult.StatusDetails = "Not found CLBS data in system";
                }
                return dataResult;
            }
            catch (Exception ex)
            {
                Log.Info(ex);
                dataResult.StatusCode = "409";
                dataResult.StatusDetails = "We found the problem in 'GetCLBSDatas process' (DateTime: " + DateTime.Now + " ). Please contact admin.";
                return dataResult;
            }
        }

        [HttpGet]
        [Produces("application/json")]
        [ResponseType(typeof(CLBS_Result))]
        [Route("api/GetCLBSDataBySurfacing")]
        public async Task<IHttpActionResult> GetCLBSDataBySurfacing(string surfacingId)
        {
            CLBSDatalist_Result dataResult = new CLBSDatalist_Result();
            try
            {
                var cLBSData = await Task.Run(() => db.CLBSDatas.Where(x => x.SurfaceID == surfacingId));
                //var cLBSData = await db.CLBSDatas.Where(x => x.SurfaceID == surfacingId).ToList();
                //  List<vCLBSData> vCLBsData = new List<vCLBSData>();
                if (cLBSData == null)
                {
                    dataResult.StatusCode = "404";
                    dataResult.StatusDetails = "Not found " + surfacingId + " in System";
                    return Ok(dataResult);
                }
                foreach (var i in cLBSData)
                {
                    var data = new CRUD_CLBSData()
                    {
                        OPC = i.OPC,
                        SurfaceID = i.SurfaceID,
                        Eye = i.Eye,
                        CreateBy = i.CreateBy,
                        CreateDate = i.CreateDate
                    };
                    dataResult.DataResult.Add(data);
                }
                dataResult.StatusCode = "200";

                return Ok(dataResult);
            }
            catch (Exception ex)
            {
                Log.Info(ex);
                dataResult.StatusCode = "409";
                dataResult.StatusDetails = "We found the problem in 'GetCLBSDatas process' (DateTime: " + DateTime.Now + " ). Please contact admin.";
                return BadRequest(dataResult.StatusDetails);
            }

        }

        // GET: api/CLBSDatas
        [HttpGet]
        [Produces("application/json")]
        [Route("api/DownloadAllCLBS")]
        public CLBSList_Result DownloadAllCLBS()
        {
            CLBSList_Result dataResult = new CLBSList_Result();
            List<SemiFinishes> XMLList = new List<SemiFinishes>();
            try
            {
                var alldata = db.CLBSDatas.OrderBy(x => x.CreateDate);
                if (alldata != null)
                {
                    foreach (var data in alldata)
                    {
                        var xmldata = getSlopXSlopeY(data);
                        XMLList.Add(xmldata);

                    }
                    // dataResult.DataResult.AddRange(alldata);
                    // GenerateXML(XMLList);
                    dataResult.StatusCode = "200";

                }
                else
                {
                    dataResult.StatusCode = "404";
                    dataResult.StatusDetails = "Not found CLBS data in system";
                }


                return dataResult;
            }
            catch (Exception ex)
            {
                Log.Info(ex);
                dataResult.StatusCode = "409";
                dataResult.StatusDetails = "We found the problem in ' Download all CLBSDatas process' (DateTime: " + DateTime.Now + " ). Please contact admin.";
                return dataResult;
            }

        }

        [HttpGet]
        // [Produces("application/json")]
        [Route("api/DownloadResult")]
        public HttpResponseMessage DownloadPrismCollection(string pathfolder)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            //HttpRequest request = Request;
            //System.Web.HttpRequest request = Request.User
            string[] filePaths = Directory.GetFiles(pathfolder);


            //Create the Zip File.
            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                zip.AddDirectoryByName("Files");
                foreach (var path in filePaths)
                {
                    zip.AddFile(path, "Files");

                }
                //Set the Name of Zip File.
                string zipName = String.Format("PrismCollection_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    //Save the Zip File to MemoryStream.
                    zip.Save(memoryStream);

                    //Set the Response Content.
                    response.Content = new ByteArrayContent(memoryStream.ToArray());

                    //Set the Response Content Length.
                    response.Content.Headers.ContentLength = memoryStream.ToArray().LongLength;

                    //Set the Content Disposition Header Value and FileName.
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = zipName;

                    //Set the File Content Type.
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");

                }
            }
            return response;
        }

        // GET: api/CLBSDatas/5
        [HttpGet]
        [Produces("application/json")]
        [ResponseType(typeof(CLBS_Result))]
        public async Task<IHttpActionResult> GetCLBSData(string id)
        {
            CLBS_Results dataResult = new CLBS_Results();
            try
            {
                CLBSData cLBSData = await db.CLBSDatas.FindAsync(id);
                if (cLBSData == null)
                {
                    dataResult.StatusCode = "404";
                    dataResult.StatusDetails = "Not found " + id + " in System";
                    return Ok(dataResult);
                }
                dataResult.DataResult = new vCLBSData()
                {
                    OPC = cLBSData.OPC,
                    SurfaceID = cLBSData.SurfaceID,
                    CreateBy = cLBSData.CreateBy,
                    CreateDate = cLBSData.CreateDate
                };
                dataResult.StatusCode = "200";

                return Ok(dataResult);
            }
            catch (Exception ex)
            {
                Log.Info(ex);
                dataResult.StatusCode = "409";
                dataResult.StatusDetails = "We found the problem in 'GetCLBSDatas process' (DateTime: " + DateTime.Now + " ). Please contact admin.";
                return BadRequest(dataResult.StatusDetails);
            }

        }


        // PUT: api/CLBSDatas/5
        [HttpPut]
        [Produces("application/json")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCLBSData(string OPC, CLBSData cLBSData)
        {
            CLBS_Results dataResult = new CLBS_Results();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (OPC != cLBSData.OPC)
            {
                dataResult.StatusDetails = "" + OPC + "is not match in System";
                return BadRequest(dataResult.StatusDetails);
            }

            db.Entry(cLBSData).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {

                if (!CLBSDataExists(OPC))
                {
                    dataResult.StatusDetails = "Not found " + OPC + " in System";
                    return BadRequest(dataResult.StatusDetails);
                }
                else
                {
                    Log.Info(ex);
                    dataResult.StatusDetails = "We found the problem in 'Update CLBS process' (DateTime: " + DateTime.Now + " ). Please contact admin.";
                    return BadRequest(dataResult.StatusDetails);
                }
            }

            dataResult.StatusCode = "200";

            return Ok(dataResult);
        }

        // POST: api/CLBSDatas
        [HttpPost]
        [ResponseType(typeof(CLBSData))]
        public async Task<IHttpActionResult> PostCLBSData(CLBSData cLBSData)
        {
            CRUD_Data dataResult = new CRUD_Data();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                cLBSData.CreateDate = DateTime.Today;
                if (CLBSDataExists(cLBSData.OPC))
                {
                    return Ok(cLBSData.OPC);
                }
                else
                {
                    db.CLBSDatas.Add(cLBSData);
                    await db.SaveChangesAsync();
                    dataResult.StatusCode = "200";
                }

            }
            catch (DbEntityValidationException e)
            {
                var newException = new FormattedDbEntityValidationException(e);
                Log.Info(newException);
                dataResult.StatusDetails = "We found the problem in 'Add CLBS process'( " + cLBSData.OPC + ") (DateTime: " + DateTime.Now + " ). Please contact admin.";
                return BadRequest(dataResult.StatusDetails);
            }

            return Ok(dataResult.StatusCode);
        }

        //POST: api/UploadFile
        [Route("api/PostUploadCLBSFile")]
        [ResponseType(typeof(CRUD_Data))]
        [HttpPost]
        public async Task<IHttpActionResult> PostUploadCLBSFile()
        {
            string x = "1";
            CRUD_Data dataResult = new CRUD_Data();
            List<SemiFinishes> XMLList = new List<SemiFinishes>();
            HttpContext context = HttpContext.Current;
            List<CLBSData> CLBS_list = new List<CLBSData>();
            CLBS_Result CLBS_Result = new CLBS_Result();
            vPrismCol SFdata = new vPrismCol();
            List<vPrismCol> SFdataList = new List<vPrismCol>();

            if (context.Request.Files.Count > 0)
            {
                try
                {
                    for (int i = 0; i < context.Request.Files.Count; i++)
                    {

                        HttpPostedFile httpPostedFile = context.Request.Files[i];
                        Stream stream = httpPostedFile.InputStream;
                        if (httpPostedFile.FileName.EndsWith(".xls"))
                        {
                            var list = ConvertXlsxToCLBS(httpPostedFile);

                            CLBS_Result = MapCLBSandSTD(list);

                        }
                        else if (httpPostedFile.FileName.EndsWith(".xml"))
                        {
                            var list = ConvertXMLToCLBS(httpPostedFile);
                            CLBS_Result = MapCLBSandSTD(list);
                        }
                        else
                        {
                            dataResult.StatusCode = "404";
                            dataResult.StatusDetails = "Not found type of file. Please use Excel file(.xls) or XML file only.";
                            return Ok(dataResult); // send Error type of file is incorrect.
                        }

                        CLBS_list = CLBS_Result.CLBSlist;
                        dataResult.No_STD = CLBS_Result.NotMatch_STD;
                        foreach (var CLBS in CLBS_list)
                        {

                            IHttpActionResult actionResult = await PostCLBSData(CLBS);
                            SFdata = new vPrismCol {
                                cceCode = CLBS.PrismCol.cceCode,
                                slopeRx = CLBS.PrismCol.slopeRx,
                                slopeRy = CLBS.PrismCol.slopeRy
                            };

                            var contentResult = actionResult as OkNegotiatedContentResult<string>;
                            if (contentResult.Content != "200")
                            {

                                var dup_data = contentResult.Content;
                                dataResult.Dup_data.Add(dup_data);
                            }
                            else
                            {
                                SFdataList.Add(SFdata);
                            }
                        }
                    }
                    string foldername = "PrismCorrect_Result_" + DateTime.Now.ToString("yyyMMdd");
                    string path = CreateFolder(foldername);
                    string xmlPath = path + '/' + "PrismCorrect_" + DateTime.Now.ToString("yyyMMdd") + ".xml";
                    string reportPath = path + '/' + "PrismCorrect_Report" + DateTime.Now.ToString("yyyMMdd") + ".txt";
                    // string filename = "PrismCorrect_" + DateTime.Now.ToString("yyyMMdd") + ".xml";

                    if (!(Directory.Exists(path)))
                    {
                        dataResult.StatusDetails = "We found the problem in 'Create report folder process (DateTime: " + DateTime.Now + " ) " + x + ". Please contact admin. =>>"+path;
                        return BadRequest(dataResult.StatusDetails);
                    }

                    string statusXML = "";
                    string statusReport = "";

                    if (SFdataList.Count >= 1 && dataResult.Dup_data.Count == 0 && dataResult.No_STD.Count == 0) // dont have dup, STD
                    {

                        dataResult.foldername = path;
                        statusXML = GenerateXML(SFdataList, xmlPath);
                        //dataResult.Prism_data = SFdataList;
                        if (statusXML == "200")
                        {
                            //var Path = WebConfigutation.Localhost + "Content/" + filename;
                            dataResult.foldername = path;
                            dataResult.StatusCode = "200";

                        }
                        if (statusXML == "500")
                        {
                            dataResult.StatusDetails = "We found the problem in 'Generate XML CLBS file process (DateTime: " + DateTime.Now + " ) " + x + ". Please contact admin.";
                            return BadRequest(dataResult.StatusDetails);
                        }
                    }
                    else if (SFdataList.Count >= 1 && dataResult.Dup_data.Count > 0 && dataResult.No_STD.Count > 0) // have dup, STD
                    {
                        string dup_data = string.Join(", ", dataResult.Dup_data);
                        string No_std = string.Join(", ", dataResult.No_STD);
                        statusReport = ResultReport(reportPath, dup_data, dataResult.No_STD);
                        statusXML = GenerateXML(SFdataList, xmlPath);
                       // dataResult.foldername = path;

                        if (statusXML == "200" && statusReport == "200")
                        {
                            // var Path = WebConfigutation.Localhost + "Content/" + filename;
                            dataResult.foldername = path;
                            dataResult.StatusCode = "111";

                        }
                        if (statusXML == "500" || statusReport == "500")
                        {
                            // var Path = WebConfigutation.Localhost + "Content/" + filename;
                            dataResult.foldername = path;
                            dataResult.StatusCode = "511";

                        }
                        if (statusXML == "500" && statusReport == "500")
                        {
                            dataResult.StatusDetails = "We found the problem in 'Generate XML CLBS file process (DateTime: " + DateTime.Now + " ) " + x + ". Please contact admin.";
                            return BadRequest(dataResult.StatusDetails);
                        }
                    }
                    else if (SFdataList.Count >= 1 && dataResult.Dup_data.Count > 0 && dataResult.No_STD.Count == 0) // dont have dup
                    {
                        statusXML = GenerateXML(SFdataList, xmlPath);
                        string dup_data = string.Join(", ", dataResult.Dup_data);
                        //string No_std = string.Join(", ", dataResult.No_STD);
                        statusReport = ResultReport(reportPath, dup_data, null);
                        //dataResult.foldername = path;
                        if (statusXML == "200" && statusReport == "200")
                        {
                            // var Path = WebConfigutation.Localhost + "Content/" + filename;
                            dataResult.foldername = path;
                            dataResult.StatusCode = "210";

                        }
                        if (statusXML == "500" || statusReport == "500")
                        {
                            // var Path = WebConfigutation.Localhost + "Content/" + filename;
                            dataResult.foldername = path;
                            dataResult.StatusCode = "510";

                        }
                        if (statusXML == "500" && statusReport == "500")
                        {
                            dataResult.StatusDetails = "We found the problem in 'Generate XML CLBS file process (DateTime: " + DateTime.Now + " ) " + x + ". Please contact admin.";
                            return BadRequest(dataResult.StatusDetails);
                        }
                    }
                    else if (SFdataList.Count >= 1 && dataResult.Dup_data.Count == 0 && dataResult.No_STD.Count > 0)// dont have STD
                    {
                        statusXML = GenerateXML(SFdataList, xmlPath);
                        //string dup_data = string.Join(", ", dataResult.Dup_data);
                        string No_std = string.Join(", ", dataResult.No_STD);
                        statusReport = ResultReport(reportPath, " - ", dataResult.No_STD);
                        //dataResult.foldername = path;
                        if (statusXML == "200" && statusReport == "200")
                        {
                            // var Path = WebConfigutation.Localhost + "Content/" + filename;
                            dataResult.foldername = path;
                            dataResult.StatusCode = "220";
                        }
                        if (statusXML == "500" || statusReport == "500")
                        {
                            // var Path = WebConfigutation.Localhost + "Content/" + filename;
                            dataResult.foldername = path;
                            dataResult.StatusCode = "520";

                        }
                        if (statusXML == "500" && statusReport == "500")
                        {
                            dataResult.StatusDetails = "We found the problem in 'Generate XML CLBS file process (DateTime: " + DateTime.Now + " ) " + x + ". Please contact admin.";
                            return BadRequest(dataResult.StatusDetails);
                        }
                    }
                    else if (SFdataList.Count == 0 && dataResult.Dup_data.Count == 0 && dataResult.No_STD.Count > 0) // have only  STD
                    {
                        //string dup_data = string.Join(", ", dataResult.Dup_data);
                        string No_std = string.Join(", ", dataResult.No_STD);
                        statusReport = ResultReport(reportPath, " - ", dataResult.No_STD);
                        dataResult.foldername = path;
                        dataResult.StatusCode = "221";
                    }
                    else if (SFdataList.Count == 0 && dataResult.Dup_data.Count > 0 && dataResult.No_STD.Count == 0) // have only  Dup
                    {
                        string dup_data = string.Join(", ", dataResult.Dup_data);
                        //string No_std = string.Join(", ", dataResult.No_STD);
                        statusReport = ResultReport(reportPath, dup_data, null);
                        dataResult.foldername = path;
                        dataResult.StatusCode = "211";
                    }
                    else if (SFdataList.Count == 0 && dataResult.Dup_data.Count > 0 && dataResult.No_STD.Count > 0) // have only Dup and STD
                    {
                        string dup_data = string.Join(", ", dataResult.Dup_data);
                        string No_std = string.Join(", ", dataResult.No_STD);
                        statusReport = ResultReport(reportPath, dup_data, dataResult.No_STD);
                        dataResult.foldername = path;
                        dataResult.StatusCode = "222";
                    }
                    else
                    {
                        dataResult.StatusCode = "0";
                    }

                    return Ok(dataResult);
                }
                catch (Exception ex)
                {
                    Log.Info(ex);
                    dataResult.StatusDetails = "We found the problem in 'Add CLBS file process [" + ex + "]'(DateTime: " + DateTime.Now + " ) " + x + ". Please contact admin.";
                    return BadRequest(dataResult.StatusDetails);
                }
            }

            else
            {
                dataResult.StatusDetails = "Not found Your in System. Please import again.";
                return BadRequest(dataResult.StatusDetails);
            }
        }

        // for import old Prism Collection

        //[Route("api/PostPrismFilee")]
        //[ResponseType(typeof(CRUD_Data))]
        //[HttpPost]
        //public async Task<IHttpActionResult> PostPrismFile()
        //{
        //    string x = "1";
        //    CRUD_Data dataResult = new CRUD_Data();
        //    List<SemiFinishes> XMLList = new List<SemiFinishes>();
        //    HttpContext context = HttpContext.Current;
        //    List<CLBSData> CLBS_list = new List<CLBSData>();
        //    CLBS_Result CLBS_Result = new CLBS_Result();
        //    vPrismCol SFdata = new vPrismCol();
        //    List<vPrismCol> SFdataList = new List<vPrismCol>();

        //    if (context.Request.Files.Count > 0)
        //    {
        //        try
        //        {
        //            for (int i = 0; i < context.Request.Files.Count; i++)
        //            {
        //                HttpPostedFile httpPostedFile = context.Request.Files[i];
        //                Stream stream = httpPostedFile.InputStream;

        //                if (httpPostedFile.FileName.EndsWith(".xml"))
        //                {
        //                   // var list = await ConvertoldXMLToCLBSAsync(httpPostedFile);
        //                    //  CLBS_Result = MapCLBSandSTD(list);
        //                }
        //                //        else
        //                //        {
        //                //            dataResult.StatusCode = "404";
        //                //            dataResult.StatusDetails = "Not found type of file. Please use Excel file(.xlxs) or XML file only.";
        //                //            return Ok(dataResult); // send Error type of file is incorrect.
        //                //        }

        //                //        CLBS_list = CLBS_Result.CLBSlist;
        //                //        dataResult.No_STD = CLBS_Result.NotMatch_STD;
        //                //        foreach (var CLBS in CLBS_list)
        //                //        {
        //                //            IHttpActionResult actionResult = await PostCLBSData(CLBS);
        //                //            SFdata = new vPrismCol
        //                //            {
        //                //                cceCode = CLBS.PrismCol.cceCode,
        //                //                slopeRx = CLBS.PrismCol.slopeRx,
        //                //                slopeRy = CLBS.PrismCol.slopeRy
        //                //            };

        //                //            var contentResult = actionResult as OkNegotiatedContentResult<string>;
        //                //            if (contentResult.Content != "200")
        //                //            {

        //                //                var dup_data = contentResult.Content;
        //                //                dataResult.Dup_data.Add(dup_data);
        //                //            }
        //                //            else
        //                //            {
        //                //                SFdataList.Add(SFdata);
        //                //            }
        //                //        }
        //                //    }
        //                //    string foldername = "PrismCorrect_Result_" + DateTime.Now.ToString("yyyMMdd");
        //                //    string path = CreateFolder(foldername);
        //                //    string xmlPath = path + '/' + "PrismCorrect_" + DateTime.Now.ToString("yyyMMdd") + ".xml";
        //                //    string reportPath = path + '/' + "PrismCorrect_Report" + DateTime.Now.ToString("yyyMMdd") + ".txt";
        //                //    // string filename = "PrismCorrect_" + DateTime.Now.ToString("yyyMMdd") + ".xml";

        //                //    if (!(Directory.Exists(path)))
        //                //    {
        //                //        dataResult.StatusDetails = "We found the problem in 'Create report folder process (DateTime: " + DateTime.Now + " ) " + x + ". Please contact admin. =>>" + path;
        //                //        return BadRequest(dataResult.StatusDetails);
        //                //    }

        //                //    string statusXML = "";
        //                //    string statusReport = "";

        //                //    if (SFdataList.Count >= 1 && dataResult.Dup_data.Count == 0 && dataResult.No_STD.Count == 0)
        //                //    {

        //                //        dataResult.foldername = foldername;
        //                //        statusXML = GenerateXML(SFdataList, xmlPath);
        //                //        if (statusXML == "200")
        //                //        {
        //                //            //var Path = WebConfigutation.Localhost + "Content/" + filename;
        //                //            dataResult.Prism_data = path;
        //                //            dataResult.StatusCode = "200";

        //                //        }
        //                //        if (statusXML == "500")
        //                //        {
        //                //            dataResult.StatusDetails = "We found the problem in 'Generate XML CLBS file process (DateTime: " + DateTime.Now + " ) " + x + ". Please contact admin.";
        //                //            return BadRequest(dataResult.StatusDetails);
        //                //        }
        //                //    }
        //                //    else if (SFdataList.Count >= 1 && dataResult.Dup_data.Count > 0 && dataResult.No_STD.Count > 0)
        //                //    {
        //                //        string dup_data = string.Join(", ", dataResult.Dup_data);
        //                //        string No_std = string.Join(", ", dataResult.No_STD);
        //                //        statusReport = ResultReport(reportPath, dup_data, No_std);
        //                //        statusXML = GenerateXML(SFdataList, xmlPath);
        //                //        dataResult.foldername = path;

        //                //        if (statusXML == "200" && statusReport == "200")
        //                //        {
        //                //            // var Path = WebConfigutation.Localhost + "Content/" + filename;
        //                //            dataResult.Prism_data = path;
        //                //            dataResult.StatusCode = "111";

        //                //        }
        //                //        if (statusXML == "500" || statusReport == "500")
        //                //        {
        //                //            // var Path = WebConfigutation.Localhost + "Content/" + filename;
        //                //            dataResult.Prism_data = path;
        //                //            dataResult.StatusCode = "511";

        //                //        }
        //                //        if (statusXML == "500" && statusReport == "500")
        //                //        {
        //                //            dataResult.StatusDetails = "We found the problem in 'Generate XML CLBS file process (DateTime: " + DateTime.Now + " ) " + x + ". Please contact admin.";
        //                //            return BadRequest(dataResult.StatusDetails);
        //                //        }
        //                //    }
        //                //    else if (SFdataList.Count >= 1 && dataResult.Dup_data.Count > 0 && dataResult.No_STD.Count == 0)
        //                //    {
        //                //        statusXML = GenerateXML(SFdataList, xmlPath);
        //                //        string dup_data = string.Join(", ", dataResult.Dup_data);
        //                //        //string No_std = string.Join(", ", dataResult.No_STD);
        //                //        statusReport = ResultReport(reportPath, dup_data, " - ");
        //                //        dataResult.foldername = path;
        //                //        if (statusXML == "200" && statusReport == "200")
        //                //        {
        //                //            // var Path = WebConfigutation.Localhost + "Content/" + filename;
        //                //            dataResult.Prism_data = path;
        //                //            dataResult.StatusCode = "210";

        //                //        }
        //                //        if (statusXML == "500" || statusReport == "500")
        //                //        {
        //                //            // var Path = WebConfigutation.Localhost + "Content/" + filename;
        //                //            dataResult.Prism_data = path;
        //                //            dataResult.StatusCode = "510";

        //                //        }
        //                //        if (statusXML == "500" && statusReport == "500")
        //                //        {
        //                //            dataResult.StatusDetails = "We found the problem in 'Generate XML CLBS file process (DateTime: " + DateTime.Now + " ) " + x + ". Please contact admin.";
        //                //            return BadRequest(dataResult.StatusDetails);
        //                //        }
        //                //    }
        //                //    else if (SFdataList.Count >= 1 && dataResult.Dup_data.Count == 0 && dataResult.No_STD.Count > 0)
        //                //    {
        //                //        statusXML = GenerateXML(SFdataList, xmlPath);
        //                //        //string dup_data = string.Join(", ", dataResult.Dup_data);
        //                //        string No_std = string.Join(", ", dataResult.No_STD);
        //                //        statusReport = ResultReport(reportPath, " - ", No_std);
        //                //        dataResult.foldername = path;
        //                //        if (statusXML == "200" && statusReport == "200")
        //                //        {
        //                //            // var Path = WebConfigutation.Localhost + "Content/" + filename;
        //                //            dataResult.Prism_data = path;
        //                //            dataResult.StatusCode = "220";
        //                //        }
        //                //        if (statusXML == "500" || statusReport == "500")
        //                //        {
        //                //            // var Path = WebConfigutation.Localhost + "Content/" + filename;
        //                //            dataResult.Prism_data = path;
        //                //            dataResult.StatusCode = "520";

        //                //        }
        //                //        if (statusXML == "500" && statusReport == "500")
        //                //        {
        //                //            dataResult.StatusDetails = "We found the problem in 'Generate XML CLBS file process (DateTime: " + DateTime.Now + " ) " + x + ". Please contact admin.";
        //                //            return BadRequest(dataResult.StatusDetails);
        //                //        }
        //                //    }
        //                //    else if (SFdataList.Count == 0 && dataResult.Dup_data.Count == 0 && dataResult.No_STD.Count > 0)
        //                //    {
        //                //        //string dup_data = string.Join(", ", dataResult.Dup_data);
        //                //        string No_std = string.Join(", ", dataResult.No_STD);
        //                //        statusReport = ResultReport(reportPath, " - ", No_std);
        //                //        dataResult.foldername = path;
        //                //        dataResult.StatusCode = "221";
        //                //    }
        //                //    else if (SFdataList.Count == 0 && dataResult.Dup_data.Count > 0 && dataResult.No_STD.Count == 0)
        //                //    {
        //                //        string dup_data = string.Join(", ", dataResult.Dup_data);
        //                //        //string No_std = string.Join(", ", dataResult.No_STD);
        //                //        statusReport = ResultReport(reportPath, dup_data, " - ");
        //                //        dataResult.foldername = path;
        //                //        dataResult.StatusCode = "211";
        //                //    }
        //                //    else if (SFdataList.Count == 0 && dataResult.Dup_data.Count > 0 && dataResult.No_STD.Count > 0)
        //                //    {
        //                //        string dup_data = string.Join(", ", dataResult.Dup_data);
        //                //        string No_std = string.Join(", ", dataResult.No_STD);
        //                //        statusReport = ResultReport(reportPath, dup_data, No_std);
        //                //        dataResult.foldername = path;
        //                //        dataResult.StatusCode = "222";
        //                //    }
        //                //    else
        //                //    {
        //                //        dataResult.StatusCode = "0";
        //                //    }

        //                //    return Ok(dataResult);
        //            }
        //            return Ok();
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.Info(ex);
        //            dataResult.StatusDetails = "We found the problem in 'Add CLBS file process [" + ex + "]'(DateTime: " + DateTime.Now + " ) " + x + ". Please contact admin.";
        //            return BadRequest(dataResult.StatusDetails);
        //        }
        //    }

        //    else
        //    {
        //        dataResult.StatusDetails = "Not found Your in System. Please import again.";
        //        return BadRequest(dataResult.StatusDetails);
        //    }
        //}

        // DELETE: api/CLBSDatas/5
        [ResponseType(typeof(CLBSData))]
        [Route("api/DeleteCLBSData")]
        public async Task<IHttpActionResult> DeleteCLBSData(string id)
        {
            CRUD_Data dataResult = new CRUD_Data();
            try
            {
                CLBSData cLBSData = await db.CLBSDatas.FindAsync(id);
                if (cLBSData == null)
                {

                    dataResult.StatusDetails = "Not found " + id + " in system.";
                    return BadRequest(dataResult.StatusDetails);
                }

                db.CLBSDatas.Remove(cLBSData);
                await db.SaveChangesAsync();

                dataResult.StatusCode = "200";

                return Ok(dataResult);
            }
            catch (Exception ex)
            {
                Log.Info(ex);
                dataResult.StatusDetails = "We found the problem in 'Delete CLBS process'(DateTime: " + DateTime.Now + " ). Please contact admin.";
                return BadRequest(dataResult.StatusDetails);
            }
        }

        [HttpDelete]
        [ResponseType(typeof(CLBSData))]
        [Route("api/DeleteFile")]
        public void DeleteFile(string filename)
        {
            try
            {
                string fullPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/" + filename);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                Log.Info(ex);
                //dataResult.StatusDetails = "We found the problem in 'Delete CLBS process'(DateTime: " + DateTime.Now + " ). Please contact admin.";

            }
        }

        [HttpDelete]
        [ResponseType(typeof(CLBSData))]
        [Route("api/DeleteFolder")]
        public void DeleteFolder()
        {
            try
            {
                string fullPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Result");
                if (Directory.Exists(fullPath))
                {
                    Directory.Delete(fullPath, true);
                }
            }
            catch (Exception ex)
            {
                Log.Info(ex);
                //dataResult.StatusDetails = "We found the problem in 'Delete CLBS process'(DateTime: " + DateTime.Now + " ). Please contact admin.";
            }
        }

        [HttpGet]
        [ResponseType(typeof(CLBSData))]
        [Route("api/CLBSFormat")]
        public HttpResponseMessage DownloadCLBSFormat()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            //  HttpContent Content = new HttpContent();

            var path = HttpContext.Current.Server.MapPath("~/Content/Format/Product_format.xls");
            if (!File.Exists(path))
            {
                //Throw 404 (Not Found) exception if File not found.
                response.StatusCode = HttpStatusCode.NotFound;
                response.ReasonPhrase = string.Format("File not found: CLBS Format document.");
                throw new HttpResponseException(response);
            }
            try
            {
                byte[] bytes = File.ReadAllBytes(path);

                //Set the Response Content Length.
                var longbyte = bytes.Length;

                string Extension = string.Empty;

                response.Content = new ByteArrayContent(bytes);
                response.Content.Headers.ContentLength = longbyte;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = "Product_format.xls";

                //Set the File Content Type.
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.Conflict;
                response.ReasonPhrase = ex.ToString();
                Log.Info(ex + "===>>" + path);
            }
            return response;
        }

        public List<CLBSData> getCLBS(string surfaceId)
        {
            List<CLBSData> Data = new List<CLBSData>();
            Data = db.CLBSDatas.Where(x => x.SurfaceID == surfaceId).ToList();
            return Data;
        }

        public string DeleteCLBS(string id)
        {
            string status = "";
            try
            {
                CLBSData cLBSData = db.CLBSDatas.Find(id);
                if (cLBSData == null)
                {
                    status = "0";
                    return status;
                }
                db.CLBSDatas.Remove(cLBSData);
                db.SaveChangesAsync();

                status = "200";

                return status;
            }
            catch (Exception ex)
            {
                Log.Info(ex);
                status = "500";
                return status;
            }

        }


        private List<CLBS_File> ConvertXlsxToCLBS(HttpPostedFile file)
        {
            Stream stream = file.InputStream;
            IExcelDataReader reader = null;

            reader = ExcelReaderFactory.CreateBinaryReader(stream);

            DataSet result = reader.AsDataSet();
            reader.Close();
            var table = result.Tables[0];
            List<CLBS_File> CLBS_List = new List<CLBS_File>();
            for (var i = 1; i < table.Rows.Count; i++)
            {
                var Rowdata = table.Rows[i];
                var CLBS = new CLBS_File()
                {
                    Material = Rowdata.ItemArray[0].ToString(),
                    OPC = Rowdata.ItemArray[1].ToString(),
                    Eye = Rowdata.ItemArray[2].ToString(),
                    Base = Convert.ToDouble(Rowdata.ItemArray[3]) / 100,
                    Addition = Convert.ToDouble(Rowdata.ItemArray[4]) / 100
                };
                CLBS_List.Add(CLBS);
            }

            return CLBS_List;
        }

        private List<CLBS_File> ConvertXMLToCLBS(HttpPostedFile file)
        {
            List<CLBS_File> CLBS_List = new List<CLBS_File>();
            String Material = "";
            Stream stream = file.InputStream;
            XmlDocument xmlDoc = new XmlDocument();
            Dictionary<string, List<XmlNode>> d = new Dictionary<string, List<XmlNode>>();
            xmlDoc.Load(stream);
            // XmlNode result = xmlDoc.DocumentElement;
            XmlNodeList MatEleList = xmlDoc.GetElementsByTagName("Material");
            XmlNodeList Products = xmlDoc.GetElementsByTagName("SKU");
            if (Products.Count % 2 == 0)
            {
                foreach (XmlElement mat in MatEleList)
                {
                    string value = mat.GetAttribute("value");
                    string valLowcase = value.ToLower();

                    string[] values = valLowcase.Split(' ');
                    if ((values[0].Equals("orma") || values[0].Equals("1.5")) && values[1].Equals("polar"))
                    {
                        Material = mapMaterial(valLowcase);
                    }
                    else
                    {
                        Material = mapMaterial(values[0]);
                    }
                }

                foreach (XmlElement Prod in Products)
                {
                    string OPC = Prod.GetAttribute("OPC");
                    string Eye = Prod.GetAttribute("eye");
                    Double Base = Int16.Parse(Prod.GetAttribute("base")) / 100.00;
                    Double Add = Int16.Parse(Prod.GetAttribute("addition")) / 100.00;
                    CLBS_File filedata = new CLBS_File()
                    {
                        OPC = OPC,
                        Eye = Eye,
                        Base = Base,
                        Addition = Add,
                        Material = Material
                    };
                    CLBS_List.Add(filedata);
                }

            }
            else
            {
                return null;
            }

            return CLBS_List;
        }

    
        
        private string mapMaterial(string mat)
        {
            string matLowcase = mat.ToLower();
            string matResult = "";
            Dictionary<List<string>, string> dataMat = new Dictionary<List<string>, string>();

            // create list orma and store values
            List<string> KeyOMA = new List<string>();
            KeyOMA.Add("1.5");
            KeyOMA.Add("orma");

            // create list orma polar and store values
            List<string> KeyOMApola = new List<string>();
            KeyOMApola.Add("1.5 polar");
            KeyOMApola.Add("orma polar");
            KeyOMApola.Add("1.5polar");
            KeyOMApola.Add("ormapolar");

            // create list MR7 and store values
            List<string> KeyMR7 = new List<string>();
            KeyMR7.Add("mr7");
            KeyMR7.Add("mr 7");
            KeyMR7.Add("1.67");

            // create list MR8 and store values
            List<string> KeyMR8 = new List<string>();
            KeyMR8.Add("mr8");
            KeyMR8.Add("mr 8");
            KeyMR8.Add("ormix");
            KeyMR8.Add("1.6");

            // create list PC and store values
            List<string> KeyPC = new List<string>();
            KeyPC.Add("pc");
            KeyPC.Add("poly");
            KeyPC.Add("1.59");

            // create list N19 and store values
            List<string> KeyN19 = new List<string>();
            KeyN19.Add("n19");
            KeyN19.Add("1.74");
            KeyN19.Add("lineis");

            // create list Trivex and store values
            List<string> KeyTrivex = new List<string>();
            KeyTrivex.Add("trivex");
            KeyTrivex.Add("1.53");

            // add values into map
            dataMat.Add(KeyOMA, "Orma");
            dataMat.Add(KeyOMApola, "OrmaPola");
            dataMat.Add(KeyMR7, "MR7");
            dataMat.Add(KeyMR8, "MR8");
            dataMat.Add(KeyPC, "PC");
            dataMat.Add(KeyN19, "N19");
            dataMat.Add(KeyTrivex, "Trivex");

            foreach (var data in dataMat)
            {

                if (data.Key.Contains(matLowcase))
                {
                    matResult = data.Value;
                    break;
                }

            }

            return matResult;
        }

        private STDResult getSTD(string mat, double Base, double Add)
        {
            STDResult result = new STDResult();
            STANDARDData STD = db.STANDARDDatas.FirstOrDefault(x => x.Material.Equals(mat) && x.Base == Base && x.Addition == Add);
            var STDGroupMat = db.STANDARDDatas.Where(x => x.Material.Equals(mat)).ToList();
            if(STDGroupMat.Count != 0)
            {
                var STDGroupBase = STDGroupMat.Where(x => x.Base == Base).ToList();

                if(STDGroupBase.Count != 0)
                {
                    STD = STDGroupBase.FirstOrDefault(x => x.Addition == Add);
                    if(STD != null)
                    {
                        result.STD = STD;
                    }
                    else
                    {
                        result.Material = null;
                        result.Base = null;
                        result.Addition = " Addition " + Add + " not match!!";
                    }
                }
                else
                {
                    result.Material = null;
                    result.Base = "Base " + Base + "  not match!! ";
                }
            }
            else
            {
                result.Material = "Material " + mat + "  not match!! ";
            }

            
            return result;
        }

        private SemiFinishes getSlopXSlopeY(CLBSData data)
        {
            var STDdata = db.STANDARDDatas.Find(data.SurfaceID);

            SemiFinishes xmldata = new SemiFinishes()
            {
                cceCode = data.OPC,
                slopeRx = STDdata.SlopeX.ToString(),
                slopeRy = STDdata.SlopeY.ToString()
            };
            return xmldata;
        }

        private CLBS_Result MapCLBSandSTD(List<CLBS_File> file)
        {
            CLBS_Result result = new CLBS_Result();
            List<CLBSData> prod_list = new List<CLBSData>();
            Thread.CurrentPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            ClaimsPrincipal principal = new ClaimsPrincipal(Thread.CurrentPrincipal);
            var name = Thread.CurrentPrincipal.Identity.Name;


            foreach (var prod in file)
            {
                var Result = getSTD(prod.Material, prod.Base, prod.Addition);
                var STD = Result.STD;
                if (STD != null)
                {
                    PrismCol prsm = new PrismCol()
                    {
                        cceCode = prod.OPC,
                        slopeRx = STD.SlopeX.ToString(),
                        slopeRy = STD.SlopeY.ToString()
                    };
                    CLBSData data = new CLBSData()
                    {
                        OPC = prod.OPC,
                        Eye = prod.Eye,
                        SurfaceID = STD.SurfaceID,
                        CreateBy = name,
                        CreateDate = DateTime.Today,
                        PrismCol = prsm

                    };
                    result.CLBSlist.Add(data);
                }
                if (STD == null)
                {
                    string report = prod.OPC.ToString();
                    if (Result.Material == null && Result.Base == null)
                    {
                        report += " : "+ Result.Addition;
                    }
                    if ( Result.Material == null)
                    {
                        report += " : " + Result.Base;
                    }
                    else
                    {
                        report += " : " + Result.Material;
                    }                 

                    result.NotMatch_STD.Add(report);
                }

            }

            return result;
        }

        private List<string> SavetoPrismDB(List<SemiFinishes> datalist)
        {
            List<string> Prismlist = new List<string>();
            try
            {
                foreach (var data in datalist)
                {
                    var prsm = new PrismCol()
                    {
                        cceCode = data.cceCode,
                        slopeRx = data.slopeRx,
                        slopeRy = data.slopeRy
                    };

                    _ = Prism.PostPrismCol(prsm);

                    Prismlist.Add(data.cceCode);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Prismlist;

        }
        private string GenerateXML(List<vPrismCol> datalist, string paths)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    XmlDocument xmlDoc = new XmlDocument();

                    XmlNode docNode = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                    xmlDoc.AppendChild(docNode);
                    XmlNode rootNode = xmlDoc.CreateElement("dataPrismCorrections");
                    xmlDoc.AppendChild(rootNode);

                    foreach (var data in datalist)
                    {
                        XmlNode semiFinished = xmlDoc.CreateElement("semiFinished");
                        XmlAttribute cceCode = xmlDoc.CreateAttribute("cceCode");
                        XmlAttribute slopeRx = xmlDoc.CreateAttribute("slopeRx");
                        XmlAttribute slopeRy = xmlDoc.CreateAttribute("slopeRy");
                        cceCode.Value = data.cceCode;
                        slopeRx.Value = data.slopeRx;
                        slopeRy.Value = data.slopeRy;
                        semiFinished.Attributes.Append(cceCode);
                        semiFinished.Attributes.Append(slopeRx);
                        semiFinished.Attributes.Append(slopeRy);
                        rootNode.AppendChild(semiFinished);
                    }
                    System.Xml.Serialization.XmlSerializer writer =
                    new System.Xml.Serialization.XmlSerializer(typeof(XmlDocument));

                    //paths = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/" + filename);
                    xmlDoc.Save(paths);
                    // DownloadPrismCollection(filename);
                }

                return "200";
            }
            catch (Exception ex)
            {

                Log.Info(ex);
                return "500";
            }


        }

        protected string ResultReport(string pathfile, string Dup_data, List<string> No_STD)
        {
            try
            {
                if (File.Exists(pathfile))
                {
                    File.Delete(pathfile);
                }

                // Create a new file     
                using (StreamWriter sw = File.CreateText(pathfile))
                {
                    sw.WriteLine("Prism Collection Report: {0}", DateTime.Now.ToString());
                    sw.WriteLine("Duplicate Product: {0}", Dup_data.ToString());
                    if (No_STD != null)
                    {
                        sw.WriteLine("No standard data support:");
                        foreach (var std in No_STD)
                        {
                            sw.WriteLine("{0}", std);
                        }
                    }
                    
                    sw.WriteLine("***Please validate your product data***");

                }
                return "200";
            }
            catch (Exception ex)
            {
                Log.Info(ex);
                return "500";
            }
        }

        protected string CreateFolder(string folderName)
        {
            string x = "";
            var parts = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Result/" + folderName);
            x += parts;
            var part = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Result");
            x += " ==>> " + part;
            try
            {
                if (!(Directory.Exists(part)))
                {
                    x += " ==>> Exists part";
                    Directory.CreateDirectory(parts);
                    return parts;
                }
                else
                {
                    x += " ==>> Before delete part ";
                    Directory.Delete(part, true);
                    x += " ==>> Before create part";
                    Directory.CreateDirectory(parts);
                    x += " ==>> after create part";
                    return parts;
                }
            }
            catch (Exception ex)
            {
                Log.Info(ex);
                return x+ex;
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CLBSDataExists(string id)
        {
            return db.CLBSDatas.Count(e => e.OPC == id) > 0;
        }

        private UserLoginInfo GetdataLoginInfo()
        {
            if (!HttpContext.Current.Request.LogonUserIdentity.IsAuthenticated)
                return null;

            return new UserLoginInfo("Windows", HttpContext.Current.Request.LogonUserIdentity.User.ToString());
        }
    }


   

    public class CLBSResult : IHttpActionResult
    {
        MemoryStream CLBSStream;
        string xmlFileName;
        HttpRequestMessage httpRequestMessage;
        HttpResponseMessage httpResponseMessage;
        public CLBSResult(MemoryStream data, HttpRequestMessage request, string filename)
        {
            CLBSStream = data;
            httpRequestMessage = request;
            xmlFileName = filename;
        }
        public Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {
            httpResponseMessage = httpRequestMessage.CreateResponse(HttpStatusCode.OK);
            httpResponseMessage.Content = new StreamContent(CLBSStream);
            //httpResponseMessage.Content = new ByteArrayContent(bookStuff.ToArray());  
            httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            httpResponseMessage.Content.Headers.ContentDisposition.FileName = xmlFileName;
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");

            return Task.FromResult(httpResponseMessage);
        }
    }

}