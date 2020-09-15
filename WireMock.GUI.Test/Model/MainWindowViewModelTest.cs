using System.Collections.Generic;
using System.Linq;
using System.Net;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using NUnit.Framework;
using WireMock.GUI.Mapping;
using WireMock.GUI.Mock;
using WireMock.GUI.Model;
using WireMock.GUI.Test.TestUtils;

namespace WireMock.GUI.Test.Model
{
    [TestFixture]
    public class MainWindowViewModelTest
    {
        #region Fixture

        private IMockServer _mockServer;
        private IMappingsProvider _mappingsProvider;
        private MainWindowViewModel _mainWindowViewModel;

        #endregion

        #region Setup & Teardown

        [SetUp]
        public void SetUp()
        {
            _mockServer = A.Fake<IMockServer>();
            _mappingsProvider = A.Fake<IMappingsProvider>();
            _mainWindowViewModel = new MainWindowViewModel(_mockServer, _mappingsProvider);
            ExecuteClearCommand();
        }

        #endregion

        #region AddCommand

        [Test]
        public void AddCommand_ShouldHadAMappingWithDefaultValues()
        {
            ExecuteAddCommand();

            var mappingInfo = _mainWindowViewModel.Mappings[0];
            ShouldHaveDefaultValues(mappingInfo);
        }

        #endregion

        #region ApplyCommand

        [Test]
        public void ApplyCommand_ShouldCallMockServerUpdateMappingsAndShouldSaveMappings()
        {
            var expectedMappings = MappingInfoViewModelTestUtils.SomeMappings();
            A.CallTo(() => _mappingsProvider.LoadMappings()).Returns(ToPersistableMappings(expectedMappings));
            _mainWindowViewModel = new MainWindowViewModel(_mockServer, _mappingsProvider);

            ExecuteApplyCommand();

            A.CallTo(() => _mockServer.UpdateMappings(_mainWindowViewModel.Mappings)).MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => _mappingsProvider.SaveMappings(ShouldMatchInfoMappingsViewModel(_mainWindowViewModel.Mappings))).MustHaveHappenedOnceExactly());
        }

        #endregion

        #region ClearCommand

        [Test]
        public void ClearCommand_ShouldRemoveAllMapping()
        {
            ExecuteAddCommand();
            ExecuteAddCommand();

            ExecuteClearCommand();

            _mainWindowViewModel.Mappings.Should().BeEmpty();
        }

        #endregion

        #region Mappings

        [Test]
        public void WhenMainWindowsViewModelIsInstantiated_Mappings_ShouldReturnTheMappingReturnByMappingsProvider()
        {
            var expectedMappings = MappingInfoViewModelTestUtils.SomeMappings();
            A.CallTo(() => _mappingsProvider.LoadMappings()).Returns(ToPersistableMappings(expectedMappings));

            _mainWindowViewModel = new MainWindowViewModel(_mockServer, _mappingsProvider);

            _mainWindowViewModel.Mappings.Should().BeEquivalentTo(expectedMappings);
        }

        #endregion

        #region Logs

        [Test]
        public void Logs_ShouldBeEmptyByDefault()
        {
            _mainWindowViewModel.Logs.Should().BeEmpty();
        }

        [Test]
        public void WhenEventOnNewRequestAreRaised_Logs_ShouldBeUpdated()
        {
            _mockServer.OnNewRequest += Raise.FreeForm.With(ANewRequestEventArgs());
            _mockServer.OnNewRequest += Raise.FreeForm.With(ANewRequestEventArgs());

            _mainWindowViewModel.Logs.Split('\n').Should().HaveCount(3);
        }

        [Test]
        public void WhenLogsIsModified_Logs_ShouldRaiseAnEvent()
        {
            using var monitor = _mainWindowViewModel.Monitor();

            _mockServer.OnNewRequest += Raise.FreeForm.With(ANewRequestEventArgs());

            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.Logs);
        }

        #endregion

        #region Stop

        [Test]
        public void Stop_ShouldCallMockServerStop()
        {
            _mainWindowViewModel.Stop();

            A.CallTo(() => _mockServer.Stop()).MustHaveHappenedOnceExactly();
        }

        #endregion

        #region DeleteMapping

        [Test]
        public void WhenDeletingAMappingLoadedFromDefaultConfiguration_ShouldRemoveThatMapping()
        {
            var mapping1 = MappingInfoViewModelTestUtils.AMapping();
            var mapping2 = MappingInfoViewModelTestUtils.AMapping();
            var mapping3 = MappingInfoViewModelTestUtils.AMapping();
            var persistableMappings = ToPersistableMappings(new List<MappingInfoViewModel> { mapping1, mapping2, mapping3 });
            A.CallTo(() => _mappingsProvider.LoadMappings()).Returns(persistableMappings);
            _mainWindowViewModel = new MainWindowViewModel(_mockServer, _mappingsProvider);
            var refreshedMapping2 = _mainWindowViewModel.Mappings.Single(m => m.Path == mapping2.Path);

            refreshedMapping2.DeleteMappingCommand.Execute(null);

            _mainWindowViewModel.Mappings.Should().BeEquivalentTo(new List<MappingInfoViewModel> { mapping1, mapping3 });
        }

        [Test]
        public void WhenDeletingAnAddedMapping_ShouldRemoveThatMapping()
        {
            ExecuteAddCommand();
            var addedMapping = _mainWindowViewModel.Mappings.Single();

            addedMapping.DeleteMappingCommand.Execute(null);

            _mainWindowViewModel.Mappings.Should().BeEmpty();
        }

        #endregion

        #region Utility Methods

        private static NewRequestEventArgs ANewRequestEventArgs()
        {
            var args = new NewRequestEventArgs
            {
                HttpMethod = FakerWrapper.Faker.PickRandom<HttpMethod>(),
                Path = FakerWrapper.Faker.Lorem.Word(),
                Body = FakerWrapper.Faker.Lorem.Sentence()
            };
            return args;
        }

        private static IEnumerable<PersistableMappingInfo> ToPersistableMappings(IEnumerable<MappingInfoViewModel> mappings)
        {
            return mappings.Select(ToPersistableMapping);
        }

        private static PersistableMappingInfo ToPersistableMapping(MappingInfoViewModel mapping)
        {
            return new PersistableMappingInfo
            {
                Path = mapping.Path,
                RequestHttpMethod = mapping.RequestHttpMethod,
                ResponseStatusCode = mapping.ResponseStatusCode,
                ResponseBody = mapping.ResponseBody,
                ResponseCacheControlMaxAge = mapping.ResponseCacheControlMaxAge
            };
        }

        private void ExecuteAddCommand()
        {
            _mainWindowViewModel.AddCommand.Execute(null);
        }

        private void ExecuteApplyCommand()
        {
            _mainWindowViewModel.ApplyCommand.Execute(null);
        }

        private void ExecuteClearCommand()
        {
            _mainWindowViewModel.ClearCommand.Execute(null);
        }

        private static void ShouldHaveDefaultValues(MappingInfoViewModel mappingInfo)
        {
            mappingInfo.Path.Should().BeNull();
            mappingInfo.RequestHttpMethod.Should().Be(HttpMethod.Get);
            mappingInfo.ResponseStatusCode.Should().Be(HttpStatusCode.OK);
            mappingInfo.ResponseCacheControlMaxAge.Should().Be(null);
        }

        private static IEnumerable<PersistableMappingInfo> ShouldMatchInfoMappingsViewModel(IReadOnlyCollection<MappingInfoViewModel> mappingInfoViewModels)
        {
            return A<IEnumerable<PersistableMappingInfo>>.That.Matches(pmi => AreEqual(pmi, ToPersistableMappings(mappingInfoViewModels)));
        }

        private static bool AreEqual(IEnumerable<PersistableMappingInfo> persistableMappings1, IEnumerable<PersistableMappingInfo> persistableMappings2)
        {
            try
            {
                persistableMappings1.Should().BeEquivalentTo(persistableMappings2);
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}