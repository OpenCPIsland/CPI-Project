using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace Disney.LaunchPadFramework
{
    public class Log
    {
        [Flags]
        public enum PriorityFlags
        {
            NONE = 0x0,
            ALL = -1,
            TRACE = 0x1,
            DEBUG = 0x2,
            INFO = 0x4,
            WARNING = 0x8,
            ERROR = 0x10,
            NETWORK_ERROR = 0x20,
            FATAL = 0x40
        }

        protected static Log instance = null;

        private Dictionary<Type, PriorityFlags> typesToLogMap = new Dictionary<Type, PriorityFlags>();

        private Dictionary<Type, PriorityFlags> typesNotToLogMap = new Dictionary<Type, PriorityFlags>();

        private int supressLogging;

        public bool LogAllTypes;

        public PriorityFlags LogAllTypesPriorities = PriorityFlags.ALL;

        public PriorityFlags NullPriorityFlags = PriorityFlags.ALL;

        public bool ExcludeAllTypes;

        public bool ExcludeNullObjects;

        public bool WriteToDeviceConsole;

        public bool WriteToUnityConsole;

        public bool ShowTimeStamp = true;

        public bool ShowPriorityNames = true;

        public bool ShowObjectName = true;

        public bool ShowObjectAsString;

        public string TimeStampFormat = "HH:mm:ss tt";

        private readonly StringBuilder messageBuilder = new StringBuilder();

        public static Log Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Log();
                }
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public event Action<UnityEngine.Object, PriorityFlags, string> LogMessageEvent;

        public Log()
        {
            instance = this;
            WriteToUnityConsole = false;
            WriteToDeviceConsole = true;
        }

        public void SetLogFlags(Type type, PriorityFlags flags)
        {
            typesToLogMap[type] = flags;
        }

        public void SetNotLogFlags(Type type, PriorityFlags flags)
        {
            typesNotToLogMap[type] = flags;
        }

        public static void PushLoggingSupression()
        {
            if (Instance.supressLogging < int.MaxValue)
            {
                Instance.supressLogging++;
            }
        }

        public static void PopLoggingSupression()
        {
            if (Instance.supressLogging > 0)
            {
                Instance.supressLogging--;
            }
        }

        public static void ClearLoggingSupression()
        {
            Instance.supressLogging = 0;
        }

        [Conditional("DO_LOGGING")]
        public static void LogDebug(object objectToLog, string message)
        {
            Instance.logMessage(objectToLog, PriorityFlags.DEBUG, message);
        }

        [Conditional("DO_LOGGING")]
        public static void LogDebugFormatted(object objectToLog, string message, params object[] args)
        {
            Instance.logMessage(objectToLog, PriorityFlags.DEBUG, message, args);
        }

        [Conditional("DO_LOGGING")]
        public static void LogTrace(object objectToLog, string message)
        {
            Instance.logMessage(objectToLog, PriorityFlags.TRACE, message);
        }

        [Conditional("DO_LOGGING")]
        public static void LogTraceFormatted(object objectToLog, string message, params object[] args)
        {
            Instance.logMessage(objectToLog, PriorityFlags.TRACE, message, args);
        }

        [Conditional("DO_LOGGING")]
        public static void LogInfo(object objectToLog, string message)
        {
            Instance.logMessage(objectToLog, PriorityFlags.INFO, message);
        }

        [Conditional("DO_LOGGING")]
        public static void LogInfoFormatted(object objectToLog, string message, params object[] args)
        {
            Instance.logMessage(objectToLog, PriorityFlags.INFO, message, args);
        }

        [Conditional("DO_LOGGING")]
        public static void LogWarning(object objectToLog, string message)
        {
            Instance.logMessage(objectToLog, PriorityFlags.WARNING, message);
        }

        [Conditional("DO_LOGGING")]
        public static void LogWarningFormatted(object objectToLog, string message, params object[] args)
        {
            Instance.logMessage(objectToLog, PriorityFlags.WARNING, message, args);
        }

        public static void LogError(object objectToLog, string message)
        {
            Instance.logMessage(objectToLog, PriorityFlags.ERROR, message);
        }

        public static void LogErrorFormatted(object objectToLog, string message, params object[] args)
        {
            Instance.logMessage(objectToLog, PriorityFlags.ERROR, message, args);
        }

        public static void LogNetworkError(object objectToLog, string message)
        {
            Instance.logMessage(objectToLog, PriorityFlags.NETWORK_ERROR, message);
        }

        public static void LogNetworkErrorFormatted(object objectToLog, string message, params object[] args)
        {
            Instance.logMessage(objectToLog, PriorityFlags.NETWORK_ERROR, message, args);
        }

        public static void LogFatal(object objectToLog, string message)
        {
            Instance.logMessage(objectToLog, PriorityFlags.FATAL, message);
        }

        public static void LogFatalFormatted(object objectToLog, string message, params object[] args)
        {
            Instance.logMessage(objectToLog, PriorityFlags.FATAL, message, args);
        }

        [Conditional("DO_LOGGING")]
        public static void LogRaw(object objectToLog, PriorityFlags priorities, string message)
        {
            Instance.logMessage(objectToLog, priorities, message);
        }

        [Conditional("DO_LOGGING")]
        public static void LogRawFormatted(object objectToLog, PriorityFlags priorities, string message, params object[] args)
        {
            Instance.logMessage(objectToLog, priorities, message, args);
        }

        public static void LogException(object objectToLog, Exception ex)
        {
            if (ex == null) return; // Ensure exception is not null before logging
            Instance.logException(objectToLog, ex);
        }

        private void generateTimeStamp()
        {
            if (ShowTimeStamp)
            {
                messageBuilder.Append('[');
                messageBuilder.Append(DateTime.Now.ToString(TimeStampFormat));
                messageBuilder.Append(']');
            }
        }

        private void generatePriorities(PriorityFlags priorities)
        {
            if (ShowPriorityNames)
            {
                messageBuilder.Append('[');
                messageBuilder.Append(priorities);
                messageBuilder.Append(']');
            }
        }

        private void generateObjectName(object objectToLog)
        {
            if (objectToLog == null || !ShowObjectName)
            {
                return;
            }
            messageBuilder.Append('[');
            Type type = objectToLog as Type;
            if (type != null)
            {
                messageBuilder.Append(type);
            }
            else
            {
                string text = objectToLog as string;
                if (text != null)
                {
                    messageBuilder.Append(text);
                }
                else
                {
                    messageBuilder.Append(objectToLog.GetType());
                }
            }
            messageBuilder.Append(']');
        }

        private void serializeObject(object objectToLog)
        {
            if (objectToLog != null && ShowObjectAsString)
            {
                messageBuilder.Append("\n" + objectToLog);
            }
        }

        public bool ShouldLogMessage(PriorityFlags priorities, object objectToLog)
        {
            if (supressLogging > 0 || ExcludeAllTypes || (!WriteToDeviceConsole && !WriteToUnityConsole))
            {
                return false;
            }
            bool flag = LogAllTypes && (priorities & LogAllTypesPriorities) > PriorityFlags.NONE;
            if (objectToLog != null)
            {
                Type key = (objectToLog as Type) ?? objectToLog.GetType();
                PriorityFlags value;
                if (!typesToLogMap.TryGetValue(key, out value) && LogAllTypes)
                {
                    value = LogAllTypesPriorities;
                }
                flag = (flag || (priorities & value) > PriorityFlags.NONE);
                if (typesNotToLogMap.TryGetValue(key, out value))
                {
                    flag = (flag && (priorities & value) <= PriorityFlags.NONE);
                }
            }
            else
            {
                flag = (!ExcludeNullObjects && (priorities & NullPriorityFlags) > PriorityFlags.NONE);
            }
            return flag;
        }

        private string assembleMessage(object objectToLog, PriorityFlags priorities, string message)
        {
            messageBuilder.Length = 0;
            generateTimeStamp();
            generatePriorities(priorities);
            generateObjectName(objectToLog);
            messageBuilder.Append(' ');
            messageBuilder.Append(message);
            serializeObject(objectToLog);
            return messageBuilder.ToString();
        }

        private void logMessage(object objectToLog, PriorityFlags priorities, string message, params object[] args)
        {
            if (!ShouldLogMessage(priorities, objectToLog))
            {
                return;
            }
            if (args.Length > 0)
            {
                message = string.Format(message, args);
            }
            string text = assembleMessage(objectToLog, priorities, message);
            UnityEngine.Object @object = objectToLog as UnityEngine.Object;
            if (WriteToUnityConsole)
            {
                if ((priorities & (PriorityFlags.ERROR | PriorityFlags.NETWORK_ERROR | PriorityFlags.FATAL)) != 0)
                {
                    UnityEngine.Debug.LogError(text, @object);
                }
                else if ((priorities & PriorityFlags.WARNING) == PriorityFlags.WARNING)
                {
                    UnityEngine.Debug.LogWarning(text, @object);
                }
                else
                {
                    UnityEngine.Debug.Log(text, @object);
                }
            }
            if (WriteToDeviceConsole)
            {
                if ((priorities & (PriorityFlags.ERROR | PriorityFlags.NETWORK_ERROR | PriorityFlags.FATAL)) != 0)
                {
                    UnityEngine.Debug.LogError(text, @object);
                }
                else if ((priorities & PriorityFlags.WARNING) == PriorityFlags.WARNING)
                {
                    UnityEngine.Debug.LogWarning(text, @object);
                }
                else
                {
                    UnityEngine.Debug.Log(text, @object);
                }
            }
            if (this.LogMessageEvent != null)
            {
                this.LogMessageEvent(@object, priorities, text);
            }
        }

        private void logException(object objectToLog, Exception ex)
        {
            if (ex == null) return; // Ensure exception is not null before logging

            string message = string.Format("Exception of type {0} caught:\n{1}", ex.GetType().Name, ex);
            logMessage(objectToLog, PriorityFlags.ERROR, message);
        }
    }
}
