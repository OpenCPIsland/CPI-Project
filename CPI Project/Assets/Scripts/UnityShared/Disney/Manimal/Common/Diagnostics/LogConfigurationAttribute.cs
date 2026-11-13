using System;
using System.Reflection;

namespace Disney.Manimal.Common.Diagnostics
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
	public class LogConfigurationAttribute : Attribute
	{
		protected static void ValidateLoggerFactoryType(Type loggerFactoryType)
		{
			if (loggerFactoryType == null)
			{
				throw new ArgumentNullException("loggerFactoryType");
			}
			if (!typeof(ILoggerFactory).IsAssignableFrom(loggerFactoryType))
			{
				throw new ArgumentOutOfRangeException("loggerFactoryType", string.Format("The type {0} does not implement the interface {1}.", loggerFactoryType.AssemblyQualifiedName, typeof(ILoggerFactory).AssemblyQualifiedName));
			}
		}

		protected static void ValidateFormatterType(Type formatterType)
		{
			if (formatterType == null)
			{
				throw new ArgumentNullException("formatterType");
			}
			if (!typeof(ILogEntryFormatter).IsAssignableFrom(formatterType))
			{
				throw new ArgumentOutOfRangeException("formatterType", string.Format("The type {0} does not implement the interface {1}.", formatterType.AssemblyQualifiedName, typeof(ILogEntryFormatter).AssemblyQualifiedName));
			}
		}

		protected static void SetupLogManagerInternal(Type loggerFactoryType, params object[] args)
		{
			try
			{
				ILoggerFactory loggerFactory2 = LogManager.Factory = ((args == null || args.Length == 0) ? ((ILoggerFactory)Activator.CreateInstance(loggerFactoryType)) : ((ILoggerFactory)Activator.CreateInstance(loggerFactoryType, args)));
			}
			catch (Exception innerException)
			{
				throw new Exception(string.Format("Unable to create logger factory of type {0}. Possibly incorrect constructor arguments.", loggerFactoryType.AssemblyQualifiedName), innerException);
			}
		}

		protected LogConfigurationAttribute()
		{
		}

		public LogConfigurationAttribute(Type loggerFactoryType, LogLevel level)
		{
			ValidateLoggerFactoryType(loggerFactoryType);
			object[] args = new object[1]
			{
				level
			};
			SetupLogManagerInternal(loggerFactoryType, args);
		}

		public LogConfigurationAttribute(Type loggerFactoryType, Type formatterType, LogLevel level)
		{
			ValidateLoggerFactoryType(loggerFactoryType);
			ValidateFormatterType(formatterType);
			ILogEntryFormatter logEntryFormatter;
			try
			{
				PropertyInfo property = formatterType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
				logEntryFormatter = (ILogEntryFormatter)property.GetValue(null, null);
			}
			catch (Exception innerException)
			{
				throw new Exception(string.Format("Unable to create log formatter of type {0}. Must implement a public static Instance property getter.", formatterType.AssemblyQualifiedName), innerException);
			}
			object[] args = new object[2]
			{
				logEntryFormatter,
				level
			};
			SetupLogManagerInternal(loggerFactoryType, args);
		}
	}
}
