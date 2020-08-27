//using System;
//using System.Collections.Generic;
//using System.Reflection;
////using System.Linq;
////using System.Web;
//using System.Web.Mvc;
//using CLBS.Models;
////using RequestManagement.Infrastructure;
//using log4net;
////using RequestManagement.Services;
////using System.Globalization;

//namespace RequestManagement.Controllers
//{
//  //  [UserDataActionFilter]
//    public partial class BaseController : Controller
//    {
//        // public UserData userData = UserData.LoadRecentData();
//        private static readonly ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//        protected override void OnStart(string[] args)
//        {
//            LogHelper logHelper = new LogHelper();
//            logHelper.LogFileName = "Logs\\RoboImport.log";
//            logHelper.Setup();

//        }

//        public void TestLog()
//        {

//            Log.Info(">>>BEGIN:OnStart()");
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        //public RequestManagementIdentity Identity
//        //{
//        //    get
//        //    {
//        //        return (RequestManagementIdentity)HttpContext.User.Identity;
//        //    }
//        //}

//        /// <summary>
//        /// Get Resource value from key
//        /// </summary>
//        /// <param name="key">Resource key</param>
//        /// <returns></returns>
//        //       public string T(string key)
//        //       {
//        //           string resourceValue = Singleton<DependencyConfigure>.Instance.ContainerManager
//        //.Resolve<ILocalizationService>().GetLanguageTextByKey(key, WebConfiguration.CurrentLanguageCode);

//        //           return resourceValue;
//        //       }

//        /// <summary>
//        /// Display success notification
//        /// </summary>
//        /// <param name="message">Message</param>
//        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
//        //protected virtual void SuccessNotification(string message, bool persistForTheNextRequest = true)
//        //{
//        //    AddNotification(NotifyType.Success, message, persistForTheNextRequest);
//        //}

//        /// <summary>
//        /// Display warning notification
//        /// </summary>
//        /// <param name="message">Message</param>
//        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
//        //protected virtual void WarningNotification(string message, bool persistForTheNextRequest = true)
//        //{
//        //    AddNotification(NotifyType.Warning, message, persistForTheNextRequest);
//        //}

//        /// <summary>
//        /// Display error notification
//        /// </summary>
//        /// <param name="exception">Exception</param>
//        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
//        /// <param name="logException">A value indicating whether exception should be logged</param>
//        //protected virtual void ErrorNotification(Exception exception, bool persistForTheNextRequest = true, bool logException = true)
//        //{
//        //    try
//        //    {
//        //        if (logException)
//        //        {
//        //            //  LogException(exception);

//        //            Log.Error(exception.Message, exception);
//        //            AddNotification(NotifyType.Error, exception.Message, persistForTheNextRequest);
//        //        }

//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Log.Error(ex.Message, ex);
//        //    }
//        //    //AddNotification(NotifyType.Error, exception.InnerException.InnerException.Message, persistForTheNextRequest);

//        //}

//        //protected virtual void ErrorNotificationMsg(string Message, bool persistForTheNextRequest = true, bool logException = true)
//        //{
//        //    if (logException)
//        //    {
//        //        //  LogException(exception);
//        //        AddNotification(NotifyType.Error, Message, persistForTheNextRequest);
//        //    }
//        //}
//        ///// <summary>
//        ///// Log exception
//        ///// </summary>
//        ///// <param name="exc">Exception</param>
//        //protected void LogException(Exception exc)
//        //{
//        //    var logger = Singleton<DependencyConfigure>.Instance.ContainerManager.Resolve<ILoggingService>();
//        //    logger.Error(new SPCException(exc));
//        //}

//        /// <summary>
//        /// Display notification
//        /// </summary>
//        /// <param name="type">Notification type</param>
//        /// <param name="message">Message</param>
//        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
//        //protected virtual void AddNotification(NotifyType type, string message, bool persistForTheNextRequest)
//        //{
//        //    string dataKey = string.Format("RequestManagement.notifications.{0}", type);
//        //    if (persistForTheNextRequest)
//        //    {
//        //        if (TempData[dataKey] == null)
//        //            TempData[dataKey] = new List<string>();
//        //        ((List<string>)TempData[dataKey]).Add(message);

//        //    }
//        //    else
//        //    {
//        //        if (ViewData[dataKey] == null)
//        //            ViewData[dataKey] = new List<string>();
//        //        ((List<string>)ViewData[dataKey]).Add(message);

//        //    }
//        //}

//        //public string getCurrentUsername()
//        //{
//        //    return userData.Username;
//        //}

//        //public string getCurrentRole()
//        //{
//        //    return userData.Role;
//        //}
//    }
//}
