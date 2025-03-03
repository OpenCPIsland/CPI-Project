using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using LitJson;
using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public class SignedResponse<T>
	{
		public long goodUntil;

		public string signature;

		public string swid;

		private JsonData _rawJson;

		[NonSerialized]
		public T Data;

		public JsonData data
		{
			get
			{
				return _rawJson;
			}
			set
			{
				_rawJson = value;
				Data = JsonMapper.ToObject<T>(value.ToJson());
			}
		}

		static SignedResponse()
		{
			JsonMapper.RegisterExporter<SignedResponse<T>>(exportSignedResponse);
		}

		private static void exportSignedResponse(SignedResponse<T> obj, JsonWriter writer)
		{
			writer.WriteObjectStart();
			writer.WritePropertyName("goodUntil");
			writer.Write(obj.goodUntil);
			writer.WritePropertyName("signature");
			writer.Write(obj.signature);
			writer.WritePropertyName("swid");
			writer.Write(obj.swid);
			writer.WritePropertyName("data");
			ICommonGameSettings commonGameSettings = Service.Get<ICommonGameSettings>();
			if (commonGameSettings.OfflineMode && !string.IsNullOrEmpty(commonGameSettings.GameServerHost))
			{
				writer.WriteRaw(JsonMapper.ToJson(obj.Data));
			}
			else if (obj._rawJson != null)
			{
				writer.WriteRaw(obj._rawJson.ToJson());
			}
			else
			{
				writer.Write("");
			}
			writer.WriteObjectEnd();
		}
	}
}
