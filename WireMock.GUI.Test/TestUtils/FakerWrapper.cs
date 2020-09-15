using Bogus;

namespace WireMock.GUI.Test.TestUtils
{
    internal static class FakerWrapper
    {
        public static Faker Faker => new Faker();
    }
}