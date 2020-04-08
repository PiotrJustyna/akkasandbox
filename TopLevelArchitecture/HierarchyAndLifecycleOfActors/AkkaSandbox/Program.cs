using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AkkaSandbox
{
	public class Program
	{
		static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
		Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
		.ConfigureServices((hostContext, services) =>
		{
			services.AddHostedService<HelloWorldHostedService>();
		});
	}

	public class HelloWorldHostedService : IHostedService
	{
		public Task StartAsync(CancellationToken cancellationToken)
		{
			Console.WriteLine("Starting the host.");

			var system = ActorSystem.Create("testSystem");
      var first = system.ActorOf(Props.Create<StartStopActor1>(), "first");
      first.Tell("stop");

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			Console.WriteLine("Stopping the host.");

			return Task.CompletedTask;
		}
	}

	public class StartStopActor1 : UntypedActor
	{
		protected override void PreStart()
		{
			Console.WriteLine("first started");
			Context.ActorOf(Props.Create<StartStopActor2>(), "second");
		}

		protected override void PostStop() => Console.WriteLine("first stopped");

		protected override void OnReceive(object message)
		{
			switch (message)
			{
				case "stop":
					Context.Stop(Self);
					break;
			}
		}
	}

	public class StartStopActor2 : UntypedActor
	{
		protected override void PreStart() => Console.WriteLine("second started");

		protected override void PostStop() => Console.WriteLine("second stopped");

		protected override void OnReceive(object message)
		{
		}
	}
}
