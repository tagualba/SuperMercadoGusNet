using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperGus.Dtos
{
    internal class ItemDto
    {
        public long barCode { get; set; }
        public String name { get; set; }
        public String shortName { get; set; }
        public int quantity { get; set; }
        public Double amount { get; set; }
    }
}
