using NUnit.Framework.Internal.Commands;
using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class RepeatAttribute : PropertyAttribute, ICommandDecorator
	{
		CommandStage ICommandDecorator.Stage
		{
			get
			{
				return CommandStage.Repeat;
			}
		}

		int ICommandDecorator.Priority
		{
			get
			{
				return 0;
			}
		}

		public RepeatAttribute(int count)
			: base(count)
		{
		}

		TestCommand ICommandDecorator.Decorate(TestCommand command)
		{
			return new RepeatedTestCommand(command);
		}
	}
}
