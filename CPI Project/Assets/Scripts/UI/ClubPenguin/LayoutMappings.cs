using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class LayoutMappings : MonoBehaviour
	{
		[Serializable]
		public class LayoutMapping
		{
			public string Name;

			public string LayoutType;

			public LayoutMapping(string name, string layoutType)
			{
				Name = name;
				LayoutType = layoutType;
			}
		}

		public List<LayoutMapping> Layouts;

		public string GetLayoutType(string layoutID)
		{
			foreach (LayoutMapping layout in Layouts)
			{
				if (layout.Name == layoutID)
				{
					return layout.LayoutType;
				}
			}
			return null;
		}

		public void SetLayoutType(string gameObjectName, string layoutType)
		{
			foreach (LayoutMapping layout in Layouts)
			{
				if (layout.Name == gameObjectName)
				{
					layout.LayoutType = layoutType;
					return;
				}
			}
			Layouts.Add(new LayoutMapping(gameObjectName, layoutType));
		}
	}
}
