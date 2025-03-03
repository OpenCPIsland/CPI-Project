using System.Runtime.Serialization;

namespace UnityEngine.UI.Extensions
{
	public sealed class Vector4Surrogate : ISerializationSurrogate
	{
		public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			Vector4 vector = (Vector4)obj;
			info.AddValue("x", vector.x);
			info.AddValue("y", vector.y);
			info.AddValue("w", vector.w);
			info.AddValue("z", vector.z);
		}

		public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			Vector4 vector = (Vector4)obj;
			vector.x = (float)info.GetValue("x", typeof(float));
			vector.y = (float)info.GetValue("y", typeof(float));
			vector.w = (float)info.GetValue("w", typeof(float));
			vector.z = (float)info.GetValue("z", typeof(float));
			obj = vector;
			return obj;
		}
	}
}
