using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace WireMock.GUI.Mapping
{
    internal class MappingsProvider : IMappingsProvider
    {
        private readonly string _mappingsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mappings.json");

        public IEnumerable<PersistableMappingInfo> LoadMappings()
        {
            if (!File.Exists(_mappingsFile))
            {
                return new List<PersistableMappingInfo>();
            }

            var serializedMappings = File.ReadAllText(_mappingsFile);
            return JsonConvert.DeserializeObject<IEnumerable<PersistableMappingInfo>>(serializedMappings);
        }

        public void SaveMappings(IEnumerable<PersistableMappingInfo> mappings)
        {
            var serializedMappings = JsonConvert.SerializeObject(mappings);
            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mappings.json"), serializedMappings);
        }
    }
}