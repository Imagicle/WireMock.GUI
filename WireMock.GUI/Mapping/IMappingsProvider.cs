using System.Collections.Generic;

namespace WireMock.GUI.Mapping
{
    public interface IMappingsProvider
    {
        IEnumerable<PersistableMappingInfo> LoadMappings();

        void SaveMappings(IEnumerable<PersistableMappingInfo> mappings);
    }
}