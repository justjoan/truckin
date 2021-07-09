using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace TruckinWebServices.DataModel
{
    public class SFOResponse
    {
        [JsonPropertyName("value")]
        public Truck[] Trucks { get; set; }
    }
}
