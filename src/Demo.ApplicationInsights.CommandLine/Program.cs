using System;
using System.IO;

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Demo.ApplicationInsights.CommandLine
{
	class Program
	{
		static void Main(string[] args)
		{
			var client = new Client();
			client.Execute();
		}
	}

	public class Client
	{
		private readonly Random _random;
		private readonly TelemetryClient _telemetryClient;

		public Client()
		{
			_random = new Random();

			_telemetryClient = new TelemetryClient() { InstrumentationKey = "fd6931c4-8b40-48ab-8d00-b391a4092a3d" };
			_telemetryClient.Context.Component.Version = "1.0";
			_telemetryClient.Context.Device.RoleName = "Demo.ApplicationInsights.CommandLine";
			_telemetryClient.Context.Device.RoleInstance = Environment.MachineName;

		}

		public void Execute()
		{
			Console.WriteLine("Enter one of the following:  1 - Execute Event A, 2 - Execute Event B, 3 - Fail, 4 - Quit");
			var isNotDone = true;

			while (isNotDone)
			{
				var key = Console.ReadKey();

				switch (key.Key)
				{
					case ConsoleKey.D1:
						HandleEvent("Event A");
						break;
					case ConsoleKey.D2:
						HandleEvent("Event B");
						break;
					case ConsoleKey.D3:
						HandleFailure();
						break;
					default:
						isNotDone = false;
						break;
				}
			}

			_telemetryClient.Flush();
		}

		public void HandleEvent(string eventName)
		{
			var interval = _random.Next(1, 10);
			var ev = new EventTelemetry(eventName) {Timestamp = DateTimeOffset.UtcNow};
			ev.Metrics.Add("ResponseTime", interval);
			_telemetryClient.TrackEvent(ev);

			Console.WriteLine("Event - {0}", eventName);
		}

		public void HandleFailure()
		{
			try
			{
				ThrowException();
			}
			catch (Exception ex)
			{
				_telemetryClient.TrackException(ex);
				Console.WriteLine("Failure - {0}", ex.GetType());
			}
		}

		private void ThrowException()
		{
			var exType = _random.Next(1, 3);

			switch (exType)
			{
				case 1:
					throw new NotImplementedException();
				case 2:
					throw new ArgumentException();
				case 3:
					throw new FileNotFoundException();
			}
		}
	}
}
