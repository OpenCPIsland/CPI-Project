using Disney.MobileNetwork;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	[RequireComponent(typeof(CanvasRenderer))]
	public class PerformanceDisplay : MonoBehaviour
	{
		protected Performance perf;

		public void OnEnable()
		{
			perf = Service.Get<Performance>();
		}

		private void Update()
		{
			perf.BeginFrame();
		}

		private void LateUpdate()
		{
			perf.EndFrame();
		}
	}
}
