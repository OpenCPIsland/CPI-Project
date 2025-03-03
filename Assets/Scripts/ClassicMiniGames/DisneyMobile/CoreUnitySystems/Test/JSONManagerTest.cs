using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace DisneyMobile.CoreUnitySystems.Test
{
	[TestFixture]
	public class JSONManagerTest
	{
		public JSONManagerTest()
		{
			JSONErrorEventHandler value = delegate
			{
				_handleJSONError();
			};
			JSONManager.ErrorEvent += value;
		}

		[Test]
		public void FacadeSimpleDictionaryTest()
		{
			string json = "\r\n                {\r\n                    \"test1\" : 5,\r\n                    \"test2\" : 10,\r\n                    \"test3\" : \"asdf\"\r\n                }";
			IDictionary dictionaryForJson = JSONManager.getDictionaryForJson(json);
			Assert.IsTrue(dictionaryForJson.Count == 3);
			Assert.AreEqual((int)dictionaryForJson["test1"], 5);
			Assert.AreEqual((int)dictionaryForJson["test2"], 10);
			Assert.AreEqual((string)dictionaryForJson["test3"], "asdf");
		}

		[Test]
		public void FacadeSimpleListTest()
		{
			string json = "[\r\n                \"Adam\",\r\n                \"Danny\",\r\n                \"James\",\r\n                \"Justin\"\r\n            ]";
			IList listForJson = JSONManager.getListForJson(json);
			Assert.IsTrue(listForJson.Count == 4);
			Assert.AreEqual((string)listForJson[0], "Adam");
			Assert.AreEqual((string)listForJson[1], "Danny");
			Assert.AreEqual((string)listForJson[2], "James");
			Assert.AreEqual((string)listForJson[3], "Justin");
		}

		[Test]
		public void FacadeNestedObjectTest()
		{
			string json = "\r\n                {\r\n                    \"testKey1\":\r\n                    {\r\n                    \"testA1\" : 4,\r\n                    \"testA2\" : 6,\r\n                    \"testA3\" : \"qwerty\"\r\n                    },\r\n\r\n                    \"testKey2\":\r\n                    {\r\n                    \"testB1\" : true,\r\n                    \"testB2\" : false,\r\n                    \"testB3\" : null\r\n                    }\r\n                }";
			IDictionary dictionaryForJson = JSONManager.getDictionaryForJson(json);
			Assert.IsTrue(dictionaryForJson != null && dictionaryForJson.Count == 2);
			IDictionary dictionary = (IDictionary)dictionaryForJson["testKey1"];
			Assert.IsTrue(dictionary != null && dictionary.Count == 3);
			Assert.AreEqual(dictionary["testA1"], 4);
			Assert.AreEqual(dictionary["testA2"], 6);
			Assert.AreEqual(dictionary["testA3"], "qwerty");
			IDictionary dictionary2 = (IDictionary)dictionaryForJson["testKey2"];
			Assert.IsTrue(dictionary2 != null && dictionary2.Count == 3);
			Assert.AreEqual(dictionary2["testB1"], true);
			Assert.AreEqual(dictionary2["testB2"], false);
			Assert.AreEqual(dictionary2["testB3"], null);
		}

		[Test]
		public void FacadeNestedArrayTest()
		{
			string json = "\r\n                [\r\n                    [\r\n                        \"testA1\",\r\n                        \"testA2\",\r\n                        \"testA3\"\r\n                    ],\r\n                    [\r\n                        \"testB1\",\r\n                        \"testB2\",\r\n                        \"testB3\"\r\n                    ]\r\n                ]";
			IList listForJson = JSONManager.getListForJson(json);
			Assert.IsTrue(listForJson != null && listForJson.Count == 2);
			IList list = (IList)listForJson[0];
			Assert.IsTrue(list != null && list.Count == 3);
			Assert.AreEqual(list[0], "testA1");
			Assert.AreEqual(list[1], "testA2");
			Assert.AreEqual(list[2], "testA3");
			IList list2 = (IList)listForJson[1];
			Assert.IsTrue(list != null && list2.Count == 3);
			Assert.AreEqual(list2[0], "testB1");
			Assert.AreEqual(list2[1], "testB2");
			Assert.AreEqual(list2[2], "testB3");
		}

		[Test]
		public void FacadeMixedNestedTest()
		{
			string json = "\r\n                {\r\n                    \"testKey1\":\r\n                    [\r\n                        {\r\n                            \"testB1\" : 6,\r\n                            \"testB2\" : \"hi\"\r\n                        },\r\n                        \"testA2\",\r\n                        \"testA3\"\r\n                    ],\r\n\r\n                    \"testKey2\":\r\n                    {\r\n                    \"testB1\" : true,\r\n                    \"testB2\" : false,\r\n                    \"testB3\" : null\r\n                    }\r\n                }";
			IDictionary dictionaryForJson = JSONManager.getDictionaryForJson(json);
			Assert.IsTrue(dictionaryForJson != null && dictionaryForJson.Count == 2);
			IList list = (IList)dictionaryForJson["testKey1"];
			Assert.IsTrue(list != null && list.Count == 3);
			IDictionary dictionary = (IDictionary)list[0];
			Assert.IsTrue(dictionary != null && dictionary.Count == 2);
			Assert.AreEqual(dictionary["testB1"], 6);
			Assert.AreEqual(dictionary["testB2"], "hi");
		}

		[Test]
		public void FacadeSerializeToJsonTest()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			List<object> list = new List<object>();
			dictionary2.Add("testB1", 7);
			dictionary2.Add("testB2", "dataB2");
			list.Add(5);
			list.Add("testC");
			dictionary.Add("testA1", dictionary2);
			dictionary.Add("testA2", list);
			string expected = JSONManager.serializeToJson(dictionary);
			string actual = "{\"testA1\":{\"testB1\":7,\"testB2\":\"dataB2\"},\"testA2\":[5,\"testC\"]}";
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void FacadeSerializeWithPrintableFormattingTest()
		{
			IDictionary dictionary = new Dictionary<string, object>();
			dictionary["rolling"] = "stones";
			dictionary["flaming"] = "pie";
			dictionary["nine"] = 9;
			string actual = "\r\n{\r\n    \"rolling\" : \"stones\",\r\n    \"flaming\" : \"pie\",\r\n    \"nine\"    : 9\r\n}";
			string expected = JSONManager.serializeToJson(dictionary, true);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void FacadeDeserializeToTemplatedObject()
		{
			string json = "\r\n                {\r\n                  \"title\"  : \"First\",\r\n                  \"name\"   : \"First Window\",\r\n                  \"width\"  : 640,\r\n                  \"height\" : 480\r\n                }";
			UiWindow uiWindow = null;
			uiWindow = JSONManager.getTemplatedTypeForJson<UiWindow>(json);
			Assert.IsNotNull(uiWindow);
			Assert.AreEqual(uiWindow.title, "First");
			Assert.AreEqual(uiWindow.name, "First Window");
			Assert.AreEqual(uiWindow.width, 640);
			Assert.AreEqual(uiWindow.height, 480);
		}

		public void _handleJSONError()
		{
			Assert.IsTrue(true);
		}
	}
}
