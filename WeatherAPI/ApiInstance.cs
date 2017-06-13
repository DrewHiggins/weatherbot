using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WeatherBot.ExtensionMethods;

namespace WeatherBot.WeatherAPI
{
    public class ApiInstance
    {
        private string _key = "10359e1adcbae61133eb1a8f1ef47561";

        public ApiInstance()
        {
        }

        public string GetUrl(string loc) => $"http://api.openweathermap.org/data/2.5/weather?q={loc.GetHTTPEncoded()}&appid={_key}";

        public WeatherReport GetWeatherReport(string location)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetUrl(location));
            var res = request.GetResponse();
            Stream responseStream = res.GetResponseStream();
            var reader = new StreamReader(responseStream);
            string responseJsonStr = reader.ReadToEnd();
            JObject response = JObject.Parse(responseJsonStr);
            double temperature = double.Parse(response["main"]["temp"].ToString());
            string report = response["weather"][0]["main"].ToString();
            return new WeatherReport(temperature, report);
        }
    }
}