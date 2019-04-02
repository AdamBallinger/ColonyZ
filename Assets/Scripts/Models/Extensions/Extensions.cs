using System;
using Newtonsoft.Json;

namespace Models.Extensions
{
    public static class Extensions
    {
        public static T Clone<T>(this T _source)
        {
            if (ReferenceEquals(_source, null))
            {
                return default;
            }

            var settings = new JsonSerializerSettings {ObjectCreationHandling = ObjectCreationHandling.Replace};
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(_source), settings);
        }
    }
}