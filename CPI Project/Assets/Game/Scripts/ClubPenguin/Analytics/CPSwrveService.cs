using SwrveUnity.ResourceManager;
using System.Collections.Generic;
using System.Globalization;

namespace ClubPenguin.Analytics
{
	public class CPSwrveService : ICPSwrveService
	{
		private const string CURRENCY_COINS = "coins";

		public const string EVENT_ACTION = "action";

		public const string EVENT_AGE_GATE = "age_gate";

		public const string EVENT_FUNNEL = "funnel";

		public const string EVENT_TIMING = "timing";

		public const string EVENT_NAVIGATION_ACTION = "navigation_action";

		public const string EVENT_CURRENCY_GIVEN_CUSTOM = "currency_given_custom";

		public const string EVENT_IAP_CUSTOM = "IAP_custom";

		public const string EVENT_PURCHASE_CUSTOM = "purchase_custom";

		public const string EVENT_ERROR = "error";

		public const string EVENT_TEST_IMPRESSION = "test_impression";

		private Dictionary<string, CPSwrveTimer> pendingTimers = new Dictionary<string, CPSwrveTimer>();

		private HashSet<string> singularCalls = new HashSet<string>();

		private SwrveResourceManager resourceManager;

		private SwrveSDK sdk;

		public SwrveResourceManager ResourceManager
		{
			get
			{
				return resourceManager;
			}
		}

		public CPSwrveService(SwrveComponent swrveComponent)
		{
			resourceManager = swrveComponent.SDK.ResourceManager;
			sdk = swrveComponent.SDK;
		}

		public Dictionary<string, string> GetDeviceInfo()
		{
			return sdk.GetDeviceInfo();
		}

		public void Pause()
		{
			sdk.SessionEnd();
			foreach (KeyValuePair<string, CPSwrveTimer> pendingTimer in pendingTimers)
			{
				pendingTimer.Value.PauseTimer();
			}
		}

		public void Resume()
		{
			foreach (KeyValuePair<string, CPSwrveTimer> pendingTimer in pendingTimers)
			{
				pendingTimer.Value.ResumeTimer();
			}
		}

		public void Quit()
		{
			sdk.SessionEnd();
		}

		public void UserUpdate(Dictionary<string, string> attributes)
		{
			sdk.UserUpdate(attributes);
		}

		public void Iap(int quantity, string productId, double productPrice, string currency, string app_store = "unknown_store", string durability = "durable", int level = 0)
		{
			sdk.Iap(quantity, productId, productPrice, currency);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("product_id", productId);
			dictionary.Add("cost", productPrice.ToString("F", CultureInfo.CreateSpecificCulture("en-US")));
			dictionary.Add("local_currency", currency);
			dictionary.Add("quantity", quantity.ToString());
			dictionary.Add("app_store", app_store);
			dictionary.Add("durability", durability);
			dictionary.Add("level", level.ToString());
			Dictionary<string, string> payload = dictionary;
			sdk.NamedEvent("IAP_custom", payload);
		}

		public void PurchaseConsumable(string consumable, int cost, int quantity, int level, string context)
		{
			sdk.Purchase("consumable." + consumable, "coins", cost, quantity);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("item", "consumable." + consumable);
			dictionary.Add("cost", cost.ToString());
			dictionary.Add("currency", "coins");
			dictionary.Add("quantity", quantity.ToString());
			dictionary.Add("durability", "consumable");
			dictionary.Add("level", level.ToString());
			Dictionary<string, string> dictionary2 = dictionary;
			if (context != null)
			{
				dictionary2.Add("context", context);
			}
			sdk.NamedEvent("purchase_custom", dictionary2);
		}

		public void PurchaseClothing(string template, int cost, int quantity, int level)
		{
			sdk.Purchase("clothing." + template, "coins", cost, quantity);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("item", "clothing." + template);
			dictionary.Add("cost", cost.ToString());
			dictionary.Add("currency", "coins");
			dictionary.Add("quantity", quantity.ToString());
			dictionary.Add("durability", "durable");
			dictionary.Add("level", level.ToString());
			Dictionary<string, string> payload = dictionary;
			sdk.NamedEvent("purchase_custom", payload);
		}

		public void PurchaseIglooItem(string iglooItem, int cost, int quantity)
		{
			sdk.Purchase("igloo." + iglooItem, "coins", cost, quantity);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("item", "igloo." + iglooItem);
			dictionary.Add("cost", cost.ToString());
			dictionary.Add("currency", "coins");
			dictionary.Add("quantity", quantity.ToString());
			Dictionary<string, string> payload = dictionary;
			sdk.NamedEvent("purchase_custom", payload);
		}

		public void PurchaseGeneral(string type, string name, int cost, int quantity, string durability = "consumable", string level = null, string context = null)
		{
			sdk.Purchase(type + "." + name, "coins", cost, quantity);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("item", type + "." + name);
			dictionary.Add("cost", cost.ToString());
			dictionary.Add("currency", "coins");
			dictionary.Add("quantity", quantity.ToString());
			dictionary.Add("durability", durability);
			dictionary.Add("level", level);
			Dictionary<string, string> dictionary2 = dictionary;
			if (context != null)
			{
				dictionary2.Add("context", context);
			}
			sdk.NamedEvent("purchase_custom", dictionary2);
		}

		public void CoinsGiven(double amount, string context = null, string source = null, string message = null)
		{
			sdk.CurrencyGiven("coins", amount);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("given_currency", "coins");
			dictionary.Add("given_amount", amount.ToString());
			Dictionary<string, string> dictionary2 = dictionary;
			if (context != null)
			{
				dictionary2.Add("context", context);
			}
			if (source != null)
			{
				dictionary2.Add("source", source);
			}
			if (message != null)
			{
				dictionary2.Add("message", message);
			}
			sdk.NamedEvent("currency_given_custom", dictionary2);
		}

		public void AgeGate(bool result, int age, string country_code)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("result", result.ToString());
			dictionary.Add("age", age.ToString());
			dictionary.Add("country_code", country_code);
			Dictionary<string, string> payload = dictionary;
			sdk.NamedEvent("age_gate", payload);
		}

		public void Action(string tier1, string tier2 = null, string tier3 = null, string tier4 = null, string context = null, string message = null, string level = null)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("tier1", tier1);
			Dictionary<string, string> dictionary2 = dictionary;
			if (tier2 != null)
			{
				dictionary2.Add("tier2", tier2);
			}
			if (tier3 != null)
			{
				dictionary2.Add("tier3", tier3);
			}
			if (tier4 != null)
			{
				dictionary2.Add("tier4", tier4);
			}
			if (context != null)
			{
				dictionary2.Add("context", context);
			}
			if (message != null)
			{
				dictionary2.Add("message", message);
			}
			if (level != null)
			{
				dictionary2.Add("level", level);
			}
			sdk.NamedEvent("action." + tier1, dictionary2);
		}

		public void Funnel(string type, string step_number, string step_name, string message = null, bool isSingular = false)
		{
			string text = "funnel." + type + "." + step_number + "_" + step_name;
			string name = "funnel." + type;
			if (!isSingular || isSingularCallValid(text))
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("type", type);
				dictionary.Add("step_number", step_number);
				dictionary.Add("step_name", step_name);
				Dictionary<string, string> dictionary2 = dictionary;
				if (message != null)
				{
					dictionary2.Add("message", message);
				}
				sdk.NamedEvent(name, dictionary2);
				singularCalls.Add(text);
			}
		}

		public void QuestFunnel(string type, string objective_number, string objective_name, string step_number, string step_name, string message = null, bool isSingular = false)
		{
			string text = "funnel." + type + "." + objective_number + "_" + objective_name + "." + step_number + "_" + step_name;
			string name = "funnel." + type;
			if (!isSingular || isSingularCallValid(text))
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("type", type);
				dictionary.Add("step_number", step_number);
				dictionary.Add("step_name", step_name);
				dictionary.Add("objective_number", objective_number);
				dictionary.Add("objective_name", objective_name);
				Dictionary<string, string> dictionary2 = dictionary;
				if (message != null)
				{
					dictionary2.Add("message", message);
				}
				sdk.NamedEvent(name, dictionary2);
				singularCalls.Add(text);
			}
		}

		public void NavigationAction(string button_pressed, string from_location = null, string to_location = null, string module = null, string order = null)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("button_pressed", button_pressed);
			Dictionary<string, string> dictionary2 = dictionary;
			if (from_location != null)
			{
				dictionary2.Add("from_location", from_location);
			}
			if (to_location != null)
			{
				dictionary2.Add("to_location", to_location);
			}
			if (module != null)
			{
				dictionary2.Add("module", module);
			}
			if (order != null)
			{
				dictionary2.Add("order", order);
			}
			sdk.NamedEvent("navigation_action." + button_pressed, dictionary2);
		}

		public void Error(string reason, string type = null, string context = null, string location = null, string message = null)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("reason", reason);
			Dictionary<string, string> dictionary2 = dictionary;
			if (type != null)
			{
				dictionary2.Add("type", type);
			}
			if (context != null)
			{
				dictionary2.Add("context", context);
			}
			if (location != null)
			{
				dictionary2.Add("location", location);
			}
			if (message != null)
			{
				dictionary2.Add("message", message);
			}
			sdk.NamedEvent("error." + reason, dictionary2);
		}

		public void TestImpression(string test_name, string shard_number, bool applied)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("test_name", test_name);
			dictionary.Add("shard_number", shard_number);
			dictionary.Add("applied", applied.ToString());
			Dictionary<string, string> payload = dictionary;
			if (isSingularCallValid("test_impression." + test_name))
			{
				sdk.NamedEvent("test_impression." + test_name, payload);
				singularCalls.Add("test_impression." + test_name);
			}
		}

		public void Timing(int elapsed_time, string context, string message = null, string step_name = null)
		{
			string text = "timing." + context;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("elapsed_time", elapsed_time.ToString());
			dictionary.Add("context", context);
			Dictionary<string, string> dictionary2 = dictionary;
			if (message != null)
			{
				dictionary2.Add("message", message);
			}
			if (step_name != null)
			{
				dictionary2.Add("step_name", step_name);
				text = text + "." + step_name;
			}
			sdk.NamedEvent(text, dictionary2);
		}

		public void StartTimer(string TimerID, string Context, string Message = null, string StepName = null)
		{
			if (pendingTimers.ContainsKey(TimerID))
			{
				pendingTimers.Remove(TimerID);
			}
			CPSwrveTimer value = new CPSwrveTimer(TimerID, Context, Message, StepName);
			pendingTimers.Add(TimerID, value);
		}

		public void EndTimer(string TimerID, string overrideContext = null, string overrideMessage = null, string overrideStepName = null)
		{
			CPSwrveTimer value;
			if (pendingTimers.TryGetValue(TimerID, out value))
			{
				int elapsed_time = (int)value.Timer.ElapsedMilliseconds / 1000;
				value.Timer.Stop();
				pendingTimers.Remove(TimerID);
				string context = value.Context;
				if (overrideContext != null)
				{
					context = overrideContext;
				}
				string message = value.Message;
				if (overrideMessage != null)
				{
					message = overrideMessage;
				}
				string step_name = value.StepName;
				if (overrideStepName != null)
				{
					step_name = overrideStepName;
				}
				Timing(elapsed_time, context, message, step_name);
			}
		}

		private bool isSingularCallValid(string callID)
		{
			return !singularCalls.Contains(callID);
		}

		public void ActionSingular(string callID, string tier1, string tier2 = null, string tier3 = null, string tier4 = null, string context = null, string message = null, string level = null)
		{
			if (isSingularCallValid(callID))
			{
				Action(tier1, tier2, tier3, tier4, context, message, level);
				singularCalls.Add(callID);
			}
		}
	}
}
