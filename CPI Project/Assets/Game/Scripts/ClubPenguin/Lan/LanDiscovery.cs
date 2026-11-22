#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClubPenguin.Net.Lan
{
	public sealed class LanBeacon
	{
		public const int Port = 9935;

		public const string Magic = "CPI-LAN|1|";

		private readonly string name;

		private volatile bool running;

		private Thread thread;

		public LanBeacon(string name)
		{
			this.name = name;
		}

		public void Start()
		{
			running = true;
			thread = new Thread(Run);
			thread.IsBackground = true;
			thread.Name = "CPI LAN Beacon";
			thread.Start();
		}

		public void Stop()
		{
			running = false;
		}

		public static List<IPAddress> BroadcastAddresses()
		{
			List<IPAddress> result = new List<IPAddress>
			{
				IPAddress.Broadcast
			};
			try
			{
				foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
				{
					if (ni.OperationalStatus != OperationalStatus.Up || ni.NetworkInterfaceType == NetworkInterfaceType.Loopback)
					{
						continue;
					}
					foreach (UnicastIPAddressInformation ua in ni.GetIPProperties().UnicastAddresses)
					{
						if (ua.Address.AddressFamily != AddressFamily.InterNetwork || ua.IPv4Mask == null)
						{
							continue;
						}
						byte[] ip = ua.Address.GetAddressBytes();
						byte[] mask = ua.IPv4Mask.GetAddressBytes();
						byte[] bc = new byte[4];
						for (int i = 0; i < 4; i++)
						{
							bc[i] = (byte)(ip[i] | ~mask[i]);
						}
						IPAddress addr = new IPAddress(bc);
						if (!result.Contains(addr))
						{
							result.Add(addr);
						}
					}
				}
			}
			catch (Exception)
			{
			}
			return result;
		}

		public static string LocalAddress()
		{
			try
			{
				foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
				{
					if (ni.OperationalStatus != OperationalStatus.Up || ni.NetworkInterfaceType == NetworkInterfaceType.Loopback)
					{
						continue;
					}
					IPInterfaceProperties props = ni.GetIPProperties();
					if (props.GatewayAddresses.Count == 0)
					{
						continue;
					}
					foreach (UnicastIPAddressInformation ua in props.UnicastAddresses)
					{
						if (ua.Address.AddressFamily == AddressFamily.InterNetwork)
						{
							return ua.Address.ToString();
						}
					}
				}
				foreach (NetworkInterface ni2 in NetworkInterface.GetAllNetworkInterfaces())
				{
					if (ni2.OperationalStatus != OperationalStatus.Up || ni2.NetworkInterfaceType == NetworkInterfaceType.Loopback)
					{
						continue;
					}
					foreach (UnicastIPAddressInformation ua2 in ni2.GetIPProperties().UnicastAddresses)
					{
						if (ua2.Address.AddressFamily == AddressFamily.InterNetwork)
						{
							return ua2.Address.ToString();
						}
					}
				}
			}
			catch (Exception)
			{
			}
			return "127.0.0.1";
		}

		private void Run()
		{
			byte[] payload = Encoding.UTF8.GetBytes(Magic + LanServer.Ports[0] + "|" + name);
			using (UdpClient udp = new UdpClient())
			{
				udp.EnableBroadcast = true;
				while (running)
				{
					foreach (IPAddress addr in BroadcastAddresses())
					{
						try
						{
							udp.Send(payload, payload.Length, new IPEndPoint(addr, Port));
						}
						catch (Exception)
						{
						}
					}
					for (int i = 0; i < 10 && running; i++)
					{
						Thread.Sleep(100);
					}
				}
			}
		}
	}

	public sealed class LanFinder
	{
		private readonly object sync = new object();

		private volatile bool running;

		private string address;

		private string hostName;

		private long lastSeenTicks;

		public void Start()
		{
			running = true;
			Thread t = new Thread(Run);
			t.IsBackground = true;
			t.Name = "CPI LAN Finder";
			t.Start();
		}

		public void Stop()
		{
			running = false;
		}

		public bool TryGetHost(double maxAgeSeconds, out string hostAddress, out string name)
		{
			lock (sync)
			{
				hostAddress = address;
				name = hostName;
				return address != null && (DateTime.UtcNow.Ticks - lastSeenTicks) < (long)(maxAgeSeconds * 10000000.0);
			}
		}

		public bool IsFresh(string hostAddress, double maxAgeSeconds)
		{
			lock (sync)
			{
				return address == hostAddress && (DateTime.UtcNow.Ticks - lastSeenTicks) < (long)(maxAgeSeconds * 10000000.0);
			}
		}

		private void Run()
		{
			while (running)
			{
				try
				{
					using (UdpClient udp = new UdpClient())
					{
						udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
						udp.Client.Bind(new IPEndPoint(IPAddress.Any, LanBeacon.Port));
						udp.Client.ReceiveTimeout = 250;
						while (running)
						{
							try
							{
								IPEndPoint from = new IPEndPoint(IPAddress.Any, 0);
								byte[] data = udp.Receive(ref from);
								string text = Encoding.UTF8.GetString(data);
								if (!text.StartsWith(LanBeacon.Magic, StringComparison.Ordinal))
								{
									continue;
								}
								string[] parts = text.Substring(LanBeacon.Magic.Length).Split(new char[1]
								{
									'|'
								}, 2);
								lock (sync)
								{
									if (address == null || address == from.Address.ToString() || (DateTime.UtcNow.Ticks - lastSeenTicks) > 50000000L)
									{
										address = from.Address.ToString();
										hostName = (parts.Length > 1) ? parts[1] : "";
										lastSeenTicks = DateTime.UtcNow.Ticks;
									}
								}
							}
							catch (SocketException)
							{
							}
						}
					}
				}
				catch (Exception)
				{
					Thread.Sleep(1000);
				}
			}
		}
	}
}
#endif
