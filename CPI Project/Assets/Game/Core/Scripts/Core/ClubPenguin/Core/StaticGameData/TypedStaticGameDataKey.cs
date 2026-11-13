using UnityEngine;

namespace ClubPenguin.Core.StaticGameData
{
	public class TypedStaticGameDataKey<T, F> : StaticGameDataKey, ISerializationCallbackReceiver where T : StaticGameDataDefinition
	{
		public F Id;

		[SerializeField]
		private string type;

		public string UnderliningTypeString
		{
			get
			{
				return type;
			}
		}

		public void OnBeforeSerialize()
		{
			if (string.IsNullOrEmpty(type))
			{
				type = StaticGameDataKey.GetTypeString(typeof(T));
			}
		}

		public void OnAfterDeserialize()
		{
		}
	}
}
