using System;
using System.Text.Json.Serialization;

namespace TruckinWebServices
{
    public class Truck
    {
        [JsonPropertyName("Applicant")]
        public string Name { get; set; }

        public string Status { get; set; }

        [JsonPropertyName("FoodItems")]
        public string Description { get; set; }

        public string Address { get; set; }

        [JsonPropertyName("dayshours")]
        public string Hours { get; set; }

        public string FacilityType { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double DistanceFromStartingPoint{ get; set; } //in meters


        private static string ApprovedStatus = "APPROVED";
        internal bool IsActive()
        {
            return this.Status.Equals(ApprovedStatus, StringComparison.OrdinalIgnoreCase);
        }

       
    }
}
