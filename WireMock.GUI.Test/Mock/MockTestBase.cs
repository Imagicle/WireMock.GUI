using System.IO;
using System.Runtime.Serialization.Json;
using NUnit.Framework;
using WireMock.GUI.Mock;

namespace WireMock.GUI.Test.Mock
{
    public class MockTestBase
    {
        #region Fixture

        protected IMockServer MockServer;

        #endregion

        #region Setup & Teardown

        [SetUp]
        public void SetUp()
        {
            MockServer = new WireMockWrapper();
        }

        [TearDown]
        public void TearDown()
        {
            MockServer.Stop();
        }

        #endregion

        #region Utility Methods

        protected static TBody ReadStream<TBody>(Stream stream)
        {
            var serializer = new DataContractJsonSerializer(typeof(TBody));
            return (TBody)serializer.ReadObject(stream);
        }

        #endregion
    }
}