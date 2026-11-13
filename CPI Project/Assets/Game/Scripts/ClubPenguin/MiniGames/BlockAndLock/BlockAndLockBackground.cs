using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.MiniGames.BlockAndLock
{
	[RequireComponent(typeof(ResourceCleaner))]
	public class BlockAndLockBackground : MonoBehaviour
	{
		private EventDispatcher dispatcher;

		private Renderer rend;

		private Color originalColor;

		private Color successColor = new Color32(byte.MaxValue, 232, 24, byte.MaxValue);

		private GameObject particlesSolvePuzzle;

		private float effectsDelay = 0f;

		private float tweenTime = 0.5f;

		private float delay = 0f;

		private Vector3 particlePos = Vector3.zero;

		private Vector3 particleScale = new Vector3(0.5f, 0.5f, 0.5f);

		private iTween.EaseType easeType = iTween.EaseType.easeInCubic;

		private EventChannel eventChannel;

		private void Awake()
		{
			dispatcher = Service.Get<EventDispatcher>();
			eventChannel = new EventChannel(dispatcher);
			rend = GetComponent<Renderer>();
			if (rend.material.HasProperty("_Color"))
			{
				originalColor = rend.material.color;
			}
			base.gameObject.SetActive(false);
		}

		private void Start()
		{
			eventChannel.AddListener<BlockAndLockEvents.PieceSolveComplete>(onPieceSolveEffectsComplete);
		}

		private void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
		}

		public void Init(Color _successColor, GameObject _particlesSolvePuzzle, float _tweenTime, float _delay, Vector3 _particlePos, Vector3 _particleScale, iTween.EaseType _easeType)
		{
			successColor = _successColor;
			particlesSolvePuzzle = _particlesSolvePuzzle;
			tweenTime = _tweenTime;
			delay = _delay;
			particlePos = _particlePos;
			particleScale = _particleScale;
			easeType = _easeType;
			if (particlesSolvePuzzle != null)
			{
				ParticleSystem component = particlesSolvePuzzle.GetComponent<ParticleSystem>();
				if (component != null)
				{
					effectsDelay = component.main.duration;
				}
			}
		}

		private bool onPieceSolveEffectsComplete(BlockAndLockEvents.PieceSolveComplete e)
		{
			base.gameObject.SetActive(true);
			if (rend.material.HasProperty("_Color"))
			{
				rend.material.color = successColor;
				iTween.ColorTo(base.gameObject, iTween.Hash("color", originalColor, "easeType", easeType, "time", tweenTime, "delay", delay, "oncomplete", "onBackgroundSolvedEffectsComplete", "oncompletetarget", base.gameObject));
			}
			else
			{
				onBackgroundSolvedEffectsComplete();
			}
			if (particlesSolvePuzzle != null)
			{
				GameObject gameObject = Object.Instantiate(particlesSolvePuzzle, particlePos, Quaternion.identity);
				gameObject.transform.SetParent(base.gameObject.transform, false);
				gameObject.transform.localScale = particleScale;
				gameObject.layer = LayerMask.NameToLayer("UI");
			}
			return false;
		}

		private void onBackgroundSolvedEffectsComplete()
		{
			StartCoroutine("AllEffectsComplete");
		}

		private IEnumerator AllEffectsComplete()
		{
			yield return new WaitForSeconds(effectsDelay);
			dispatcher.DispatchEvent(default(BlockAndLockEvents.BackgroundSolveComplete));
		}
	}
}
