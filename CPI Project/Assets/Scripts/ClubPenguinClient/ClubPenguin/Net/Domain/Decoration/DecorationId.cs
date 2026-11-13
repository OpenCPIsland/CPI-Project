using System;

namespace ClubPenguin.Net.Domain.Decoration
{
	[Serializable]
	public class DecorationId
	{
		public int definitionId;

		public DecorationType type;

		public long? customId;

		public DecorationId()
		{
		}

		public DecorationId(int definitionId, DecorationType type)
			: this(definitionId, type, null)
		{
		}

		public DecorationId(int definitionId, DecorationType type, long? customId)
		{
			this.definitionId = definitionId;
			this.type = type;
			this.customId = customId;
		}

		public override string ToString()
		{
			return definitionId + ":" + (int)type + (customId.HasValue ? (":" + customId.Value) : "");
		}

		public static DecorationId FromString(string str)
		{
			string[] array = str.Split(':');
			long? num = null;
			if (array.Length > 2)
			{
				num = Convert.ToInt64(array[2]);
			}
			return new DecorationId(Convert.ToInt32(array[0]), (DecorationType)Convert.ToInt32(array[1]), num);
		}
	}
}
