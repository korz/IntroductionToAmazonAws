namespace AmazonAws.Ses
{
    class Program
    {
        static void Main(string[] args)
        {
            Repository.Send("example@live.com",
                "example@live.com", null,"Subject",
                "Hello World");
        }
    }
}
