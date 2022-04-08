namespace AmazonAws.S3
{
    class Program
    {
        private const string BucketName = "korzs3";

        static async Task Main(string[] args)
        {
            await First();
            await Second();
            await Third();
            await Fourth();
            await Fifth();
        }

        static async Task First()
        {
            await Repository.CreateBucket(BucketName);

            var buckets = await Repository.FindBuckets();

            var bucket = await Repository.FindBucketByName(BucketName);
        }

        static async Task Second()
        {
            var content = "Hello World";

            await Repository.WriteToBucket(BucketName, "hello-world-key", content);

            var returnedContent = await Repository.ReadFromBucket(BucketName, "hello-world-key");
        }

        static async Task Third()
        {
            await Repository.WriteFileToBucket(BucketName, "Word.docx", @"Samples\Word.docx");
            await Repository.WriteFileToBucket(BucketName, "Pdf.pdf", @"Samples\Pdf.pdf");
            await Repository.WriteFileToBucket(BucketName, "Customers", @"Samples\Customer.json");

            await Repository.DownloadFileFromBucket(BucketName, "Word.docx", @"Samples\Word-Downloaded.docx");
            await Repository.DownloadFileFromBucket(BucketName, "Pdf.pdf", @"Samples\Pdf-Downloaded.pdf");
            await Repository.DownloadFileFromBucket(BucketName, "Customers", @"Samples\Customer-Downloaded.json");
        }

        static async Task Fourth()
        {
            var itemsWithinBucket = await Repository.ListFromBucket(BucketName);

            var itemCount = await Repository.GetBucketItemCount(BucketName);

            var totalBucketSize = await Repository.GetBucketSizeInBytes(BucketName);

            var wordFileSize = await Repository.GetBucketItemSizeInBytes(BucketName, "Word.docx");
        }

        static async Task Fifth()
        {
            await Repository.DeleteAllBucketItems(BucketName);
            await Repository.DeleteBucket(BucketName);
        }
    }
}
