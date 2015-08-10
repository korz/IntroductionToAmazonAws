namespace AmazonAws.Sqs
{
    class Program
    {
        private const string QueueName = "thatconference";

        static void Main(string[] args)
        {
            Repository.CreateQueue(QueueName);

            var queueUrl = Repository.FindQueueUrlByQueueName(QueueName);

            var content = "hello world";

            Repository.EnqueueByQueueUrl(queueUrl, content);

            var result = Repository.Dequeue(QueueName);

            //Delete Queue
            //Purge Queue
            //Get length of queue
            //See if a queue has items
        }
    }
}
