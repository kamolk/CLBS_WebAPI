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
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using System.Xml;
using CLBS;
using CLBS.Domain;
using CLBS.Models;
using log4net;
using EntityState = System.Data.Entity.EntityState;
using HttpDeleteAttribute = System.Web.Http.HttpDeleteAttribute;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;

namespace CLBS.Controllers
{
    public class PrismColsController : ApiController
    {
        private PRISMCOLLECTION db = new PRISMCOLLECTION();
        private static readonly ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // GET: api/createDataBackup
        [HttpGet]
        [Route("api/createDataBackup")]
        [ResponseType(typeof(CRUD_Data))]
        public CRUD_Data GetPrismCols()
        {
            CRUD_Data dataResult = new CRUD_Data();

            try
            {
                var filename = "PrismCollection_Databackup_"+ DateTime.Now.ToString("yyyMMdd") + ".xml";
                var results = new List<vPrismCol>();
                var data = from a in db.PrismColss
                           select new vPrismCol
                           {
                               cceCode = a.cceCode,
                               slopeRx = a.slopeRx,
                               slopeRy = a.slopeRy
                           };
                results.AddRange(data);
          
                if (results != null)
                {
                    var part = this.GenerateXML(results, filename);
                    dataResult.StatusCode = "404";
                    dataResult.foldername = part;

                }
                else
                {
                    dataResult.StatusCode = "404";
                    dataResult.StatusDetails = "Not found Prism Collection data in system";
                }
            }
            catch(Exception ex)
            {
                Log.Info(ex);
                dataResult.StatusCode = "409";
                dataResult.StatusDetails = "We found the problem in 'GetPrismCollection data backup process' (DateTime: " + DateTime.Now + " ). Please contact admin.";
                return dataResult;
            }
            

            return dataResult;
        }

        [HttpGet]
        [Route("api/DownloadPrismCollection")]
        public HttpResponseMessage DownloadPrismCollection(string filename)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            var name = "PrismCollection_Databackup_" + DateTime.Now.ToString("yyyMMdd") + ".xml";

            var path = HttpContext.Current.Server.MapPath("~/Content/Result") + filename;
            if (!File.Exists(filename))
            {
                //Throw 404 (Not Found) exception if File not found.
                response.StatusCode = HttpStatusCode.NotFound;
                response.ReasonPhrase = string.Format("File not found: {0} .", filename);
                throw new HttpResponseException(response);
            }
            try
            {
                byte[] bytes = File.ReadAllBytes(filename);

                //Set the Response Content Length.
                var longbyte = bytes.Length;

                string Extension = string.Empty;

                response.Content = new ByteArrayContent(bytes);
                response.Content.Headers.ContentLength = longbyte;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = name;

                //Set the File Content Type.
                response.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/xml");

            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.Conflict;
                response.ReasonPhrase = ex.ToString();
                Log.Info(ex + "===>>" + path);
            }
            return response;
        }

        // GET: api/PrismCols/5
        [ResponseType(typeof(PrismCol_Result))]
        public async Task<IHttpActionResult> GetPrismCol(string id)
        {
            PrismCol_Result dataResult = new PrismCol_Result();
            try
            {
                PrismCol prismCol = await db.PrismColss.FindAsync(id);
                if (prismCol == null)
                {
                    dataResult.StatusCode = "404";
                    dataResult.StatusDetails = "Not found " + id + " in System";
                    return Ok(dataResult);
                }
                dataResult.DataResult = new vPrismCol
                {
                    cceCode = prismCol.cceCode,
                    slopeRx = prismCol.slopeRx,
                    slopeRy = prismCol.slopeRy
                };
                dataResult.StatusCode = "200";
            }
            catch(Exception ex)
            {
                Log.Info(ex);
                dataResult.StatusCode = "409";
                dataResult.StatusDetails = "We found the problem in 'GetPrismCollection data process' (DateTime: " + DateTime.Now + " ). Please contact admin.";
                return BadRequest(dataResult.StatusDetails);
            }
            

            return Ok(dataResult);
        }

        // PUT: api/PrismCols/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPrismCol(string cceCode, PrismCol prismCol)
        {
            PrismCol_Result dataResult = new PrismCol_Result();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (cceCode != prismCol.cceCode)
            {
                dataResult.StatusDetails = "" + cceCode + "is not match in System";
                return BadRequest(dataResult.StatusDetails);
            }

            db.Entry(prismCol).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PrismColExists(cceCode))
                {
                    dataResult.StatusDetails = "Not found " + cceCode + " in System";
                    return BadRequest(dataResult.StatusDetails);
                }
                else
                {
                    Log.Info(ex);
                    dataResult.StatusDetails = "We found the problem in 'Update PrismCol process' (DateTime: " + DateTime.Now + " ). Please contact admin.";
                    return BadRequest(dataResult.StatusDetails);
                }
            }

            dataResult.StatusCode = "200";

            return Ok(dataResult);
        }

        // POST: api/PrismCols
        [HttpPost]
        [ResponseType(typeof(CLBSData))]
        [ValidateAntiForgeryToken]
        [ResponseType(typeof(PrismCol))]
        public async Task<IHttpActionResult> PostPrismCol(PrismCol prismCol)
        {
            CRUD_Data dataResult = new CRUD_Data();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if (PrismColExists(prismCol.cceCode))
                {
                    return Ok(prismCol.cceCode);
                }
                else
                {
                    db.PrismColss.Add(prismCol);
                    await db.SaveChangesAsync();
                    dataResult.StatusCode = "200";
                }
            }      
            catch (DbEntityValidationException e)
            {
                var newException = new FormattedDbEntityValidationException(e);
                Log.Info(newException);
                dataResult.StatusDetails = "We found the problem in 'Add CLBS process'( " + prismCol.cceCode + ") (DateTime: " + DateTime.Now + " ). Please contact admin.";
                return BadRequest(dataResult.StatusDetails);
            }

            return Ok(dataResult.StatusCode);
        }

        // DELETE: api/PrismCols/5
        [HttpDelete]
        [Route("api/DeletePrismCol")]
        [ResponseType(typeof(PrismCol))]
        public async Task<IHttpActionResult> DeletePrismCol(string id)
        {
            CRUD_Data dataResult = new CRUD_Data();
            try
            {
                PrismCol prismCol = await db.PrismColss.FindAsync(id);
                if (prismCol == null)
                {
                    dataResult.StatusDetails = "Not found " + id + " in system.";
                    return BadRequest(dataResult.StatusDetails);
                }

                db.PrismColss.Remove(prismCol);
                await db.SaveChangesAsync();

                dataResult.StatusCode = "200";

                return Ok(dataResult);
            }
            catch(Exception ex)
            {
                Log.Info(ex);
                dataResult.StatusDetails = "We found the problem in 'Delete CLBS process'(DateTime: " + DateTime.Now + " ). Please contact admin.";
                return BadRequest(dataResult.StatusDetails);
            }         
        }

        public string DeletePrism(string id)
        {
            //CRUD_Data dataResult = new CRUD_Data();
            string status = "";
            try
            {
                PrismCol prismCol = db.PrismColss.Find(id);
                if (prismCol == null)
                {
                    status = "0";
                }

                db.PrismColss.Remove(prismCol);
                db.SaveChangesAsync();

                status =  "200";

                return status;
            }
            catch (Exception ex)
            {
                Log.Info(ex);
                status = "500";
                return status;
            }
        }

        public PrismCol getPrism( string OPC)
        {
            try
            {
                PrismCol data = db.PrismColss.Find(OPC);
                return data;
            }
            catch(Exception ex)
            {
                PrismCol data = null;
                Log.Info(ex);
                return data;
            }
            
        }
        private string GenerateXML(List<vPrismCol> datalist, string filename)
        {
            var paths = "";
            try
            {
                var part = CreateFolder();
                if (!(Directory.Exists(part)))
                {
                    return "500";
                }

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
                        XmlAttribute slopeRy = xmlDoc.CreateAttribute("slopeRy");
                        XmlAttribute slopeRx = xmlDoc.CreateAttribute("slopeRx");                        
                        XmlAttribute cceCode = xmlDoc.CreateAttribute("cceCode");
 
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

                    paths = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Result/" + filename);
                    xmlDoc.Save(paths);

                }

                return paths;
            }
            catch (Exception ex)
            {

                Log.Info(ex);
                return "500";
            }
        }

        protected string CreateFolder()
        {
            string x = "";
          //  var parts = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Result/" + folderName);
            var part = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Result");
            x += " ==>> " + part;
            try
            {
                if (!(Directory.Exists(part)))
                {
                    x += " ==>> Exists part";
                    Directory.CreateDirectory(part);
                    return part;
                }
                else
                {
                    x += " ==>> Before delete part ";
                    Directory.Delete(part, true);
                    x += " ==>> Before create part";
                    Directory.CreateDirectory(part);
                    x += " ==>> after create part";
                    return part;
                }
            }
            catch (Exception ex)
            {
                Log.Info(ex);
                return x + ex;
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

        private bool PrismColExists(string id)
        {
            return db.PrismColss.Count(e => e.cceCode == id) > 0;
        }
    }
}