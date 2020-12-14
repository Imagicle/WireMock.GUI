using System.Globalization;
using System.Net;
using FluentAssertions;
using NUnit.Framework;
using WireMock.GUI.WPF;

namespace WireMock.GUI.Test.WPF
{
    [TestFixture]
    public class HttpStatusCodeConverterTest
    {
        #region Fixture

        private HttpStatusCodeConverter _httpStatusCodeConverter;

        #endregion

        #region Setup & Teardown

        [SetUp]
        public void SetUp()
        {
            _httpStatusCodeConverter = new HttpStatusCodeConverter();
        }

        #endregion

        [TestCase(HttpStatusCode.OK, "200 - OK")]
        [TestCase(HttpStatusCode.Created, "201 - Created")]
        [TestCase(HttpStatusCode.Redirect, "302 - Redirect")]
        [TestCase(HttpStatusCode.NotFound, "404 - NotFound")]
        [TestCase(HttpStatusCode.InternalServerError, "500 - InternalServerError")]
        public void Convert(HttpStatusCode httpStatusCode, string expectedHttpStatusCode)
        {
            var result = _httpStatusCodeConverter.Convert(httpStatusCode, typeof(NotUsed), null, CultureInfo.InvariantCulture);

            ((string)result).Should().Be(expectedHttpStatusCode);
        }

        [TestCase("200 - OK", HttpStatusCode.OK)]
        [TestCase("201 - Created", HttpStatusCode.Created)]
        [TestCase("302 - Redirect", HttpStatusCode.Redirect)]
        [TestCase("404 - NotFound", HttpStatusCode.NotFound)]
        [TestCase("500 - InternalServerError", HttpStatusCode.InternalServerError)]
        public void ConvertBack(string httpStatusCode, HttpStatusCode expectedHttpStatusCode)
        {
            var result = _httpStatusCodeConverter.ConvertBack(httpStatusCode, typeof(NotUsed), null, CultureInfo.InvariantCulture);

            ((HttpStatusCode)result).Should().Be(expectedHttpStatusCode);
        }
    }
}