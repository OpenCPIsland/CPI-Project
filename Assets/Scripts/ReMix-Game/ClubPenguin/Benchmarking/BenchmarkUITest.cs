using Disney.Kelowna.Common.GameObjectTree;
using Disney.Kelowna.Common.SEDFSM;
using System;

namespace ClubPenguin.Benchmarking
{
	public class BenchmarkUITest : BenchmarkTest
	{
		public StateMachineDefinition RootStateMachine;

		public TreeNodeDefinitionContentKey RootNode;

		public override void Run(Action<int> onFinishDelegate)
		{
			base.Run(onFinishDelegate);
		}
	}
}
