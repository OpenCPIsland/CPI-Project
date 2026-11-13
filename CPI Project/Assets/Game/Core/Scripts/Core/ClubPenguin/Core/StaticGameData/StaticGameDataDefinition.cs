using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ClubPenguin.Core.StaticGameData
{
	public abstract class StaticGameDataDefinition : ScriptableObject
	{
		[JsonIgnore]
		public string DefinitionPath
		{
			get
			{
				return StaticGameDataUtils.GetDefinitionPath(GetType());
			}
		}

		[JsonIgnore]
		public string ExportPath
		{
			get
			{
				return StaticGameDataUtils.GetExportPath(GetType());
			}
		}

		[JsonIgnore]
		public string ManifestPath
		{
			get
			{
				return StaticGameDataUtils.GetManifestPath(GetType());
			}
		}

		[JsonIgnore]
		public ManifestContentKey ManifestContentKey
		{
			get
			{
				return StaticGameDataUtils.GetManifestContentKey(GetType());
			}
		}

		public static List<T> ToList<T>(StaticGameDataDefinition[] array, FieldInfo idField = null)
		{
			List<T> list = new List<T>();
			if (array != null)
			{
				foreach (object obj in array)
				{
					if (idField == null)
					{
						idField = StaticGameDataDefinitionIdAttribute.GetAttributedField(obj.GetType());
					}
					list.Add((T)idField.GetValue(obj));
				}
			}
			return list;
		}
	}
}
