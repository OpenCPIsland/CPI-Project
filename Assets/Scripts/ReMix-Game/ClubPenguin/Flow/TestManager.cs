using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Flow
{
	public class TestManager : MonoBehaviour
	{
		public void Start()
		{
			Service.Set(new EventDispatcher());
			GameObject gameObject = new GameObject("Services");
			Object.DontDestroyOnLoad(gameObject);
			Service.Set(gameObject);
		}
	}
}
