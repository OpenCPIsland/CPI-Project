using DisneyMobile.CoreUnitySystems;
using System.Collections.Generic;

public class GeomListJson
{
	public List<GeomDataJson> Geoms = new List<GeomDataJson>();

	public string Serialize()
	{
		return JSONManager.serializeToJson(this, true);
	}

	public static GeomListJson Deserialize(string jsonstring)
	{
		return JSONManager.getTemplatedTypeForJson<GeomListJson>(jsonstring);
	}

	public GeomDataJson FindByResourceName(string rname)
	{
		foreach (GeomDataJson geom in Geoms)
		{
			if (geom.ResourceName == rname)
			{
				return geom;
			}
		}
		Logger.LogWarning(this, "Parent not found " + rname);
		return null;
	}

	public GeomDataJson FindRoot()
	{
		foreach (GeomDataJson geom in Geoms)
		{
			if (string.IsNullOrEmpty(geom.ParentName))
			{
				return geom;
			}
		}
		return null;
	}
}
