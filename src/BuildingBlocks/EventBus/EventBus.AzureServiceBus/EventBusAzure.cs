using EventBus.Base;
using EventBus.Base.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.AzureServiceBus
{
    public class EventBusAzure : BaseEventBus
    {
        private ITopicClient topicClient;
        private ManagementClient managmentClient;
        private ILogger logger;

        public EventBusAzure(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
        {
            logger = serviceProvider.GetService(typeof(ILogger<EventBusAzure>)) as ILogger<EventBusAzure>;
            managmentClient = new ManagementClient(config.EventBusConnectionString);
            topicClient = createTopicClient();
        }


        private ITopicClient createTopicClient()
        {
            if(topicClient == null || topicClient.IsClosedOrClosing)
            {
                topicClient = new TopicClient(EventBusConfig.EventBusConnectionString,EventBusConfig.DefaultTopicName,retryPolicy:RetryPolicy.Default);
                
            }
            if (!managmentClient.TopicExistsAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult())
                managmentClient.CreateTopicAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult();
                
            return topicClient;
        }

        public override void Publish(IntegrationEvent @event)
        {
            var eventName = @event.GetType().Name;
            eventName = ProcessEventName(eventName);

            
            var eventStr = JsonConvert.SerializeObject(@event);
            var boyArr = Encoding.UTF8.GetBytes(eventStr);

            var message = new Message()
            {
                MessageId = Guid.NewGuid().ToString(),
                Body = boyArr,
                Label = eventName,
            };
            topicClient.SendAsync(message).GetAwaiter().GetResult();
        }

        public override void Subscribe<T, TH>()
        {
            var eventName = typeof(T).Name;
            SubsManager.HasSubscriptionsForEvent(eventName);

            if (!SubsManager.HasSubscriptionsForEvent(eventName))
            {
                var subscriptionClient = CreateSubscriptionClient(eventName);
                RegisterSubscriptionClientMessageHandler(subscriptionClient);
            }

            logger.LogInformation($"Subscribing to event {eventName} with {typeof(T).Name}");
        }

        public override void Unsubscribe<T, TH>()
        {
            var eventName = typeof(T).Name;

            try
            {
                var subscriptionClient = CreateSubscriptionClient(eventName);
                subscriptionClient
                    .RemoveRuleAsync(eventName)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (MessagingEntityNotFoundException)
            {
                logger.LogWarning($"The messaging entity {eventName} Could not be found");
                
            }

            logger.LogInformation("Unsubscribing from event {eventName}",eventName);
            SubsManager.RemoveSubcription<T, TH>();
        }

        private void RegisterSubscriptionClientMessageHandler(ISubscriptionClient subscriptionClient)
        {
            subscriptionClient.RegisterMessageHandler(
                async (message, token) =>
                {
                    var eventName = $"{message.Label}";
                    var messageData = Encoding.UTF8.GetString(message.Body);

                    if (await ProcessEvent(ProcessEventName(eventName), messageData))
                    {
                        await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                    }
                },new MessageHandlerOptions(exceptionReceivedHandler) { MaxConcurrentCalls=10,AutoComplete=false}
                );
        }

        private Task exceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var ex = exceptionReceivedEventArgs.Exception;
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

            logger.LogError(ex,$"ERROR handling message : {ex.Message}- context : {context}");
            return Task.CompletedTask;
        }

        private ISubscriptionClient CreateSubscriptionClientNotExist(string eventName)
        {
            var subClient = CreateSubscriptionClient(eventName);
            var exist = managmentClient.SubscriptionExistsAsync(EventBusConfig.DefaultTopicName,GetSubName(eventName)).GetAwaiter().GetResult();
            if (!exist)
            {
                managmentClient.CreateSubscriptionAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName)).GetAwaiter().GetResult();
                RemoveDefaultRule(subClient);   
            }

            CreateRuleIfExist(ProcessEventName(eventName), subClient);
            return subClient;
        }

        private void CreateRuleIfExist(string eventName,ISubscriptionClient subscriptionClient)
        {
            bool ruleExist;

            try
            {
                var rule = managmentClient.GetRuleAsync(EventBusConfig.DefaultTopicName, eventName, eventName).GetAwaiter().GetResult();
                ruleExist = rule != null;
            }
            catch (MessagingEntityNotFoundException)
            {

                ruleExist = false;
            }

            if (!ruleExist)
            {
                subscriptionClient.AddRuleAsync(new RuleDescription
                {
                    Filter = new CorrelationFilter { Label = eventName },
                    Name = eventName
                }).GetAwaiter().GetResult();
            }
        }

        private void RemoveDefaultRule(SubscriptionClient subscriptionClient)
        {
            try
            {
                subscriptionClient
                    .RemoveRuleAsync(RuleDescription.DefaultRuleName)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (MessagingEntityNotFoundException)
            {

                logger.LogWarning($"The messaging entity {RuleDescription.DefaultRuleName} Could not be found ");
            }
        }

        private SubscriptionClient CreateSubscriptionClient(string eventName)
        {
            return new SubscriptionClient(EventBusConfig.EventBusConnectionString, EventBusConfig.DefaultTopicName, GetSubName(eventName));
        }

        public override void Dispose()
        {
            base.Dispose();

            managmentClient.CloseAsync();
            topicClient.CloseAsync().GetAwaiter().GetResult();
            topicClient = null;
            managmentClient = null;
        }
    }
}
