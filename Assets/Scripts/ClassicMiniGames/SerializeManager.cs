using DisneyMobile.CoreUnitySystems;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SerializeManager : MonoBehaviour
{
	public static string ResPath = "Parts/";

	public static byte[] Serialize(Object obj)
	{
		return null;
	}

	public static byte[] Serialize(GameObject obj)
	{
		GeomSerializer[] componentsInChildren = obj.GetComponentsInChildren<GeomSerializer>();
		GeomListJson geomListJson = new GeomListJson();
		GeomSerializer[] array = componentsInChildren;
		foreach (GeomSerializer geomSerializer in array)
		{
			geomListJson.Geoms.Add(geomSerializer.GetGeomDataJson());
		}
		string text = geomListJson.Serialize();
		Debug.LogError(text);
		return Encoding.ASCII.GetBytes(text);
	}

	public static GameObject CreateGeom(GeomDataJson gd)
	{
		GameObject result = null;
		Object @object = Resources.Load(ResPath + gd.ResourceName);
		if (@object != null)
		{
			DisneyMobile.CoreUnitySystems.Logger.LogWarning(null, "Geom found " + gd.ResourceName);
			GameObject gameObject = @object as GameObject;
			if (gameObject != null)
			{
				result = Object.Instantiate(gameObject);
			}
		}
		else
		{
			DisneyMobile.CoreUnitySystems.Logger.LogWarning(null, "Geom not found " + gd.ResourceName);
		}
		return result;
	}

	public static GameObject Deserialize(byte[] data)
	{
		string @string = Encoding.ASCII.GetString(data);
		GeomListJson geomListJson = GeomListJson.Deserialize(@string);
		List<GameObject> list = new List<GameObject>();
		foreach (GeomDataJson geom in geomListJson.Geoms)
		{
			GameObject gameObject = geom.Instance = CreateGeom(geom);
			if (gameObject != null)
			{
				list.Add(gameObject);
			}
		}
		foreach (GeomDataJson geom2 in geomListJson.Geoms)
		{
			if (geom2.Instance != null && !string.IsNullOrEmpty(geom2.ParentName))
			{
				GeomDataJson geomDataJson = geomListJson.FindByResourceName(geom2.ParentName);
				if (geomDataJson != null)
				{
					geom2.ApplyToGeom(geomDataJson);
				}
			}
		}
		return geomListJson.FindRoot().Instance;
	}
}
