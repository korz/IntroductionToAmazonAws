using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using AmazonAws.Shared;

namespace AmazonAws.Sns
{
    public class Repository
    {
        public static async Task<IEnumerable<string>> ListTopicArns()
        {
            using (var client = new AmazonSimpleNotificationServiceClient(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var response = await client.ListTopicsAsync(new ListTopicsRequest());

                return response.Topics.Select(x => x.TopicArn).ToList();
            }
        }

        public static async Task<string> CreateTopic(string topicName)
        {
            using (var client = new AmazonSimpleNotificationServiceClient(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var request = new CreateTopicRequest(topicName);

                var response = await client.CreateTopicAsync(request);
                
                return response.TopicArn;
            }
        }

        public static async Task<IEnumerable<Subscription>> ListSubscriptions(string topicArn)
        {
            using (var client = new AmazonSimpleNotificationServiceClient(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var request = new ListSubscriptionsByTopicRequest(topicArn);

                var response = await client.ListSubscriptionsByTopicAsync(request);
                
                return response.Subscriptions;
            }
        }

        public static async Task CreateEmailSubscription(string topicArn, string emailAddress)
        {
            using (var client = new AmazonSimpleNotificationServiceClient(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var request = new SubscribeRequest(topicArn, "email", emailAddress);

                await client.SubscribeAsync(request);
            }
        }

        public static async Task PublishMessage(string topicArn, string message, string subject = null)
        {
            using (var client = new AmazonSimpleNotificationServiceClient(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                await client.PublishAsync(topicArn, message, subject);
            }
        }
    }
}
