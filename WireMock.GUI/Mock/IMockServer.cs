using System.Collections.Generic;
using WireMock.GUI.Model;

namespace WireMock.GUI.Mock
{
    public interface IMockServer
    {
        public delegate void NewRequest(NewRequestEventArgs e);
        public event NewRequest OnNewRequest;

        void UpdateMappings(IEnumerable<MappingInfoViewModel> mappingInfos);

        void Stop();
    }
}