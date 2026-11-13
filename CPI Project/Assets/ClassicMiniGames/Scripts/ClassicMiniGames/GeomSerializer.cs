using UnityEngine;

public class GeomSerializer : MonoBehaviour
{
	public string ResourceName = "";

	private void Start()
	{
		if (ResourceName == "")
		{
			ResourceName = base.name;
		}
	}

	public GeomDataJson GetGeomDataJson()
	{
		return GeomDataJson.GetGeomDataJson(base.transform);
	}
}
