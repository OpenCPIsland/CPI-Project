namespace NUnit.Framework.Internal.Commands
{
	public abstract class DelegatingTestCommand : TestCommand
	{
		protected TestCommand innerCommand;

		protected DelegatingTestCommand(TestCommand innerCommand)
			: base(innerCommand.Test)
		{
			this.innerCommand = innerCommand;
		}
	}
}
