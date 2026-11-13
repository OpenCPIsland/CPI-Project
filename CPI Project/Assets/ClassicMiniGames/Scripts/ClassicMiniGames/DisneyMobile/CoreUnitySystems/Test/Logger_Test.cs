using DisneyMobile.CoreUnitySystems.Utility;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.Test
{
	[TestFixture]
	public class Logger_Test
	{
		private Logger mPreviousLogger = null;

		private Logger mLogger = null;

		private Configurator mConfigurator = null;

		private string mLowestPriorityConfigFile;

		private string mLogFile;

		private IDictionary<string, object> mDictionary;

		[SetUp]
		public void SetUp()
		{
			mPreviousLogger = Logger.Instance;
			string text = Application.dataPath + "/Core/GameSystems/DisneyMobile.CoreUnitySystems.Logger/UnitTest/";
			mLowestPriorityConfigFile = text + "ApplicationConfig.txt";
			FileHelper.DeleteIfExists(mLowestPriorityConfigFile);
			mConfigurator = new Configurator();
			mConfigurator.SetConfigurationPath(text);
			mConfigurator.Init(true);
			mLogger = new Logger();
			mLogger.DestroyLogFile();
			mDictionary = mConfigurator.GetDictionaryForSystem(mLogger.GetType());
			mLogger.Configure(mDictionary);
			mLogFile = Application.persistentDataPath + (mDictionary["values"].AsDic()["mLogFile"] as string);
		}

		[TearDown]
		public void TearDown()
		{
			mLogger.DestroyLogFile();
			mLogger = null;
			mConfigurator = null;
			FileHelper.DeleteIfExists(mLowestPriorityConfigFile);
			Logger.Instance = mPreviousLogger;
		}

		[Test]
		public void TestCreation()
		{
			Assert.NotNull(mLogger);
		}

		[Test]
		public void TestLogDebug()
		{
			mDictionary["values"].AsDic()["mPrioritiesToLogCSV"] = "DEBUG";
			mLogger.Configure(mDictionary);
			Logger.LogDebug(this, "Unit Test: TestLogDebug");
			Assert.That(FileHelper.CountLinesInFile(mLogFile), Is.EqualTo(1));
		}

		[Test]
		public void TestLogTrace()
		{
			mDictionary["values"].AsDic()["mPrioritiesToLogCSV"] = "Trace";
			mLogger.Configure(mDictionary);
			Logger.LogTrace(this, "Unit Test: TestLogTrace");
			Assert.That(FileHelper.CountLinesInFile(mLogFile), Is.EqualTo(1));
		}

		[Test]
		public void TestLogWarning()
		{
			Logger.LogWarning(this, "Unit Test: TestLogWarning");
			Assert.That(FileHelper.CountLinesInFile(mLogFile), Is.EqualTo(1));
		}

		[Test]
		public void TestLogFatal()
		{
			Logger.LogFatal(this, "Unit Test: TestLogFatal");
			Assert.That(FileHelper.CountLinesInFile(mLogFile), Is.EqualTo(1));
		}

		[Test]
		public void TestLogMultipleFlags()
		{
			Logger.Log(this, Logger.PriorityFlags.DEBUG | Logger.PriorityFlags.WARNING, Logger.TagFlags.NO_TAG, "Unit Test: Test Multiple Flags");
			Assert.That(FileHelper.CountLinesInFile(mLogFile), Is.EqualTo(1));
		}

		[Test]
		public void TestLogMultipleLogsToFile()
		{
			mDictionary["values"].AsDic()["mPrioritiesToLogCSV"] = "ALL";
			mDictionary["values"].AsDic()["mTagsToLogCSV"] = "ALL";
			mDictionary["values"].AsDic()["mTypesToLogCSV"] = "ALL";
			mLogger.Reconfigure(mDictionary);
			Logger.Log(Logger.Instance, Logger.PriorityFlags.DEBUG | Logger.PriorityFlags.WARNING, Logger.TagFlags.NO_TAG, "1) Unit Test: Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.WARNING, Logger.TagFlags.CORE, "2) Unit Test: Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.TRACE, Logger.TagFlags.INIT, "3) Unit Test: Test Multiple Logs to file");
			Assert.That(FileHelper.CountLinesInFile(mLogFile), Is.EqualTo(3));
		}

		[Test]
		public void TestLogMultipleLogsToFile_FilterByPriorities1()
		{
			mDictionary["values"].AsDic()["mPrioritiesToLogCSV"] = "WARNING";
			mDictionary["values"].AsDic()["mTagsToLogCSV"] = "ALL";
			mDictionary["values"].AsDic()["mTypesToLogCSV"] = "ALL";
			mLogger.Reconfigure(mDictionary);
			Logger.Log(Logger.Instance, Logger.PriorityFlags.DEBUG | Logger.PriorityFlags.WARNING, Logger.TagFlags.NO_TAG, "1) Unit Test: Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.WARNING, Logger.TagFlags.CORE, "2) Unit Test: Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.TRACE, Logger.TagFlags.INIT, "3) Unit Test: Test Multiple Logs to file");
			Assert.That(FileHelper.CountLinesInFile(mLogFile), Is.EqualTo(2));
		}

		[Test]
		public void TestLogMultipleLogsToFile_FilterByPriorities2()
		{
			mDictionary["values"].AsDic()["mPrioritiesToLogCSV"] = "TRACE";
			mDictionary["values"].AsDic()["mTagsToLogCSV"] = "ALL";
			mDictionary["values"].AsDic()["mTypesToLogCSV"] = "ALL";
			mLogger.Reconfigure(mDictionary);
			Logger.Log(Logger.Instance, Logger.PriorityFlags.DEBUG | Logger.PriorityFlags.WARNING, Logger.TagFlags.NO_TAG, "1) Unit Test: Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.WARNING, Logger.TagFlags.CORE, "2) Unit Test: Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.TRACE, Logger.TagFlags.INIT, "3) Unit Test: Test Multiple Logs to file");
			Assert.That(FileHelper.CountLinesInFile(mLogFile), Is.EqualTo(1));
		}

		[Test]
		public void TestLogMultipleLogsToFile_FilterByMultiplePriorities1()
		{
			mDictionary["values"].AsDic()["mPrioritiesToLogCSV"] = "DEBUG, TRACE";
			mDictionary["values"].AsDic()["mTagsToLogCSV"] = "ALL";
			mDictionary["values"].AsDic()["mTypesToLogCSV"] = "ALL";
			mLogger.Reconfigure(mDictionary);
			Logger.Log(Logger.Instance, Logger.PriorityFlags.DEBUG | Logger.PriorityFlags.WARNING, Logger.TagFlags.NO_TAG, "1) Unit Test: Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.WARNING, Logger.TagFlags.CORE, "2) Unit Test: Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.TRACE, Logger.TagFlags.INIT, "3) Unit Test: Test Multiple Logs to file");
			Assert.That(FileHelper.CountLinesInFile(mLogFile), Is.EqualTo(2));
		}

		[Test]
		public void TestLogMultipleLogsToFile_FilterByMultiplePriorities2()
		{
			mDictionary["values"].AsDic()["mPrioritiesToLogCSV"] = "WARNING, TRACE";
			mDictionary["values"].AsDic()["mTagsToLogCSV"] = "ALL";
			mDictionary["values"].AsDic()["mTypesToLogCSV"] = "ALL";
			mLogger.Reconfigure(mDictionary);
			Logger.Log(Logger.Instance, Logger.PriorityFlags.DEBUG | Logger.PriorityFlags.WARNING, Logger.TagFlags.NO_TAG, "1) Unit Test: Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.WARNING, Logger.TagFlags.CORE, "2) Unit Test: Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.TRACE, Logger.TagFlags.INIT, "3) Unit Test: Test Multiple Logs to file");
			Assert.That(FileHelper.CountLinesInFile(mLogFile), Is.EqualTo(3));
		}

		[Test]
		public void TestLogMultipleLogsToFile_FilterByMultipleTags1()
		{
			mDictionary["values"].AsDic()["mPrioritiesToLogCSV"] = "ALL";
			mDictionary["values"].AsDic()["mTagsToLogCSV"] = "CORE";
			mDictionary["values"].AsDic()["mTypesToLogCSV"] = "ALL";
			mLogger.Reconfigure(mDictionary);
			Logger.Log(Logger.Instance, Logger.PriorityFlags.DEBUG | Logger.PriorityFlags.WARNING, Logger.TagFlags.NO_TAG, "1) Unit Test: Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.WARNING, Logger.TagFlags.CORE, "2) Unit Test: Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.TRACE, Logger.TagFlags.INIT, "3) Unit Test: Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.TRACE, Logger.TagFlags.CORE, "4) Unit Test: Test Multiple Logs to file");
			Assert.That(FileHelper.CountLinesInFile(mLogFile), Is.EqualTo(2));
		}

		[Test]
		public void TestLogMultipleLogsToFile_FilterByMultipleTags2()
		{
			mDictionary["values"].AsDic()["mPrioritiesToLogCSV"] = "ALL";
			mDictionary["values"].AsDic()["mTagsToLogCSV"] = "CORE, NO_TAG";
			mDictionary["values"].AsDic()["mTypesToLogCSV"] = "ALL";
			mLogger.Reconfigure(mDictionary);
			Logger.Log(Logger.Instance, Logger.PriorityFlags.DEBUG | Logger.PriorityFlags.WARNING, Logger.TagFlags.NO_TAG, "1) Unit Test: Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.WARNING, Logger.TagFlags.CORE, "2) Unit Test: Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.TRACE, Logger.TagFlags.INIT, "3) Unit Test: Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.TRACE, Logger.TagFlags.CORE, "4) Unit Test: Test Multiple Logs to file");
			Assert.That(FileHelper.CountLinesInFile(mLogFile), Is.EqualTo(3));
		}

		[Test]
		public void TestLogMultipleLogsToFile_FilterByMultipleTags3()
		{
			mDictionary["values"].AsDic()["mPrioritiesToLogCSV"] = "ALL";
			mDictionary["values"].AsDic()["mTagsToLogCSV"] = "CORE, NO_TAG, INIT";
			mDictionary["values"].AsDic()["mTypesToLogCSV"] = "ALL";
			mLogger.Reconfigure(mDictionary);
			Logger.Log(Logger.Instance, Logger.PriorityFlags.DEBUG | Logger.PriorityFlags.WARNING, Logger.TagFlags.NO_TAG, "1) Unit Test: Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.WARNING, Logger.TagFlags.CORE, "2) Unit Test: Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.TRACE, Logger.TagFlags.INIT, "3) Unit Test: Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.TRACE, Logger.TagFlags.CORE, "4) Unit Test: Test Multiple Logs to file");
			Assert.That(FileHelper.CountLinesInFile(mLogFile), Is.EqualTo(4));
		}

		[Test]
		public void TestLogMultipleLogsToFile_FilterByMultipleTypes1()
		{
			string value = GetType().ToString();
			mDictionary["values"].AsDic()["mPrioritiesToLogCSV"] = "INFO,WARNING,FATAL";
			mDictionary["values"].AsDic()["mTagsToLogCSV"] = "ALL";
			mDictionary["values"].AsDic()["mTypesToLogCSV"] = value;
			mLogger.Reconfigure(mDictionary);
			Logger.Log(Logger.Instance, Logger.PriorityFlags.DEBUG, Logger.TagFlags.NO_TAG, "1) Unit Test: NOT print Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.WARNING, Logger.TagFlags.CORE, "2) Unit Test: WILL printTest Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.TRACE, Logger.TagFlags.INIT, "3) Unit Test: WILL print Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.TRACE, Logger.TagFlags.CORE, "4) Unit Test: WILL print Test Multiple Logs to file");
			Assert.That(FileHelper.CountLinesInFile(mLogFile), Is.EqualTo(3));
		}

		[Test]
		public void TestLogMultipleLogsToFile_FilterByMultipleTypes2()
		{
			string value = Logger.Instance.GetType().ToString();
			mDictionary["values"].AsDic()["mPrioritiesToLogCSV"] = "INFO,WARNING,FATAL";
			mDictionary["values"].AsDic()["mTagsToLogCSV"] = "ALL";
			mDictionary["values"].AsDic()["mTypesToLogCSV"] = value;
			mLogger.Reconfigure(mDictionary);
			Logger.Log(Logger.Instance, Logger.PriorityFlags.DEBUG | Logger.PriorityFlags.WARNING, Logger.TagFlags.NO_TAG, "1) Unit Test: WILL print Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.DEBUG, Logger.TagFlags.CORE, "2) Unit Test: NOT print Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.DEBUG, Logger.TagFlags.INIT, "3) Unit Test: NOT print Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.DEBUG, Logger.TagFlags.CORE, "4) Unit Test: NOT print Test Multiple Logs to file");
			Assert.That(FileHelper.CountLinesInFile(mLogFile), Is.EqualTo(1));
		}

		[Test]
		public void TestLogMultipleLogsToFile_FilterByMultipleTypes3()
		{
			string value = Logger.Instance.GetType().ToString() + ", " + GetType().ToString();
			mDictionary["values"].AsDic()["mPrioritiesToLogCSV"] = "ALL";
			mDictionary["values"].AsDic()["mTagsToLogCSV"] = "ALL";
			mDictionary["values"].AsDic()["mTypesToLogCSV"] = value;
			mLogger.Reconfigure(mDictionary);
			Logger.Log(Logger.Instance, Logger.PriorityFlags.DEBUG | Logger.PriorityFlags.WARNING, Logger.TagFlags.NO_TAG, "1) Unit Test: Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.WARNING, Logger.TagFlags.CORE, "2) Unit Test: Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.TRACE, Logger.TagFlags.INIT, "3) Unit Test: Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.TRACE, Logger.TagFlags.CORE, "4) Unit Test: Test Multiple Logs to file");
			Assert.That(FileHelper.CountLinesInFile(mLogFile), Is.EqualTo(4));
		}

		[Test]
		public void TestLogMultipleLogsToFile_FilterComplex()
		{
			string value = GetType().ToString();
			mDictionary["values"].AsDic()["mPrioritiesToLogCSV"] = "TRACE, WARNING";
			mDictionary["values"].AsDic()["mTagsToLogCSV"] = "CORE, INIT";
			mDictionary["values"].AsDic()["mTypesToLogCSV"] = value;
			mLogger.Reconfigure(mDictionary);
			Logger.Log(Logger.Instance, Logger.PriorityFlags.DEBUG | Logger.PriorityFlags.WARNING, Logger.TagFlags.NO_TAG, "1) Unit Test: NOT print Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.WARNING, Logger.TagFlags.CORE, "2) Unit Test: WILL print Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.TRACE, Logger.TagFlags.INIT, "3) Unit Test: WILL print Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.INFO, Logger.TagFlags.CORE, "4) Unit Test: WILL print Test Multiple Logs to file");
			Logger.Log(this, Logger.PriorityFlags.TRACE, Logger.TagFlags.INIT, "5) Unit Test: WILL PRINT Test Multiple Logs to file");
			Logger.Log(Logger.Instance, Logger.PriorityFlags.TRACE | Logger.PriorityFlags.DEBUG | Logger.PriorityFlags.WARNING, Logger.TagFlags.CORE, "6) Unit Test: NOT print Test Multiple Logs to file");
			Assert.That(FileHelper.CountLinesInFile(mLogFile), Is.EqualTo(4));
		}

		[Test]
		public void TestLogMultipleLogsToFile_FilterByTypeAndPriorityLevel()
		{
			string value = GetType().ToString() + ":DEBUG";
			mDictionary["values"].AsDic()["mPrioritiesToLogCSV"] = "INFO, WARNING, FATAL";
			mDictionary["values"].AsDic()["mTagsToLogCSV"] = "ALL";
			mDictionary["values"].AsDic()["mTypesToLogCSV"] = value;
			mLogger.Reconfigure(mDictionary);
			Logger.LogTrace(this, "1) Unit Test: TRACE NOT print Test FilterByTypeAndPriorityLevel");
			Logger.LogDebug(this, "2) Unit Test: DEBUG WILL print Test FilterByTypeAndPriorityLevel");
			Logger.LogInfo(this, "3) Unit Test: INFO WILL print Test  FilterByTypeAndPriorityLevel");
			Logger.LogWarning(this, "4) Unit Test: WARNING WILL print Test  FilterByTypeAndPriorityLevel");
			Logger.LogFatal(this, "5) Unit Test: FATAL WILL print Test  FilterByTypeAndPriorityLevel");
			Assert.That(FileHelper.CountLinesInFile(mLogFile), Is.EqualTo(4));
		}

		[Test]
		public void TestTagAndTypeRestrictive()
		{
			string value = GetType().ToString() + ":DEBUG";
			mDictionary["values"].AsDic()["mPrioritiesToLogCSV"] = "INFO, WARNING, FATAL";
			mDictionary["values"].AsDic()["mTagsToLogCSV"] = "CORE";
			mDictionary["values"].AsDic()["mTypesToLogCSV"] = value;
			mLogger.Reconfigure(mDictionary);
			Logger.LogDebug(this, "1) Unit Test: DEBUG NOT print Test TestTagAndTypeRestrictive");
			Logger.LogTrace(this, "2) Unit Test: TRACE NOT print Test  TestTagAndTypeRestrictive", Logger.TagFlags.CORE);
			Logger.LogDebug(this, "3) Unit Test: DEBUG WILL print Test  TestTagAndTypeRestrictive", Logger.TagFlags.CORE);
			Assert.That(FileHelper.CountLinesInFile(mLogFile), Is.EqualTo(1));
		}

		[Test]
		public void TestTagAndTypeExpansive()
		{
			string value = GetType().ToString() + ":DEBUG";
			mDictionary["values"].AsDic()["mPrioritiesToLogCSV"] = "INFO, WARNING, FATAL";
			mDictionary["values"].AsDic()["mTagsToLogCSV"] = "CORE";
			mDictionary["values"].AsDic()["mTypesToLogCSV"] = value;
			mDictionary["values"].AsDic()["mTagAndTypeRestrictive"] = false;
			mLogger.Reconfigure(mDictionary);
			Logger.LogDebug(this, "1) Unit Test: DEBUG WILL print Test TestTagAndTypeExpansive");
			Logger.LogTrace(this, "2) Unit Test: TRACE WILL print Test TestTagAndTypeExpansive", Logger.TagFlags.CORE);
			Logger.LogDebug(this, "3) Unit Test: DEBUG WILL print Test TestTagAndTypeExpansive", Logger.TagFlags.CORE);
			Assert.That(FileHelper.CountLinesInFile(mLogFile), Is.EqualTo(3));
		}
	}
}
