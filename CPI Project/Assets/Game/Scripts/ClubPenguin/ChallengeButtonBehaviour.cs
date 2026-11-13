using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	[RequireComponent(typeof(Button))]
	[RequireComponent(typeof(SpriteSelector))]
	public class ChallengeButtonBehaviour : MonoBehaviour
	{
		private const int DISABLED_SPRITE_INDEX = 0;

		private const int ENABLED_SPRITE_INDEX = 1;

		private DanceGame danceGame;

		private SpriteSelector spriteSelector;

		private void Awake()
		{
			spriteSelector = GetComponent<SpriteSelector>();
			Button component = GetComponent<Button>();
			component.onClick.AddListener(onClick);
		}

		private void Start()
		{
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			danceGame = localPlayerGameObject.GetComponent<DanceGame>();
		}

		private void onClick()
		{
			bool flag = danceGame.ToggleChallenge();
			spriteSelector.SelectSprite(flag ? 1 : 0);
		}
	}
}
