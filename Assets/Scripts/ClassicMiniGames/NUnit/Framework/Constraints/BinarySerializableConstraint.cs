using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NUnit.Framework.Constraints
{
	public class BinarySerializableConstraint : Constraint
	{
		private readonly BinaryFormatter serializer = new BinaryFormatter();

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
				serializer.Serialize(memoryStream, actual);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				object obj = serializer.Deserialize(memoryStream);
				return obj != null;
			}
			catch (SerializationException)
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
