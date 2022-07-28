using Nancy.Json;
using SuperGus.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SuperGus.Services
{
    internal class TransactionService
    {
        private const String itemTranApiProductUrl = "http://localhost:8080/biller/transactions";

        public TransactionDto createTransaction(TransactionDto transactionDto)
        {
            String url = itemTranApiProductUrl;
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            String data = "{\"totalAmount\": @TOTAL_AMOUNT, \"transactionItems\" : [@PRODUCTS]}";

            String product = "{\"barCode\": @BARCODE, \"quantity\" : @QUANTITY, \"amount\" : @AMOUNT}";

            String productsData = String.Join(",", transactionDto.products.Select(p => product.Replace("@BARCODE", p.barCode).Replace("@QUANTITY", p.quantity.ToString()).Replace("@AMOUNT", p.amount.ToString().Replace(",", "."))).ToList());
    
            data = data.Replace("@TOTAL_AMOUNT", transactionDto.totalAmount.ToString().Replace(",", "."));
            data = data.Replace("@PRODUCTS", productsData);

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(data);
                streamWriter.Flush();
                streamWriter.Close();
            }
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        if (strReader == null) return null;
                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            string responseBody = objReader.ReadToEnd();

                            return new JavaScriptSerializer().Deserialize<TransactionDto>(responseBody);
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public TransactionResponseDto getTransaction(long id)
        {
            String url = itemTranApiProductUrl + "/" + id;
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        if (strReader == null) return null;
                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            string responseBody = objReader.ReadToEnd();

                            TransactionResponseDto itemDto = new JavaScriptSerializer().Deserialize<TransactionResponseDto>(responseBody);

                            return itemDto;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        internal List<TransactionResponseDto> getFromToDate(string fromDate, string toDate)
        {
            String url = itemTranApiProductUrl + "/report?dateFrom="+ fromDate + "&dateTo="+ toDate;
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        if (strReader == null) return null;
                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            string responseBody = objReader.ReadToEnd();

                            return new JavaScriptSerializer().Deserialize<List<TransactionResponseDto>>(responseBody);
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
