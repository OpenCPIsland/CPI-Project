using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public class ScriptedCameraSource
	{
		public enum SourceType
		{
			None,
			Transform,
			Dolly,
			Anim
		}

		public SourceType Type;

		private Vector3 offset;

		private RailDolly dolly;

		private Animator anim;

		private int animTrigger;

		public Transform Transform
		{
			get;
			private set;
		}

		public void Reset()
		{
			Transform = null;
			dolly = null;
			anim = null;
			offset = Vector3.zero;
			Type = SourceType.None;
		}

		public bool IsValid()
		{
			return Type != 0 && Transform != null;
		}

		public void Set(Transform source, int _animTrigger, Vector3 _offset)
		{
			Reset();
			if (!(source != null))
			{
				return;
			}
			anim = source.GetComponent<Animator>();
			if (animTrigger != 0 && anim != null)
			{
				animTrigger = _animTrigger;
				Type = SourceType.Anim;
			}
			else
			{
				dolly = source.GetComponent<RailDolly>();
				if (dolly != null)
				{
					Type = SourceType.Dolly;
				}
				else
				{
					Type = SourceType.Transform;
				}
			}
			offset = _offset;
			Transform = source;
		}

		public void Activate()
		{
			switch (Type)
			{
			case SourceType.Transform:
				break;
			case SourceType.Anim:
				if (animTrigger != 0)
				{
					anim.SetTrigger(animTrigger);
				}
				break;
			case SourceType.Dolly:
				dolly.Timer = 0f;
				dolly.Active = true;
				break;
			}
		}

		public Vector3 GetPosition()
		{
			if (Type == SourceType.Dolly)
			{
				return dolly.GetDollyPosition() + offset;
			}
			return Transform.position + offset;
		}

		public Quaternion GetRotation()
		{
			return Transform.rotation;
		}

		public bool IsFinished()
		{
			bool flag = false;
			switch (Type)
			{
			case SourceType.Transform:
				flag = true;
				break;
			case SourceType.Anim:
				flag |= (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
				break;
			case SourceType.Dolly:
				flag |= dolly.IsComplete;
				break;
			}
			return flag;
		}
	}
}
