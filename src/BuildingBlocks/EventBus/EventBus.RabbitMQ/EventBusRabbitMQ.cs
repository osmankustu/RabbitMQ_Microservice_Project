using EventBus.Base;
using EventBus.Base.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Text;

namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : BaseEventBus
    {
        private RabbitMQPersistentConnection persistentConnection;
        private IConnectionFactory connectionFactory;
        private IModel consumerChannel;
        private ILogger logger;

        public EventBusRabbitMQ(EventBusConfig config , IServiceProvider serviceProvider) : base(config, serviceProvider)
        {
            logger = serviceProvider.GetService(typeof(ILogger<EventBusRabbitMQ>)) as ILogger<EventBusRabbitMQ>;

            
            if (config.Connection != null)
            {
                var connJson = JsonConvert.SerializeObject(config.Connection, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                });
                connectionFactory = JsonConvert.DeserializeObject<ConnectionFactory>(connJson);
                
            }

            else connectionFactory = new ConnectionFactory() { HostName="192.168.1.39",UserName="root",Password="root"};

            persistentConnection = new RabbitMQPersistentConnection(connectionFactory, serviceProvider,EventBusConfig.ConnectionRetryCount);

            consumerChannel = CreateConsumerChannel();

            SubsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }

        public override void Publish(IntegrationEvent @event)
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }
            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(EventBusConfig.ConnectionRetryCount, retryAttemp => TimeSpan.FromSeconds(Math.Pow(2 ,retryAttemp)), (ex, time) =>
                {
                    logger.LogInformation("Retry the Connection is AMQP {a}", ex.Message);
                });

            var eventName = @event.GetType().Name;
            eventName = ProcessEventName(eventName);

            consumerChannel.ExchangeDeclare(
                exchange: EventBusConfig.DefaultTopicName,
                type: "direct"
                );
            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            policy.Execute(() =>
            {
                var properties = consumerChannel.CreateBasicProperties();
                properties.DeliveryMode = 2; //persistent

                consumerChannel.QueueDeclare(queue: GetSubName(eventName),
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                    );

                consumerChannel.BasicPublish(
                    exchange: EventBusConfig.DefaultTopicName,
                    routingKey:eventName,
                    mandatory:true,
                    basicProperties:properties,
                    body:body
                    );
            });
        }

        public override void Subscribe<T, TH>()
        {
            var eventName = typeof(T).Name;
            eventName = ProcessEventName(eventName);
            if (!SubsManager.HasSubscriptionsForEvent(eventName))
            {
                if (!persistentConnection.IsConnected)
                {
                    persistentConnection.TryConnect();
                }
                consumerChannel.QueueDeclare(
                    queue:GetSubName(eventName), //Consume edeceğimiz q nun daha önce oluştu mu oluşmadı mı ?
                    durable:true,
                    exclusive:false,
                    autoDelete:false,
                    arguments:null
                    );

                consumerChannel.QueueBind(
                    queue: GetSubName(eventName),
                    exchange: EventBusConfig.DefaultTopicName,
                    routingKey: eventName
                    );
            }
            
            logger.LogInformation($"Subscribing to event {eventName} with {typeof(TH).Name}");
            SubsManager.AddSubscription<T, TH>();
            StartBasicConsume(eventName);
            
        }

        public override void Unsubscribe<T, TH>()
        {
            SubsManager.RemoveSubcription<T, TH>();
        }

        private IModel CreateConsumerChannel()
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            var channel = persistentConnection.CreateModel();
            channel.ExchangeDeclare(
                exchange: EventBusConfig.DefaultTopicName,
                type: "direct"
                );
            return channel;
        }

        private void StartBasicConsume(string eventName)
        {
            if(consumerChannel != null)
            {
                var consumer = new EventingBasicConsumer(consumerChannel);
                consumer.Received += Consumer_Received;

                consumerChannel.BasicConsume(queue: GetSubName(eventName),
                    autoAck: false,
                    consumer: consumer
                    );
            }
        }

        private async void Consumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;
            eventName = ProcessEventName(eventName);
            var message = Encoding.UTF8.GetString(e.Body.Span);

            try
            {
                await ProcessEvent(eventName, message);
                logger.LogInformation("consumed event is {event} message is : {message}",eventName, message);
            }
            catch (Exception ex)
            {
                logger.LogCritical("{eventName} not consumed exception message : {ex}",eventName,ex);
            }
            consumerChannel.BasicAck(e.DeliveryTag,multiple:false);
        }

        private void SubsManager_OnEventRemoved(object? sender, string eventName)
        {
            eventName = ProcessEventName(eventName);
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }
            consumerChannel.QueueUnbind(queue: eventName,
                exchange: EventBusConfig.DefaultTopicName,
                routingKey: eventName
                );
            if (SubsManager.IsEmpty)
            {
                consumerChannel.Close();
            }
        }
    }
}
