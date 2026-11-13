using NUnit.Framework.Internal.Commands;
using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class MaxTimeAttribute : PropertyAttribute, ICommandDecorator
	{
		CommandStage ICommandDecorator.Stage
		{
			get
			{
				return CommandStage.PreSetUpPostTearDown;
			}
		}

		int ICommandDecorator.Priority
		{
			get
			{
				return 0;
			}
		}

		public MaxTimeAttribute(int milliseconds)
			: base(milliseconds)
		{
		}

		TestCommand ICommandDecorator.Decorate(TestCommand command)
		{
			return new MaxTimeCommand(command);
		}
	}
}
