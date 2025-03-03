using SwrveUnity.Helpers;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace SwrveUnity.Messaging
{
	public class SwrveMessageFormat
	{
		public string Name;

		public List<SwrveButton> Buttons;

		public List<SwrveImage> Images;

		public string Language;

		public SwrveOrientation Orientation;

		public Point Size = new Point(0, 0);

		public SwrveMessage Message;

		public float Scale = 1f;

		public Color? BackgroundColor = null;

		public ISwrveInstallButtonListener InstallButtonListener;

		public ISwrveCustomButtonListener CustomButtonListener;

		public ISwrveMessageListener MessageListener;

		public bool Closing = false;

		public bool Dismissed = false;

		public bool Rotate = false;

		private SwrveMessageFormat(SwrveMessage message)
		{
			Message = message;
			Buttons = new List<SwrveButton>();
			Images = new List<SwrveImage>();
		}

		public static SwrveMessageFormat LoadFromJSON(ISwrveAssetsManager swrveAssetsManager, SwrveMessage message, Dictionary<string, object> messageFormatData, Color? defaultBackgroundColor)
		{
			SwrveMessageFormat swrveMessageFormat = new SwrveMessageFormat(message);
			swrveMessageFormat.Name = (string)messageFormatData["name"];
			swrveMessageFormat.Language = (string)messageFormatData["language"];
			if (messageFormatData.ContainsKey("scale"))
			{
				swrveMessageFormat.Scale = MiniJsonHelper.GetFloat(messageFormatData, "scale", 1f);
			}
			if (messageFormatData.ContainsKey("orientation"))
			{
				swrveMessageFormat.Orientation = SwrveOrientationHelper.Parse((string)messageFormatData["orientation"]);
			}
			swrveMessageFormat.BackgroundColor = defaultBackgroundColor;
			if (messageFormatData.ContainsKey("color"))
			{
				string text = (string)messageFormatData["color"];
				Color? backgroundColor = swrveMessageFormat.BackgroundColor;
				if (text.Length == 8)
				{
					byte a = byte.Parse(text.Substring(0, 2), NumberStyles.HexNumber);
					byte r = byte.Parse(text.Substring(2, 2), NumberStyles.HexNumber);
					byte g = byte.Parse(text.Substring(4, 2), NumberStyles.HexNumber);
					byte b = byte.Parse(text.Substring(6, 2), NumberStyles.HexNumber);
					backgroundColor = new Color32(r, g, b, a);
				}
				else if (text.Length == 6)
				{
					byte r = byte.Parse(text.Substring(0, 2), NumberStyles.HexNumber);
					byte g = byte.Parse(text.Substring(2, 2), NumberStyles.HexNumber);
					byte b = byte.Parse(text.Substring(4, 2), NumberStyles.HexNumber);
					backgroundColor = new Color32(r, g, b, byte.MaxValue);
				}
				swrveMessageFormat.BackgroundColor = backgroundColor;
			}
			Dictionary<string, object> dictionary = (Dictionary<string, object>)messageFormatData["size"];
			swrveMessageFormat.Size.X = MiniJsonHelper.GetInt((Dictionary<string, object>)dictionary["w"], "value");
			swrveMessageFormat.Size.Y = MiniJsonHelper.GetInt((Dictionary<string, object>)dictionary["h"], "value");
			IList<object> list = (List<object>)messageFormatData["buttons"];
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				SwrveButton item = LoadButtonFromJSON(message, (Dictionary<string, object>)list[i]);
				swrveMessageFormat.Buttons.Add(item);
			}
			IList<object> list2 = (List<object>)messageFormatData["images"];
			int j = 0;
			for (int count2 = list2.Count; j < count2; j++)
			{
				SwrveImage item2 = LoadImageFromJSON(message, (Dictionary<string, object>)list2[j]);
				swrveMessageFormat.Images.Add(item2);
			}
			return swrveMessageFormat;
		}

		protected static int IntValueFromAttribute(Dictionary<string, object> data, string attribute)
		{
			return MiniJsonHelper.GetInt((Dictionary<string, object>)data[attribute], "value");
		}

		protected static string StringValueFromAttribute(Dictionary<string, object> data, string attribute)
		{
			return (string)((Dictionary<string, object>)data[attribute])["value"];
		}

		protected static SwrveButton LoadButtonFromJSON(SwrveMessage message, Dictionary<string, object> buttonData)
		{
			SwrveButton swrveButton = new SwrveButton();
			swrveButton.Position.X = IntValueFromAttribute(buttonData, "x");
			swrveButton.Position.Y = IntValueFromAttribute(buttonData, "y");
			swrveButton.Size.X = IntValueFromAttribute(buttonData, "w");
			swrveButton.Size.Y = IntValueFromAttribute(buttonData, "h");
			swrveButton.Image = StringValueFromAttribute(buttonData, "image_up");
			swrveButton.Message = message;
			if (buttonData.ContainsKey("name"))
			{
				swrveButton.Name = (string)buttonData["name"];
			}
			string text = StringValueFromAttribute(buttonData, "type");
			SwrveActionType actionType = SwrveActionType.Dismiss;
			if (text.ToLower().Equals("install"))
			{
				actionType = SwrveActionType.Install;
			}
			else if (text.ToLower().Equals("custom"))
			{
				actionType = SwrveActionType.Custom;
			}
			swrveButton.ActionType = actionType;
			swrveButton.Action = StringValueFromAttribute(buttonData, "action");
			if (swrveButton.ActionType == SwrveActionType.Install)
			{
				string text2 = StringValueFromAttribute(buttonData, "game_id");
				if (text2 != null && text2 != string.Empty)
				{
					swrveButton.AppId = int.Parse(text2);
				}
			}
			return swrveButton;
		}

		protected static SwrveImage LoadImageFromJSON(SwrveMessage message, Dictionary<string, object> imageData)
		{
			SwrveImage swrveImage = new SwrveImage();
			swrveImage.Position.X = IntValueFromAttribute(imageData, "x");
			swrveImage.Position.Y = IntValueFromAttribute(imageData, "y");
			swrveImage.Size.X = IntValueFromAttribute(imageData, "w");
			swrveImage.Size.Y = IntValueFromAttribute(imageData, "h");
			swrveImage.File = StringValueFromAttribute(imageData, "image");
			return swrveImage;
		}

		public void Dismiss()
		{
			if (!Closing)
			{
				Closing = true;
				CustomButtonListener = null;
				InstallButtonListener = null;
				Message.Campaign.MessageDismissed();
				if (MessageListener != null)
				{
					MessageListener.OnDismiss(this);
					MessageListener = null;
				}
			}
		}

		public void Init(SwrveOrientation deviceOrientation)
		{
			Closing = false;
			Dismissed = false;
			Rotate = (Orientation != deviceOrientation);
			if (MessageListener != null)
			{
				MessageListener.OnShow(this);
			}
		}

		public void InitAnimation(Point startPoint, Point endPoint)
		{
			Message.Position = startPoint;
			Message.TargetPosition = endPoint;
		}

		public void UnloadAssets()
		{
			for (int i = 0; i < Images.Count; i++)
			{
				SwrveImage swrveImage = Images[i];
				Object.Destroy(swrveImage.Texture);
				swrveImage.Texture = null;
			}
			for (int j = 0; j < Buttons.Count; j++)
			{
				SwrveButton swrveButton = Buttons[j];
				Object.Destroy(swrveButton.Texture);
				swrveButton.Texture = null;
			}
		}
	}
}
