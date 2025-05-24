using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Language
{
    enum LanguageType
    {
        Chinese,
        English
    }
    public class LanguageProxy
    {
        private static string connectionString = $"Data Source={Environment.CurrentDirectory}\\Language.db;Version=3;";
        private static LanguageType _currentLanguage;
        private static Dictionary<string, string> _languageDic = new Dictionary<string, string>();
        private static Dictionary<string, string> _languageBaseDic = new Dictionary<string, string>();
        static LanguageProxy()
        {
            string cultureName = Thread.CurrentThread.CurrentCulture.Name;
            switch (cultureName)
            {
                case "en":
                    _currentLanguage = LanguageType.English;
                    break;
                case "zh-CN":
                    _currentLanguage = LanguageType.Chinese;
                    break;
                default:
                    _currentLanguage = LanguageType.Chinese;
                    break;
            }
            //连接数据库并加载多语言
            CreateTableIfNoExist();
            _languageBaseDic = _languageDic = GetLanguageDict(_currentLanguage);
            if(_currentLanguage != LanguageType.Chinese)
                _languageBaseDic = GetLanguageDict(LanguageType.Chinese);

        }
        public static string GetLanguage(string key)
        {
            try { 
                if(_languageDic.ContainsKey(key))
                    return _languageDic[key];
                if (_languageBaseDic.ContainsKey(key))
                    return _languageBaseDic[key];
                return $"#{key}#";
            }
            catch (Exception )
            {
                return $"#{key}#";
            }
        }
        public static string GetLanguageWithDefault(string key,string defaultValue)
        {
            try
            {
                if (_languageDic.ContainsKey(key))
                    return _languageDic[key];
                if (_languageBaseDic.ContainsKey(key))
                    return _languageBaseDic[key];
                AddDefaultData(key, defaultValue);
                _languageBaseDic[key] = defaultValue;
                return defaultValue;
            }
            catch (Exception)
            {
                return $"#{key}#";
            }
        }

        // 查数据库,获取对应的字符串,然后调用format函数把参数插进去
        public static string GetLanguage(string key, params object[] param)
        {
            try
            {
                if (_languageDic.ContainsKey(key))
                    return string.Format(_languageDic[key],param);
                if (_languageBaseDic.ContainsKey(key))
                    return string.Format(_languageBaseDic[key], param);
                return $"#{key}#";
            }
            catch (Exception)
            {
                return $"#{key}#";
            }
        }
        public static string GetLanguageWithDefault(string key, string defaultValue, params object[] param)
        {
            try
            {
                if (_languageDic.ContainsKey(key))
                    return string.Format(_languageDic[key], param);
                if (_languageBaseDic.ContainsKey(key))
                    return string.Format(_languageBaseDic[key], param);
                AddDefaultData(key, defaultValue);
                _languageBaseDic[key] = defaultValue;
                return string.Format(_languageBaseDic[key], param);
            }
            catch (Exception)
            {
                return $"#{key}#";
            }
        }
        static void CreateTableIfNoExist()
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = @"CREATE TABLE IF NOT EXISTS Language (
                            ID INTEGER PRIMARY KEY AUTOINCREMENT,
                            KEY TEXT NOT NULL,
                            ZHCN TEXT NOT NULL,
                            EN TEXT)";
                new SQLiteCommand(sql, conn).ExecuteNonQuery();
            }
        }
        static Dictionary<string, string> GetLanguageDict(LanguageType lang)
        {
            Dictionary<string, string> languagedata = new Dictionary<string, string>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = @"";
                switch (lang)
                {
                    case LanguageType.English:
                        sql = "SELECT (KEY,ZHCN) FROM Language";
                        break;
                    case LanguageType.Chinese:
                        sql = "SELECT (KEY,EN) FROM Language";
                        break;
                    default:
                        sql = "SELECT (KEY,ZHCN) FROM Language";
                        break;
                }
                using (var reader = new SQLiteCommand(sql, conn).ExecuteReader())
                {
                    while (reader.Read())
                    {
                        languagedata.Add(reader.GetString(0), reader.GetString(1));
                    }
                }
            }
            return languagedata;
        }

        static void AddDefaultData(string key,string value)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = $"INSERT INTO Language (KEY, ZHCN) VALUES ({key}, {value})";
                var cmd = new SQLiteCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
