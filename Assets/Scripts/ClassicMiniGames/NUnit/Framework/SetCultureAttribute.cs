using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using System;
using System.Globalization;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class SetCultureAttribute : PropertyAttribute, ICommandDecorator
	{
		private class SetCultureCommand : DelegatingTestCommand
		{
			public SetCultureCommand(TestCommand command)
				: base(command)
			{
			}

			public override TestResult Execute(TestExecutionContext context)
			{
				string text = (string)base.Test.Properties.Get(PropertyNames.SetCulture);
				if (text != null)
				{
					context.CurrentCulture = new CultureInfo(text);
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

		public SetCultureAttribute(string culture)
			: base(PropertyNames.SetCulture, culture)
		{
		}

		TestCommand ICommandDecorator.Decorate(TestCommand command)
		{
			return new SetCultureCommand(command);
		}
	}
}
