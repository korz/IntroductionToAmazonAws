using System;
using System.Collections.Generic;
using System.Linq;
using DocumentDb.Data;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace DocumentDb
{
    class Program
    {
        private const string EndpointUrl = "endpoint";
        private const string AuthorizationKey = "authKey";

        private static DocumentClient _client;

        static void Main(string[] args)
        {
            _client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            var database = FindDatabase(_client, "erp");
            var collection = FindDocumentCollection(_client, database, "Customers");

            var repository = new CustomerRepository(_client, collection.SelfLink);

            var customer = new Customer()
            {
                Id = "BA761989-DA77-494D-B354-2CDB2DB420E2",
                FirstName = "John",
                LastName = "Smith",
                Email = "john@example.com",
                Orders = new List<Order>
                {
                    new Order
                    {
                        Number = "123-45", 
                        Date = new DateTime(2015, 1, 1), 
                        Amount = 245.78m
                    },
                    
                    new Order
                    {
                        Number = "123-46",
                        Date = new DateTime(2015, 1, 23),
                        Amount = 70.21m
                    },
                }
            };

            //Create Customer
            repository.Create(customer);

            //Find Customer
            //var found = repository.Read(customer.Id);

            //Update Customer
            //found.LastName = "Test";
            //repository.Update(customer.Id, found);

            //Delete Customer
            repository.Delete(customer.Id);
        }

        static Database FindDatabase(DocumentClient client, string name)
        {
            return client.CreateDatabaseQuery()
                .Where(x => x.Id == name)
                .ToList()
                .SingleOrDefault();
        }

        static DocumentCollection FindDocumentCollection(DocumentClient client, Database database, string name)
        {
            return client.CreateDocumentCollectionQuery(database.SelfLink)
                .Where(x => x.Id == name)
                .ToList()
                .SingleOrDefault();
        }
    }
}
