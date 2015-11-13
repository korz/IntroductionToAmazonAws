using System.Collections.Generic;
using System.Linq;
using Amazon.S3;
using Amazon.S3.Model;
using AmazonAws.Shared;

namespace AmazonAws.S3
{
    public static class Repository
    {
        public static IEnumerable<string> FindBuckets()
        {
            using (var client = new AmazonS3Client(Settings.AccessKey, Settings.Secret))
            {
                var response = client.ListBuckets();

                foreach (var bucket in response.Buckets)
                {
                    yield return bucket.BucketName;
                }
            }
        }

        public static S3Bucket FindBucketByName(string name)
        {
            using (var client = new AmazonS3Client(Settings.AccessKey, Settings.Secret))
            {
                var response = client.ListBuckets();

                foreach (var bucket in response.Buckets.Where(x => x.BucketName == name))
                {
                    return bucket;
                }
            }

            return null;
        }

        public static void CreateBucket(string name)
        {
            using (var client = new AmazonS3Client(Settings.AccessKey, Settings.Secret))
            {
                var request = new PutBucketRequest
                {
                    BucketName = name,
                    BucketRegionName = Settings.Region
                };

                client.PutBucket(request);
            }
        }

        public static void WriteToBucket(string bucketName, string key, string content)
        {
            using (var client = new AmazonS3Client(Settings.AccessKey, Settings.Secret))
            {
                var request = new PutObjectRequest()
                {
                    ContentBody = content,
                    BucketName = bucketName,
                    Key = key
                };

                client.PutObject(request);
            }
        }

        public static string ReadFromBucket(string bucketName, string key)
        {
            using (var client = new AmazonS3Client(Settings.AccessKey, Settings.Secret))
            {
                var request = new GetObjectRequest()
                {
                    BucketName = bucketName,
                    Key = key
                };

                return client.GetObject(request).ResponseStream.ToContentString();
            }
        }

        public static void DeleteFromBucket(string bucketName, string key)
        {
            using (var client = new AmazonS3Client(Settings.AccessKey, Settings.Secret))
            {
                var request = new DeleteObjectRequest()
                {
                    BucketName = bucketName,
                    Key = key
                };

                client.DeleteObject(request);
            }
        }

        public static IEnumerable<string> ListFromBucket(string bucketName)
        {
            using (var client = new AmazonS3Client(Settings.AccessKey, Settings.Secret))
            {
                var request = new ListObjectsRequest
                {
                    BucketName = bucketName
                };

                var response = client.ListObjects(request);

                foreach (var entry in response.S3Objects)
                {
                    yield return entry.Key;
                }
            }
        }

        public static void DeleteBucket(string bucketName)
        {
            using (var client = new AmazonS3Client(Settings.AccessKey, Settings.Secret))
            {
                client.DeleteBucket(bucketName);
            }
        }

        public static void DeleteAllBuckets()
        {
            using (var client = new AmazonS3Client(Settings.AccessKey, Settings.Secret))
            {
                var response = client.ListBuckets();

                foreach (var bucket in response.Buckets)
                {
                    DeleteAllBucketItems(bucket.BucketName);

                    client.DeleteBucket(bucket.BucketName);
                }
            }
        }

        public static void DeleteAllBucketItems(string bucketName)
        {
            using (var client = new AmazonS3Client(Settings.AccessKey, Settings.Secret))
            {
                var request = new ListObjectsRequest
                {
                    BucketName = bucketName
                };

                var response = client.ListObjects(request);

                foreach (var entry in response.S3Objects)
                {
                    client.DeleteObject(bucketName, entry.Key);
                }
            }
        }

        public static int GetBucketItemCount(string bucketName)
        {
            using (var client = new AmazonS3Client(Settings.AccessKey, Settings.Secret))
            {
                var request = new ListObjectsRequest
                {
                    BucketName = bucketName
                };

                var count = 0;

                var response = client.ListObjects(request);

                count = count + response.S3Objects.Count();

                while (response.IsTruncated)
                {
                    request.Marker = response.NextMarker;

                    response = client.ListObjects(request);

                    count = count + response.S3Objects.Count();
                }

                return count;
            }
        }

        public static long GetBucketSizeInBytes(string bucketName)
        {
            using (var client = new AmazonS3Client(Settings.AccessKey, Settings.Secret))
            {
                var request = new ListObjectsRequest
                {
                    BucketName = bucketName
                };

                long size = 0;

                var response = client.ListObjects(request);

                size = size + response.S3Objects.Sum(x => x.Size);

                while (response.IsTruncated)
                {
                    request.Marker = response.NextMarker;

                    response = client.ListObjects(request);

                    size = size + response.S3Objects.Sum(x => x.Size);
                }

                return size;
            }
        }

        public static long GetBucketItemSizeInBytes(string bucketName, string key)
        {
            using (var client = new AmazonS3Client(Settings.AccessKey, Settings.Secret))
            {
                var request = new ListObjectsRequest
                {
                    BucketName = bucketName,
                    Prefix = key
                };

                var response = client.ListObjects(request).S3Objects.FirstOrDefault();
                if (response == null) return 0;

                return response.Size;
            }
        }

        public static void WriteFileToBucket(string bucketName, string key, string filename)
        {
            using (var client = new AmazonS3Client(Settings.AccessKey, Settings.Secret))
            {
                var request = new PutObjectRequest()
                {
                    FilePath = filename,
                    BucketName = bucketName,
                    Key = key
                };

                client.PutObject(request);
            }
        }

        public static void DownloadFileFromBucket(string bucketName, string key, string filename)
        {
            using (var client = new AmazonS3Client(Settings.AccessKey, Settings.Secret))
            {
                var request = new GetObjectRequest()
                {
                    BucketName = bucketName,
                    Key = key
                };

                using (var response = client.GetObject(request))
                {
                    response.WriteResponseStreamToFile(filename);
                }
            }
        }
    }
}
