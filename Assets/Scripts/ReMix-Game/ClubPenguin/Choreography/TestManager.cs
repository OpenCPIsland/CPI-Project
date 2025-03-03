using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Choreography
{
	public class TestManager : MonoBehaviour
	{
		public GameObject Cannon;

		public GameObject Firee;

		public GameObject Firer;

		public InteractionDefinition Interaction;

		public void Awake()
		{
			Service.Set(new EventDispatcher());
			GameObject gameObject = new GameObject("Services");
			CoroutineRunner instance = gameObject.AddComponent<CoroutineRunner>();
			Object.DontDestroyOnLoad(gameObject);
			Service.Set(instance);
			Service.Set(gameObject);
		}

		public void Start()
		{
			Interaction.Begin(Cannon, Firee, Firer);
		}
	}
}
