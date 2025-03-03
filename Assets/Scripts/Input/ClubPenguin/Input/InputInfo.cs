using DevonLocalization.Core;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Input
{
	public abstract class InputInfo
	{
		public abstract void Populate(ControlScheme controlScheme);

		protected string getKeyCodeTranslation(KeyCode keyCode)
		{
			if (keyCode == KeyCode.None)
			{
				return string.Empty;
			}
			string key = string.Format("Input.KeyCodes.{0}", keyCode);
			string value;
			return Service.Get<Localizer>().tokens.TryGetValue(key, out value) ? value : keyCode.ToString();
		}
	}
}
