﻿namespace RabbitMQConsumer
{
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    using System;
    using System.Text;

    static class PointToPointConsumer
    {
        static readonly string queueName = "test.pointToPoint.queue";

        private static void Main(string[] args)
        {
            Console.WriteLine($"{nameof(PointToPointConsumer)}:");

            // RabbitMQ连接工厂
            ConnectionFactory factory = BaseConsumer.CreateRabbitMqConnection();

            // 建立连接
            using IConnection connection = factory.CreateConnection();

            // 创建信道
            using IModel channel = connection.CreateModel();

            EventingBasicConsumer consumer = new(channel);

            // 每次只能向消费者发送一条信息,在消费者未确认之前,不再向它发送信息
            channel.BasicQos(0, 1, false);

            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

            // 绑定消息接收后的事件委托
            consumer.Received += (model, message) =>
            {
                Console.WriteLine($"Message:{Encoding.UTF8.GetString(message.Body.ToArray())}");

                channel.BasicAck(
                    deliveryTag: message.DeliveryTag,
                    // 是否一次性确认多条数据
                    multiple: false);
            };
            Console.ReadLine();
        }
    }
}
