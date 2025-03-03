using Disney.Kelowna.Common.SEDFSM;
using System;

namespace ClubPenguin.Core
{
	[Serializable]
	public class StateMachineLoaderSettings : AbstractPlatformSpecificSettings
	{
		public StateMachineLoader.Binding[] Bindings;
	}
}
