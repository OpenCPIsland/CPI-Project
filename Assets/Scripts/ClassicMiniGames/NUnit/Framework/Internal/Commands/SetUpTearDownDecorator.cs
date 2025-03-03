namespace NUnit.Framework.Internal.Commands
{
	public class SetUpTearDownDecorator : ICommandDecorator
	{
		CommandStage ICommandDecorator.Stage
		{
			get
			{
				return CommandStage.SetUpTearDown;
			}
		}

		int ICommandDecorator.Priority
		{
			get
			{
				return 0;
			}
		}

		TestCommand ICommandDecorator.Decorate(TestCommand command)
		{
			return new SetUpTearDownCommand(command);
		}
	}
}
