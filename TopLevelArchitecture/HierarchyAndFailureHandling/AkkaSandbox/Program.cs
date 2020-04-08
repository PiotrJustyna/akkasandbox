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
      var supervisingActor = system.ActorOf(Props.Create<SupervisingActor>(), "supervising-actor");
      supervisingActor.Tell("failChild");

      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      Console.WriteLine("Stopping the host.");

      return Task.CompletedTask;
    }
  }

  public class SupervisingActor : UntypedActor
  {
    private IActorRef child = Context.ActorOf(Props.Create<SupervisedActor>(), "supervised-actor");

    protected override void OnReceive(object message)
    {
      switch (message)
      {
        case "failChild":
          child.Tell("fail");
          break;
      }
    }
  }

  public class SupervisedActor : UntypedActor
  {
    protected override void PreStart() => Console.WriteLine("supervised actor started");

    protected override void PostStop() => Console.WriteLine("supervised actor stopped");

    protected override void OnReceive(object message)
    {
      switch (message)
      {
        case "fail":
          Console.WriteLine("supervised actor fails now");
          throw new Exception("I failed!");
      }
    }
  }
}
