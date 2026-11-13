using System.Collections.Generic;

namespace NUnit.Framework.Internal.Commands
{
	public class CommandDecoratorList : List<ICommandDecorator>
	{
		public void OrderByStage()
		{
			Sort(CommandDecoratorComparison);
		}

		private int CommandDecoratorComparison(ICommandDecorator x, ICommandDecorator y)
		{
			return x.Stage.CompareTo(y.Stage);
		}
	}
}
