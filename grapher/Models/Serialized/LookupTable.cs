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
    public static class LookupTable
    {
        public static void Deserialize(string lutFile, ref DriverSettings settings)
        {
            if (!File.Exists(lutFile))
            {
                throw new Exception($"LUT file does not exist at {lutFile}.");
            }

            JObject lutJObject = JObject.Parse(File.ReadAllText(lutFile));

            var spacedLut = lutJObject.ToObject<SpacedTable>(JsonSerializer.Create(RawAccelSettings.SerializerSettings));

            if (spacedLut is null)
            {
                var arbitraryLut = lutJObject.ToObject<ArbitraryTable>(JsonSerializer.Create(RawAccelSettings.SerializerSettings));

                if (arbitraryLut is null || arbitraryLut.points is null)
                {
                    throw new Exception($"{lutFile} does not contain valid lookuptable json.");
                }

                settings.ArbitraryTable = arbitraryLut;
                settings.args.x.lutArgs.type = TableType.arbitrary;
            }
            else
            {
                if (spacedLut.points is null)
                {
                    throw new Exception($"{lutFile} does not contain valid lookuptable json.");
                }

                settings.SpacedTable = spacedLut;
                settings.args.x.lutArgs = spacedLut.args;
            }
        }
    }
}
