using Disney.LaunchPadFramework;
using Foundation.Unity;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class GpuSkinnedRenderer : MonoBehaviour
	{
		private struct BindPose
		{
			public Quaternion q;

			public Vector3 p;

			public BindPose(Quaternion _q, Vector3 _p)
			{
				q = _q;
				p = _p;
			}
		}

		private const int MAX_BONES = 48;

		private Transform[] m_Bones;

		private BindPose[] m_BindPose;

		private Vector4[] m_BonesQuat;

		private Vector4[] m_BonesPos;

		private int m_QuatUniform;

		private int m_PosUniform;

		private Material m_Material;

		public void init(Transform[] bones, bool calculateWeights = false)
		{
			m_Bones = bones;
			setMaterial();
			m_QuatUniform = Shader.PropertyToID("bonequat");
			m_PosUniform = Shader.PropertyToID("bonepos");
			Mesh sharedMesh = GetComponent<MeshFilter>().sharedMesh;
			computeBindPoses(sharedMesh);
			if (calculateWeights)
			{
				storeWeightsInTangents(sharedMesh);
			}
		}

		private void setMaterial()
		{
			MeshRenderer component = GetComponent<MeshRenderer>();
			component.material = component.material;
			m_Material = component.material;
		}

		public void OnDestroy()
		{
			if (m_Material != null)
			{
				ComponentExtensions.DestroyIfInstance(m_Material);
				m_Material = null;
			}
		}

		public void LateUpdate()
		{
			if (m_Bones != null)
			{
				int num = m_Bones.Length;
				for (int i = 0; i < num; i++)
				{
					Transform transform = m_Bones[i];
					Quaternion rotation = transform.rotation;
					Vector3 position = transform.position;
					Quaternion quaternion = rotation * m_BindPose[i].q;
					Vector3 vector = rotation * m_BindPose[i].p + position;
					m_BonesQuat[i] = new Vector4(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
					m_BonesPos[i] = new Vector4(vector.x, vector.y, vector.z, 1f);
				}
				if (m_Material == null)
				{
					setMaterial();
				}
				m_Material.SetVectorArray(m_QuatUniform, m_BonesQuat);
				m_Material.SetVectorArray(m_PosUniform, m_BonesPos);
			}
		}

		private void computeBindPoses(Mesh mesh)
		{
			Matrix4x4[] bindposes = mesh.bindposes;
			int num = bindposes.Length;
			if (num > 48)
			{
				Log.LogErrorFormatted(this, "Invalid mesh {0} with too many bones {1} > {2}", mesh.name, num, 48);
				return;
			}
			m_BindPose = new BindPose[num];
			m_BonesQuat = new Vector4[num];
			m_BonesPos = new Vector4[num];
			for (int i = 0; i < num; i++)
			{
				Vector3 p = bindposes[i].GetColumn(3);
				Quaternion q = Quaternion.LookRotation(bindposes[i].GetColumn(2), bindposes[i].GetColumn(1));
				m_BindPose[i] = new BindPose(q, p);
			}
		}

		public static void storeWeightsInTangents(Mesh mesh)
		{
			int vertexCount = mesh.vertexCount;
			Vector4[] array = new Vector4[vertexCount];
			BoneWeight[] boneWeights = mesh.boneWeights;
			for (int i = 0; i < vertexCount; i++)
			{
				BoneWeight boneWeight = boneWeights[i];
				array[i] = packInfluences(boneWeight);
			}
			mesh.tangents = array;
		}

		private static Vector4 packInfluences(BoneWeight boneWeight)
		{
			return new Vector4((float)boneWeight.boneIndex0 + boneWeight.weight0 * 0.5f, (float)boneWeight.boneIndex1 + boneWeight.weight1 * 0.5f, (float)boneWeight.boneIndex2 + boneWeight.weight2 * 0.5f, (float)boneWeight.boneIndex3 + boneWeight.weight3 * 0.5f);
		}
	}
}
