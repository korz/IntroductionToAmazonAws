using System.Collections.Generic;
using System.Linq;
using Amazon.SQS;
using Amazon.SQS.Model;
using AmazonAws.Shared;

namespace AmazonAws.Sqs
{
    public class Repository
    {
        public static IEnumerable<string> FindQueueUrls()
        {
            using (var client = new AmazonSQSClient(Settings.AccessKey, Settings.Secret))
            {
                var response = client.ListQueues(new ListQueuesRequest());

                foreach (var queueUrl in response.QueueUrls)
                {
                    yield return queueUrl;
                }
            }
        }

        public static string FindQueueUrlByQueueName(string name)
        {
            using (var client = new AmazonSQSClient(Settings.AccessKey, Settings.Secret))
            {
                ListQueuesResponse response = client.ListQueues(new ListQueuesRequest(name));

                foreach (string queueUrl in response.QueueUrls)
                {
                    return queueUrl;
                }
            }

            return null;
        }

        public static string CreateQueue(string name)
        {
            using (var client = new AmazonSQSClient(Settings.AccessKey, Settings.Secret))
            {
                var request = new CreateQueueRequest
                {
                    QueueName = name
                };

                var response = client.CreateQueue(request);

                return response.QueueUrl;
            }
        }

        public static void Enqueue(string queueName, string message)
        {
            EnqueueByQueueUrl(FindQueueUrlByQueueName(queueName), message);
        }

        public static void EnqueueByQueueUrl(string queueUrl, string message)
        {
            using (var client = new AmazonSQSClient(Settings.AccessKey, Settings.Secret))
            {
                var request = new SendMessageRequest()
                {
                    QueueUrl = queueUrl,
                    MessageBody = message,
                };

                client.SendMessage(request);
            }
        }

        public static void EnqueueList(string queueName, IEnumerable<string> messages)
        {
            EnqueueListByQueueUrl(FindQueueUrlByQueueName(queueName), messages);
        }

        public static void EnqueueListByQueueUrl(string queueUrl, IEnumerable<string> messages)
        {
            using (var client = new AmazonSQSClient(Settings.AccessKey, Settings.Secret))
            {
                foreach (var message in messages)
                {
                    var request = new SendMessageRequest()
                    {
                        QueueUrl = queueUrl,
                        MessageBody = message,
                    };

                    client.SendMessage(request);
                }
            }
        }

        public static string Dequeue(string queueName)
        {
            return DequeueByQueueUrl(FindQueueUrlByQueueName(queueName));
        }

        public static string DequeueByQueueUrl(string queueUrl)
        {
            using (var client = new AmazonSQSClient(Settings.AccessKey, Settings.Secret))
            {
                var request = new ReceiveMessageRequest()
                {
                    QueueUrl = queueUrl
                };

                var response = client.ReceiveMessage(request).Messages.First();

                var body = response.Body;

                var deleteRequest = new DeleteMessageRequest
                {
                    QueueUrl = queueUrl,
                    ReceiptHandle = response.ReceiptHandle
                };

                client.DeleteMessage(deleteRequest);

                return body;
            }
        }

        public static void DeleteQueue(string queueName)
        {
            DeleteQueueByQueueUrl(FindQueueUrlByQueueName(queueName));
        }

        public static void DeleteQueueByQueueUrl(string queueUrl)
        {
            using (var client = new AmazonSQSClient(Settings.AccessKey, Settings.Secret))
            {
                client.DeleteQueue(queueUrl);
            }
        }

        public static void DeleteAllQueues()
        {
            using (var client = new AmazonSQSClient(Settings.AccessKey, Settings.Secret))
            {
                var response = client.ListQueues(new ListQueuesRequest());

                foreach (var queueUrl in response.QueueUrls)
                {
                    PurgeQueueByQueueUrl(queueUrl);

                    client.DeleteQueue(queueUrl);
                }
            }
        }

        public static void PurgeQueue(string queueName)
        {
            PurgeQueueByQueueUrl(FindQueueUrlByQueueName(queueName));
        }

        public static void PurgeQueueByQueueUrl(string queueUrl)
        {
            using (var client = new AmazonSQSClient(Settings.AccessKey, Settings.Secret))
            {
                var request = new PurgeQueueRequest()
                {
                    QueueUrl = queueUrl
                };

                client.PurgeQueue(request);
            }
        }

        public static int QueueLength(string queueName)
        {
            return QueueLengthByQueueUrl(FindQueueUrlByQueueName(queueName));
        }

        public static int QueueLengthByQueueUrl(string queueUrl)
        {
            using (var client = new AmazonSQSClient(Settings.AccessKey, Settings.Secret))
            {
                var request = new GetQueueAttributesRequest
                {
                    QueueUrl = queueUrl,
                    AttributeNames = new List<string> { "ApproximateNumberOfMessages" }
                };

                return client.GetQueueAttributes(request).ApproximateNumberOfMessages;
            }
        }

        public static bool QueueHasItems(string queueName)
        {
            return QueueHasItemsByQueueUrl(FindQueueUrlByQueueName(queueName));
        }

        public static bool QueueHasItemsByQueueUrl(string queueUrl)
        {
            return QueueLengthByQueueUrl(queueUrl) > 0;
        }
    }
}
