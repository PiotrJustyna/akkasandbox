using System;
using Akka.Actor;

namespace AkkaSandbox
{
    public class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("testSystem");
            var firstRef = system.ActorOf(Props.Create<PrintMyActorRefActor>(), "first-actor");
            Console.WriteLine($"First: {firstRef}");
            firstRef.Tell("printit", ActorRefs.NoSender);
        }
    }
}
