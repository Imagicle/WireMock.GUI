using System.Collections.Generic;

namespace WireMock.GUI.Mapping
{
    internal interface IMappingsProvider
    {
        IEnumerable<PersistableMappingInfo> LoadMappings();

        void SaveMappings(IEnumerable<PersistableMappingInfo> mappings);
    }
}