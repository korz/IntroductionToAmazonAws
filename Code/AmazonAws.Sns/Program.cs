namespace AmazonAws.Sns
{
    class Program
    {
        static void Main(string[] args)
        {
            var topicArn = Repository.CreateTopic("korzSNS");

            Repository.CreateEmailSubscription(topicArn, "example@live.com");

            Repository.PublishMessage(topicArn, "Hello World");
        }
    }
}
