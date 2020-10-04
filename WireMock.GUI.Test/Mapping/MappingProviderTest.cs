using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using NUnit.Framework;
using WireMock.GUI.Mapping;

namespace WireMock.GUI.Test.Mapping
{
    [TestFixture]
    public class MappingProviderTest
    {
        #region Fixture

        private IMappingsProvider _mappingsProvider;
        private FileInfo _mappingsFile;

        #endregion

        #region Setup & Teardown

        [SetUp]
        public void SetUp()
        {
            _mappingsFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mappings.json"));
            _mappingsFile.Delete();

            _mappingsProvider = new MappingsProvider();
        }

        [TearDown]
        public void TearDown()
        {
            _mappingsFile.Delete();
        }

        #endregion

        [Test]
        public void IfNoMappingsJsonFileExist_LoadMappings_ShouldReturnAnEmptyList()
        {
            var mappings = _mappingsProvider.LoadMappings();

            mappings.Should().BeEmpty();
        }

        [Test]
        public void IfAMappingsJsonFileExists_LoadMappings_ShouldReturnTheExpectedValues()
        {
            File.Copy(GetTestFilePath(), _mappingsFile.FullName);

            var mappings = _mappingsProvider.LoadMappings();

            var expectedMappings = Mappings();
            mappings.Should().BeEquivalentTo(expectedMappings);
        }

        [Test]
        public void IfNoMappingsJsonFileExist_SaveMappings_ShouldSaveGivenMappings()
        {
            _mappingsProvider.SaveMappings(Mappings());

            File.ReadAllText(_mappingsFile.FullName).Should().Be(ExpectedMappingsFileContent());
        }

        [Test]
        public void IfAMappingsJsonFileExists_SaveMappings_ShouldSaveGivenMappings()
        {
            File.WriteAllText(_mappingsFile.FullName, @"Something that needs to be overridden");

            _mappingsProvider.SaveMappings(Mappings());

            File.ReadAllText(_mappingsFile.FullName).Should().Be(ExpectedMappingsFileContent());
        }

        #region Utility Methods

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private static string GetTestFilePath()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            var testAssemblyPath = new DirectoryInfo(path).Parent.Parent.Parent;
            return Path.Combine(testAssemblyPath.FullName, "Mapping", "mappings.json");
        }

        private static IEnumerable<PersistableMappingInfo> Mappings()
        {
            return new List<PersistableMappingInfo>
            {
                new PersistableMappingInfo
                {
                    Path = "a/path",
                    RequestHttpMethod = HttpMethod.Put,
                    ResponseStatusCode = HttpStatusCode.NoContent
                },
                new PersistableMappingInfo
                {
                    RequestHttpMethod = HttpMethod.Delete,
                    ResponseStatusCode = HttpStatusCode.InternalServerError,
                    ResponseBody = "A response body",
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json"},
                        { "Cache-Control", "max-age=60"}
                    }
                }
            };
        }

        private static string ExpectedMappingsFileContent()
        {
            return "[{\"Path\":\"a/path\",\"RequestHttpMethod\":1,\"ResponseStatusCode\":204,\"ResponseBody\":null,\"Headers\":null},{\"Path\":null,\"RequestHttpMethod\":2,\"ResponseStatusCode\":500,\"ResponseBody\":\"A response body\",\"Headers\":{\"Content-Type\":\"application/json\",\"Cache-Control\":\"max-age=60\"}}]";
        }

        #endregion
    }
}
