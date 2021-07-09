using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TruckinWebServices.DataModel;
using System.Device.Location;

namespace TruckinWebServices.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ILogger<SearchController> _logger;


        private static string url = $"https://data.sfgov.org/api/odata/v4/rqzj-sfat";  //SFO endpoint.
        private static double ThresholdMetersDefault = 800;  //About a half mile.
        private static double ThresholdMetersBuffer = 150;  //A somewhat arbitrary buffer beyond the suggested threshold for display radius.

        public SearchController(ILogger<SearchController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Trucks(double startLat, double startLong, bool truncate)
        {
            //Calculate a recommended distance threshold of the 5th nearest truck plus a small buffer.  
            Truck[] trucks = GetTrucksByDistanceAsync(startLat, startLong).Result.ToArray();
            double threshold = ThresholdMetersDefault; 
            if (trucks.Length > 0)
            {
                threshold = trucks[Math.Min(4, trucks.Length - 1)].DistanceFromStartingPoint + ThresholdMetersBuffer;
            }

            //Conditionally truncate response to only include those within the threshold.
            if (truncate)
            {
                trucks = trucks.Where(t => t.DistanceFromStartingPoint <= threshold).ToArray();
            }

            //Serialize response.
            GetTrucksByDistanceResponse response = new GetTrucksByDistanceResponse()
            {
                CurrentLocationLatitude = startLat,
                CurrentLocationLongitude = startLong,
                ThresholdFromStartingPoint = threshold, 
                Trucks = trucks
            };

            return JsonSerializer.Serialize(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startLat">The latitute of the starting point.</param>
        /// <param name="startLong">The longitude of the starting point.</param>
        /// <returns></returns>
        private async Task<List<Truck>> GetTrucksByDistanceAsync(double startLat, double startLong)
        {
            //Attempt to get starting location as a GeoCoordinate.
            GeoCoordinate startLoc; 
            try
            {
                startLoc = new GeoCoordinate(startLat, startLong);
            }
            catch(ArgumentOutOfRangeException ex)
            {
                throw new ArgumentException("Starting location cannot be determined."); 
            }

            //Load active trucks from source data.
            string json = await LoadFromEndpointAsync();
            var activeTrucks = JsonSerializer.Deserialize<SFOResponse>(json).Trucks?.Where(t => t.IsActive());

            //Calculate distance from current location.
            foreach(Truck t in activeTrucks)
            {
                try
                {
                    t.DistanceFromStartingPoint = new GeoCoordinate(t.Latitude, t.Longitude).GetDistanceTo(startLoc);
                }
                catch(ArgumentOutOfRangeException)
                {
                    //Skip trucks with invalid coordinates.
                }
            }

            return activeTrucks?.OrderBy(t => t.DistanceFromStartingPoint).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task<string> LoadFromEndpointAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
        }


    }
}
