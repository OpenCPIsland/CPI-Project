using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

public class RemoteSwitchInverse : Switch
{
	private EventDispatcher dispatcher;

	public Switch SourceSwitch;

	public new void Awake()
	{
		dispatcher = Service.Get<EventDispatcher>();
		base.Awake();
	}

	public void OnEnable()
	{
		dispatcher.AddListener<SwitchEvents.SwitchChange>(OnSwitchChange);
	}

	public void OnDisable()
	{
		dispatcher.RemoveListener<SwitchEvents.SwitchChange>(OnSwitchChange);
	}

	private bool OnSwitchChange(SwitchEvents.SwitchChange evt)
	{
		Switch component = evt.Owner.GetComponent<Switch>();
		if (component != null && component == SourceSwitch)
		{
			Change(!evt.Value);
		}
		return false;
	}

	public override string GetSwitchType()
	{
		return "eventInverse";
	}

	public override object GetSwitchParameters()
	{
		return SourceSwitch.gameObject.GetPath();
	}
}
