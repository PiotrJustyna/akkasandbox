using System;
using Akka.Actor;

public class PrintMyActorRefActor : UntypedActor
{
    protected override void OnReceive(object message)
    {
        switch (message)
        {
            case "printit":
                try
                {
                    IActorRef secondRef = Context.ActorOf(Props.Empty, "second-actor");
                    Console.WriteLine($"Second: {secondRef}");
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                break;
        }
    }
}
