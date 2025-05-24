using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAI;
using log4net;
using log4net.Config;

namespace Core
{
    class AutoProcess
    {

    }
    public static class Log4
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));
        private static string GetSourceName(this object source)
        {
            return (source is Type) ? ((Type)source).Name : source.GetType().Name;
        }
        private static bool _isInit = false;
        private static bool IsInit {
            get
            {
                return false;
                if (_isInit)
                    return true;
                XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));
                _isInit = true;
                return _isInit;
            }
        }
        public static void Error(this object obj, string msg, params object[] objects)
        {
            if(!IsInit)
                return;
            if (objects != null && objects.Length > 0) msg = string.Format(msg, objects);
            Log.Error($"Sender:{obj.GetSourceName()},Msg:{msg}");
        }
        public static void Warn(this object obj, string msg, params object[] objects)
        {
            if (!IsInit)
                return;
            if (objects != null && objects.Length > 0) msg = string.Format(msg, objects);
            Log.Warn($"Sender:{obj.GetSourceName()},Msg:{msg}");
        }

        public static void Info(this object obj, string msg, params object[] objects)
        {
            if (!IsInit)
                return;
            if (objects != null && objects.Length > 0) msg = string.Format(msg, objects);
            Log.Info($"Sender:{obj.GetSourceName()},Msg:{msg}");
        }

        public static void Debug(this object obj, string msg, params object[] objects)
        {
            if (!IsInit)
                return;
            if (objects != null && objects.Length > 0) msg = string.Format(msg, objects);
            Log.Debug($"Sender:{obj.GetSourceName()},Msg:{msg}");
        }
    }
}
