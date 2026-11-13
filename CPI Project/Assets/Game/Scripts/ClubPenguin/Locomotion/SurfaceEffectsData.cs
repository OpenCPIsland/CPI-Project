using System;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public class SurfaceEffectsData : ScriptableObject
	{
		[Serializable]
		public struct AudioSwitch
		{
			public string EventName;

			public string SwitchValue;
		}

		[Serializable]
		public struct Effect
		{
			public ParticleSystem System;

			public LayerMask SurfaceLayer;

			public string SurfaceTag;

			public bool UseCollisionHeight;

			public AudioSwitch WalkSwitch;

			public AudioSwitch JogSwitch;

			public AudioSwitch StoppingSwitch;

			public AudioSwitch SprintSwitch;

			public AudioSwitch LandSwitch;

			public AudioSwitch TubeSlideLoopSwitch;
		}

		public Effect[] Effects;

		public AudioSwitch DefaultWalkSwitch;

		public AudioSwitch DefaultJogSwitch;

		public AudioSwitch DefaultStoppingSwitch;

		public AudioSwitch DefaultSprintSwitch;

		public AudioSwitch DefaultLandSwitch;

		public AudioSwitch DefaultTubeSlideLoopSwitch;
	}
}
