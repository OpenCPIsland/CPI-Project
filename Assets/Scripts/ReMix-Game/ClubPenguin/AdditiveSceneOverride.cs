using ClubPenguin.Core;
using System;

namespace ClubPenguin
{
	[Serializable]
	public struct AdditiveSceneOverride
	{
		public ScheduledEventDateDefinitionKey DateDefinitionKey;

		public string PlayerPrefsKey;

		public string[] AdditiveScenes;
	}
}
