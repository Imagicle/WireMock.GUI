using System;
using System.Threading;

namespace WireMock.GUI.Test.TestUtils
{
    internal static class CommonTestUtils
    {
        public static void RunInStaThread(Action action)
        {
            var th = new Thread(action.Invoke);
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
            th.Join();
        }
    }
}