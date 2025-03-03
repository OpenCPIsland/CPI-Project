// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Sends a Message to a Game Object. See Unity docs for SendMessage.")]
	public class SendMessage : FsmStateAction
	{
		public enum MessageType
		{
			SendMessage,
			SendMessageUpwards,
			BroadcastMessage
		}

		[RequiredField]
        [Tooltip("The Game Object to send a message to.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Pick between <a href=\"http://unity3d.com/support/documentation/ScriptReference/GameObject.SendMessage.html\" rel=\"nofollow\">SendMessage</a>, <a href=\"http://unity3d.com/support/documentation/ScriptReference/GameObject.SendMessageUpwards.html\" rel=\"nofollow\">SendMessageUpwards</a>, or <a href=\"http://unity3d.com/support/documentation/ScriptReference/GameObject.BroadcastMessage.html\" rel=\"nofollow\">BroadcastMessage</a>.")]
        public MessageType delivery;

        [Tooltip("Message delivery options. See <a href=\"http://unity3d.com/support/documentation/ScriptReference/SendMessageOptions.html\" rel=\"nofollow\">SendMessageOptions</a> in Unity Docs.")]
        public SendMessageOptions options;
		
        [RequiredField]
        [Tooltip("Select a Method Name first then Parameters.")]
		public FunctionCall functionCall;

		public override void Reset()
		{
			gameObject = null;
			delivery = MessageType.SendMessage;
			options = SendMessageOptions.DontRequireReceiver;
			functionCall = null;
		}

		public override void OnEnter()
		{
			DoSendMessage();
			
			Finish();
		}

		void DoSendMessage()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null)
			{
				return;
			}			
			
			object parameter = null;

			switch (functionCall.ParameterType)
			{
				case "None":
					break;

				case "bool":
					parameter = functionCall.BoolParameter.Value;
					break;

				case "int":
					parameter = functionCall.IntParameter.Value;
					break;

				case "float":
					parameter = functionCall.FloatParameter.Value;
					break;

				case "string":
					parameter = functionCall.StringParameter.Value;
					break;

                case "Vector2":
                    parameter = functionCall.Vector2Parameter.Value;
                    break;

				case "Vector3":
					parameter = functionCall.Vector3Parameter.Value;
					break;

				case "Rect":
					parameter = functionCall.RectParamater.Value;
					break;

				case "GameObject":
					parameter = functionCall.GameObjectParameter.Value;
					break;

				case "Material":
					parameter = functionCall.MaterialParameter.Value;
					break;

				case "Texture":
					parameter = functionCall.TextureParameter.Value;
					break;

                case "Color":
                    parameter = functionCall.ColorParameter.Value;
                    break;

				case "Quaternion":
					parameter = functionCall.QuaternionParameter.Value;
					break;

				case "Object":
					parameter = functionCall.ObjectParameter.Value;
					break;

                case "Enum":
                    parameter = functionCall.EnumParameter.Value;
                    break;

                case "Array":
                    parameter = functionCall.ArrayParameter.Values;
                    break;
			}

			switch (delivery)
			{
				case MessageType.SendMessage:

					go.SendMessage(functionCall.FunctionName, parameter, options);
					return;
				
				case MessageType.SendMessageUpwards:

					go.SendMessageUpwards(functionCall.FunctionName, parameter, options);
					return;
					
				case MessageType.BroadcastMessage:

					go.BroadcastMessage(functionCall.FunctionName, parameter, options);
					return;

			}
		}
	}
}