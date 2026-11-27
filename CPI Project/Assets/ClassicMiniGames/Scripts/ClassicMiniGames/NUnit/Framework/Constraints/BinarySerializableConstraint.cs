using System;
using System.IO;
using System.Runtime.Serialization;

namespace NUnit.Framework.Constraints
{
	public class BinarySerializableConstraint : Constraint
	{
		public override bool Matches(object actual)
		{
			base.actual = actual;
			if (actual == null)
			{
				throw new ArgumentException();
			}
			DataContractSerializer serializer = new DataContractSerializer(actual.GetType());
			MemoryStream memoryStream = new MemoryStream();
			try
			{
				serializer.WriteObject(memoryStream, actual);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				object obj = serializer.ReadObject(memoryStream);
				return obj != null;
			}
			catch (SerializationException)
			{
				return false;
			}
			catch (InvalidDataContractException)
			{
				return false;
			}
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write("binary serializable");
		}

		public override void WriteActualValueTo(MessageWriter writer)
		{
			writer.Write("<{0}>", actual.GetType().Name);
		}

		protected override string GetStringRepresentation()
		{
			return "<binaryserializable>";
		}
	}
}
