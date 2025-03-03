using Disney.LaunchPadFramework;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	public class ToggleAnimationsOnTrigger : MonoBehaviour
	{
		public GameObject Target;

		public bool WaitForInteraction = false;

		public string AnimTriggerOn;

		public string AnimTriggerOff;

		[Header("Audio Settings")]
		public string AudioActivated;

		public string AudioDeactivated;

		private Animator anim;

		private Collider coll;

		private int HashTriggerOn;

		private int HashTriggerOff;

		private bool isActive = false;

		private void Awake()
		{
			if (Target == null)
			{
				anim = GetComponent<Animator>();
			}
			else
			{
				anim = Target.GetComponent<Animator>();
			}
			if (anim == null)
			{
				string arg = (!(Target == null)) ? Target.GetPath() : base.gameObject.GetPath();
				Log.LogError(this, string.Format("O_o\t Animator component required on {0}", arg));
				Object.Destroy(this);
			}
			coll = GetComponent<Collider>();
			if (coll == null)
			{
				string arg = base.gameObject.GetPath();
				Log.LogError(this, string.Format("O_o\t Collider component required on {0}", arg));
				Object.Destroy(this);
			}
			HashTriggerOn = Animator.StringToHash(AnimTriggerOn);
			HashTriggerOff = Animator.StringToHash(AnimTriggerOff);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!WaitForInteraction && other.CompareTag("Player"))
			{
				setTrigger(HashTriggerOn);
				SoundUtils.PlayAudioEvent(AudioActivated, base.gameObject);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (!WaitForInteraction && other.CompareTag("Player"))
			{
				setTrigger(HashTriggerOff);
				SoundUtils.PlayAudioEvent(AudioDeactivated, base.gameObject);
			}
		}

		private void setTrigger(int triggerID)
		{
			if (anim != null && triggerID != 0)
			{
				anim.SetTrigger(triggerID);
			}
		}

		public void OnInteraction()
		{
			if (!isActive)
			{
				setTrigger(HashTriggerOn);
				SoundUtils.PlayAudioEvent(AudioActivated, base.gameObject);
			}
			else
			{
				setTrigger(HashTriggerOff);
				SoundUtils.PlayAudioEvent(AudioDeactivated, base.gameObject);
			}
			isActive = !isActive;
		}
	}
}
