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
    internal class ProductService
    {
        private const String itemTranApiProductUrl = "http://localhost:8080/biller/items";
        internal ProductDto getProduct(string barCode)
        {
            String url = itemTranApiProductUrl + "/" + barCode;
            var request = (HttpWebRequest) WebRequest.Create(url);
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
                            
                            ItemDto itemDto = new JavaScriptSerializer().Deserialize<ItemDto>(responseBody);

                            return new ProductDto(itemDto);
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
       

        internal ProductDto save(ProductDto product)
        {

            String url = itemTranApiProductUrl;
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            String data = "{\"barCode\": @BARCODE, \"name\" : \"@NAME\",\"shortName\" : \"@SHORT_NAME\", \"quantity\" : @QUANTITY, \"amount\" : @AMOUNT}";

            data = data.Replace("@BARCODE", product.barCode);
            data = data.Replace("@NAME", product.name);
            data = data.Replace("@SHORT_NAME", product.nameTicket);
            data = data.Replace("@QUANTITY", product.quantity.ToString());
            data = data.Replace("@AMOUNT", product.amount.ToString());

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

                            ItemDto itemDto = new JavaScriptSerializer().Deserialize<ItemDto>(responseBody);

                            return new ProductDto(itemDto);
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

        internal List<ProductDto> searchProductsByName(string name)
        {
                String url = itemTranApiProductUrl + "/list?name=" + name;
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

                                List<ItemDto> itemDto = new JavaScriptSerializer().Deserialize<List<ItemDto>>(responseBody);

                                return itemDto.Select(item => new ProductDto(item)).ToList();

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
