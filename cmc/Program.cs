using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Web;

namespace cmc
{
    class Program
    {
        static readonly string API_KEY = "";
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    var usdtPrice = CoinMarketCapAPICall();
                    Console.WriteLine(usdtPrice);
                    File.WriteAllText("price.txt", usdtPrice);

                }
                catch (WebException e)
                {
                    Console.WriteLine(e.Message);
                }
                Thread.Sleep(60000); //CoinMarketCap cache / update frequency: Every 60 seconds.
            }
        }

        static string CoinMarketCapAPICall()
        {
            var URL = new UriBuilder("https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest");

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["symbol"] = "USDT";
            queryString["convert"] = "BTC";

            URL.Query = queryString.ToString();

            var client = new WebClient();
            client.Headers.Add("X-CMC_PRO_API_KEY", API_KEY);
            client.Headers.Add("Accepts", "application/json");

            var result = client.DownloadString(URL.ToString());

            var USDT_BTC = JObject.Parse(result);
            var price = USDT_BTC["data"]["USDT"]["quote"]["BTC"]["price"].ToString();

            return price;
        }
    }
}
