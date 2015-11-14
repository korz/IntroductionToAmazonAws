using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace DocumentDb.CreateDatabase
{
    class Program
    {
        private const string EndpointUrl = "endpoint";
        private const string AuthorizationKey = "authKey";

        private static DocumentClient _client;

        static void Main(string[] args)
        {
            _client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            var database = CreateDatabase("erp");

            var collection = CreateCollection(database, "customers");
        }

        static Database CreateDatabase(string name)
        {
            return _client.CreateDatabaseAsync(new Database { Id = name }).Result;
        }

        static DocumentCollection CreateCollection(Database database, string collectionName)
        {
            return _client.CreateDocumentCollectionAsync(database.CollectionsLink, new DocumentCollection { Id = collectionName }).Result;
        }
    }
}
