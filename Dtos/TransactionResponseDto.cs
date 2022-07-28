using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperGus.Dtos
{
    internal class TransactionResponseDto
    {
        public int id { get; set; }
        public Double totalAmount { get; set; }
        public DateTime creationDate { get; set; }
        public List<TransactionItemDto> transactionitems { get; set; }



    }
}
