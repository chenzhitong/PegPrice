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
        static string NeoURL;
        static string EthURL;
        static void Main(string[] args)
        {
            var config = JObject.Parse(File.ReadAllText("config.json"));
            API_KEY = config["key"].ToString();
            NeoURL = config["NeoURL"].ToString();
            EthURL = config["EthURL"].ToString();

            while (true)
            {
                try
                {
                    var usdt_btc = CoinMarketCapAPICall("USDT", "BTC");
                    var usdt_usd = CoinMarketCapAPICall("USDT", "USD");
                    var json = new JObject { { "USDT-BTC", usdt_btc }, { "USDT-USD", usdt_usd }, { "DateTime", DateTime.Now.ToString() } };
                    Console.WriteLine(json);
                    File.WriteAllText("price.txt", json.ToString());
                }
                catch (WebException e)
                {
                    Console.WriteLine(e.Message);
                }
                try
                {
                    var json = new JObject { { "USDT", GetUsdtBalance() }, { "PEG", GetPegBalance() }, { "DateTime", DateTime.Now.ToString() } };
                    Console.WriteLine(json);
                    File.WriteAllText("wallet.txt", json.ToString());
                }
                catch (Exception e)
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

        static string GetPegBalance()
        {
            var response = new WebClient().DownloadString($"{NeoURL}?jsonrpc=2.0&method=getbalance&params=['075383de6638e042efc4bd3daca4ceb516ec1c6b']&id=1");
            return JObject.Parse(response)["result"]["balance"].ToString();
        }

        static string GetUsdtBalance()
        {
            var response = new WebClient().DownloadString($"{EthURL}/Eth/BalanceOfWallet");
            return JObject.Parse(response)["data"].ToString();
        }
    }
}
