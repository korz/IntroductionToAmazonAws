using Amazon.SQS;
using Amazon.SQS.Model;
using AmazonAws.Shared;

namespace AmazonAws.Sqs
{
    public class Repository
    {
        public static async Task<IEnumerable<string>> FindQueueUrls()
        {
            using (var client = new AmazonSQSClient(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var response = await client.ListQueuesAsync(new ListQueuesRequest());

                return response.QueueUrls;
            }
        }

        public static async Task<string?> FindQueueUrlByQueueName(string name)
        {
            using (var client = new AmazonSQSClient(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                ListQueuesResponse response = await client.ListQueuesAsync(new ListQueuesRequest(name));

                return response.QueueUrls?.FirstOrDefault();
            }
        }

        public static async Task<string> CreateQueue(string name)
        {
            using (var client = new AmazonSQSClient(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var request = new CreateQueueRequest
                {
                    QueueName = name
                };

                var response = await client.CreateQueueAsync(request);

                return response.QueueUrl;
            }
        }

        public static async Task Enqueue(string queueName, string message)
        {
            await EnqueueByQueueUrl(await FindQueueUrlByQueueName(queueName), message);
        }

        public static async Task EnqueueByQueueUrl(string queueUrl, string message)
        {
            using (var client = new AmazonSQSClient(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var request = new SendMessageRequest()
                {
                    QueueUrl = queueUrl,
                    MessageBody = message,
                };

                await client.SendMessageAsync(request);
            }
        }

        public static async Task EnqueueList(string queueName, IEnumerable<string> messages)
        {
            await EnqueueListByQueueUrl(await FindQueueUrlByQueueName(queueName), messages);
        }

        public static async Task EnqueueListByQueueUrl(string queueUrl, IEnumerable<string> messages)
        {
            using (var client = new AmazonSQSClient(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                foreach (var message in messages)
                {
                    var request = new SendMessageRequest()
                    {
                        QueueUrl = queueUrl,
                        MessageBody = message,
                    };

                    await client.SendMessageAsync(request);
                }
            }
        }

        public static async Task<string> Dequeue(string queueName)
        {
            return await DequeueByQueueUrl(await FindQueueUrlByQueueName(queueName));
        }

        public static async Task<string> DequeueByQueueUrl(string queueUrl)
        {
            using (var client = new AmazonSQSClient(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var request = new ReceiveMessageRequest()
                {
                    QueueUrl = queueUrl
                };

                var response = await client.ReceiveMessageAsync(request);
                var firstMessage = response.Messages.First();
                var body = firstMessage.Body;

                var deleteRequest = new DeleteMessageRequest
                {
                    QueueUrl = queueUrl,
                    ReceiptHandle = firstMessage.ReceiptHandle
                };

                await client.DeleteMessageAsync(deleteRequest);

                return body;
            }
        }

        public static async Task DeleteQueue(string queueName)
        {
            await DeleteQueueByQueueUrl(await FindQueueUrlByQueueName(queueName));
        }

        public static async Task DeleteQueueByQueueUrl(string queueUrl)
        {
            using (var client = new AmazonSQSClient(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                await client.DeleteQueueAsync(queueUrl);
            }
        }

        public static async Task DeleteAllQueues()
        {
            using (var client = new AmazonSQSClient(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var response = await client.ListQueuesAsync(new ListQueuesRequest());

                foreach (var queueUrl in response.QueueUrls)
                {
                    await PurgeQueueByQueueUrl(queueUrl);

                    await client.DeleteQueueAsync(queueUrl);
                }
            }
        }

        public static async Task PurgeQueue(string queueName)
        {
            await PurgeQueueByQueueUrl(await FindQueueUrlByQueueName(queueName));
        }

        public static async Task PurgeQueueByQueueUrl(string queueUrl)
        {
            using (var client = new AmazonSQSClient(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var request = new PurgeQueueRequest()
                {
                    QueueUrl = queueUrl
                };

                await client.PurgeQueueAsync(request);
            }
        }

        public static async Task<int> QueueLength(string queueName)
        {
            return await QueueLengthByQueueUrl(await FindQueueUrlByQueueName(queueName));
        }

        public static async Task<int> QueueLengthByQueueUrl(string queueUrl)
        {
            using (var client = new AmazonSQSClient(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var request = new GetQueueAttributesRequest
                {
                    QueueUrl = queueUrl,
                    AttributeNames = new List<string> { "ApproximateNumberOfMessages" }
                };

                var response = await client.GetQueueAttributesAsync(request);
                
                return response.ApproximateNumberOfMessages;
            }
        }

        public static async Task<bool> QueueHasItems(string queueName)
        {
            return await QueueHasItemsByQueueUrl(await FindQueueUrlByQueueName(queueName));
        }

        public static async Task<bool> QueueHasItemsByQueueUrl(string queueUrl)
        {
            return await QueueLengthByQueueUrl(queueUrl) > 0;
        }
    }
}
