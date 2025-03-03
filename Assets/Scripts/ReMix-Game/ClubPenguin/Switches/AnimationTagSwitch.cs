using ClubPenguin.Core;
using System;
using UnityEngine;

namespace ClubPenguin.Switches
{
	public class AnimationTagSwitch : Switch
	{
		public int AnimLayer = 0;

		public string[] AnimTags = new string[0];

		private int[] animTagHashes;

		private Animator anim;

		private int curTagHash;

		public override string GetSwitchType()
		{
			throw new NotImplementedException();
		}

		public override object GetSwitchParameters()
		{
			throw new NotImplementedException();
		}

		public void Start()
		{
			if (AnimTags.Length > 0)
			{
				animTagHashes = new int[AnimTags.Length];
				for (int i = 0; i < animTagHashes.Length; i++)
				{
					animTagHashes[i] = Animator.StringToHash(AnimTags[i]);
				}
			}
			else
			{
				base.enabled = false;
			}
			if (!SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.IsDestroyed())
			{
				anim = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<Animator>();
				Change(false);
			}
		}

		public void LateUpdate()
		{
			if (!(anim != null) || !anim.isActiveAndEnabled)
			{
				return;
			}
			int tagHash = anim.GetCurrentAnimatorStateInfo(AnimLayer).tagHash;
			if (tagHash == curTagHash)
			{
				return;
			}
			bool onoff = false;
			for (int i = 0; i < animTagHashes.Length; i++)
			{
				if (tagHash == animTagHashes[i])
				{
					onoff = true;
					break;
				}
			}
			curTagHash = tagHash;
			Change(onoff);
		}
	}
}
