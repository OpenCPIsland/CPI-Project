using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ClubPenguin.Tests
{
	public class TestGuestBuilder
	{
		private const string ClientIdTemplateToken = "{client-id}";

		private const string BasePathTemplate = "/client/{client-id}";

		private const string RegisterPathTemplate = "/client/{client-id}/guest/register";

		private const int MaxRetries = 2;

		private static readonly Regex httpStatusCodeRegex = new Regex("HTTP/\\d\\.\\d (\\d+)");

		private readonly Uri registerUri;

		public TestGuestBuilder(string host, string oneIdClientId)
		{
			registerUri = new Uri(host + "/client/{client-id}/guest/register".Replace("{client-id}", oneIdClientId));
		}

		public IEnumerable CreateAdultAccount(Action<TestGuest> callback)
		{
			string email = generateRandomEmail();
			string dateOfBirth = DateTime.Now.Subtract(TimeSpan.FromDays(36500.0)).ToString("yyyy-MM-dd");
			foreach (object item in createAccount(callback, email, "CpIntTestLastName", null, null, dateOfBirth, 0))
			{
				yield return item;
			}
		}

		public IEnumerable CreateChildAccount(Action<TestGuest> callback)
		{
			string username = Path.GetRandomFileName();
			string parentEmail = generateRandomEmail();
			foreach (object item in createAccount(callback, null, null, username, parentEmail, null, 0))
			{
				yield return item;
			}
		}

		private WWW makeRegisterWww(RegisterRequest args)
		{
			string text = registerUri.AbsoluteUri;
			if (args.profile.username == null)
			{
				text += "?autogenerateUsername=true";
			}
			Dictionary<string, string> headers = createHeaders();
			string s = serializeRequestObject(args);
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			return new WWW(text, bytes, headers);
		}

		private static Dictionary<string, string> createHeaders()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Content-Type", "application/json");
			dictionary.Add("Accept", "application/json");
			return dictionary;
		}

		private static uint getStatusCode(IDictionary<string, string> responseHeaders)
		{
			string value;
			if (responseHeaders.TryGetValue("STATUS", out value))
			{
				Match match = httpStatusCodeRegex.Match(value);
				Group group = match.Groups[1];
				string value2 = group.Value;
				return uint.Parse(value2);
			}
			return 0u;
		}

		private static string serializeRequestObject(object request)
		{
			return JsonMapper.ToJson(request);
		}

		private IEnumerable createAccount(Action<TestGuest> callback, string email, string lastName, string username, string parentEmail, string dateOfBirth, int numRetries)
		{
			RegisterRequest args = new RegisterRequest
			{
				legalAssertions = new List<string>
				{
					"ppV2",
					"GTOU"
				},
				marketing = new List<MarketingItem>
				{
					new MarketingItem
					{
						code = "WDIGFamilySites",
						subscribed = true
					}
				},
				password = "CpIntTestPassword1",
				profile = new RegisterProfile
				{
					dateOfBirth = dateOfBirth,
					email = email,
					username = username,
					parentEmail = parentEmail,
					firstName = "CpIntTestFirstName",
					lastName = lastName,
					testProfileFlag = "y",
					region = null
				}
			};
			WWW www = makeRegisterWww(args);
			while (!www.isDone)
			{
				yield return null;
			}
			uint statusCode = getStatusCode(www.responseHeaders);
			if (statusCode >= 200 && statusCode <= 299)
			{
				string responseText = www.text;
				LogInResponse response = JsonMapper.ToObject<LogInResponse>(responseText);
				if (response.data != null && response.data.profile != null)
				{
					TestGuest obj = new TestGuest(response.data, "CpIntTestPassword1");
					callback(obj);
				}
				else if (numRetries < 2)
				{
					foreach (object item in retryCreateAccount(callback, email, lastName, username, parentEmail, dateOfBirth, numRetries))
					{
						yield return item;
					}
				}
				else
				{
					IntegrationTest.Fail("Got invalid data when creating TestGuest: " + JsonMapper.ToJson(response));
					callback(null);
				}
			}
			else if (numRetries < 2)
			{
				foreach (object item2 in retryCreateAccount(callback, email, lastName, username, parentEmail, dateOfBirth, numRetries))
				{
					yield return item2;
				}
			}
			else
			{
				IntegrationTest.Fail("Error creating TestGuest: " + www.error + "\n" + www.text);
				callback(null);
			}
		}

		private IEnumerable retryCreateAccount(Action<TestGuest> callback, string email, string lastName, string username, string parentEmail, string dateOfBirth, int numRetries)
		{
			if (email != null)
			{
				email = generateRandomEmail();
			}
			if (parentEmail != null)
			{
				parentEmail = generateRandomEmail();
			}
			if (username != null)
			{
				username = Path.GetRandomFileName();
			}
			foreach (object item in createAccount(callback, email, lastName, username, parentEmail, dateOfBirth, numRetries + 1))
			{
				yield return item;
			}
		}

		private static string generateRandomEmail()
		{
			return Path.GetRandomFileName() + "@dispostable.com";
		}
	}
}
