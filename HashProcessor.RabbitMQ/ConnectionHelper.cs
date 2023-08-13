using RabbitMQ.Client;

namespace HashProcessor.RabbitMQ;
public static class ConnectionHelper
{
    public static void DeclareRabbitMQEntities(IConnection connection)
    {
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(Constants.HashExchangeName, ExchangeType.Topic);
        channel.QueueDeclare(Constants.HashQueueName, durable: false, exclusive: false, autoDelete: false);
        channel.QueueBind(Constants.HashQueueName, Constants.HashExchangeName, Constants.HashRoutingKey);

        channel.Close();
    }
}
