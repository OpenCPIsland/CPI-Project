using ClubPenguin.Core;
using System.Collections.Generic;

namespace ClubPenguin.Switches
{
	public class CompoundSwitch : Switch
	{
		public enum Operators
		{
			OR,
			AND
		}

		public Operators Operator;

		private Switch[] switches;

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
				}
			}
			switches = list.ToArray();
		}

		protected override void Change(bool onoff)
		{
			switch (Operator)
			{
			case Operators.AND:
				onoff = doAnd();
				break;
			case Operators.OR:
				onoff = doOr();
				break;
			}
			base.Change(onoff);
		}

		private bool doAnd()
		{
			bool flag = true;
			for (int i = 0; i < switches.Length; i++)
			{
				flag &= switches[i].OnOff;
			}
			return flag;
		}

		private bool doOr()
		{
			bool flag = false;
			for (int i = 0; i < switches.Length; i++)
			{
				flag |= switches[i].OnOff;
			}
			return flag;
		}

		public override string GetSwitchType()
		{
			return "compound";
		}

		public override object GetSwitchParameters()
		{
			return Operator;
		}
	}
}
