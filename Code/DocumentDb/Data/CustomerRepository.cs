using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace DocumentDb.Data
{
    public class CustomerRepository
    {
        private readonly DocumentClient _client;
        private readonly string _collectionLink;

        public CustomerRepository(DocumentClient client, string collectionLink)
        {
            _client = client;
            _collectionLink = collectionLink;
        }

        public void Create(Customer customer)
        {
            _client.CreateDocumentAsync(_collectionLink, customer).Wait();
        }

        public IList<Customer> Read()
        {
            return _client.CreateDocumentQuery<Customer>(_collectionLink)
                .AsEnumerable()
                .ToList();
        }

        public Customer Read(string customerId)
        {
            return _client.CreateDocumentQuery<Customer>(_collectionLink)
                .AsEnumerable()
                .FirstOrDefault(x => x.Id == customerId);
        }

        public void Update(string customerId, Customer customer)
        {
            var current = _client.CreateDocumentQuery(_collectionLink)
                .Where(x => x.Id == customerId)
                .AsEnumerable()
                .FirstOrDefault();

            _client.ReplaceDocumentAsync(current.SelfLink, customer).Wait();
        }

        public void Delete(string customerId)
        {
            var customer = _client.CreateDocumentQuery(_collectionLink)
                .Where(x => x.Id == customerId)
                .AsEnumerable()
                .FirstOrDefault();

            _client.DeleteDocumentAsync(customer.SelfLink).Wait();
        }
    }
}
