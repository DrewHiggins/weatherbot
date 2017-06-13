namespace WeatherBot.WeatherAPI
{
    public class WeatherReport
    {
        public double Temperature { get; private set; }
        public string BriefReport { get; private set; }

        public WeatherReport(double temperature, string briefReport)
        {
            Temperature = convertKelvinToFahrenheit(temperature);
            BriefReport = briefReport;
        }

        private double convertKelvinToFahrenheit(double degreesKelvin)
        {
            return (9 / 5.0)*(degreesKelvin - 273.15) + 32;
        }
    }
}