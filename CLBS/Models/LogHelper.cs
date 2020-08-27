using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;
using CLBS.Infrastructure;

namespace CLBS.Models
{
    public class LogHelper
    {
        public string LogPath { get; set; }
        public string LogFileName { get; set; }

        public void Setup()
        {
            if (string.IsNullOrEmpty(LogPath))
            {
                LogPath = WebConfigutation.LogPath;
            }
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "%date [%thread] %-5level %logger - %message%newline";
            patternLayout.ActivateOptions();

            RollingFileAppender roller = new RollingFileAppender();
            roller.AppendToFile = true;
            roller.File = Path.Combine(LogPath, LogFileName); //"Log\\RobotExport.log"
            roller.DatePattern = "_yyyy-MM-dd'.log'";
            roller.RollingStyle = RollingFileAppender.RollingMode.Date;
            roller.Layout = patternLayout;
            roller.MaxSizeRollBackups = 5;
            roller.MaximumFileSize = "1GB";
            // roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            roller.StaticLogFileName = true;
            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            MemoryAppender memory = new MemoryAppender();
            memory.ActivateOptions();
            hierarchy.Root.AddAppender(memory);

            DebugAppender Debug = new DebugAppender();
            Debug.Layout = patternLayout;
            Debug.ActivateOptions();
            hierarchy.Root.AddAppender(Debug);

            hierarchy.Root.Level = Level.All;
            hierarchy.Configured = true;
        }
    }


}