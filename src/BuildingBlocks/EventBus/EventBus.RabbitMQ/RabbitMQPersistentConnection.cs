using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.RabbitMQ
{
    public class RabbitMQPersistentConnection : IDisposable
    {
        private readonly IConnectionFactory connectionFactory;
        private IConnection connection;
        private readonly int retryCount;
        private object lock_object = new object();
        private bool _disposed;
        private ILogger logger;
        public RabbitMQPersistentConnection(IConnectionFactory connectionFactory,  IServiceProvider serviceProvider, int retryCount = 5)
        {
            this.logger = serviceProvider.GetService(typeof(ILogger<RabbitMQPersistentConnection>)) as ILogger<RabbitMQPersistentConnection>;
            this.connectionFactory = connectionFactory;
            this.retryCount = retryCount;
        }
        public bool IsConnected => connection != null && connection.IsOpen;

        public IModel CreateModel()
        {
            return connection.CreateModel();
        }
        public void Dispose()
        {
            _disposed = true;
            connection.Dispose();
        }

        public bool TryConnect()
        {
            lock (lock_object)
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(retryCount, retryAttemp => TimeSpan.FromSeconds(Math.Pow(2, retryAttemp)), (ex, time) =>
                    {
                    }
                    );
                policy.Execute(() =>
                {
                    connection = connectionFactory.CreateConnection();
                });
                if (IsConnected)
                {
                    connection.ConnectionShutdown += Connection_ConnectionShutdown;
                    connection.CallbackException += Connection_CallbackException;
                    connection.ConnectionBlocked += Connection_ConnectionBlocked;
                    logger.LogInformation("Connection is success  !");
                    return true;
                }
                return false;
            }
        }

        private void Connection_ConnectionBlocked(object? sender, global::RabbitMQ.Client.Events.ConnectionBlockedEventArgs e)
        {
            logger.LogWarning("Connection is blocked Args : {e}", e);
            if (_disposed) return;
            TryConnect();

        }

        private void Connection_CallbackException(object? sender, global::RabbitMQ.Client.Events.CallbackExceptionEventArgs e)
        {
            logger.LogCritical("Connection is Callback Excep Args : {e}", e);
            if (_disposed) return;
            TryConnect();
        }

        private void Connection_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            logger.LogCritical("Connection is ShutDown Args : {e}", e);
            if (_disposed) return;
            TryConnect();
        }
    }
}
