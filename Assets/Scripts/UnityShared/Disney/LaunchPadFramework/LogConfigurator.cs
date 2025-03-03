using Disney.Manimal.Common.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tweaker.Core;
using UnityEngine;

namespace Disney.LaunchPadFramework
{
	public class LogConfigurator : IConfigurable
	{
		public const string LOG_EVERYTHING_ARG = "-log-everything";

		private bool m_logAllTypes = false;

		private Log.PriorityFlags m_logAllTypesPriorities = Log.PriorityFlags.ALL;

		private bool m_excludeAllTypes = false;

		private Log.PriorityFlags m_nullPriorityFlags = Log.PriorityFlags.ALL;

		private bool m_excludeNullObjects = false;

		private ArrayList m_typesToLog = new ArrayList();

		private bool m_writeToDeviceConsole = false;

		private bool m_writeToUnityConsole = false;

		private bool m_showTimeStamp = true;

		private bool m_showPriorityNames = true;

		private bool m_showObjectName = true;

		private bool m_showObjectAsString = false;

		private bool m_showStackTrace = true;

		private string m_timeStampFormat = "HH:mm:ss tt";

		private readonly Log m_logger;

		[Tweakable("Debug.Logger.StackTraceLogType")]
		public static StackTraceLogType StackTraceLogType
		{
			get
			{
				return Application.GetStackTraceLogType(LogType.Log);
			}
			set
			{
				Application.SetStackTraceLogType(LogType.Log, value);
			}
		}

		public LogConfigurator(Log logger)
		{
			m_logger = logger;
		}

		public void Configure(IDictionary<string, object> dictionary)
		{
			AutoConfigurable.AutoConfigureObject(this, dictionary);
			StackTraceLogType stackTraceType = StackTraceLogType.ScriptOnly;
			if (!m_showStackTrace)
			{
				stackTraceType = StackTraceLogType.None;
			}
			Application.SetStackTraceLogType(LogType.Log, stackTraceType);
			m_logger.LogAllTypes = m_logAllTypes;
			m_logger.LogAllTypesPriorities = m_logAllTypesPriorities;
			m_logger.ExcludeAllTypes = m_excludeAllTypes;
			m_logger.NullPriorityFlags = m_nullPriorityFlags;
			m_logger.ExcludeNullObjects = m_excludeNullObjects;
			m_logger.WriteToDeviceConsole = m_writeToDeviceConsole;
			m_logger.WriteToUnityConsole = m_writeToUnityConsole;
			m_logger.ShowTimeStamp = m_showTimeStamp;
			m_logger.ShowPriorityNames = m_showPriorityNames;
			m_logger.ShowObjectName = m_showObjectName;
			m_logger.ShowObjectAsString = m_showObjectAsString;
			m_logger.TimeStampFormat = m_timeStampFormat;
			for (int i = 0; i < m_typesToLog.Count; i++)
			{
				ParseTypesToLog(m_logger, m_typesToLog[i] as string);
			}
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			if (commandLineArgs != null && commandLineArgs.Contains("-log-everything"))
			{
				m_logger.LogAllTypes = true;
				m_logger.LogAllTypesPriorities = Log.PriorityFlags.ALL;
				m_logger.ExcludeAllTypes = false;
				m_logger.ExcludeNullObjects = false;
				m_logger.NullPriorityFlags = Log.PriorityFlags.ALL;
			}
		}

		public static void Setup(Configurator configurator)
		{
			if (configurator.IsSystemEnabled("Logger"))
			{
				IDictionary<string, object> dictionaryForSystem = configurator.GetDictionaryForSystem("Logger");
				LogConfigurator logConfigurator = new LogConfigurator(Log.Instance);
				logConfigurator.Configure(dictionaryForSystem);
			}
			new LogConfigurationAttribute(typeof(LPFMUTLoggerFactory), typeof(LPFMUTLogEntryFormatter), LogLevel.Trace);
		}

		[Invokable("Debug.Logger.PushSupression", Description = "Disable logging by incrementing the supression count.")]
		public static void PushLoggingSupression()
		{
			Log.PushLoggingSupression();
		}

		[Invokable("Debug.Logger.PopSupression", Description = "Decrement the surpression count. If zero, logging will be enabled.")]
		public static void PopLoggingSupression()
		{
			Log.PopLoggingSupression();
		}

		[Invokable("Debug.Logger.ClearSupression", Description = "Ensure logging is enabled by reseting the surpression count.")]
		public static void ClearLoggingSupression()
		{
			Log.ClearLoggingSupression();
		}

		[Invokable("Debug.Logger.SetTypesToLog")]
		public static void ParseTypesToLogInvokable(string typeField = "ALL:WARNING,ERROR,FATAL,INFO,DEBUG,TRACE")
		{
			ParseTypesToLog(Log.Instance, typeField);
		}

		public static void ParseTypesToLog(Log log, string typeField)
		{
			if (!string.IsNullOrEmpty(typeField) && typeField.Trim().Length != 0)
			{
				string[] array = typeField.Split(':');
				string text = array[0].Trim();
				Log.PriorityFlags flags = Log.PriorityFlags.ALL;
				if (array.Length > 1)
				{
					try
					{
						flags = (Log.PriorityFlags)Enum.Parse(typeof(Log.PriorityFlags), array[1]);
					}
					catch (Exception ex)
					{
						Debug.LogWarning(string.Concat(ex, " parsing ", array[1], " for type ", text, " defaulting to ALL"));
					}
				}
				MapTypeAndFlags(log, text, flags);
			}
		}

		private static void MapTypeAndFlags(Log log, string typeName, Log.PriorityFlags flags)
		{
			string text = typeName;
			bool flag = false;
			if (typeName.StartsWith("!"))
			{
				flag = true;
				text = typeName.Substring(1);
			}
			if (text.Equals("ALL", StringComparison.OrdinalIgnoreCase))
			{
				log.LogAllTypes = true;
				log.LogAllTypesPriorities = flags;
				return;
			}
			if (text.Equals("NONE", StringComparison.OrdinalIgnoreCase))
			{
				log.ExcludeAllTypes = true;
				return;
			}
			if (text.Equals("NULL", StringComparison.OrdinalIgnoreCase))
			{
				if (flag)
				{
					log.NullPriorityFlags = flags;
					log.ExcludeNullObjects = true;
				}
				else
				{
					log.NullPriorityFlags = flags;
					log.ExcludeNullObjects = false;
				}
				return;
			}
			Type type = Type.GetType(text);
			if (type == null)
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (Assembly assembly in assemblies)
				{
					type = assembly.GetType(text);
					if (type != null)
					{
						break;
					}
				}
			}
			if (type != null)
			{
				if (flag)
				{
					log.SetNotLogFlags(type, flags);
				}
				else
				{
					log.SetLogFlags(type, flags);
				}
			}
			else
			{
				Debug.LogError("LogConfigurator: Could not find type: " + text);
			}
		}
	}
}
