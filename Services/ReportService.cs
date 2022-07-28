using SuperGus.Dtos;
using SuperGus.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperGus.Services
{
    internal class ReportService
    {
        private TransactionService transactionService;
        public ReportService()
        {
            transactionService = new TransactionService();
        }
        public void printTransaction(long id)
        {
            TransactionResponseDto res = transactionService.getTransaction(id);

            PrintUtil ticket = new PrintUtil();
       //     ticket.HeaderImage = Image.FromFile("C:\\Biller\\printer.png");
            ticket.AddHeaderLine("Super Mercado");
            ticket.AddSubHeaderLine("GENERADO POR:");
            ticket.AddSubHeaderLine("Hardach Gustavo Javier");
            ticket.AddSubHeaderLine("CUIT:");
            ticket.AddSubHeaderLine("23-21434171-9");
            ticket.AddSubHeaderLine(res.creationDate.ToString());
            ticket.AddSubHeaderLine("id: " + res.id.ToString());

            res.transactionitems.ForEach(itemTran => ticket.AddItem(itemTran.item.barCode.ToString(), itemTran.item.name, itemTran.quantity.ToString(), "$" + itemTran.item.amount.ToString(), "$" + itemTran.amount.ToString()));

            ticket.AddTotal("Total:", "$" + res.totalAmount.ToString());

            ticket.AddFooterLine("Gracias por elegirnos!");
            ticket.AddFooterLine("Brandsen 3285, ituzaingo");
            ticket.PrintTicket(); //impresora primaria por defecto
        }

        public void printCloseBox(List<TransactionResponseDto> transactions, String dateFrom, String dateTo)
        {
            PrintUtil ticket = new PrintUtil();
            ticket.HeaderImage = Image.FromFile("C:\\Biller\\printer.png");
            ticket.AddHeaderLine("Super Mercado");
            ticket.AddSubHeaderLine("GENERADO POR:");
            ticket.AddSubHeaderLine("Hardach Gustavo Javier");
            ticket.AddSubHeaderLine("CUIT:");
            ticket.AddSubHeaderLine("23-21434171-9");
            ticket.AddSubHeaderLine("Cierre de caja:");
            ticket.AddSubHeaderLine("Desde: " + dateFrom);
            ticket.AddSubHeaderLine("Hasta: " + dateTo);

            transactions.ForEach(t => t.transactionitems.ForEach(itemTran => ticket.AddItem(itemTran.item.barCode.ToString(), itemTran.item.shortName, itemTran.quantity.ToString(), "$" + itemTran.item.amount.ToString(), "$" + itemTran.amount.ToString())));
           
            ticket.AddTotal("Total Cierre:", "$" + transactions.Sum(t => t.totalAmount));
            ticket.AddFooterLine("Gracias por elegirnos!");
            ticket.AddFooterLine("Brandsen 3285, ituzaingo");
            ticket.PrintTicket(); //impresora primaria por defecto
        }
    }
}
