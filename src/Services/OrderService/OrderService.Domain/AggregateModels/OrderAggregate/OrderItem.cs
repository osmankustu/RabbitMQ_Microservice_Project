using OrderService.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.AggregateModels.OrderAggregate
{
    public class Orderitem : BaseEntity, IValidatableObject
    {

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal UnitPrice { get; set; }

        public int Units { get; set; }

        protected Orderitem()
        {

        }

        public Orderitem(int productId, string productName, decimal unitPrice, int units)
        {
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Units = units;
        }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (Units <= 0) results.Add(new ValidationResult("Invalid number of units", new[] { "Units" }));

            return results;
        }
    }
}
