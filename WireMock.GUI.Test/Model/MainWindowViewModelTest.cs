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
using static WireMock.GUI.Test.TestUtils.FakerWrapper;

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

        #region Constructor

        [Test]
        public void Constructor_ShouldCallMockServerUpdateMappingsWithLoadedMappings()
        {
            var expectedMappings = MappingInfoViewModelTestUtils.SomeMappings();
            A.CallTo(() => _mappingsProvider.LoadMappings()).Returns(ToPersistableMappings(expectedMappings));

            _mainWindowViewModel = new MainWindowViewModel(_mockServer, _mappingsProvider);

            A.CallTo(() => _mockServer.UpdateMappings(A<IEnumerable<MappingInfoViewModel>>.That.Matches(map => AreEqual(map, expectedMappings)))).MustHaveHappenedOnceExactly();
        }

        #endregion

        #region StartServerCommand

        [Test]
        public void StartServerCommand_ShouldStartTheServer()
        {
            var expectedServerUrl = Faker.Lorem.Word();
            _mainWindowViewModel.ServerUrl = expectedServerUrl;

            ExecuteStartServerCommand();

            A.CallToSet(() => _mockServer.Url).WhenArgumentsMatch(args => args.Get<string>(0) == expectedServerUrl).MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => _mockServer.Start()).MustHaveHappenedOnceExactly());
        }

        #endregion

        #region StopServerCommand

        [Test]
        public void StopServerCommand_ShouldStopTheServer()
        {
            ExecuteStopServerCommand();

            A.CallTo(() => _mockServer.Stop()).MustHaveHappenedOnceExactly();
        }

        #endregion

        #region ServerUrl

        [Test]
        public void ServerUrl_ShouldBeInitializedWithDefaultMockServerUrl()
        {
            var expectedUrl = Faker.Internet.Url();
            _mockServer.Url = expectedUrl;

            _mainWindowViewModel = new MainWindowViewModel(_mockServer, _mappingsProvider);

            _mainWindowViewModel.ServerUrl.Should().Be(expectedUrl);
        }

        [Test]
        public void WhenServerUrlIsModified_ServerUrl_ShouldRaiseAnEvent()
        {
            var expectedServerUrl = Faker.Lorem.Word();
            using var monitor = _mainWindowViewModel.Monitor();

            _mainWindowViewModel.ServerUrl = expectedServerUrl;

            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.ServerUrl);
        }

        [Test]
        public void ServerUrl_ShouldBeEditable()
        {
            var expectedServerUrl = Faker.Lorem.Word();

            _mainWindowViewModel.ServerUrl = expectedServerUrl;

            _mainWindowViewModel.ServerUrl.Should().Be(expectedServerUrl);
        }

        #endregion

        #region IsServerStarted

        [Test]
        public void IsServerStarted_ShouldHaveDefaultValue()
        {
            _mainWindowViewModel.IsServerStarted.Should().BeTrue();
        }

        [Test]
        public void WhenEventOnServerStatusChangeIsRaised_IsServerStarted_ShouldBeUpdated([Values] bool expectedIsServerStarted)
        {
            _mockServer.OnServerStatusChange += Raise.FreeForm.With(AServerStatusChangeEventArgs(expectedIsServerStarted));

            _mainWindowViewModel.IsServerStarted.Should().Be(expectedIsServerStarted);
        }

        [Test]
        public void WhenIsServerStartedIsModified_IsServerStarted_ShouldRaiseAnEvent()
        {
            using var monitor = _mainWindowViewModel.Monitor();

            _mockServer.OnServerStatusChange += Raise.FreeForm.With(AServerStatusChangeEventArgs(Faker.Random.Bool()));

            monitor.Should().RaisePropertyChangeFor(viewModel => viewModel.IsServerStarted);
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
            Fake.ClearRecordedCalls(_mockServer);

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
            var refreshedMapping2 = _mainWindowViewModel.Mappings.Single(m => AreEqual(m, mapping2));

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

        private static ServerStatusChangeEventArgs AServerStatusChangeEventArgs(bool isStarted)
        {
            return new ServerStatusChangeEventArgs
            {
                IsStarted = isStarted
            };
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
                Headers = mapping.ResponseHeaders
            };
        }

        private void ExecuteStartServerCommand()
        {
            _mainWindowViewModel.StartServerCommand.Execute(null);
        }

        private void ExecuteStopServerCommand()
        {
            _mainWindowViewModel.StopServerCommand.Execute(null);
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
            mappingInfo.ResponseHeaders.Should().BeEmpty();
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

        private static bool AreEqual(IEnumerable<MappingInfoViewModel> mappings1, IEnumerable<MappingInfoViewModel> mappings2)
        {
            try
            {
                mappings1.Should().BeEquivalentTo(mappings2);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private static bool AreEqual(MappingInfoViewModel mappingInfoViewModel1, MappingInfoViewModel mappingInfoViewModel2)
        {
            try
            {
                mappingInfoViewModel1.Should().BeEquivalentTo(mappingInfoViewModel2);
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