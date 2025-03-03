using LitJson;
using UnityEngine;

public class GeomDataJson
{
	public string ResourceName = "";

	public string ParentName = "";

	public Vector3 Position;

	public Quaternion Rotation;

	public Vector3 Scale;

	public string TextureName = "";

	[JsonIgnore(JsonIgnoreWhen.Serializing | JsonIgnoreWhen.Deserializing)]
	public GameObject Instance;

	public static string GetResourceName(Transform t)
	{
		string result = t.name;
		GeomSerializer component = t.gameObject.GetComponent<GeomSerializer>();
		if (component != null)
		{
			result = component.ResourceName;
		}
		return result;
	}

	public static GeomDataJson GetGeomDataJson(Transform t)
	{
		GeomDataJson geomDataJson = new GeomDataJson();
		geomDataJson.ResourceName = GetResourceName(t);
		if (t.parent != null)
		{
			geomDataJson.ParentName = GetResourceName(t.parent);
		}
		else
		{
			geomDataJson.ParentName = "";
		}
		geomDataJson.Position = t.localPosition;
		geomDataJson.Scale = t.localScale;
		geomDataJson.Rotation = t.localRotation;
		return geomDataJson;
	}

	public void ApplyToGeom(GeomDataJson parent)
	{
		if (parent != null && parent.Instance != null)
		{
			Instance.transform.parent = parent.Instance.transform;
		}
		else
		{
			Instance.transform.parent = null;
		}
		Instance.transform.localPosition = Position;
		Instance.transform.localRotation = Rotation;
		Instance.transform.localScale = Scale;
	}
}
