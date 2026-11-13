using System;
using Tweaker.UI;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class Vector2Serializer : CustomTypeSerializer
	{
		[Serializable]
		private class SerializableVector2
		{
			public float x;

			public float y;

			public SerializableVector2(float x, float y)
			{
				this.x = x;
				this.y = y;
			}
		}

		public Vector2Serializer(ITweakerSerializer baseSerializer)
			: base(baseSerializer, typeof(Vector2))
		{
		}

		public override string Serialize(object objectToSerialize)
		{
			Vector2 vector = (Vector2)objectToSerialize;
			SerializableVector2 objectToSerialize2 = new SerializableVector2(vector.x, vector.y);
			return base.BaseSerializer.Serialize(objectToSerialize2);
		}

		public override object Deserialize(string stringToDeserialize)
		{
			SerializableVector2 serializableVector = (SerializableVector2)base.BaseSerializer.Deserialize(stringToDeserialize, typeof(SerializableVector2));
			Vector2 vector = new Vector2(serializableVector.x, serializableVector.y);
			return vector;
		}
	}
}
