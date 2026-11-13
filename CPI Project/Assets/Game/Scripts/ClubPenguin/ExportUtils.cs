using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class ExportUtils
	{
		private const int DECIMAL_PLACES = 4;

		public static bool TryConvert(float floatValue, out decimal decimalValue)
		{
			try
			{
				decimalValue = decimal.Round((decimal)floatValue, 4);
			}
			catch
			{
				decimalValue = 0m;
				return false;
			}
			return true;
		}

		public static List<Dictionary<string, object>> GetColliderBounds(Collider[] colliders)
		{
			List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
			foreach (Collider collider in colliders)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				if (collider is BoxCollider)
				{
					BoxCollider boxCollider = collider as BoxCollider;
					dictionary.Add("type", "rotated_box");
					dictionary.Add("min", new DecimalVector3(boxCollider.center - boxCollider.size / 2f));
					dictionary.Add("max", new DecimalVector3(boxCollider.center + boxCollider.size / 2f));
					dictionary.Add("transform", exportMaxtrix4x4(boxCollider.transform.worldToLocalMatrix));
				}
				else if (collider is SphereCollider)
				{
					SphereCollider sphereCollider = collider as SphereCollider;
					dictionary.Add("type", "sphere");
					dictionary.Add("center", new DecimalVector3(sphereCollider.bounds.center));
					dictionary.Add("radius", new DecimalVector3(sphereCollider.transform.lossyScale * sphereCollider.radius));
				}
				else
				{
					if (!(collider is CapsuleCollider))
					{
						throw new NotSupportedException("No server support to export a switch volume with collider of type " + collider.GetType());
					}
					CapsuleCollider capsuleCollider = collider as CapsuleCollider;
					decimal decimalValue;
					TryConvert(capsuleCollider.height, out decimalValue);
					dictionary.Add("type", "rotated_capsule");
					dictionary.Add("center", new DecimalVector3(capsuleCollider.center));
					dictionary.Add("heightAxis", capsuleCollider.direction);
					dictionary.Add("heightScale", decimalValue);
					dictionary.Add("radius", new DecimalVector3(Vector3.one * capsuleCollider.radius));
					dictionary.Add("transform", exportMaxtrix4x4(capsuleCollider.transform.worldToLocalMatrix));
				}
				list.Add(dictionary);
			}
			return list;
		}

		private static decimal[][] exportMaxtrix4x4(Matrix4x4 matrix)
		{
			decimal[][] array = new decimal[4][];
			for (int i = 0; i < 4; i++)
			{
				array[i] = new decimal[4];
				Vector4 row = matrix.GetRow(i);
				for (int j = 0; j < 4; j++)
				{
					decimal decimalValue;
					TryConvert(row[j], out decimalValue);
					array[i][j] = decimalValue;
				}
			}
			return array;
		}
	}
}
