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

            //var bus = ConfigureRabbitMQ();
            var bus = ConfigureActiveMQ();

            Console.WriteLine("Starting...");
            await bus.StartAsync();

            Console.WriteLine("Publishing message...");
            await bus.Publish(new Message { Text = "Hi" });

            Console.WriteLine("Waiting for message to arrive...");
            await Task.Delay(60000);

            await bus.StopAsync();
        }

        private static IBusControl ConfigureActiveMQ()
        {
            return Bus.Factory.CreateUsingActiveMq(sbc =>
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
                        return Console.Out.WriteLineAsync($"Received from AMQ: {context.Message.Text}");
                    });
                });
            });
        }

        private static IBusControl ConfigureRabbitMQ()
        {
            return Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host("broker", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                sbc.ReceiveEndpoint("test-queue", ep =>
                {
                    ep.Handler<Message>(context =>
                    {
                        return Console.Out.WriteLineAsync($"Received from RabbitMQ: {context.Message.Text}");
                    });
                });
            });
        }
    }
}
