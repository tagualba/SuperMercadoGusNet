using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperGus.Dtos
{
    internal class TransactionDto
    {
        public int id { get; set; }
        public List<ProductDto> products { get; set; } 

        public Double totalAmount { get; set; }

        public TransactionDto() { }
        public TransactionDto(List<ProductDto> products, Double totalAmount)
        {
            this.products = products;
            this.totalAmount = totalAmount;
        }

    }
}
