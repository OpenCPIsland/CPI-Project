using UnityEngine;
using UnityEngine.UI.Extensions;

public class TestScript : MonoBehaviour
{
	public string testString = "Hello";

	public GameObject someGameObject;

	public string someGameObject_id;

	public TestClass testClass = new TestClass();

	public TestClass[] testClassArray = new TestClass[2];

	[DontSaveField]
	public Transform TransformThatWontBeSaved;

	public void OnSerialize()
	{
		if (someGameObject != null && (bool)someGameObject.GetComponent<ObjectIdentifier>())
		{
			someGameObject_id = someGameObject.GetComponent<ObjectIdentifier>().id;
		}
		else
		{
			someGameObject_id = null;
		}
		if (testClassArray == null)
		{
			return;
		}
		TestClass[] array = testClassArray;
		foreach (TestClass testClass in array)
		{
			if (testClass.go != null && (bool)testClass.go.GetComponent<ObjectIdentifier>())
			{
				testClass.go_id = testClass.go.GetComponent<ObjectIdentifier>().id;
			}
			else
			{
				testClass.go_id = null;
			}
		}
	}

	public void OnDeserialize()
	{
		ObjectIdentifier[] array = Object.FindObjectsOfType(typeof(ObjectIdentifier)) as ObjectIdentifier[];
		if (!string.IsNullOrEmpty(someGameObject_id))
		{
			ObjectIdentifier[] array2 = array;
			foreach (ObjectIdentifier objectIdentifier in array2)
			{
				if (!string.IsNullOrEmpty(objectIdentifier.id) && objectIdentifier.id == someGameObject_id)
				{
					someGameObject = objectIdentifier.gameObject;
					break;
				}
			}
		}
		if (testClassArray == null)
		{
			return;
		}
		TestClass[] array3 = testClassArray;
		foreach (TestClass testClass in array3)
		{
			if (string.IsNullOrEmpty(testClass.go_id))
			{
				continue;
			}
			ObjectIdentifier[] array4 = array;
			foreach (ObjectIdentifier objectIdentifier2 in array4)
			{
				if (!string.IsNullOrEmpty(objectIdentifier2.id) && objectIdentifier2.id == testClass.go_id)
				{
					testClass.go = objectIdentifier2.gameObject;
					break;
				}
			}
		}
	}
}
