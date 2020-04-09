using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
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

      var system = ActorSystem.Create("iot-system");
      var supervisor = system.ActorOf(IotSupervisor.Props(), "iot-supervisor");

      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      Console.WriteLine("Stopping the host.");

      return Task.CompletedTask;
    }
  }

  public class IotSupervisor : UntypedActor
  {
    public ILoggingAdapter Log { get; } = Context.GetLogger();

    protected override void PreStart() => Log.Info("IoT Application started");

    protected override void PostStop() => Log.Info("IoT Application stopped");

    // No need to handle any messages
    protected override void OnReceive(object message) { }

    public static Props Props() => Akka.Actor.Props.Create<IotSupervisor>();
  }
}
