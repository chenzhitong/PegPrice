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
        static string API_KEY;
        static void Main(string[] args)
        {
            API_KEY = File.ReadAllText("config.txt");
            while (true)
            {
                try
                {
                    var usdt_btc = CoinMarketCapAPICall("USDT", "BTC");
                    var usdt_usd = CoinMarketCapAPICall("USDT", "USD");
                    var json = new JObject { { "USDT-BTC", usdt_btc }, { "USDT-USD", usdt_usd } };
                    Console.WriteLine(json);
                    File.WriteAllText("price.txt", json.ToString());
                }
                catch (WebException e)
                {
                    Console.WriteLine(e.Message);
                }
                Thread.Sleep(60000); //CoinMarketCap cache / update frequency: Every 60 seconds.
            }
        }

        static string CoinMarketCapAPICall(string token, string convert)
        {
            var URL = new UriBuilder("https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest");

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["symbol"] = token;
            queryString["convert"] = convert;

            URL.Query = queryString.ToString();

            var client = new WebClient();
            client.Headers.Add("X-CMC_PRO_API_KEY", API_KEY);
            client.Headers.Add("Accepts", "application/json");

            var result = client.DownloadString(URL.ToString());

            var USDT_BTC = JObject.Parse(result);
            var price = USDT_BTC["data"][token]["quote"][convert]["price"].ToString();

            return price;
        }
    }
}
