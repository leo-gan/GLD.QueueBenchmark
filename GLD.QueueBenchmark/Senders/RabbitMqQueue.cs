// Download and Install RabbitMQ server: https://www.rabbitmq.com/download.html
// it would force you to install Erlang. Install it.
// Start RabbitMQ Windows service.
// PM> Install-Package RabbitMQ.Client

using System.Configuration;
using RabbitMQ.Client;

namespace GLD.QueueBenchmark.Senders
{
    internal class RabbitMqQueue : IQueueSender
    {
        private readonly string _connectionString =
            ConfigurationManager.AppSettings["RabbitMqQueue.ConnectionString"];
        private readonly string _hostName =
            ConfigurationManager.AppSettings["RabbitMqQueue.HostName"];

        private readonly string _exchangeName =
            ConfigurationManager.AppSettings["RabbitMqQueue.ExchangeName"];


        private readonly string _queueName =
            ConfigurationManager.AppSettings["RabbitMqQueue.QueueName"];

        private readonly string _routingKey =
            ConfigurationManager.AppSettings["RabbitMqQueue.RoutingKey"];

        private readonly IModel _model;
        private IBasicProperties _properties;

        public RabbitMqQueue()
        {
            var factory = new ConnectionFactory
            {
                Uri = _connectionString // @"amqp://guest:guest@localhost:4369/"
                // HostName = _hostName 
            };

            IConnection conn = factory.CreateConnection();
            _model = conn.CreateModel();
            _model.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
            _model.QueueDeclare(_queueName, false, false, false, null);
            _model.QueueBind(_queueName, _exchangeName, _routingKey, null);

            var _properties = _model.CreateBasicProperties();
            _properties.ContentType = "text/plain";
            _properties.DeliveryMode = 2;


        }

        #region IQueueSender Members

        public void Send(byte[] buffer)
        {
            _model.BasicPublish(_exchangeName, _routingKey, _properties, buffer);
        }

        #endregion

        public void Dispose()
        {
            if (_model != null) _model.Close();
            if (_model != null) _model.Dispose();
        }
    }
}