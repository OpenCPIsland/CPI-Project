using System;
using System.Collections.Generic;

[Serializable]
public class AMPVersionResult
{
	public AMPVersionMeta meta;

	public List<AMPResourceData> data = null;
}
