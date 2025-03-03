using Disney.LaunchPadFramework;
using UnityEngine;

namespace ClubPenguin.Holiday
{
	public class InteractiveDecorationTarget : InteractiveDecoration
	{
		public InteractiveDecorationController DecorationController;

		public Animator TargetAnimator;

		public string HitAnimName;

		public string SwitchAnimName;

		[Header("Audio")]
		public string AudioTargetSwitch;

		public string AudioTargetHit;

		private ParticleSystem changeParticles;

		public override void Start()
		{
			base.Start();
			if (DuringHidePhase())
			{
				TargetColorChange(true);
			}
			else
			{
				TargetColorChange();
			}
			if (DecorationController == null)
			{
				Log.LogError(this, string.Format("O_o\t Error: {0} does not have a controller set", base.gameObject.GetPath()));
			}
			changeParticles = GetComponentInChildren<ParticleSystem>();
		}

		public override void OnColorChange()
		{
			base.OnColorChange();
			TargetColorChange();
		}

		public void TargetColorChange(bool isInitializing = false)
		{
			if (!isInitializing)
			{
				int num = (int)(CurrentColor + 1);
				if (num >= 6)
				{
					num = 1;
				}
				CurrentColor = (DecorationColor)num;
				if (changeParticles != null && !changeParticles.isPlaying)
				{
					changeParticles.Play();
				}
			}
			Renderer componentInParent = GetComponentInParent<Renderer>();
			if (componentInParent != null)
			{
				base.ChangeColor(componentInParent, CurrentColor);
				if (TargetAnimator != null && !string.IsNullOrEmpty(SwitchAnimName))
				{
					TargetAnimator.SetTrigger(SwitchAnimName);
					PlayAudioEvent(AudioTargetSwitch, base.gameObject);
				}
			}
			else
			{
				Log.LogError(this, string.Format("O_o\t Error: {0} can not find renderer on it's parent", base.gameObject.GetPath()));
			}
		}

		private void OnCollisionEnter(Collision coll)
		{
			if (coll.gameObject.CompareTag("Snowball"))
			{
				if (DecorationController != null)
				{
					DecorationController.OnTargetHit();
				}
				if (TargetAnimator != null && !string.IsNullOrEmpty(HitAnimName))
				{
					TargetAnimator.SetTrigger(HitAnimName);
					PlayAudioEvent(AudioTargetHit, base.gameObject);
				}
			}
		}
	}
}
