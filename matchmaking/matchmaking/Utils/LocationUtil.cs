using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace matchmaking.Utils
{
    internal class LocationUtil
    {
        private readonly string _locationsFile;
        private readonly Dictionary<string, (float lat, float lon)> _locations;

        public LocationUtil()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _locationsFile = Path.Combine(baseDirectory, "locations.csv");

            _locations = new Dictionary<string, (float, float)>();

            if (File.Exists(_locationsFile))
            {
                foreach (var line in File.ReadLines(_locationsFile))
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        var parts = line.Split(',');

                        if (parts.Length < 3)
                            continue;

                        string county = parts[0].Trim();

                        if (string.IsNullOrWhiteSpace(county))
                            continue;

                        if (float.TryParse(parts[1], out float lat) &&
                            float.TryParse(parts[2], out float lon))
                        {
                            _locations[county] = (lat, lon);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            else
            {
                throw new FileNotFoundException("The file doesn't exist");
            }
        }

        public List<string> GetAllLocations()
        {
            return _locations.Keys.ToList();
        }

        public (float lat, float lon)? GetCoords(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return null;

            var key = _locations.Keys.FirstOrDefault(k =>
                k.Equals(location.Trim(), StringComparison.OrdinalIgnoreCase));

            if (key != null)
            {
                return _locations[key];
            }
            return null;
        }
    }
}