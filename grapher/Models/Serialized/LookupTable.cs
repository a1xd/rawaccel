using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grapher.Models.Serialized
{
    [Serializable]
    public class LookupTable
    {
        [Serializable]
        public class Point
        {
            public Point(double x, double y)
            {
                X = x;
                Y = y;
            }

            double X { get; set; }

            double Y { get; set; }
        }

        public LookupTable(Point[] points)
        {
            Points = points;
        }

        public Point[] Points { get; }

        public bool InSensGraph { get; }

        public static LookupTable Deserialize(string lutFile)
        {
            if (!File.Exists(lutFile))
            {
                throw new Exception($"LUT file does not exist at {lutFile}.");
            }

            JObject lutJObject = JObject.Parse(File.ReadAllText(lutFile));

            var lut = lutJObject.ToObject<LookupTable>(JsonSerializer.Create(RawAccelSettings.SerializerSettings));

            if (lut is null || lut.Points is null)
            {
                throw new Exception($"{lutFile} does not contain valid lookuptable json.");
            }

            lut.Verify();

            return lut;
        }

        private void Verify()
        {
            if (Points.Length >= short.MaxValue)
            {
                throw new Exception($"LUT file with {Points.Length} points is too long. Max points: {short.MaxValue}");
            }
        }
    }
}
