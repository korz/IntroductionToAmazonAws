namespace AmazonAws.S3
{
    class Program
    {
        private const string BucketName = "thatconference";

        static void Main(string[] args)
        {
            First();
            Second();
            Third();
            Fourth();
            Fifth();
        }

        static void First()
        {
            Repository.CreateBucket(BucketName);

            var buckets = Repository.FindBuckets();

            var bucket = Repository.FindBucketByName(BucketName);
        }

        static void Second()
        {
            var content = "Hello World";

            Repository.WriteToBucket(BucketName, "hello-world-key", content);

            var returnedContent = Repository.ReadFromBucket(BucketName, "hello-world-key");
        }

        static void Third()
        {
            Repository.WriteFileToBucket(BucketName, "Word.docx", @"Samples\Word.docx");
            Repository.WriteFileToBucket(BucketName, "Pdf.pdf", @"Samples\Pdf.pdf");
            Repository.WriteFileToBucket(BucketName, "Customers", @"Samples\Customer.json");

            Repository.DownloadFileFromBucket(BucketName, "Word.docx", @"Samples\Word-Downloaded.docx");
            Repository.DownloadFileFromBucket(BucketName, "Pdf.pdf", @"Samples\Pdf-Downloaded.pdf");
            Repository.DownloadFileFromBucket(BucketName, "Customers", @"Samples\Customer-Downloaded.json");
        }

        static void Fourth()
        {
            var itemsWithinBucket = Repository.ListFromBucket(BucketName);

            var itemCount = Repository.GetBucketItemCount(BucketName);

            var totalBucketSize = Repository.GetBucketSizeInBytes(BucketName);

            var wordFileSize = Repository.GetBucketItemSizeInBytes(BucketName, "Word.docx");
        }

        static void Fifth()
        {
            Repository.DeleteAllBucketItems(BucketName);
            Repository.DeleteBucket(BucketName);
        }
    }
}
