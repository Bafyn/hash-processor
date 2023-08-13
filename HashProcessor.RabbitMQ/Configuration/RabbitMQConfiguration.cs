using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashProcessor.RabbitMQ.Configuration;

public class RabbitMQConfiguration
{
    public string Host { get; init; }

    public ushort Port { get; init; }

    public string Username { get; set; }

    public string Password { get; set; }

}
