namespace AmazonAws.Sns
{
    class Program
    {
        static void Main(string[] args)
        {
            var topicArn = Repository.CreateTopic("thatconference");

            Repository.CreateEmailSubscription(topicArn, "example@live.com");

            Repository.PublishMessage(topicArn, "Hello That Conference");
        }
    }
}
