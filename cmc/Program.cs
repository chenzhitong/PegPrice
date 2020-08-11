using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Timers;
using System.Web;

namespace cmc
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = new Timer(new TimeSpan(0, 10, 0).TotalMilliseconds);
            t.Elapsed += GetPrice;
            t.Start();
            GetPrice(null, null);

            var t2 = new Timer(new TimeSpan(0, 1, 0).TotalMilliseconds);
            t2.Elapsed += GetBalance;
            t2.Start();
            GetBalance(null, null);

            Console.ReadLine();
        }

        private static void GetPrice(object sender, ElapsedEventArgs e)
        {
            try
            {
                var key = JObject.Parse(File.ReadAllText("config.json"))["key"].ToString();
                var usdt_btc = CoinMarketCapAPICall("USDT", "BTC", key);
                var usdt_usd = CoinMarketCapAPICall("USDT", "USD", key);
                var json = new JObject { { "USDT-BTC", usdt_btc }, { "USDT-USD", usdt_usd }, { "DateTime", DateTime.Now.ToString() } };
                Console.WriteLine(json);
                File.WriteAllText("price.txt", json.ToString());
            }
            catch (WebException ex)
            {
                Console.WriteLine($"CoinMarketCapAPICall\t{ex.Message}");
            }
        }

        private static void GetBalance(object sender, ElapsedEventArgs e)
        {
            try
            {
                var json = new JObject { { "USDT", GetUsdtBalance() }, { "PEG", GetPegBalance() }, { "DateTime", DateTime.Now.ToString() } };
                Console.WriteLine(json);
                File.WriteAllText("wallet.txt", json.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static string CoinMarketCapAPICall(string token, string convert, string apiKey)
        {
            var URL = new UriBuilder("https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest");

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["symbol"] = token;
            queryString["convert"] = convert;

            URL.Query = queryString.ToString();

            var client = new WebClient();
            client.Headers.Add("X-CMC_PRO_API_KEY", apiKey);
            client.Headers.Add("Accepts", "application/json");

            var result = client.DownloadString(URL.ToString());

            var USDT_BTC = JObject.Parse(result);
            var price = USDT_BTC["data"][token]["quote"][convert]["price"].ToString();

            return price;
        }

        static string GetPegBalance()
        {
            try
            {
                var url = JObject.Parse(File.ReadAllText("config.json"))["NeoURL"].ToString();
                var response = PostWebRequest($"{url}", "{\"jsonrpc\": \"2.0\",\"method\": \"getPegBalance\",\"params\": [],\"id\": 1}");
                return JObject.Parse(response)["result"]["balance"].ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine($"GetPegBalance\t{e.Message}");
                return "0";
            }
        }

        static string GetUsdtBalance()
        {
            try
            {
                var url = JObject.Parse(File.ReadAllText("config.json"))["EthURL"].ToString();
                var response = new WebClient().DownloadString($"{url}/Eth/BalanceOfWallet");
                return JObject.Parse(response)["data"].ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine($"GetUsdtBalance\t{e.Message}");
                return "0";
            }
        }

        public static string PostWebRequest(string postUrl, string paramData)
        {
            try
            {
                var byteArray = Encoding.UTF8.GetBytes(paramData);
                var request = WebRequest.Create(postUrl);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.GetRequestStream().Write(byteArray, 0, byteArray.Length);
                using var response = request.GetResponse();
                using var sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                return sr.ReadToEnd();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
