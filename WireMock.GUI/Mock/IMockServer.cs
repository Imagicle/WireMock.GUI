using System.Collections.Generic;
using WireMock.GUI.Model;

namespace WireMock.GUI.Mock
{
    public interface IMockServer
    {
        event NewRequest OnNewRequest;

        event ServerStatus OnServerStatusChange;

        string Url { get; set; }

        void UpdateMappings(IEnumerable<MappingInfoViewModel> mappingInfos);

        void Start();

        void Stop();
    }
}