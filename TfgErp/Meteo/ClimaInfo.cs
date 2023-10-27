using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TfgErp.Meteo
{

    public class ClimaInfo
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string TimeZone { get; set; }
        public string Elevation { get; set; }
        public List<DateTime> HourlyTime { get; set; }
        public List<double> HourlyTemperature { get; set; }


        // Constructor para inicializar la clase con datos
        public ClimaInfo(double latitude, double longitude, string timeZone, string elevation, List<DateTime> hourlyTime, List<double> hourlyTemperature)
        {
            Latitude = latitude;
            Longitude = longitude;
            TimeZone = timeZone;
            Elevation = elevation;
            HourlyTime = hourlyTime;
            HourlyTemperature = hourlyTemperature;
        }

        // Método estático para obtener datos climáticos desde la API
        public static async Task<ClimaInfo> ObtenerDatosClimaticosAsync()
        {
            string apiUrl = "https://api.open-meteo.com/v1/forecast?latitude=41.6561&longitude=-0.8773&hourly=temperature_2m";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<JObject>(json);
                        JObject jsonObject = JObject.Parse(json);

                        double latitude = (double)jsonObject["latitude"];
                        double longitude = (double)jsonObject["longitude"];
                        string timeZone = (string)jsonObject["timezone"];
                        string elevation = (string)jsonObject["elevation"];

                        List<DateTime> hourlyTime = data["hourly"]["time"]
                            .Select(token => DateTime.Parse((string)token))
                            .ToList();

                        List<double> hourlyTemperature = JsonConvert.DeserializeObject<List<double>>(jsonObject["hourly"]["temperature_2m"].ToString());
                        return new ClimaInfo(latitude, longitude, timeZone, elevation, hourlyTime, hourlyTemperature);
                    }
                    else
                    {
                        Console.WriteLine("Error al obtener los datos climáticos.");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return null;
                }
            }
        }
    }
}


