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
using CLBS;
using CLBS.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HttpContext = System.Web.HttpContext;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using ExcelDataReader;
using Newtonsoft.Json.Serialization;
using System.Web.Http.Results;
using log4net;
using System.Reflection;
using EntityState = System.Data.Entity.EntityState;
using System.Web.Http.Cors;
using BadRequestResult = System.Web.Http.Results.BadRequestResult;
using System.Net.Http.Headers;

namespace CLBS.Controllers
{
   // [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class STANDARDDatasController : ApiController
    {
        private PRISMCOLLECTION db = new PRISMCOLLECTION();
        private static readonly ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        // GET: api/STANDARDDatas
        public STDList_Result GetSTANDARDDatas()
        {
            STDList_Result dataResult = new STDList_Result();

            try
            {
                var result = from b in db.STANDARDDatas
                          select
                           new
                           {
                               b.SurfaceID,
                               b.SlopeX,
                               b.SlopeY,
                               b.constantX,
                               b.constantY,
                               b.Base,
                               b.Addition,
                               b.Material,
                               b.Design,
                               b.CreateBy
                           };
                if(result != null)
                {
                    var STD = result.Where(x => !x.SurfaceID.StartsWith("_old"));
                    foreach (var x in STD)
                    {
                        var data = new vStandardData
                        {
                            SurfaceID = x.SurfaceID,
                            SlopeX = x.SlopeX,
                            SlopeY = x.SlopeY,
                            constantX = x.constantX,
                            constantY = x.constantY,
                            Base = x.Base,
                            Addition = x.Addition,
                            Material = x.Material,
                            Design = x.Design,
                            CreateBy = x.CreateBy
                        };
                        dataResult.DataResult.Add(data);
                    }
                    dataResult.StatusCode = "200";
                }
                else
                {
                    dataResult.StatusCode = "404";
                    dataResult.StatusDetails = "Not found CLBS data in system";
                }


                // var Standard = transferSTDTovSTD(STD)
                return dataResult;
            }
            catch (Exception ex)
            {
                Log.Info(ex);
                dataResult.StatusCode = "409";
                dataResult.StatusDetails = "We found the problem in 'Get Standard datas process' (DateTime: " + DateTime.Now + " ). Please contact admin.";
                return dataResult;
            }

        }

        // GET: api/STANDARDDatas/5
        [HttpGet]
        [Produces("application/json")]
        [ResponseType(typeof(STD_Result))]
        public async Task<IHttpActionResult> GetSTANDARDData(string id)
        {
            STD_Result dataResult = new STD_Result();
            try
            {
                STANDARDData sTANDARDData = await db.STANDARDDatas.FindAsync(id);
                if (sTANDARDData == null)
                {
                    dataResult.StatusCode = "404";
                    dataResult.StatusDetails = "Not found " + id + " in System";
                    return Ok(dataResult);
                }

                dataResult.DataResult = new vStandardData
                {
                    SurfaceID = sTANDARDData.SurfaceID,
                    SlopeX = sTANDARDData.SlopeX,
                    SlopeY = sTANDARDData.SlopeY,
                    constantX = sTANDARDData.constantX,
                    constantY = sTANDARDData.constantY,
                    Base = sTANDARDData.Base,
                    Addition = sTANDARDData.Addition,
                    Material = sTANDARDData.Material,
                    Design = sTANDARDData.Design,
                    CreateBy = sTANDARDData.CreateBy
                };
                dataResult.StatusCode = "200";

                return Ok(dataResult);
            }
            catch (Exception ex)
            {
                Log.Info(ex);
                dataResult.StatusCode = "409";
                dataResult.StatusDetails = "We found the problem in 'Get Standard data process' (DateTime: " + DateTime.Now + " ). Please contact admin.";
                return BadRequest(dataResult.StatusDetails);
            }
        }

        // PUT: api/STANDARDDatas/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSTANDARDData(string SurfaceID, STANDARDData sTANDARDData)
        {
            STD_Result dataResult = new STD_Result();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (SurfaceID != sTANDARDData.SurfaceID)
            {
                dataResult.StatusDetails = "" + SurfaceID + "is not match in System";
                return BadRequest(dataResult.StatusDetails);
            }

            db.Entry(sTANDARDData).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!STANDARDDataExists(SurfaceID))
                {
                    dataResult.StatusDetails = "Not found " + SurfaceID + " in System";
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

        // POST: api/STANDARDDatas
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PostSTANDARDData(STANDARDData sTANDARDData)
        {
            STD_Result dataResult = new STD_Result();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (STANDARDDataExists(sTANDARDData.SurfaceID))
                {
                    return Ok(sTANDARDData.SurfaceID);
                }
                else
                {
                    db.STANDARDDatas.Add(sTANDARDData);
                    await db.SaveChangesAsync();
                    dataResult.StatusCode = "200";
                }             
                
            }
            catch (DbEntityValidationException e)
            {
                var newException = new FormattedDbEntityValidationException(e);
                Log.Info(newException);
                dataResult.StatusDetails = "We found the problem in 'Add CLBS process'( " + sTANDARDData.SurfaceID + ") (DateTime: " + DateTime.Now + " ). Please contact admin.";
            }

            return Ok(dataResult.StatusCode);
        }

        // POST: api/UploadFile
        [Route("api/PostUploadSTDFile")]
        [ResponseType(typeof(void))]
        [HttpPost, DisableRequestSizeLimit]
        public async Task<IHttpActionResult> PostUploadSTDFile()
        {
            CRUD_Data dataResult = new CRUD_Data();
            HttpContext context = HttpContext.Current;
            List <STANDARDData> STD_list = new List<STANDARDData>();

            if (context.Request.Files.Count > 0)
            {
                try
                {
                    for (int i = 0; i < context.Request.Files.Count; i++)
                    {
                        HttpPostedFile httpPostedFile = context.Request.Files[i];
                        Stream stream = httpPostedFile.InputStream;
                        if (httpPostedFile.FileName.EndsWith(".xlsx"))
                        {
                            var list = ConvertfileToSTD(httpPostedFile);
                            STD_list.AddRange(list);
                        }
                        else
                        {
                            dataResult.StatusCode = "404";
                            dataResult.StatusDetails = "Not found type of file. Please use Excel file(.xlxs) only.";
                            return Ok(dataResult); // send Error type of file is incorrect.
                        }

                        foreach (var STD in STD_list)
                        {
                            IHttpActionResult actionResult = await PostSTANDARDData(STD);
                            if (actionResult is OkNegotiatedContentResult<string> contentResult && contentResult.Content != "200")
                            {
                                dataResult.Dup_data.Add(contentResult.Content);
                            }
                            if( actionResult is BadRequestResult)
                            {
                                dataResult.StatusCode = "400";
                                dataResult.StatusDetails = "We found problem in add standard data process.";
                            }
                        }
                    }
                    if(dataResult.Dup_data.Count == 0 && dataResult.StatusCode != "400")
                    {
                        dataResult.StatusCode = "200";
                    }
                    else if (dataResult.Dup_data.Count > 0 && dataResult.StatusCode != "400")
                    {
                        var part = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Result/STDResult.txt");
                        var dup_data = string.Join(", ", dataResult.Dup_data);
                        ResultReport(part, dup_data);
                        dataResult.foldername = part;
                        dataResult.StatusCode = "201";

                        dataResult.StatusDetails = "We found duplicate data.";
                    }
                    else if (dataResult.Dup_data.Count == 0 && dataResult.StatusCode == "400")
                    {
                        dataResult.StatusCode = "400";
                        dataResult.StatusDetails = "We found problem in add standard data process.";
                    }
                    else if (dataResult.Dup_data.Count > 0 && dataResult.StatusCode == "400")
                    {
                        var part = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Result/STDResult.txt");
                        var dup_data = string.Join(", ", dataResult.Dup_data);
                        ResultReport(part, dup_data);
                        dataResult.foldername = part;
                        dataResult.StatusCode = "420";
                        dataResult.StatusDetails = "We found duplicate data and problem in add standard data process.";
                    }

                    return Ok(dataResult);
                }
                catch(Exception ex)
                {
                    Log.Info(ex);
                    dataResult.StatusDetails = "We found the problem in 'Add Standard data file process'(DateTime: " + DateTime.Now + " ). Please contact admin.";
                    return BadRequest(dataResult.StatusDetails);
                }               
            }
            else
            {
                return NotFound();
            }
        }

        // DELETE: api/STANDARDDatas/5
        [ResponseType(typeof(STANDARDData))]
        public async Task<IHttpActionResult> DeleteSTANDARDData(string id)
        {
            CRUD_Data dataResult = new CRUD_Data();
            List<string> prob = new List<string>();
            try
            {
                STANDARDData sTANDARDData = await db.STANDARDDatas.FindAsync(id);
                if (sTANDARDData == null)
                {
                    dataResult.StatusDetails = "Not found " + id + " in system.";
                    return BadRequest(dataResult.StatusDetails);
                }

                db.STANDARDDatas.Remove(sTANDARDData);
                await db.SaveChangesAsync();
                dataResult.StatusCode = "200";

                return Ok(dataResult);
            }
            catch (Exception ex)
            {
                Log.Info(ex);
                dataResult.StatusDetails = "We found the problem in 'Delete Stan process'(DateTime: " + DateTime.Now + " ). Please contact admin.";
                return BadRequest(dataResult.StatusDetails);
            }

        }


        [HttpGet]
        [ResponseType(typeof(CLBSData))]
        [Route("api/STDFormat")]
        public HttpResponseMessage DownloadSTDFormat()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            //  HttpContent Content = new HttpContent();

            var path = HttpContext.Current.Server.MapPath("~/Content/Format/StandardData_format.xls");
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
                response.Content.Headers.ContentDisposition.FileName = "StandardData_format.xlsx";

                //Set the File Content Type.
                response.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.Conflict;
                response.ReasonPhrase = ex.ToString();
                Log.Info(ex + "===>>" + path);
            }
            return response;
        }

        [HttpGet]
        [ResponseType(typeof(CLBSData))]
        [Route("api/STDResult")]
        public HttpResponseMessage DownloadSTDResult(string part)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            if (!File.Exists(part))
            {
                //Throw 404 (Not Found) exception if File not found.
                response.StatusCode = HttpStatusCode.NotFound;
                response.ReasonPhrase = string.Format("File not found: Standard data result.");
                throw new HttpResponseException(response);
            }
            try
            {
                byte[] bytes = File.ReadAllBytes(part);

                //Set the Response Content Length.
                var longbyte = bytes.Length;

                string Extension = string.Empty;

                response.Content = new ByteArrayContent(bytes);
                response.Content.Headers.ContentLength = longbyte;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = "STDResult.txt";

                //Set the File Content Type.
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/text");
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.Conflict;
                response.ReasonPhrase = ex.ToString();
                Log.Info(ex + "===>>" + part);
            }
            return response;
        }

        protected string ResultReport(string pathfile, string Dup_data)
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
                    sw.WriteLine("Standard data Report: {0}", DateTime.Now.ToString());
                    sw.WriteLine("Duplicate Product: {0}", Dup_data.ToString());
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



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool STANDARDDataExists(string id)
        {
            return db.STANDARDDatas.Count(e => e.SurfaceID == id) > 0;
        }


        private List<STANDARDData> ConvertfileToSTD(HttpPostedFile file)
        {
            Stream stream = file.InputStream;
            IExcelDataReader reader = null;
            string PCName = HttpContext.Current.Request.LogonUserIdentity.Name;
            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            DataSet result = reader.AsDataSet();
            reader.Close();
            var table = result.Tables[0];
            List<STANDARDData> STD_List = new List<STANDARDData>();
            for(var i = 1; i<table.Rows.Count; i++)
            {
                var Rowdata = table.Rows[i];
                var STD = new STANDARDData()
                {
                    SurfaceID = Rowdata.ItemArray[0].ToString(),
                    SlopeX = Rowdata.ItemArray[1].ToString(),
                    SlopeY = Rowdata.ItemArray[2].ToString(),
                    constantX = Rowdata.ItemArray[3].ToString(),
                    constantY = Rowdata.ItemArray[4].ToString(),
                    Base = Convert.ToDouble(Rowdata.ItemArray[5]),
                    Addition = Convert.ToDouble(Rowdata.ItemArray[6]),
                    Material = Rowdata.ItemArray[7].ToString(),
                    Design = Rowdata.ItemArray[8].ToString(),
                    CreateBy = PCName

                };
                STD_List.Add(STD);
   
            }

            return STD_List;
        }

    
    }

    

}