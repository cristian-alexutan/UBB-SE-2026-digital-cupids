using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace matchmaking.Utils
{
    internal class LocationUtil
    {
        private String locationsFile;
        private Dictionary<String, Tuple<float, float>> locations = new Dictionary<string, Tuple<float, float>>();

        public LocationUtil()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            locationsFile = Path.Combine(baseDirectory, "locations.csv");

            if (File.Exists(locationsFile))
            {
                foreach ( var line in File.ReadLines(locationsFile))
                {
                    var parts = line.Split(',');
                    string county = parts[0];
                    float lat=float.Parse(parts[1]);
                    float lon=float.Parse(parts[2]);
                    locations[county]=new Tuple<float, float>(lat, lon);
                }                
            }
            else
            {
                throw new FileNotFoundException("The file doesn't exist");
            }
        }

        public List<String> GetAllLocations()
        {
            return locations.Keys.ToList();
        }

        public Tuple<float,float> GetCoords(String location)
        {
            if (locations.ContainsKey(location))
            {
                return locations[location];
            }
            return null;
        }
    }
}
