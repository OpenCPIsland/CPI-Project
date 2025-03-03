using System;
using UnityEngine;

namespace Tweaker.UI.Testbed
{
	public class Vector3Serializer : CustomTypeSerializer
	{
		[Serializable]
		private class SerializableVector3
		{
			public float x;

			public float y;

			public float z;
		}

		public Vector3Serializer(ITweakerSerializer baseSerializer)
			: base(baseSerializer, typeof(Vector3))
		{
		}

		public override string Serialize(object objectToSerialize)
		{
			Vector3 vector = (Vector3)objectToSerialize;
			SerializableVector3 serializableVector = new SerializableVector3();
			serializableVector.x = vector.x;
			serializableVector.y = vector.y;
			serializableVector.z = vector.z;
			return base.BaseSerializer.Serialize(serializableVector);
		}

		public override object Deserialize(string stringToDeserialize)
		{
			SerializableVector3 serializableVector = (SerializableVector3)base.BaseSerializer.Deserialize(stringToDeserialize, typeof(SerializableVector3));
			Vector3 vector = new Vector3(serializableVector.x, serializableVector.y, serializableVector.z);
			return vector;
		}
	}
}
