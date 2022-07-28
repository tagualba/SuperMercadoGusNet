using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperGus.Dtos
{
    internal class TransactionItemDto
    {
        public long id { get; set; }
        public ItemDto item { get; set; }
        public int quantity { get; set; }
        public Double amount { get; set; }

        public TransactionItemDto(long id, ItemDto item, int quantity, Double amount)
        {
            this.id = id; 
            this.quantity = quantity;   
            this.amount = amount;
            this.item = item;
        }
    }
}
