using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace UnityTest
{
	[Serializable]
	public class ResultDTO
	{
		public enum MessageType : byte
		{
			Ping,
			RunStarted,
			RunFinished,
			TestStarted,
			TestFinished,
			RunInterrupted,
			AllScenesFinished
		}

		public MessageType messageType;

		public int levelCount;

		public int loadedLevel;

		public string loadedLevelName;

		public string testName;

		public float testTimeout;

		public ITestResult testResult;

		private ResultDTO(MessageType messageType)
		{
			this.messageType = messageType;
			levelCount = SceneManager.sceneCountInBuildSettings;
			loadedLevel = SceneManager.GetActiveScene().buildIndex;
			loadedLevelName = SceneManager.GetActiveScene().name;
		}

		public static ResultDTO CreatePing()
		{
			return new ResultDTO(MessageType.Ping);
		}

		public static ResultDTO CreateRunStarted()
		{
			return new ResultDTO(MessageType.RunStarted);
		}

		public static ResultDTO CreateRunFinished(List<TestResult> testResults)
		{
			return new ResultDTO(MessageType.RunFinished);
		}

		public static ResultDTO CreateTestStarted(TestResult test)
		{
			ResultDTO resultDTO = new ResultDTO(MessageType.TestStarted);
			resultDTO.testName = test.FullName;
			resultDTO.testTimeout = test.TestComponent.timeout;
			return resultDTO;
		}

		public static ResultDTO CreateTestFinished(TestResult test)
		{
			ResultDTO resultDTO = new ResultDTO(MessageType.TestFinished);
			resultDTO.testName = test.FullName;
			resultDTO.testResult = GetSerializableTestResult(test);
			return resultDTO;
		}

		public static ResultDTO CreateAllScenesFinished()
		{
			return new ResultDTO(MessageType.AllScenesFinished);
		}

		private static ITestResult GetSerializableTestResult(TestResult test)
		{
			SerializableTestResult serializableTestResult = new SerializableTestResult();
			serializableTestResult.resultState = test.ResultState;
			serializableTestResult.message = test.messages;
			serializableTestResult.executed = test.Executed;
			serializableTestResult.name = test.Name;
			serializableTestResult.fullName = test.FullName;
			serializableTestResult.id = test.id;
			serializableTestResult.isSuccess = test.IsSuccess;
			serializableTestResult.duration = test.duration;
			serializableTestResult.stackTrace = test.stacktrace;
			serializableTestResult.isIgnored = test.IsIgnored;
			return serializableTestResult;
		}
	}
}
