using Newtonsoft.Json;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartWalkingStick.Model
{
    class Direction
    {
        HttpClient httpclient = new HttpClient();
        public async Task<DirectionObject> getDirection(Position startPosition,Position endPosition)
        {
            string url = "https://maps.googleapis.com/maps/api/directions/json?origin=" + $"{startPosition.Latitude},{startPosition.Longitude}&destination={endPosition.Latitude},{endPosition.Longitude}&key=AIzaSyBto9QrP9nShvJYX_lJzDdiW6n_bJktOC8";
            var json = await httpclient.GetStringAsync(url);
           return JsonConvert.DeserializeObject<DirectionObject>(json);
        }
    }
    public class DirectionObject
    {
        public List<Route> Routes { get; set; }
        public string status { get; set; }
    }

    public class Distance
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class Duration
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class EndLocation
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class StartLocation
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Distance2
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class Duration2
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class EndLocation2
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class StartLocation2
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Step
    {
        public Distance2 distance { get; set; }
        public Duration2 duration { get; set; }
        public EndLocation2 end_location { get; set; }
        public string html_instructions { get; set; }
        public StartLocation2 start_location { get; set; }
        public string travel_mode { get; set; }
        public string maneuver { get; set; }
    }

    public class Leg
    {
        public Distance distance { get; set; }
        public Duration duration { get; set; }
        public string end_address { get; set; }
        public EndLocation end_location { get; set; }
        public string start_address { get; set; }
        public StartLocation start_location { get; set; }
        public List<Step> steps { get; set; }
    }

    public  class Route
    {
        public string copyrights { get; set; }
        public List<Leg> legs { get; set; }
        public string summary { get; set; }
    }
}
