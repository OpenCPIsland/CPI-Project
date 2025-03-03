using ClubPenguin.Core;
using System.Collections.Generic;

namespace ClubPenguin.Switches
{
	public class SequentialSwitch : Switch
	{
		private Switch[] switches;

		private Switch lastSwitch;

		private int currentIndex = 0;

		public new void Awake()
		{
			base.Awake();
			List<Switch> list = new List<Switch>();
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Switch component = base.transform.GetChild(i).GetComponent<Switch>();
				if (component != null)
				{
					list.Add(component);
					lastSwitch = component;
					component.gameObject.SetActive(false);
				}
			}
			switches = list.ToArray();
		}

		public void Start()
		{
			switches[currentIndex].gameObject.SetActive(true);
		}

		protected override void Change(bool onoff)
		{
			if (switches[currentIndex].OnOff && currentIndex < switches.Length - 1)
			{
				currentIndex++;
				switches[currentIndex].gameObject.SetActive(true);
			}
			base.Change(lastSwitch.OnOff);
		}

		public override object GetSwitchParameters()
		{
			return null;
		}

		public override string GetSwitchType()
		{
			return "sequential";
		}
	}
}
