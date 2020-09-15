using System;
using System.Diagnostics;
using System.Threading;

namespace WireMock.GUI
{
    // TODO: IMALIB-67
    public class Wait
    {
        public static void ForCondition(Func<bool> condition)
        {
            ForCondition(condition, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(50));
        }

        private static void ForCondition(Func<bool> condition, TimeSpan timeout, TimeSpan pollInterval, string timeoutMessage = null)
        {
            var sw = Stopwatch.StartNew();
            while (!condition.Invoke())
            {
                if (sw.Elapsed > timeout)
                {
                    var message = timeoutMessage ?? $"The specified condition was always false for more than the specified timeout ({timeout})";
                    throw new TimeoutException(message);
                }

                Thread.Sleep(pollInterval);
            }
        }
    }
}