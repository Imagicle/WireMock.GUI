using System.Collections.Generic;
using System.Globalization;
using System.Net;
using FluentAssertions;
using NUnit.Framework;
using WireMock.GUI.WPF;

namespace WireMock.GUI.Test.WPF
{
    [TestFixture]
    public class HttpStatusCodesConverterTest
    {
        #region Fixture

        private HttpStatusCodesConverter _httpStatusCodesConverter;

        #endregion

        #region Setup & Teardown

        [SetUp]
        public void SetUp()
        {
            _httpStatusCodesConverter = new HttpStatusCodesConverter();
        }

        #endregion

        [Test]
        public void Convert()
        {
            var result = _httpStatusCodesConverter.Convert(new[]
            {
                HttpStatusCode.OK,
                HttpStatusCode.Created,
                HttpStatusCode.Redirect,
                HttpStatusCode.NotFound,
                HttpStatusCode.InternalServerError
            }, typeof(NotUsed), null, CultureInfo.InvariantCulture);

            ((List<string>)result).Should().BeEquivalentTo("200 - OK", "201 - Created", "302 - Redirect", "404 - NotFound", "500 - InternalServerError");
        }
    }
}