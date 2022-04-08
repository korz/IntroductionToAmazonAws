namespace AmazonAws.Sqs
{
    class Program
    {
        private const string QueueName = "korzSQS";

        static async Task Main(string[] args)
        {
            await Repository.CreateQueue(QueueName);

            var queueUrl = await Repository.FindQueueUrlByQueueName(QueueName);

            var content = "hello world";

            await Repository.EnqueueByQueueUrl(queueUrl, content);

            var result = await Repository.Dequeue(QueueName);

            //Delete Queue
            //Purge Queue
            //Get length of queue
            //See if a queue has items
        }
    }
}
