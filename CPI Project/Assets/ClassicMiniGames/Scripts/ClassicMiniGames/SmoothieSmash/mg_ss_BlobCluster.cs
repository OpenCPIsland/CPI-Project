using MinigameFramework;
using System.Collections.Generic;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_BlobCluster
	{
		private List<mg_ss_BlobObject> m_blobs;

		private float m_blobTTL;

		private float m_duration;

		private float m_timeActive;

		private bool m_showSplatter;

		public Color Color
		{
			get;
			private set;
		}

		public bool ToRemove
		{
			get
			{
				return m_timeActive > m_duration;
			}
		}

		public mg_ss_BlobCluster(int p_numBlobs, Vector2 p_initialOffset, Vector2 p_finalOffset, float p_initialClusterRadiusX, float p_initialClusterRadiusY, float p_finalClusterRadiusX, float p_finalClusterRadiusY, float p_initialBlobScale, float p_finalBlobScale, float p_blobScaleVariationPercentage, float p_duration, Color p_color, float p_blobDelay, bool p_showSplatter, float p_blobSinMin, float p_blobSinMax, Transform p_parentTransform)
		{
			Color = p_color;
			m_blobTTL = p_duration;
			m_timeActive = 0f;
			m_showSplatter = p_showSplatter;
			m_duration = p_duration + p_blobDelay * (float)p_numBlobs;
			m_blobs = new List<mg_ss_BlobObject>();
			for (int i = 0; i < p_numBlobs; i++)
			{
				mg_ss_BlobObject component = MinigameManager.GetActive<mg_SmoothieSmash>().Resources.GetInstancedResource(mg_ss_EResourceList.GAME_SPLAT_BLOB).GetComponent<mg_ss_BlobObject>();
				MinigameSpriteHelper.AssignParentTransform(component.gameObject, p_parentTransform);
				m_blobs.Add(component);
				component.Initialize(i, p_initialOffset, p_finalOffset, p_initialClusterRadiusX, p_initialClusterRadiusY, p_finalClusterRadiusX, p_finalClusterRadiusY, p_initialBlobScale, p_finalBlobScale, p_blobScaleVariationPercentage, p_color, p_blobDelay, p_blobSinMin, p_blobSinMax);
			}
		}

		public void MinigameUpdate(float p_deltaTime)
		{
			m_timeActive += p_deltaTime;
			if (m_timeActive <= m_duration)
			{
				foreach (mg_ss_BlobObject blob in m_blobs)
				{
					blob.MinigameUpdate(m_timeActive, m_blobTTL, m_showSplatter);
				}
			}
		}

		public void Remove()
		{
			m_blobs.ForEach(delegate(mg_ss_BlobObject blob)
			{
				Object.Destroy(blob.gameObject);
			});
		}

		public List<Vector3> GetSplatPositions()
		{
			List<Vector3> list = new List<Vector3>();
			List<mg_ss_BlobObject> list2 = m_blobs.FindAll((mg_ss_BlobObject blob) => blob.DoSplat);
			foreach (mg_ss_BlobObject item in list2)
			{
				list.Add(item.GetSplatPosition());
			}
			return list;
		}
	}
}
