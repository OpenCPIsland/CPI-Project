using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Misc")]
	public class ChangeFogAction : FsmStateAction
	{
		public bool FogEnabled = true;

		public Color Color = new Color(0.96f, 0.84f, 0.99f, 1f);

		public float Density = 0.5f;

		public FogMode FogMode = FogMode.Linear;

		public float StartDistance = 15f;

		public float EndDistance = 160f;

		public override void OnEnter()
		{
			RenderSettings.fog = FogEnabled;
			RenderSettings.fogColor = Color;
			RenderSettings.fogDensity = Density;
			RenderSettings.fogMode = FogMode;
			RenderSettings.fogStartDistance = StartDistance;
			RenderSettings.fogEndDistance = EndDistance;
			Finish();
		}
	}
}
