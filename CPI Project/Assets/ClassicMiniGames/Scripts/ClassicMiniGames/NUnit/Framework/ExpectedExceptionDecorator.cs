using NUnit.Framework.Api;
using NUnit.Framework.Internal.Commands;

namespace NUnit.Framework
{
	public class ExpectedExceptionDecorator : ICommandDecorator
	{
		private ExpectedExceptionData exceptionData;

		CommandStage ICommandDecorator.Stage
		{
			get
			{
				return CommandStage.PostSetUpPreTearDown;
			}
		}

		int ICommandDecorator.Priority
		{
			get
			{
				return 0;
			}
		}

		public ExpectedExceptionDecorator(ExpectedExceptionData exceptionData)
		{
			this.exceptionData = exceptionData;
		}

		TestCommand ICommandDecorator.Decorate(TestCommand command)
		{
			return new ExpectedExceptionCommand(command, exceptionData);
		}
	}
}
