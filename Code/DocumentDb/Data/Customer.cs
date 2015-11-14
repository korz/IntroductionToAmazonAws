using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DocumentDb.Data
{
    public class Customer
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public IList<Order> Orders { get; set; }

        public Customer()
        {
            Id = Guid.NewGuid().ToString();
            Orders = new List<Order>();
        }
    }
}
