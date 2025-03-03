using SwrveUnity.ResourceManager;
using System.Collections.Generic;

namespace ClubPenguin.Analytics
{
	public class NullCPSwrveService : ICPSwrveService
	{
		public SwrveResourceManager ResourceManager
		{
			get
			{
				return new SwrveResourceManager();
			}
		}

		public Dictionary<string, string> GetDeviceInfo()
		{
			return null;
		}

		public void Pause()
		{
		}

		public void Resume()
		{
		}

		public void Quit()
		{
		}

		public void UserUpdate(Dictionary<string, string> attributes)
		{
		}

		public void Action(string tier1, string tier2 = null, string tier3 = null, string tier4 = null, string context = null, string message = null, string level = null)
		{
		}

		public void AgeGate(bool result, int age, string country_code)
		{
		}

		public void CoinsGiven(double amount, string context = null, string source = null, string message = null)
		{
		}

		public void Error(string reason, string type = null, string context = null, string location = null, string message = null)
		{
		}

		public void TestImpression(string test_name, string shard_number, bool applied)
		{
		}

		public void Funnel(string type, string step_number, string step_name, string message = null, bool isSingular = false)
		{
		}

		public void QuestFunnel(string type, string objective_number, string objective_name, string step_number, string step_name, string message = null, bool isSingular = false)
		{
		}

		public void Iap(int quantity, string productId, double productPrice, string currency, string app_store = "unknown_store", string durability = "durable", int level = 0)
		{
		}

		public void NavigationAction(string button_pressed, string from_location = null, string to_location = null, string module = null, string order = null)
		{
		}

		public void PurchaseConsumable(string consumable, int cost, int quantity, int level, string context)
		{
		}

		public void PurchaseClothing(string template, int cost, int quantity, int level)
		{
		}

		public void PurchaseIglooItem(string iglooItem, int cost, int quantity)
		{
		}

		public void PurchaseGeneral(string type, string name, int cost, int quantity, string durability = "consumable", string level = null, string context = null)
		{
		}

		public void Timing(int elapsed_time, string context, string message = null, string step_name = null)
		{
		}

		public void StartTimer(string TimerID, string Context, string Message = null, string StepName = null)
		{
		}

		public void EndTimer(string TimerID, string overrideContext = null, string overrideMessage = null, string overrideStepName = null)
		{
		}

		public void ActionSingular(string callID, string tier1, string tier2 = null, string tier3 = null, string tier4 = null, string context = null, string message = null, string level = null)
		{
		}
	}
}
