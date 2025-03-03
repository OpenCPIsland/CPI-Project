using System.Collections;
using UnityEngine;

[AddComponentMenu("Simple Mesh Combine")]
public class SimpleMeshCombine : MonoBehaviour
{
	public GameObject[] combinedGameOjects;

	public GameObject combined;

	public string meshName = "Combined_Meshes";

	public bool _canGenerateLightmapUV;

	public int vCount;

	public bool generateLightmapUV;

	public GameObject copyTarget;

	public bool destroyOldColliders;

	public bool keepStructure = true;

	public void EnableRenderers(bool e)
	{
		for (int i = 0; i < combinedGameOjects.Length && !(combinedGameOjects[i] == null); i++)
		{
			Renderer component = combinedGameOjects[i].GetComponent<Renderer>();
			if (component != null)
			{
				component.enabled = e;
			}
		}
	}

	public MeshFilter[] FindEnabledMeshes()
	{
		MeshFilter[] array = null;
		int num = 0;
		array = base.transform.GetComponentsInChildren<MeshFilter>();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].GetComponent<MeshRenderer>() != null && array[i].GetComponent<MeshRenderer>().enabled)
			{
				num++;
			}
		}
		MeshFilter[] array2 = new MeshFilter[num];
		num = 0;
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j].GetComponent<MeshRenderer>() != null && array[j].GetComponent<MeshRenderer>().enabled)
			{
				array2[num] = array[j];
				num++;
			}
		}
		return array2;
	}

	public void CombineMeshes()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "_Combined Mesh [" + base.name + "]";
		gameObject.gameObject.AddComponent<MeshFilter>();
		gameObject.gameObject.AddComponent<MeshRenderer>();
		MeshFilter[] array = null;
		array = FindEnabledMeshes();
		ArrayList arrayList = new ArrayList();
		ArrayList arrayList2 = new ArrayList();
		combinedGameOjects = new GameObject[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			MeshFilter[] componentsInChildren = array[i].GetComponentsInChildren<MeshFilter>();
			combinedGameOjects[i] = array[i].gameObject;
			MeshFilter[] array2 = componentsInChildren;
			foreach (MeshFilter meshFilter in array2)
			{
				MeshRenderer component = meshFilter.GetComponent<MeshRenderer>();
				array[i].transform.gameObject.GetComponent<Renderer>().enabled = false;
				if (array[i].sharedMesh == null)
				{
					Debug.LogWarning(string.Concat("SimpleMeshCombine : ", meshFilter.gameObject, " [Mesh Filter] has no [Mesh], mesh will not be included in combine.."));
					break;
				}
				for (int k = 0; k < meshFilter.sharedMesh.subMeshCount; k++)
				{
					if (component == null)
					{
						Debug.LogWarning(string.Concat("SimpleMeshCombine : ", meshFilter.gameObject, "has a [Mesh Filter] but no [Mesh Renderer], mesh will not be included in combine."));
						break;
					}
					if (k < component.sharedMaterials.Length && k < meshFilter.sharedMesh.subMeshCount)
					{
						int num = Contains(arrayList, component.sharedMaterials[k]);
						if (num == -1)
						{
							arrayList.Add(component.sharedMaterials[k]);
							num = arrayList.Count - 1;
						}
						arrayList2.Add(new ArrayList());
						CombineInstance combineInstance = default(CombineInstance);
						combineInstance.transform = component.transform.localToWorldMatrix;
						combineInstance.subMeshIndex = k;
						combineInstance.mesh = meshFilter.sharedMesh;
						(arrayList2[num] as ArrayList).Add(combineInstance);
					}
				}
			}
		}
		Mesh[] array3 = new Mesh[arrayList.Count];
		CombineInstance[] array4 = new CombineInstance[arrayList.Count];
		for (int l = 0; l < arrayList.Count; l++)
		{
			CombineInstance[] combine = (arrayList2[l] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
			array3[l] = new Mesh();
			array3[l].CombineMeshes(combine, true, true);
			array4[l] = default(CombineInstance);
			array4[l].mesh = array3[l];
			array4[l].subMeshIndex = 0;
		}
		gameObject.GetComponent<MeshFilter>().sharedMesh = new Mesh();
		gameObject.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(array4, false, false);
		Mesh[] array5 = array3;
		foreach (Mesh mesh in array5)
		{
			mesh.Clear();
			Object.DestroyImmediate(mesh);
		}
		MeshRenderer meshRenderer = gameObject.GetComponent<MeshFilter>().GetComponent<MeshRenderer>();
		if (meshRenderer == null)
		{
			meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
		}
		Material[] array7 = meshRenderer.materials = (arrayList.ToArray(typeof(Material)) as Material[]);
		combined = gameObject.gameObject;
		EnableRenderers(false);
		gameObject.transform.parent = base.transform;
		vCount = gameObject.GetComponent<MeshFilter>().sharedMesh.vertexCount;
		if (vCount > 65536)
		{
			Debug.LogWarning("Vertex Count: " + vCount + "- Vertex Count too high, please divide mesh combine into more groups. Max 65536 for each mesh");
			_canGenerateLightmapUV = false;
		}
		else
		{
			_canGenerateLightmapUV = true;
		}
	}

	public int Contains(ArrayList l, Material n)
	{
		for (int i = 0; i < l.Count; i++)
		{
			if (l[i] as Material == n)
			{
				return i;
			}
		}
		return -1;
	}
}
