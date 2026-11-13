using UnityEngine;

namespace ClubPenguin.Actions
{
	public class BroadcastMessageAction : Action
	{
		public GameObject TargetObject;

		public string MethodName;

		public string DataString;

		public bool IsReceiverRequired = false;

		protected override void CopyTo(Action _messageData)
		{
			BroadcastMessageAction broadcastMessageAction = _messageData as BroadcastMessageAction;
			broadcastMessageAction.TargetObject = TargetObject;
			broadcastMessageAction.MethodName = MethodName;
			broadcastMessageAction.DataString = DataString;
			broadcastMessageAction.IsReceiverRequired = IsReceiverRequired;
			base.CopyTo(_messageData);
		}

		protected override void Update()
		{
			if (TargetObject != null && !string.IsNullOrEmpty(MethodName))
			{
				SendMessageOptions options = SendMessageOptions.DontRequireReceiver;
				if (IsReceiverRequired)
				{
					options = SendMessageOptions.RequireReceiver;
				}
				TargetObject.BroadcastMessage(MethodName, DataString, options);
			}
			Completed();
		}
	}
}
