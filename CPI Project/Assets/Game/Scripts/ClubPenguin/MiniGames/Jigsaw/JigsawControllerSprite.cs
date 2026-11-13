using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.MiniGames.Jigsaw
{
	public class JigsawControllerSprite : MonoBehaviour
	{
		private EventDispatcher dispatcher;

		private void Awake()
		{
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.AddListener<JigsawEventsSprite.RegisterRespond>(onRegisterRespond);
		}

		private void Start()
		{
			dispatcher.DispatchEvent(default(JigsawEventsSprite.Register));
		}

		private void OnDestroy()
		{
			dispatcher.RemoveListener<JigsawEventsSprite.RegisterRespond>(onRegisterRespond);
		}

		private bool onRegisterRespond(JigsawEventsSprite.RegisterRespond e)
		{
			return false;
		}
	}
}
