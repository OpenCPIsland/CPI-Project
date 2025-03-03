using UnityEngine;

namespace ClubPenguin.Actions
{
	public class SendMessageAction : Action
	{
		public GameObject TargetObject;

		public string MethodName;

		public string DataString;

		public bool IsReceiverRequired = false;

		protected override void CopyTo(Action _messageData)
		{
			SendMessageAction sendMessageAction = _messageData as SendMessageAction;
			sendMessageAction.TargetObject = TargetObject;
			sendMessageAction.MethodName = MethodName;
			sendMessageAction.DataString = DataString;
			sendMessageAction.IsReceiverRequired = IsReceiverRequired;
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
				if (string.IsNullOrEmpty(DataString))
				{
					TargetObject.SendMessage(MethodName, GetTarget(), options);
				}
				else
				{
					TargetObject.SendMessage(MethodName, DataString, options);
				}
			}
			Completed();
		}
	}
}
