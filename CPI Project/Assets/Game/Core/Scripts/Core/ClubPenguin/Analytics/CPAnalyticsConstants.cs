namespace ClubPenguin.Analytics
{
	public static class CPAnalyticsConstants
	{
		public enum Methods
		{
			game_action,
			timing,
			page_view,
			test_impression,
			in_app_currency_action,
			payment_action
		}

		public const string CONTEXT_TIMING = "timing";

		public const string CONTEXT_INITACTION = "initaction";

		public const int MAXMEMORY_LIMIT = 180;

		public const string DATA_PARAMETER_CONTEXT = "context";

		public const string DATA_PARAMETER_ACTION = "action";

		public const string DATA_PARAMETER_LOCATION = "location";

		public const string DATA_PARAMETER_PATHNAME = "path_name";

		public const string DATA_PARAMETER_RESULT = "result";

		public const string DATA_PARAMETER_ELAPSEDTIME = "elapsed_time";

		public const string DATA_PARAMETER_MESSAGE = "message";

		public const string DATA_PARAMETER_TYPE = "type";

		public const string DATA_PARAMETER_LEVEL = "level";

		public const string DATA_PARAMETER_TEST = "test";

		public const string DATA_PARAMETER_CONTROL = "control";

		public const string DATA_PARAMETER_VARIANT = "variant";

		public const string DATA_PARAMETER_VARIANT_TITLE = "variant_title";

		public const string DATA_PARAMETER_CURRENCY = "currency";

		public const string DATA_PARAMETER_AMOUNT = "amount";

		public const string DATA_PARAMETER_AMOUNT_PAID = "amount_paid";

		public const string DATA_PARAMETER_ITEM = "item";

		public const string DATA_PARAMETER_BALANCE = "balance";

		public const string LOCATION_BACKGROUNDTIME = "from_background";

		public const string LOCATION_STARTTIME = "start_to_login";

		public const string LOCATION_JOINROOM = "join_room";

		public const string LOCATION_LOADROOM = "load_room";

		public const string CURRENCY_COINS = "coins";

		public const string CURRENCY_REALWORLD = "real_world";

		public const string CALL_ID_JOYSTICK = "joystick";
	}
}
