using SwrveUnity.ResourceManager;
using System.Collections.Generic;

namespace ClubPenguin.Analytics
{
	public interface ICPSwrveService
	{
		SwrveResourceManager ResourceManager
		{
			get;
		}

		Dictionary<string, string> GetDeviceInfo();

		void Pause();

		void Resume();

		void Quit();

		void Iap(int quantity, string productId, double productPrice, string currency, string app_store = "unknown_store", string durability = "durable", int level = 0);

		void UserUpdate(Dictionary<string, string> attributes);

		void PurchaseConsumable(string consumable, int cost, int quantity, int level, string context);

		void PurchaseClothing(string template, int cost, int quantity, int level);

		void PurchaseIglooItem(string iglooItem, int cost, int quantity);

		void PurchaseGeneral(string type, string name, int cost, int quantity, string durability = "consumable", string level = null, string context = null);

		void CoinsGiven(double amount, string context = null, string source = null, string message = null);

		void AgeGate(bool result, int age, string country_code);

		void Action(string tier1, string tier2 = null, string tier3 = null, string tier4 = null, string context = null, string message = null, string level = null);

		void Funnel(string type, string step_number, string step_name, string message = null, bool isSingular = false);

		void QuestFunnel(string type, string objective_number, string objective_name, string step_number, string step_name, string message = null, bool isSingular = false);

		void Timing(int elapsed_time, string context, string message = null, string step_name = null);

		void NavigationAction(string button_pressed, string from_location = null, string to_location = null, string module = null, string order = null);

		void Error(string reason, string type = null, string context = null, string location = null, string message = null);

		void TestImpression(string test_name, string shard_number, bool applied);

		void StartTimer(string TimerID, string Context, string Message = null, string StepName = null);

		void EndTimer(string TimerID, string overrideContext = null, string overrideMessage = null, string overrideStepName = null);

		void ActionSingular(string callID, string tier1, string tier2 = null, string tier3 = null, string tier4 = null, string context = null, string message = null, string level = null);
	}
}
