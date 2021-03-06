﻿using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ProphetsWay.Utilities
{
	public static class Serializer 
	{
		public static MemoryStream SerializeAsByteArr(this string stringToSerialize)
		{
			var bytes = Encoding.UTF8.GetBytes(stringToSerialize);
			var ms = new MemoryStream(bytes);
			return ms;
		}

		public static MemoryStream SerializeAsByteArr<T>(this T objectToSerialize)
		{
			var formatter = new BinaryFormatter();
			var s = new MemoryStream();

			formatter.Serialize(s, objectToSerialize);
			s.Flush();
			s.Position = 0;

			return s;
		}

        public static string DeserializeFromByteArr(this Stream binaryStream)
        {
            var byteArr = binaryStream.ReadToEnd();
            return Encoding.UTF8.GetString(byteArr);
        }

        public static T DeserializeFromByteArr<T>(this Stream binaryStream)
        {
            var formatter = new BinaryFormatter();
			var obj = (T)formatter.Deserialize(binaryStream);
			binaryStream.Close();

			return obj;
        }

        public static void SerializeAsByteArrToFile<T>(this T objectToSerialize, string targetFileName)
        {
            using (var ms = objectToSerialize.SerializeAsByteArr())
            using (var s = File.Open(targetFileName, FileMode.Create))
            {
                ms.WriteTo(s);
                s.Flush();
                s.Close();
            }
        }

		public static T DeserializeFromByteArrInFile<T>(string targetFileName)
		{
			T obj;

			using (var s = File.Open(targetFileName, FileMode.Open))
				obj = s.DeserializeFromByteArr<T>();

			return obj;
		}

        public static string SerializeAsXml<T>(this T objectToSerialize)
        {
            var formatter = new XmlSerializer(typeof(T));
            var s = new StringWriter();
            var x = new XmlTextWriter(s);

            formatter.Serialize(x, objectToSerialize);

            return s.ToString();
        }

        public static T DeserializeFromXml<T>(this XmlDocument xmlDocument)
		{
            var reader = new StringReader(xmlDocument.OuterXml);
            var formatter = new XmlSerializer(typeof(T));
			var obj = (T)formatter.Deserialize(reader);

			return obj;
		}

	}
}
