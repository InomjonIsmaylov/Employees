using System;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;

namespace binance_0._1
{
    class Program
    {
        static void Main(string[] args)
        {
            //заходим на биржу
            string url = "https://lms.tuit.uz";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Console.WriteLine(httpWebResponse.StatusDescription);

            //перевод в джонсон
            string response;
            using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                response = streamReader.ReadToEnd();
            }

            //выцепляем из динамического джонсна пары и их назхвания
            int pr_btc = 0;
            int pr_usd = 0;
            dynamic stuff = JsonConvert.DeserializeObject(response);
            JObject jObject = (JObject)stuff["symbol"];
            List<string> pair_btc = new List<string>(); // массив коинов с BTC
            List<string> pair_usd = new List<string>(); // с пары usdt

            //выцепляем из динамического джонсoна пары и их назхвания
            foreach (var data in jObject)
            {

                //формируем массив пар с btc
                if ((data.Key.Contains("BTC")))
                {

                    pair_btc.Add(data.Key);
                    pr_btc++;

                }

                //формируем массив пар с usdt
                if ((data.Key.Contains("USDT")))
                {

                    pair_usd.Add(data.Key);
                    pr_usd++;
                }

                Console.WriteLine(jObject);
                Console.ReadLine();

            }
        }
    }
}