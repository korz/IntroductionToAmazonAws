namespace AmazonAws.Sns
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var topicArn = await Repository.CreateTopic("korzSNS");

            await Repository.CreateEmailSubscription(topicArn, "example@live.com");

            await Repository.PublishMessage(topicArn, "Hello World");
        }
    }
}
