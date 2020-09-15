using System.Collections.Generic;
using System.Net;
using FakeItEasy;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using WireMock.GUI.Model;
using WireMock.GUI.Window;

namespace WireMock.GUI.Test.TestUtils
{
    internal static class MappingInfoViewModelTestUtils
    {
        internal static IList<MappingInfoViewModel> SomeMappings()
        {
            var result = new List<MappingInfoViewModel>();
            for (var i = 0; i < FakerWrapper.Faker.Random.Int(3, 10); i++)
            {
                result.Add(AMapping());
            }

            return result;
        }

        internal static MappingInfoViewModel AMapping()
        {
            return new MappingInfoViewModel(A.Fake<ITextAreaWindowFactory>())
            {
                Path = FakerWrapper.Faker.Lorem.Word(),
                RequestHttpMethod = GetValidHttpMethod(),
                ResponseStatusCode = FakerWrapper.Faker.PickRandom<HttpStatusCode>(),
                ResponseBody = FakerWrapper.Faker.Lorem.Sentence(),
                ResponseCacheControlMaxAge = FakerWrapper.Faker.Lorem.Word()
            };
        }

        #region Utility Methods

        private static HttpMethod GetValidHttpMethod()
        {
            return FakerWrapper.Faker.PickRandom(new List<HttpMethod>
            {
                HttpMethod.Get,
                HttpMethod.Post,
                HttpMethod.Put,
                HttpMethod.Patch,
                HttpMethod.Delete
            });
        }

        #endregion
    }
}
