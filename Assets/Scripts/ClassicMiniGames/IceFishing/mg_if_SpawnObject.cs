using System;

namespace IceFishing
{
	public class mg_if_SpawnObject
	{
		public int Time
		{
			get;
			private set;
		}

		public mg_if_EObjectType Type
		{
			get;
			private set;
		}

		public mg_if_SpawnObject(string p_time, string p_type)
		{
			Time = Convert.ToInt32(p_time);
			switch (p_type)
			{
			case "OBJ_YELLOWFISH":
			case "OBJ_GREYFISH":
				Type = mg_if_EObjectType.OBJ_YELLOWFISH;
				break;
			case "OBJ_KICKER":
				Type = mg_if_EObjectType.OBJ_KICKER;
				break;
			case "OBJ_JELLYFISH":
				Type = mg_if_EObjectType.OBJ_JELLYFISH;
				break;
			case "OBJ_SHARK":
				Type = mg_if_EObjectType.OBJ_SHARK;
				break;
			case "OBJ_CRAB":
				Type = mg_if_EObjectType.OBJ_CRAB;
				break;
			case "OBJ_WORMCAN":
				Type = mg_if_EObjectType.OBJ_WORMCAN;
				break;
			case "OBJ_PUFFLE":
				Type = mg_if_EObjectType.OBJ_PUFFLE;
				break;
			default:
				Type = mg_if_EObjectType.OBJ_NONE;
				break;
			}
		}
	}
}
