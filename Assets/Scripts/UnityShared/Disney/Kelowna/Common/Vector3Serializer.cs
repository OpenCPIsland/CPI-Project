using System;
using Tweaker.UI;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class Vector3Serializer : CustomTypeSerializer
	{
		[Serializable]
		private class SerializableVector3
		{
			public float x;

			public float y;

			public float z;

			public SerializableVector3()
			{
			}

			public SerializableVector3(float x, float y, float z)
			{
				this.x = x;
				this.y = y;
				this.z = z;
			}
		}

		public Vector3Serializer(ITweakerSerializer baseSerializer)
			: base(baseSerializer, typeof(Vector3))
		{
		}

		public override string Serialize(object objectToSerialize)
		{
			Vector3 vector = (Vector3)objectToSerialize;
			SerializableVector3 objectToSerialize2 = new SerializableVector3(vector.x, vector.y, vector.z);
			return base.BaseSerializer.Serialize(objectToSerialize2);
		}

		public override object Deserialize(string stringToDeserialize)
		{
			SerializableVector3 serializableVector = (SerializableVector3)base.BaseSerializer.Deserialize(stringToDeserialize, typeof(SerializableVector3));
			Vector3 vector = new Vector3(serializableVector.x, serializableVector.y, serializableVector.z);
			return vector;
		}
	}
}
