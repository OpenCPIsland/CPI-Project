using LitJson;
using System.Runtime.Serialization;

namespace ClubPenguin.Net.Client
{
	internal sealed class JsonDataSerializationSurrogate : ISerializationSurrogate
	{
		public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			JsonData jsonData = (JsonData)obj;
			info.AddValue("d", jsonData.ToJson());
		}

		public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			JsonData jsonData = (JsonData)obj;
			jsonData = JsonMapper.ToObject((string)info.GetValue("d", typeof(string)));
			obj = jsonData;
			return obj;
		}
	}
}
