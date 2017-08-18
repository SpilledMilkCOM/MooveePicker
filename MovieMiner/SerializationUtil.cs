using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MovieMiner
{
	public static class SerializationUtil
	{
		/// <summary> Deserializes Xml string of type T. </summary>
		public static T DeserializeXmlString<T>(string xmlString)
		{
			T tempObject = default(T);

			using (var memoryStream = new MemoryStream(StringToUTF8ByteArray(xmlString)))
			{
				var xs = new XmlSerializer(typeof(T));
				var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);

				tempObject = (T)xs.Deserialize(memoryStream);
			}

			return tempObject;
		}

		/// <summary> Convert String to Array </summary>
		private static Byte[] StringToUTF8ByteArray(string xmlString)
		{
			return new UTF8Encoding().GetBytes(xmlString);
		}
	}
}