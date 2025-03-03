using System;
using System.IO;
using System.Xml.Serialization;

namespace NUnit.Framework.Constraints
{
	public class XmlSerializableConstraint : Constraint
	{
		private XmlSerializer serializer;

		public override bool Matches(object actual)
		{
			base.actual = actual;
			if (actual == null)
			{
				throw new ArgumentException();
			}
			MemoryStream memoryStream = new MemoryStream();
			try
			{
				serializer = new XmlSerializer(actual.GetType());
				serializer.Serialize(memoryStream, actual);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				object obj = serializer.Deserialize(memoryStream);
				return obj != null;
			}
			catch (NotSupportedException)
			{
				return false;
			}
			catch (InvalidOperationException)
			{
				return false;
			}
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write("xml serializable");
		}

		public override void WriteActualValueTo(MessageWriter writer)
		{
			writer.Write("<{0}>", actual.GetType().Name);
		}

		protected override string GetStringRepresentation()
		{
			return "<xmlserializable>";
		}
	}
}
