using System.Collections;
using UnityEngine;

namespace Fabric
{
	public abstract class CustomAudioClipLoader : MonoBehaviour
	{
		public AudioClip _audioClip;

		public abstract IEnumerator LoadAudioClip(string audioClipName, LanguageProperties language);
	}
}
