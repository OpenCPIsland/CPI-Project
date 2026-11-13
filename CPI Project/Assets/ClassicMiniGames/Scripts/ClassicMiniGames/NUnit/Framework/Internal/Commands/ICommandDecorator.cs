namespace NUnit.Framework.Internal.Commands
{
	public interface ICommandDecorator
	{
		CommandStage Stage
		{
			get;
		}

		int Priority
		{
			get;
		}

		TestCommand Decorate(TestCommand command);
	}
}
