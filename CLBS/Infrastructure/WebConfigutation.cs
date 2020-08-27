using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CLBS.Infrastructure
{
    public class WebConfigutation
    {
        public static string databaseConnection
        {
            get
            {
                //data source=THESL00639\SQLEXPRESS;initial catalog=PRISMCOLLECTION;user id=PrismCol;password=1qazZAQ!;MultipleActiveResultSets=True;App=EntityFramework
                string strConn = ConfigurationManager.ConnectionStrings["PRISMCOLLECTION"].ToString();
                return strConn;
            }
        }

        public static string Localhost
        {
            get
            {
                return ConfigurationManager.AppSettings["PRISMCOLLECTION.localhost"];
            }
        }

        public static string WebPath
        {
            get
            {
                return ConfigurationManager.AppSettings["PRISMCOLLECTION.WebPath"];
            }
        }
        public static string LogPath
        {
            get
            {
                return ConfigurationManager.AppSettings["PRISMCOLLECTION.LogPath"];
            }
        }
    }
}