using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using Newtonsoft.Json;

namespace Core
{
    /// <summary>
    ///     序列化辅助类
    /// </summary>
    public static class SerializeExtend
    {
        static SerializeExtend()
        {
            JsonConvert.DefaultSettings = () =>
            {
                var setting = new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Include,
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                    DateFormatString = "yyyy-MM-dd HH:mm:ss",
                    NullValueHandling = NullValueHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.Auto,
                };
                return setting;
            };
        }

        #region < 二进制序列化 >

        /// <summary>
        ///     序列化对象
        /// </summary>
        /// <param name="entity">可序列化的对象</param>
        /// <param name="stream">序列化到的目标流</param>
        public static void SerializeToBinary(this object entity, Stream stream)
        {
            if (entity == null)
            {
                return;
            }

            stream.Seek(0, SeekOrigin.Begin);
            var bf = new BinaryFormatter();
            bf.TypeFormat = FormatterTypeStyle.TypesWhenNeeded;

            bf.Serialize(stream, entity);
        }

        /// <summary>
        ///     序列化对象
        /// </summary>
        /// <param name="entity">可序列化的对象</param>
        /// <returns>序列化后的字节数组</returns>
        public static byte[] SerializeToBinary(this object entity)
        {
            using (var ms = new MemoryStream())
            {
                SerializeToBinary(entity, ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        ///     反序列化对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="stream">要反序列化的流</param>
        /// <returns>对象实例</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static T DeserializeToBinary<T>(Stream stream)
        {
            if (stream == null)
            {
                return default(T);
            }

            stream.Seek(0, SeekOrigin.Begin);
            var bf = new BinaryFormatter();
            bf.TypeFormat = FormatterTypeStyle.TypesWhenNeeded;
            return (T)bf.Deserialize(stream);
        }

        /// <summary>
        ///     反序列化对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="data">要反序列化的字节数组</param>
        /// <returns>对象实例</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static T DeserializeToBinary<T>(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                return DeserializeToBinary<T>(ms);
            }
        }

        #endregion

        #region < Xml序列化 >

        /// <summary>
        ///     序列化对象
        /// </summary>
        /// <param name="entity">可序列化的对象</param>
        /// <returns>序列化后的xml文档</returns>
        [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes",
            MessageId = "System.Xml.XmlNode")]
        public static XmlDocument ToXmlDocument(this object entity)
        {
            var document = new XmlDocument();
            document.LoadXml(SerializeToXml(entity));
            return document;
        }

        /// <summary>
        ///     序列化对象
        /// </summary>
        /// <param name="entity">可序列化的对象</param>
        /// <returns>XML字符串</returns>
        [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes",
            MessageId = "System.Xml.XmlNode")]
        public static string SerializeToXml(this object entity)
        {
            if (entity == null)
            {
                return string.Empty;
            }

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var xs = GetSerializer(entity.GetType());
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                xs.Serialize(writer, entity, ns);
            }
            return sb.ToString();
        }


        /// <summary>
        ///     反序列化对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="document">要反序列化的xml文档</param>
        /// <returns>对象实例</returns>
        [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes",
            MessageId = "System.Xml.XmlNode")]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static T DeserializeFromDocument<T>(XmlDocument document)
        {
            if (document == null)
            {
                return default(T);
            }

            using (var xmlReader = new XmlNodeReader(document.DocumentElement))
            {
                var time = Environment.TickCount;
                var xs = GetSerializer(typeof(T));
                Console.WriteLine(Environment.TickCount - time);
                return (T)xs.Deserialize(xmlReader);
            }
        }

        public static T DeserializeFromFile<T>(string fileName)
        {
            var document = new XmlDocument();
            document.Load(fileName);
            return DeserializeFromDocument<T>(document);
        }

        public static T DeserializeFromXml<T>(string xmlString)
        {
            if (string.IsNullOrEmpty(xmlString))
                return default(T);

            xmlString = xmlString.Replace("True", "true");
            xmlString = xmlString.Replace("False", "false");

            var document = new XmlDocument();
            document.LoadXml(xmlString);
            return DeserializeFromDocument<T>(document);
        }

        /// <summary>
        ///     XmlSerializer缓冲集合，用于性能提升
        ///     XmlSerializer构造时过于耗时
        /// </summary>
        private static readonly ConcurrentDictionary<Type, XmlSerializer> m_serializers =
            new ConcurrentDictionary<Type, XmlSerializer>();

        private static XmlSerializer GetSerializer(Type type)
        {
            return m_serializers.GetOrAdd(type, t => new XmlSerializer(t));
        }

        #endregion

        #region < Xml序列化 >

        public static string SerializeToJson(this object entity)
        {
            if (entity == null)
            {
                return string.Empty;
            }
            var json = JsonConvert.SerializeObject(entity, Newtonsoft.Json.Formatting.Indented);
            return json;
        }

        public static T DeserializeFromJson<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        #endregion
    }
}
