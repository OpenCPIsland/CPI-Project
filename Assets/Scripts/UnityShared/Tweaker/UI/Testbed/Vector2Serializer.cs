using System;
using UnityEngine;

namespace Tweaker.UI.Testbed
{
	public class Vector2Serializer : CustomTypeSerializer
	{
		[Serializable]
		private class SerializableVector2
		{
			public float x;

			public float y;
		}

		public Vector2Serializer(ITweakerSerializer baseSerializer)
			: base(baseSerializer, typeof(Vector2))
		{
		}

		public override string Serialize(object objectToSerialize)
		{
			Vector2 vector = (Vector2)objectToSerialize;
			SerializableVector2 serializableVector = new SerializableVector2();
			serializableVector.x = vector.x;
			serializableVector.y = vector.y;
			return base.BaseSerializer.Serialize(serializableVector);
		}

		public override object Deserialize(string stringToDeserialize)
		{
			SerializableVector2 serializableVector = (SerializableVector2)base.BaseSerializer.Deserialize(stringToDeserialize, typeof(SerializableVector2));
			Vector2 vector = new Vector2(serializableVector.x, serializableVector.y);
			return vector;
		}
	}
}
