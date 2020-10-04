using System.Collections.Generic;

namespace WireMock.GUI.Window
{
    public interface IEditResponseWindow : IWindowWrapper
    {
        string Body { get; set; }

        IDictionary<string, string> Headers { get; set; }
    }
}