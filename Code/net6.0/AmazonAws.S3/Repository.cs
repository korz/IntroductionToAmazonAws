using Amazon.S3;
using Amazon.S3.Model;
using AmazonAws.Shared;

namespace AmazonAws.S3
{
    public static class Repository
    {
        public static async Task<IEnumerable<string>> FindBuckets()
        {
            using (var client = new AmazonS3Client(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var response = await client.ListBucketsAsync();

                return response.Buckets?.Select(x => x.BucketName).ToList() ?? new List<string>();
            }
        }

        public static async Task<S3Bucket?> FindBucketByName(string name)
        {
            using (var client = new AmazonS3Client(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var response = await client.ListBucketsAsync();

                foreach (var bucket in response.Buckets.Where(x => x.BucketName == name))
                {
                    return bucket;
                }
            }

            return null;
        }

        public static async Task CreateBucket(string name)
        {
            using (var client = new AmazonS3Client(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var request = new PutBucketRequest
                {
                    BucketName = name,
                    //BucketRegion = S3Region.USWest2
                    //BucketRegionName = ConfigManager.ConfigSettings.Region
                };

                await client.PutBucketAsync(request);
            }
        }

        public static async Task WriteToBucket(string bucketName, string key, string content)
        {
            using (var client = new AmazonS3Client(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var request = new PutObjectRequest()
                {
                    ContentBody = content,
                    BucketName = bucketName,
                    Key = key
                };

                await client.PutObjectAsync(request);
            }
        }

        public static async Task<string> ReadFromBucket(string bucketName, string key)
        {
            using (var client = new AmazonS3Client(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var request = new GetObjectRequest()
                {
                    BucketName = bucketName,
                    Key = key
                };
                var response = await client.GetObjectAsync(request);
                return response.ResponseStream.ToContentString();
            }
        }

        public static async Task DeleteFromBucket(string bucketName, string key)
        {
            using (var client = new AmazonS3Client(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var request = new DeleteObjectRequest()
                {
                    BucketName = bucketName,
                    Key = key
                };

                await client.DeleteObjectAsync(request);
            }
        }

        public static async Task<IEnumerable<string>> ListFromBucket(string bucketName)
        {
            using (var client = new AmazonS3Client(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var request = new ListObjectsRequest
                {
                    BucketName = bucketName
                };

                var response = await client.ListObjectsAsync(request);

                return response.S3Objects.Select(x => x.Key).ToList();
            }
        }

        public static async Task DeleteBucket(string bucketName)
        {
            using (var client = new AmazonS3Client(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                await client.DeleteBucketAsync(bucketName);
            }
        }

        public static async Task DeleteAllBuckets()
        {
            using (var client = new AmazonS3Client(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var response = await client.ListBucketsAsync();

                foreach (var bucket in response.Buckets)
                {
                    await DeleteAllBucketItems(bucket.BucketName);

                    await client.DeleteBucketAsync(bucket.BucketName);
                }
            }
        }

        public static async Task DeleteAllBucketItems(string bucketName)
        {
            using (var client = new AmazonS3Client(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var request = new ListObjectsRequest
                {
                    BucketName = bucketName
                };

                var response = await client.ListObjectsAsync(request);

                foreach (var entry in response.S3Objects)
                {
                    await client.DeleteObjectAsync(bucketName, entry.Key);
                }
            }
        }

        public static async Task<int> GetBucketItemCount(string bucketName)
        {
            using (var client = new AmazonS3Client(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var request = new ListObjectsRequest
                {
                    BucketName = bucketName
                };

                var count = 0;

                var response = await client.ListObjectsAsync(request);

                count = count + response.S3Objects.Count();

                while (response.IsTruncated)
                {
                    request.Marker = response.NextMarker;

                    response = await client.ListObjectsAsync(request);

                    count = count + response.S3Objects.Count();
                }

                return count;
            }
        }

        public static async Task<long> GetBucketSizeInBytes(string bucketName)
        {
            using (var client = new AmazonS3Client(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var request = new ListObjectsRequest
                {
                    BucketName = bucketName
                };

                long size = 0;

                var response = await client.ListObjectsAsync(request);

                size = size + response.S3Objects.Sum(x => x.Size);

                while (response.IsTruncated)
                {
                    request.Marker = response.NextMarker;

                    response = await client.ListObjectsAsync(request);

                    size = size + response.S3Objects.Sum(x => x.Size);
                }

                return size;
            }
        }

        public static async Task<long> GetBucketItemSizeInBytes(string bucketName, string key)
        {
            using (var client = new AmazonS3Client(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var request = new ListObjectsRequest
                {
                    BucketName = bucketName,
                    Prefix = key
                };

                var response = await client.ListObjectsAsync(request);
                var firstObject = response.S3Objects.FirstOrDefault();
                if (firstObject == null) return 0;

                return firstObject.Size;
            }
        }

        public static async Task WriteFileToBucket(string bucketName, string key, string filename)
        {
            using (var client = new AmazonS3Client(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var request = new PutObjectRequest()
                {
                    FilePath = filename,
                    BucketName = bucketName,
                    Key = key
                };

                await client.PutObjectAsync(request);
            }
        }

        public static async Task DownloadFileFromBucket(string bucketName, string key, string filename)
        {
            using (var client = new AmazonS3Client(ConfigManager.ConfigSettings.AccessKey, ConfigManager.ConfigSettings.Secret, Amazon.RegionEndpoint.USWest2))
            {
                var request = new GetObjectRequest()
                {
                    BucketName = bucketName,
                    Key = key
                };

                using (var response = await client.GetObjectAsync(request))
                {
                    await response.WriteResponseStreamToFileAsync(filename, false, CancellationToken.None);
                }
            }
        }
    }
}
