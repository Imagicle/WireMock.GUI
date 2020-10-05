using System;

namespace WireMock.GUI.Mock
{
    public class ServerStatusChangeEventArgs : EventArgs
    {
        public bool IsStarted { get; set; }
    }
}