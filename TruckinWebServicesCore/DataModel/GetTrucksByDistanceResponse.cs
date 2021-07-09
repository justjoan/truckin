using System;

namespace TruckinWebServices
{
    public class GetTrucksByDistanceResponse
    {
        public double CurrentLocationLatitude { get; set; }

        public double CurrentLocationLongitude { get; set; }

        public double ThresholdFromStartingPoint{ get; set; } //in meters

        public Truck[] Trucks { get; set; }
    }
}
