using System.Collections.Generic;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public interface ICacheableContent
	{
		List<Object> InternalReferences();
	}
}
