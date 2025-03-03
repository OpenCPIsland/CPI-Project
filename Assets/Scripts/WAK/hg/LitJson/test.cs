using UnityEngine;

namespace hg.LitJson
{
	public class test : MonoBehaviour
	{
		public void Start()
		{
			JsonMapper.UnregisterImporters();
			TextAsset textAsset = Resources.Load("sampleJson") as TextAsset;
			string text = textAsset.text;
			testModel testModel = JsonMapper.ToObject<testModel>(text);
		}
	}
}
