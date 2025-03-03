using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityTest.IntegrationTestRunner;

namespace UnityTest
{
	public class NetworkResultSender : ITestRunnerCallback
	{
		private readonly TimeSpan m_ConnectionTimeout = TimeSpan.FromSeconds(5.0);

		private readonly string m_Ip;

		private readonly int m_Port;

		private bool m_LostConnection;

		public NetworkResultSender(string ip, int port)
		{
			m_Ip = ip;
			m_Port = port;
		}

		private bool SendDTO(ResultDTO dto)
		{
			if (m_LostConnection)
			{
				return false;
			}
			try
			{
				using (TcpClient tcpClient = new TcpClient())
				{
					IAsyncResult asyncResult = tcpClient.BeginConnect(m_Ip, m_Port, null, null);
					if (!asyncResult.AsyncWaitHandle.WaitOne(m_ConnectionTimeout))
					{
						return false;
					}
					try
					{
						tcpClient.EndConnect(asyncResult);
					}
					catch (SocketException)
					{
						m_LostConnection = true;
						return false;
					}
					DTOFormatter dTOFormatter = new DTOFormatter();
					dTOFormatter.Serialize(tcpClient.GetStream(), dto);
					tcpClient.GetStream().Close();
					tcpClient.Close();
					Debug.Log("Sent " + dto.messageType);
				}
			}
			catch (SocketException exception)
			{
				Debug.LogException(exception);
				m_LostConnection = true;
				return false;
			}
			return true;
		}

		public bool Ping()
		{
			bool result = SendDTO(ResultDTO.CreatePing());
			m_LostConnection = false;
			return result;
		}

		public void RunStarted(string platform, List<TestComponent> testsToRun)
		{
			SendDTO(ResultDTO.CreateRunStarted());
		}

		public void RunFinished(List<TestResult> testResults)
		{
			SendDTO(ResultDTO.CreateRunFinished(testResults));
		}

		public void TestStarted(TestResult test)
		{
			SendDTO(ResultDTO.CreateTestStarted(test));
		}

		public void TestFinished(TestResult test)
		{
			SendDTO(ResultDTO.CreateTestFinished(test));
		}

		public void AllScenesFinished()
		{
			SendDTO(ResultDTO.CreateAllScenesFinished());
		}

		public void TestRunInterrupted(List<ITestComponent> testsNotRun)
		{
			RunFinished(new List<TestResult>());
		}
	}
}
