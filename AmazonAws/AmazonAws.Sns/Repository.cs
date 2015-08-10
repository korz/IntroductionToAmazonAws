using System.Collections.Generic;
using System.Linq;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using AmazonAws.Shared;

namespace AmazonAws.Sns
{
    public class Repository
    {
        public static IEnumerable<string> ListTopicArns()
        {
            using (var client = new AmazonSimpleNotificationServiceClient(Settings.AccessKey, Settings.Secret))
            {
                var response = client.ListTopics(new ListTopicsRequest());

                return response.Topics.Select(x => x.TopicArn).ToList();
            }
        }

        public static string CreateTopic(string topicName)
        {
            using (var client = new AmazonSimpleNotificationServiceClient(Settings.AccessKey, Settings.Secret))
            {
                var request = new CreateTopicRequest(topicName);

                return client.CreateTopic(request).TopicArn;
            }
        }

        public static IEnumerable<Subscription> ListSubscriptions(string topicArn)
        {
            using (var client = new AmazonSimpleNotificationServiceClient(Settings.AccessKey, Settings.Secret))
            {
                var request = new ListSubscriptionsByTopicRequest(topicArn);

                return client.ListSubscriptionsByTopic(request).Subscriptions;
            }
        }

        public static void CreateEmailSubscription(string topicArn, string emailAddress)
        {
            using (var client = new AmazonSimpleNotificationServiceClient(Settings.AccessKey, Settings.Secret))
            {
                var request = new SubscribeRequest(topicArn, "email", emailAddress);

                client.Subscribe(request);
            }
        }

        public static void PublishMessage(string topicArn, string message, string subject = null)
        {
            using (var client = new AmazonSimpleNotificationServiceClient(Settings.AccessKey, Settings.Secret))
            {
                client.Publish(topicArn, message, subject);
            }
        }
    }
}
