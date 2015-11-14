using System;

namespace DocumentDb.Data
{
    public class Order
    {
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}
