using ClubPenguin.Core;
using Disney.Kelowna.Common.SEDFSM;
using System;

namespace ClubPenguin
{
	[Serializable]
	public class StateMachineLoaderSettings : AbstractAspectRatioSpecificSettings
	{
		public StateMachineLoader.Binding[] BindingOverrides;
	}
}
