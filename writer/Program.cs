using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.IO;

namespace writer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var serializerSettings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Error,
                };
                var token = JObject.Parse(File.ReadAllText(args[0]))["AccelerationSettings"];
                var settings = token.ToObject<DriverSettings>(JsonSerializer.Create(serializerSettings));
                DriverInterop.SetActiveSettings(settings);
            }
            catch (Exception e)
            { 
                Console.WriteLine(e);
                Console.ReadLine();
            }
        }
    }
}
