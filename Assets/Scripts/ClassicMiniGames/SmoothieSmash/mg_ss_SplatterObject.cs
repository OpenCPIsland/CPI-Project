using MinigameFramework;
using System.Collections.Generic;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_SplatterObject : MonoBehaviour
	{
		private List<mg_ss_BlobCluster> m_blobClusters;

		private float m_initialRadiusX;

		private float m_initialRadiusY;

		private float m_finalRadiusX;

		private float m_finalRadiusY;

		private float m_blobSinModMin;

		private float m_blobSinModMax;

		private float m_initialScale;

		private float m_finalScale;

		private float m_blobScaleVariationPercentage;

		private float m_minSplatScale;

		private float m_maxSplatScale;

		private Transform m_blobFinish;

		protected void Awake()
		{
			m_blobClusters = new List<mg_ss_BlobCluster>();
		}

		public void Initialize(Transform p_blobFinish, Camera p_mainCamera)
		{
			m_blobFinish = p_blobFinish;
			SetBlobScale(0.1f, 1.2f, 25f);
			SetSplatScale(0.45f, 1f);
			float num = p_mainCamera.orthographicSize * 2f;
			m_blobSinModMin = num * (5f / 32f);
			m_blobSinModMax = num * (185f / 384f);
		}

		public void SetBlobRadii(float p_initialRadiusX, float p_initialRadiusY, float p_finalRadiusX, float p_finalRadiusY)
		{
			m_initialRadiusX = Mathf.Max(0f, p_initialRadiusX);
			m_initialRadiusY = Mathf.Max(0f, p_initialRadiusY);
			m_finalRadiusX = Mathf.Max(0f, p_finalRadiusX);
			m_finalRadiusY = Mathf.Max(0f, p_finalRadiusY);
		}

		private void SetBlobScale(float p_initialScale, float p_finalScale, float p_blobScaleVariationPercentage)
		{
			m_initialScale = p_initialScale;
			m_finalScale = p_finalScale;
			m_blobScaleVariationPercentage = p_blobScaleVariationPercentage;
		}

		private void SetSplatScale(float p_minSplatScale, float p_maxSplatScale)
		{
			m_minSplatScale = p_minSplatScale;
			m_maxSplatScale = p_maxSplatScale;
		}

		public void Smash(int p_numBlobs, Vector2 p_initialOffset, float p_duration, Color p_color, float p_blobDelay, bool p_showSplatter)
		{
			SmashTo(p_numBlobs, p_initialOffset, m_blobFinish.position, p_duration, p_color, p_blobDelay, p_showSplatter);
		}

		public void SmashTo(int p_numBlobs, Vector2 p_initialOffset, Vector2 p_finalOffset, float p_duration, Color p_color, float p_blobDelay, bool p_showSplatter)
		{
			mg_ss_BlobCluster item = new mg_ss_BlobCluster(p_numBlobs, p_initialOffset, p_finalOffset, m_initialRadiusX, m_initialRadiusY, m_finalRadiusX, m_finalRadiusY, m_initialScale, m_finalScale, m_blobScaleVariationPercentage, p_duration, p_color, p_blobDelay, p_showSplatter, m_blobSinModMin, m_blobSinModMax, base.transform);
			m_blobClusters.Add(item);
		}

		public void MinigameUpdate(float p_deltaTime)
		{
			foreach (mg_ss_BlobCluster blobCluster in m_blobClusters)
			{
				blobCluster.MinigameUpdate(p_deltaTime);
				CheckForSplats(blobCluster);
				if (blobCluster.ToRemove)
				{
					blobCluster.Remove();
				}
			}
			m_blobClusters.RemoveAll((mg_ss_BlobCluster cluster) => cluster.ToRemove);
		}

		private void CheckForSplats(mg_ss_BlobCluster p_cluster)
		{
			List<Vector3> splatPositions = p_cluster.GetSplatPositions();
			float num = 1f;
			mg_ss_SplatObject mg_ss_SplatObject = null;
			foreach (Vector3 item in splatPositions)
			{
				num = Random.Range(m_minSplatScale, m_maxSplatScale);
				mg_ss_SplatObject = ((Random.Range(0, 5) != 0) ? MinigameManager.GetActive<mg_SmoothieSmash>().Resources.GetInstancedResource(mg_ss_EResourceList.GAME_SPLAT).GetComponent<mg_ss_SplatObject>() : MinigameManager.GetActive<mg_SmoothieSmash>().Resources.GetInstancedResource(mg_ss_EResourceList.GAME_SPLAT_LITTLE).GetComponent<mg_ss_SplatObject>());
				mg_ss_SplatObject.Initialize(item, num, p_cluster.Color);
				MinigameSpriteHelper.AssignParentTransform(mg_ss_SplatObject.gameObject, base.transform);
			}
		}
	}
}
