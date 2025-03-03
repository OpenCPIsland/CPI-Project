using Disney.LaunchPadFramework.Utility;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Net
{
	[RequireComponent(typeof(Animator))]
	public class NetworkAnimationSyncController : MonoBehaviour
	{
		[Range(0f, 100f)]
		public int AcceptablePercentDsync = 5;

		[Range(1f, 100f)]
		public int SpeedAdjustmentPercent = 5;

		private Animator animator;

		private INetworkServicesManager network;

		private float targetSpeed;

		private int[] animationLengthsMS;

		private int lcmAnimationLengthMS = 1;

		private void Awake()
		{
			animator = GetComponent<Animator>();
			network = Service.Get<INetworkServicesManager>();
			targetSpeed = animator.speed;
			animationLengthsMS = new int[animator.layerCount];
		}

		private void Start()
		{
			for (int i = 0; i < animator.layerCount; i++)
			{
				animationLengthsMS[i] = (int)animator.GetCurrentAnimatorStateInfo(i).length * 1000;
				lcmAnimationLengthMS = MathHelper.LCM(lcmAnimationLengthMS, animationLengthsMS[i]);
			}
		}

		private void Update()
		{
			for (int i = 0; i < animator.layerCount; i++)
			{
				float num = Mathf.Clamp((float)(network.GameTimeMilliseconds % lcmAnimationLengthMS % animationLengthsMS[i]) * targetSpeed / (float)animationLengthsMS[i], 0f, 1f);
				float num2 = animator.GetCurrentAnimatorStateInfo(i).normalizedTime % 1f;
				float num3 = num - num2;
				if (Mathf.Abs(num3) < (float)AcceptablePercentDsync / 100f || Mathf.Abs(num3) > 1f - (float)AcceptablePercentDsync / 100f)
				{
					animator.speed = targetSpeed;
				}
				else if ((num3 > 0f && (double)num3 <= 0.5) || (double)num3 < -0.5)
				{
					animator.speed = targetSpeed * (float)(100 + SpeedAdjustmentPercent) / 100f;
				}
				else
				{
					animator.speed = targetSpeed * (float)(100 - SpeedAdjustmentPercent) / 100f;
				}
			}
		}
	}
}
