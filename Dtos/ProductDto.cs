using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperGus.Dtos
{
    internal class ProductDto
    {
        public String barCode { get; set; }
        public String name { get; set; }
        public String nameTicket { get; set; }
        public int quantity { get; set; }
        public Double amount { get; set; }

        public ProductDto(String barCode, String name, String nameTicket, int quantity, Double amount)
        {
            this.barCode = barCode;
            this.name = name;
            this.nameTicket = nameTicket;
            this.quantity = quantity;
            this.amount = amount;
        }

        public ProductDto(String barCode, int quantity, Double amount)
        {
            this.barCode = barCode;
            this.quantity = quantity;
            this.amount = amount;
        }

        public ProductDto()
        {
            this.barCode = "";
            this.name = "";
            this.nameTicket = "";
            this.quantity = 0;
            this.amount = 0;
        }

        public ProductDto(ItemDto itemDto)
        {
            this.barCode = itemDto.barCode.ToString();
            this.name = itemDto.name;
            this.nameTicket = itemDto.shortName;
            this.quantity = itemDto.quantity; 
            this.amount = itemDto.amount;
        }


    }
}
