using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.ActiveMqTransport;

namespace mt_amq_test
{
    public class Message
    {
        public string Text { get; set; }
    }

    class Program
    {
        public static async Task Main()
        {
            Console.WriteLine("Wait a bit for the bus to get ready");

            await Task.Delay(5000);

            Console.WriteLine("Configuring...");

            var bus = Bus.Factory.CreateUsingActiveMq(sbc =>
            {
                sbc.Host("broker", h =>
                {
                    //h.UseSsl();
                    h.Username("admin");
                    h.Password("admin");
                });

                sbc.ReceiveEndpoint("test-queue", ep =>
                {
                    ep.Handler<Message>(context =>
                    {
                        return Console.Out.WriteLineAsync($"Received: {context.Message.Text}");
                    });
                });
            });

            Console.WriteLine("Starting...");
            await bus.StartAsync();

            Console.WriteLine("Publishing message...");
            await bus.Publish(new Message { Text = "Hi" });

            Console.WriteLine("Waiting for message to arrive...");
            await Task.Delay(15000);

            await bus.StopAsync();
        }
    }
}
