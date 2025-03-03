using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine.UI.Extensions
{
	public class SaveLoadMenu : MonoBehaviour
	{
		public bool showMenu;

		public bool usePersistentDataPath = true;

		public string savePath;

		public Dictionary<string, GameObject> prefabDictionary;

		private void Start()
		{
			if (usePersistentDataPath)
			{
				savePath = Application.persistentDataPath + "/Saved Games/";
			}
			prefabDictionary = new Dictionary<string, GameObject>();
			GameObject[] array = Resources.LoadAll<GameObject>("");
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				if ((bool)gameObject.GetComponent<ObjectIdentifier>())
				{
					prefabDictionary.Add(gameObject.name, gameObject);
					Debug.Log("Added GameObject to prefabDictionary: " + gameObject.name);
				}
			}
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				showMenu = !showMenu;
			}
			if (Input.GetKeyDown(KeyCode.F5))
			{
				SaveGame();
			}
			if (Input.GetKeyDown(KeyCode.F9))
			{
				LoadGame();
			}
		}

		private void OnGUI()
		{
			if (showMenu)
			{
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Exit to Windows"))
				{
					Application.Quit();
					return;
				}
				if (GUILayout.Button("Save Game"))
				{
					SaveGame();
					return;
				}
				if (GUILayout.Button("Load Game"))
				{
					LoadGame();
					return;
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
		}

		private IEnumerator wait(float time)
		{
			yield return new WaitForSeconds(time);
		}

		public void SaveGame()
		{
			SaveGame("QuickSave");
		}

		public void SaveGame(string saveGameName)
		{
			if (string.IsNullOrEmpty(saveGameName))
			{
				Debug.Log("SaveGameName is null or empty!");
				return;
			}
			SaveLoad.saveGamePath = savePath;
			SaveGame saveGame = new SaveGame();
			saveGame.savegameName = saveGameName;
			List<GameObject> list = new List<GameObject>();
			ObjectIdentifier[] array = Object.FindObjectsOfType(typeof(ObjectIdentifier)) as ObjectIdentifier[];
			ObjectIdentifier[] array2 = array;
			foreach (ObjectIdentifier objectIdentifier in array2)
			{
				if (objectIdentifier.dontSave)
				{
					Debug.Log("GameObject " + objectIdentifier.gameObject.name + " is set to dontSave = true, continuing loop.");
					continue;
				}
				if (string.IsNullOrEmpty(objectIdentifier.id))
				{
					objectIdentifier.SetID();
				}
				list.Add(objectIdentifier.gameObject);
			}
			foreach (GameObject item in list)
			{
				item.SendMessage("OnSerialize", SendMessageOptions.DontRequireReceiver);
			}
			foreach (GameObject item2 in list)
			{
				saveGame.sceneObjects.Add(PackGameObject(item2));
			}
			SaveLoad.Save(saveGame);
		}

		public void LoadGame()
		{
			LoadGame("QuickSave");
		}

		public void LoadGame(string saveGameName)
		{
			ClearScene();
			SaveGame saveGame = SaveLoad.Load(saveGameName);
			if (saveGame == null)
			{
				Debug.Log("saveGameName " + saveGameName + "couldn't be found!");
				return;
			}
			List<GameObject> list = new List<GameObject>();
			foreach (SceneObject sceneObject in saveGame.sceneObjects)
			{
				GameObject gameObject = UnpackGameObject(sceneObject);
				if (gameObject != null)
				{
					list.Add(gameObject);
				}
			}
			foreach (GameObject item in list)
			{
				string idParent = item.GetComponent<ObjectIdentifier>().idParent;
				if (!string.IsNullOrEmpty(idParent))
				{
					foreach (GameObject item2 in list)
					{
						if (item2.GetComponent<ObjectIdentifier>().id == idParent)
						{
							item.transform.parent = item2.transform;
						}
					}
				}
			}
			foreach (GameObject item3 in list)
			{
				item3.SendMessage("OnDeserialize", SendMessageOptions.DontRequireReceiver);
			}
		}

		public void ClearScene()
		{
			object[] array = Object.FindObjectsOfType(typeof(GameObject));
			object[] array2 = array;
			foreach (object obj in array2)
			{
				GameObject gameObject = (GameObject)obj;
				if (gameObject.CompareTag("DontDestroy"))
				{
					Debug.Log("Keeping GameObject in the scene: " + gameObject.name);
				}
				else
				{
					Object.Destroy(gameObject);
				}
			}
		}

		public SceneObject PackGameObject(GameObject go)
		{
			ObjectIdentifier component = go.GetComponent<ObjectIdentifier>();
			SceneObject sceneObject = new SceneObject();
			sceneObject.name = go.name;
			sceneObject.prefabName = component.prefabName;
			sceneObject.id = component.id;
			if (go.transform.parent != null && (bool)go.transform.parent.GetComponent<ObjectIdentifier>())
			{
				sceneObject.idParent = go.transform.parent.GetComponent<ObjectIdentifier>().id;
			}
			else
			{
				sceneObject.idParent = null;
			}
			List<string> list = new List<string>();
			list.Add("UnityEngine.MonoBehaviour");
			List<string> list2 = list;
			List<object> list3 = new List<object>();
			object[] components = go.GetComponents<Component>();
			object[] array = components;
			foreach (object obj in array)
			{
				if (list2.Contains(obj.GetType().BaseType.FullName))
				{
					list3.Add(obj);
				}
			}
			foreach (object item in list3)
			{
				sceneObject.objectComponents.Add(PackComponent(item));
			}
			sceneObject.position = go.transform.position;
			sceneObject.localScale = go.transform.localScale;
			sceneObject.rotation = go.transform.rotation;
			sceneObject.active = go.activeSelf;
			return sceneObject;
		}

		public ObjectComponent PackComponent(object component)
		{
			ObjectComponent objectComponent = new ObjectComponent();
			objectComponent.fields = new Dictionary<string, object>();
			Type type = component.GetType();
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			objectComponent.componentName = type.ToString();
			FieldInfo[] array = fields;
			foreach (FieldInfo fieldInfo in array)
			{
				if (fieldInfo == null || !fieldInfo.FieldType.IsSerializable)
				{
					continue;
				}
				if (TypeSystem.IsEnumerableType(fieldInfo.FieldType) || TypeSystem.IsCollectionType(fieldInfo.FieldType))
				{
					Type elementType = TypeSystem.GetElementType(fieldInfo.FieldType);
					if (!elementType.IsSerializable)
					{
						continue;
					}
				}
				object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(DontSaveField), true);
				bool flag = false;
				object[] array2 = customAttributes;
				for (int j = 0; j < array2.Length; j++)
				{
					Attribute attribute = (Attribute)array2[j];
					if (attribute.GetType() == typeof(DontSaveField))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					objectComponent.fields.Add(fieldInfo.Name, fieldInfo.GetValue(component));
				}
			}
			return objectComponent;
		}

		public GameObject UnpackGameObject(SceneObject sceneObject)
		{
			if (!prefabDictionary.ContainsKey(sceneObject.prefabName))
			{
				Debug.Log("Can't find key " + sceneObject.prefabName + " in SaveLoadMenu.prefabDictionary!");
				return null;
			}
			GameObject go = Object.Instantiate(prefabDictionary[sceneObject.prefabName], sceneObject.position, sceneObject.rotation);
			go.name = sceneObject.name;
			go.transform.localScale = sceneObject.localScale;
			go.SetActive(sceneObject.active);
			if (!go.GetComponent<ObjectIdentifier>())
			{
				go.AddComponent<ObjectIdentifier>();
			}
			ObjectIdentifier component = go.GetComponent<ObjectIdentifier>();
			component.id = sceneObject.id;
			component.idParent = sceneObject.idParent;
			UnpackComponents(ref go, sceneObject);
			ObjectIdentifier[] componentsInChildren = go.GetComponentsInChildren<ObjectIdentifier>();
			ObjectIdentifier[] array = componentsInChildren;
			foreach (ObjectIdentifier objectIdentifier in array)
			{
				if (objectIdentifier.transform != go.transform && string.IsNullOrEmpty(objectIdentifier.id))
				{
					Object.Destroy(objectIdentifier.gameObject);
				}
			}
			return go;
		}

		public void UnpackComponents(ref GameObject go, SceneObject sceneObject)
		{
			foreach (ObjectComponent objectComponent in sceneObject.objectComponents)
			{
				if (!go.GetComponent(objectComponent.componentName))
				{
					Type type = Type.GetType(objectComponent.componentName);
					go.AddComponent(type);
				}
				object component = go.GetComponent(objectComponent.componentName);
				Type type2 = component.GetType();
				foreach (KeyValuePair<string, object> field2 in objectComponent.fields)
				{
					FieldInfo field = type2.GetField(field2.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetField);
					if (field != null)
					{
						object value = field2.Value;
						field.SetValue(component, value);
					}
				}
			}
		}
	}
}
