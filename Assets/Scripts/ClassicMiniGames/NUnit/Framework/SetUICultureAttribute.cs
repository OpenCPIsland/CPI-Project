using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using System;
using System.Globalization;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class SetUICultureAttribute : PropertyAttribute, ICommandDecorator
	{
		private class SetUICultureCommand : DelegatingTestCommand
		{
			public SetUICultureCommand(TestCommand command)
				: base(command)
			{
			}

			public override TestResult Execute(TestExecutionContext context)
			{
				string text = (string)base.Test.Properties.Get(PropertyNames.SetUICulture);
				if (text != null)
				{
					context.CurrentUICulture = new CultureInfo(text);
				}
				return innerCommand.Execute(context);
			}
		}

		CommandStage ICommandDecorator.Stage
		{
			get
			{
				return CommandStage.SetContext;
			}
		}

		int ICommandDecorator.Priority
		{
			get
			{
				return 0;
			}
		}

		public SetUICultureAttribute(string culture)
			: base("SetUICulture", culture)
		{
		}

		TestCommand ICommandDecorator.Decorate(TestCommand command)
		{
			return new SetUICultureCommand(command);
		}
	}
}
