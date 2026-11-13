using JetpackReboot;
using MinigameFramework;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class mg_jr_GameScreen : MinigameScreen
{
	private mg_jr_CountDisplay m_coinCountDisplay = null;

	private mg_jr_CountDisplay m_distanceCountDisplay = null;

	private Animator m_coinCollectAnimator = null;

	private mg_jr_TurboBar m_turboBar = null;

	private mg_jr_UINotification m_notification = null;

	public override void LoadUI(Dictionary<string, string> propertyList = null)
	{
		mg_jr_CountDisplay[] componentsInChildren = GetComponentsInChildren<mg_jr_CountDisplay>();
		mg_jr_CountDisplay[] array = componentsInChildren;
		foreach (mg_jr_CountDisplay mg_jr_CountDisplay in array)
		{
			if (mg_jr_CountDisplay.gameObject.name == "mg_jr_Coins")
			{
				m_coinCountDisplay = mg_jr_CountDisplay;
			}
			else if (mg_jr_CountDisplay.gameObject.name == "mg_jr_Distance")
			{
				m_distanceCountDisplay = mg_jr_CountDisplay;
			}
		}
		m_turboBar = GetComponentInChildren<mg_jr_TurboBar>();
		Assert.NotNull(m_turboBar, "Turbobar not found");
		m_coinCollectAnimator = GetComponentInChildren<Animator>();
		Assert.IsTrue(m_coinCollectAnimator.gameObject.name.Equals("mg_jr_CoinIcon"), "More animators added? additional logic required?");
		m_notification = GetComponentInChildren<mg_jr_UINotification>();
		mg_JetpackReboot active = MinigameManager.GetActive<mg_JetpackReboot>();
		active.GameLogic.Player.CoinCountChanged += m_coinCountDisplay.OnCountChange;
		active.GameLogic.Odometer.DistanceChanged += m_distanceCountDisplay.OnCountChange;
		m_coinCountDisplay.CurrentCount = active.GameLogic.Player.CoinsCollected;
		m_distanceCountDisplay.CurrentCount = (int)active.GameLogic.Odometer.DistanceTravelledThisRun;
		active.GoalManager.OnGoalCompleted += OnGoalCompleted;
		active.GameLogic.Player.CoinCountChanged += OnCoinCollected;
		m_turboBar.LoadUIElement();
		m_turboBar.SetDisplayedDevice(active.GameLogic.Player.TurboDevice);
		base.LoadUI(propertyList);
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		m_turboBar.UnloadUIElement();
		m_turboBar.SetDisplayedDevice(null);
		mg_JetpackReboot active = MinigameManager.GetActive<mg_JetpackReboot>();
		active.GameLogic.Player.CoinCountChanged -= m_coinCountDisplay.OnCountChange;
		active.GameLogic.Odometer.DistanceChanged -= m_distanceCountDisplay.OnCountChange;
		active.GameLogic.Player.CoinCountChanged -= OnCoinCollected;
		active.GoalManager.OnGoalCompleted -= OnGoalCompleted;
	}

	private void OnGoalCompleted(mg_jr_Goal _goal)
	{
		m_notification.QueueNotificationForDisplay(_goal.TextDescriptionOfGoal());
	}

	private void OnCoinCollected(int _newCount)
	{
		m_coinCollectAnimator.SetTrigger("Pickup");
	}
}
