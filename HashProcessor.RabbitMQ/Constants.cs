namespace HashProcessor.RabbitMQ;

// These keys can be configured in app settings, but for the sake of simplicity they are part of this project
public class Constants
{
    public const string HashExchangeName = "hashes-exchange";
    public const string HashQueueName = "hashes-queue";
    public const string HashRoutingKey = "*";
}
