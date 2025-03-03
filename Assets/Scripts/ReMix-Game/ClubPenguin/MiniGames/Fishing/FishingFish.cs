using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.MiniGames.Fishing
{
	public class FishingFish : MonoBehaviour
	{
		public enum Rarities
		{
			Common,
			Rare,
			Legendary
		}

		public Transform cachedTransform = null;

		private Vector3 _lastPosition = Vector3.zero;

		public Transform scaleHandle = null;

		private Vector3 _originalScale = Vector3.zero;

		private float _scaleFactor = 1f;

		public Collider fishCollider = null;

		private bool _isVisible = true;

		private bool _isHiding = false;

		private bool _isActive = false;

		private FishingGameConfig _config = null;

		public Animator animator = null;

		private ICoroutine _visibleRoutine = null;

		public ParticleSystem fxTrail = null;

		private Rarities _rarity = Rarities.Common;

		private float _offsetT = 0f;

		private float _speed = 1f;

		private float _reelStrength = 1f;

		public Rarities rarity
		{
			get
			{
				return _rarity;
			}
		}

		public float offsetT
		{
			get
			{
				return _offsetT;
			}
		}

		public float speed
		{
			get
			{
				return _speed;
			}
		}

		public float reelStrength
		{
			get
			{
				return _reelStrength * Mathf.Abs(_speed);
			}
		}

		private void Awake()
		{
			_originalScale = scaleHandle.localScale;
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
		}

		public void Init(FishingGameConfig config, FishPatternData patternData, float offsetT)
		{
			_config = config;
			_offsetT = offsetT;
			_rarity = patternData.rarity;
			_speed = patternData.speed;
			if (_speed == 0f)
			{
				_speed = 1f;
			}
			_scaleFactor = 1f;
			_reelStrength = 1f;
			switch (_rarity)
			{
			case Rarities.Legendary:
				_scaleFactor = config.fishScaleLegendary;
				_reelStrength = config.fishReelStrengthLegendary;
				break;
			case Rarities.Rare:
				_scaleFactor = config.fishScaleRare;
				_reelStrength = config.fishReelStrengthRare;
				break;
			default:
				_scaleFactor = config.fishScaleCommon;
				_reelStrength = config.fishReelStrengthCommon;
				break;
			}
			animator.speed = Mathf.Abs(speed);
		}

		public void SetActive(bool isActive)
		{
			_isActive = isActive;
		}

		public void SetPosition(Vector3 position)
		{
			if (_lastPosition != position)
			{
				_lastPosition = cachedTransform.position;
				cachedTransform.position = position;
				Vector3 normalized = (position - _lastPosition).normalized;
				cachedTransform.LookAt(position + normalized);
			}
		}

		public void Scare()
		{
			if (!_isHiding)
			{
				CoroutineRunner.Start(ExecuteHide(), this, "ExecuteHide");
			}
		}

		private IEnumerator ExecuteHide()
		{
			if (!base.gameObject.IsDestroyed())
			{
				_isHiding = true;
				SetVisible(false);
				yield return new WaitForSeconds(_config.fishHideDuration);
				if (_isActive)
				{
					SetVisible(true);
				}
				_isHiding = false;
			}
		}

		public void SetVisible(bool isVisible, bool instant = false)
		{
			if (base.gameObject.IsDestroyed() || (isVisible == _isVisible && !instant))
			{
				return;
			}
			_isVisible = isVisible;
			fishCollider.enabled = _isVisible;
			UpdateParticleState();
			if (_visibleRoutine != null)
			{
				_visibleRoutine.Stop();
				_visibleRoutine = null;
			}
			if (instant)
			{
				float scale = 0f;
				if (isVisible)
				{
					scale = 1f;
				}
				SetScale(scale);
			}
			else
			{
				_visibleRoutine = CoroutineRunner.Start(ExecuteVisible(isVisible), this, "ExecuteVisible");
			}
		}

		private IEnumerator ExecuteVisible(bool isVisible)
		{
			if (!base.gameObject.IsDestroyed())
			{
				float fromInterp = 0f;
				float toInterp = 1f;
				if (!isVisible)
				{
					float num = fromInterp;
					fromInterp = toInterp;
					toInterp = num;
				}
				for (float animT = 0f; animT < _config.fishScaleDuration; animT += Time.deltaTime)
				{
					float interp = Mathf.Clamp01(animT / _config.fishScaleDuration);
					float scaleInterp = Mathf.Lerp(fromInterp, toInterp, interp);
					SetScale(scaleInterp);
					yield return null;
				}
				SetScale(toInterp);
				_visibleRoutine = null;
			}
		}

		private void SetScale(float interp)
		{
			float d = _scaleFactor * interp;
			scaleHandle.localScale = _originalScale * d;
		}

		private void UpdateParticleState()
		{
			if (_isVisible)
			{
				fxTrail.Play();
			}
			else
			{
				fxTrail.Stop();
			}
		}
	}
}
