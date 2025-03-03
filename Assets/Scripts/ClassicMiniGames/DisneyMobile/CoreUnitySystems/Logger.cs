using DisneyMobile.CoreUnitySystems.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems
{
	public class Logger : AutoConfigurable
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
			FATAL = 0x10
		}

		[Flags]
		public enum TagFlags
		{
			ALL = -1,
			NO_TAG = 0x1,
			CORE = 0x2,
			GAME = 0x4,
			INIT = 0x8,
			FLOW = 0x10,
			ASSET = 0x20,
			PROFILING = 0x40,
			AUDIO = 0x80,
			MEMORY = 0x100
		}

		private static readonly string mAllTypes = "ALL";

		private PriorityFlags mPrioritiesToLog = PriorityFlags.NONE;

		private TagFlags mTagsToLog = TagFlags.ALL;

		private Dictionary<Type, PriorityFlags> mTypesToLogMap = new Dictionary<Type, PriorityFlags>();

		private static Logger m_instance = null;

		private bool mLogAllTypes = false;

		private int mSuppressLogging = 0;

		private Queue<KeyValuePair<PriorityFlags, string>> mLoggerQueue = new Queue<KeyValuePair<PriorityFlags, string>>();

		private string mPrioritiesToLogCSV = PriorityFlags.ALL.ToString();

		private string mTagsToLogCSV = TagFlags.ALL.ToString();

		private string mTypesToLogCSV = mAllTypes;

		private ArrayList mTypesToLog = new ArrayList();

		private bool mWriteToDeviceConsole = false;

		private bool mWriteToUnityConsole = false;

		private bool mWriteToFile = false;

		private bool mFlushAfterEachLog = true;

		private bool mShowTimeStamp = true;

		private bool mShowPriorityNames = true;

		private bool mShowTagNames = true;

		private bool mShowObjectName = true;

		private bool mShowObjectAsString = false;

		private string mLogFile = "/Logs/Log.txt";

		private bool mTagAndTypeRestrictive = true;

		private string mTimeStampFormat = "{0:HH:mm:ss tt}";

		public static Logger Instance
		{
			get
			{
				if (m_instance == null)
				{
					Instantiate();
				}
				return m_instance;
			}
			set
			{
				m_instance = value;
			}
		}

		public Logger()
		{
			m_instance = this;
			if (Application.isEditor)
			{
				mWriteToUnityConsole = true;
			}
		}

		~Logger()
		{
		}

		public override void Configure(IDictionary<string, object> dictionary)
		{
			base.Configure(dictionary);
			mPrioritiesToLog = (PriorityFlags)Enum.Parse(typeof(PriorityFlags), mPrioritiesToLogCSV, true);
			mTagsToLog = (TagFlags)Enum.Parse(typeof(TagFlags), mTagsToLogCSV, true);
			if (mTypesToLog.Count > 0)
			{
				mLogAllTypes = false;
				for (int i = 0; i < mTypesToLog.Count; i++)
				{
					ParseTypesToLog(mTypesToLog[i] as string);
				}
			}
			else
			{
				ParseTypesToLog(mTypesToLogCSV);
			}
			CreateLogFilesDirectory(mLogFile);
		}

		public override void Reconfigure(IDictionary<string, object> dictionary)
		{
			mTypesToLogMap.Clear();
			mLogAllTypes = false;
			Configure(dictionary);
		}

		public void DestroyLogFile()
		{
			if (File.Exists(Application.persistentDataPath + mLogFile))
			{
				File.Delete(Application.persistentDataPath + mLogFile);
			}
		}

		public static void LogDebug(object objectToLog, string message, TagFlags tags = TagFlags.NO_TAG)
		{
			Instance.LogMessage(objectToLog, PriorityFlags.DEBUG, tags, message);
		}

		public static void LogTrace(object objectToLog, string message, TagFlags tags = TagFlags.NO_TAG)
		{
			Instance.LogMessage(objectToLog, PriorityFlags.TRACE, tags, message);
		}

		public static void LogInfo(object objectToLog, string message, TagFlags tags = TagFlags.NO_TAG)
		{
			Instance.LogMessage(objectToLog, PriorityFlags.INFO, tags, message);
		}

		public static void LogWarning(object objectToLog, string message, TagFlags tags = TagFlags.NO_TAG)
		{
			Instance.LogMessage(objectToLog, PriorityFlags.WARNING, tags, message);
		}

		public static void LogFatal(object objectToLog, string message, TagFlags tags = TagFlags.NO_TAG)
		{
			Instance.LogMessage(objectToLog, PriorityFlags.FATAL, tags, message);
		}

		public static void Log(object objectToLog, PriorityFlags priorities, TagFlags tags, string message)
		{
			Instance.LogMessage(objectToLog, priorities, tags, message);
		}

		private static void Instantiate()
		{
			Configurator configurator = new Configurator();
			configurator.Init(false);
			if (configurator.IsSystemEnabled(typeof(Logger)))
			{
				IDictionary<string, object> dictionaryForSystem = configurator.GetDictionaryForSystem(typeof(Logger));
				m_instance = new Logger();
				m_instance.Configure(dictionaryForSystem);
			}
			else
			{
				m_instance = new Logger();
			}
		}

		private void CreateLogFilesDirectory(string filepath)
		{
			int num = filepath.LastIndexOf("/");
			if (num > -1)
			{
				string str = filepath.Substring(0, num);
				Directory.CreateDirectory(Application.persistentDataPath + str);
			}
			else
			{
				Debug.Log("[Logging directory couldn't be created]");
			}
		}

		private string GenerateTimeStamp()
		{
			if (mShowTimeStamp)
			{
				StringBuilder stringBuilder = new StringBuilder("[");
				stringBuilder.AppendFormat(mTimeStampFormat, DateTime.Now);
				stringBuilder.Append("]");
				return stringBuilder.ToString();
			}
			return "";
		}

		private string GeneratePriorities(PriorityFlags priorities)
		{
			if (mShowPriorityNames)
			{
				return "[" + priorities.ToString() + "]";
			}
			return "";
		}

		private string GenerateTagNames(TagFlags tags)
		{
			if (mShowTagNames)
			{
				return "[" + tags.ToString() + "]";
			}
			return "";
		}

		private string GenerateObjectName(object objectToLog)
		{
			if (objectToLog != null && mShowObjectName)
			{
				if (objectToLog is Type)
				{
					return "[" + objectToLog.ToString() + "]";
				}
				if (objectToLog is string)
				{
					return "[" + (objectToLog as string) + "]";
				}
				return "[" + objectToLog.GetType().ToString() + "]";
			}
			return "";
		}

		private string SerializeObject(object objectToLog)
		{
			if (objectToLog != null && mShowObjectAsString)
			{
				return objectToLog.ToString();
			}
			return "";
		}

		private bool ShouldLogMessage(PriorityFlags priorities, TagFlags tags, object objectToLog)
		{
			if (!mWriteToDeviceConsole && !mWriteToFile && !mWriteToUnityConsole)
			{
				return false;
			}
			bool flag = (priorities & mPrioritiesToLog) > PriorityFlags.NONE;
			bool flag2 = (tags & mTagsToLog) > (TagFlags)0;
			bool flag3 = mLogAllTypes;
			if (!flag3)
			{
				Type type = null;
				if (objectToLog is string)
				{
					type = Type.GetType(objectToLog as string);
				}
				else if (objectToLog is Type)
				{
					type = (objectToLog as Type);
				}
				else if (objectToLog != null)
				{
					type = objectToLog.GetType();
				}
				if (type != null)
				{
					PriorityFlags value;
					if (mTypesToLogMap.TryGetValue(type, out value))
					{
						switch (value)
						{
						case PriorityFlags.NONE:
							flag3 = false;
							break;
						case PriorityFlags.ALL:
							flag3 = true;
							break;
						default:
							flag3 = (value <= priorities);
							break;
						}
					}
				}
				else
				{
					flag3 = false;
				}
			}
			bool result = false;
			if (mLogAllTypes)
			{
				result = ((flag2 && flag3 && flag) ? true : false);
			}
			else if (mTagsToLog != TagFlags.ALL)
			{
				result = ((!mTagAndTypeRestrictive) ? (flag3 || flag2) : (flag3 && flag2));
			}
			else if (flag3)
			{
				result = true;
			}
			else if (flag2)
			{
				result = (flag ? true : false);
			}
			else if (flag)
			{
				result = true;
			}
			return result;
		}

		private string FormatMessage(object objectToLog, PriorityFlags priorities, TagFlags tags, string message)
		{
			string text = GenerateTimeStamp();
			string text2 = GeneratePriorities(priorities);
			string text3 = GenerateTagNames(tags);
			string text4 = GenerateObjectName(objectToLog);
			string text5 = SerializeObject(objectToLog);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0}{1}{2}{3} {4} {5}", text, text3, text2, text4, message, text5);
			return stringBuilder.ToString();
		}

		private void LogMessage(object objectToLog, PriorityFlags priorities, TagFlags tags, string message)
		{
			if (mSuppressLogging == 0 && ShouldLogMessage(priorities, tags, objectToLog))
			{
				string value = FormatMessage(objectToLog, priorities, tags, message);
				mLoggerQueue.Enqueue(new KeyValuePair<PriorityFlags, string>(priorities, value));
				if (mFlushAfterEachLog)
				{
					FlushLog();
				}
			}
		}

		private void FlushLogToFile()
		{
			string path = Application.persistentDataPath + mLogFile;
			using (StreamWriter streamWriter = new StreamWriter(path, true))
			{
				foreach (KeyValuePair<PriorityFlags, string> item in mLoggerQueue)
				{
					streamWriter.Write(item.Value + Environment.NewLine);
				}
			}
		}

		private void FlushLogToDeviceConsole()
		{
			foreach (KeyValuePair<PriorityFlags, string> item in mLoggerQueue)
			{
				consoleLog(item.Value + item);
			}
		}

		private void FlushLogToUnityConsole()
		{
			foreach (KeyValuePair<PriorityFlags, string> item in mLoggerQueue)
			{
				if ((item.Key & PriorityFlags.FATAL) == PriorityFlags.FATAL)
				{
					Debug.LogError(item.Value);
				}
				else if ((item.Key & PriorityFlags.WARNING) == PriorityFlags.WARNING)
				{
					Debug.LogWarning(item.Value);
				}
				else
				{
					Debug.Log(item.Value);
				}
			}
		}

		public void FlushLog()
		{
			if (mWriteToFile)
			{
				FlushLogToFile();
			}
			if (mWriteToDeviceConsole)
			{
				FlushLogToDeviceConsole();
			}
			if (mWriteToUnityConsole)
			{
				FlushLogToUnityConsole();
			}
			mLoggerQueue.Clear();
		}

		public static void PushLoggingSupression()
		{
			Instance.mSuppressLogging++;
			Instance.mSuppressLogging.Clamp(0, int.MaxValue);
		}

		public static void PopLoggingSupression()
		{
			Instance.mSuppressLogging--;
			Instance.mSuppressLogging.Clamp(0, int.MaxValue);
		}

		private void ParseTypesToLog(string csvList)
		{
			if (string.IsNullOrEmpty(csvList) || csvList.Trim().Length == 0)
			{
				return;
			}
			string[] array = csvList.Split(',');
			string[] array2 = array;
			int num = 0;
			while (true)
			{
				if (num >= array2.Length)
				{
					return;
				}
				string text = array2[num];
				string[] array3 = text.Split(':');
				string text2 = array3[0].Trim();
				Type type = Type.GetType(text2);
				if (string.Equals(text2, mAllTypes, StringComparison.OrdinalIgnoreCase))
				{
					mLogAllTypes = true;
				}
				else
				{
					if (type == null)
					{
						break;
					}
					if (array3.Length > 1)
					{
						try
						{
							PriorityFlags value = (PriorityFlags)Enum.Parse(typeof(PriorityFlags), array3[1]);
							mTypesToLogMap[type] = value;
						}
						catch (Exception ex)
						{
							Debug.LogWarning(string.Concat(ex, " parsing ", array3[1], " for type ", text2, " defaulting to ALL"));
							mTypesToLogMap[type] = PriorityFlags.ALL;
						}
					}
					else
					{
						mTypesToLogMap[type] = PriorityFlags.ALL;
					}
				}
				num++;
			}
			Debug.LogError("Attempting to do per type logging on a type that does not exist. Verify the namespace and type name!");
			throw new InvalidCastException();
		}

		private static void consoleLog(string text)
		{
		}
	}
}
