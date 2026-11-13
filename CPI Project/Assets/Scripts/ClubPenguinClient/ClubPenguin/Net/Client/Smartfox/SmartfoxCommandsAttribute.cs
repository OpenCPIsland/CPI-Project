using System;

namespace ClubPenguin.Net.Client.Smartfox
{
	internal class SmartfoxCommandsAttribute : Attribute
	{
		public string Command
		{
			get;
			private set;
		}

		internal SmartfoxCommandsAttribute(string command)
		{
			Command = command;
		}
	}
}
