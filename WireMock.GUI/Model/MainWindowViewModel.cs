using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows.Input;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using WireMock.GUI.Mapping;
using WireMock.GUI.Mock;
using WireMock.GUI.Window;
using WireMock.GUI.WPF;

namespace WireMock.GUI.Model
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Private fields

        private readonly IMockServer _mockServer;
        private readonly IMappingsProvider _mappingsProvider;
        private readonly ITextAreaWindowFactory _textAreaWindowFactory;
        private string _logs;

        #endregion

        #region Initialization

        public MainWindowViewModel(IMockServer mockServer, IMappingsProvider mappingsProvider)
        {
            _mockServer = mockServer;
            _mockServer.OnNewRequest += OnNewRequest;
            _mappingsProvider = mappingsProvider;
            _textAreaWindowFactory = new TextAreaWindowFactory();
            AddCommand = new RelayCommand(o => ExecuteAddCommand(), o => true, this);
            ApplyCommand = new RelayCommand(o => ExecuteApplyCommand(), o => true, this);
            ClearCommand = new RelayCommand(o => ExecuteClearCommand(), o => true, this);
            InitMappings();
            Logs = string.Empty;
        }

        #endregion

        #region Commands

        public ICommand AddCommand { get; }
        public ICommand ApplyCommand { get; }
        public ICommand ClearCommand { get; }

        #endregion

        #region Properties

        public ObservableCollection<MappingInfoViewModel> Mappings { get; private set; }

        public string Logs
        {
            get => _logs;
            set
            {
                _logs = value;
                OnPropertyChanged(nameof(Logs));
            }
        }

        #endregion

        #region Methods

        public void Stop()
        {
            _mockServer.Stop();
        }

        #endregion

        #region Utility Methods

        private void InitMappings()
        {
            Mappings = new ObservableCollection<MappingInfoViewModel>();
            foreach (var mapping in _mappingsProvider.LoadMappings())
            {
                AddMapping(ToMappingInfoViewModel(mapping));
            }
        }

        private void ExecuteAddCommand()
        {
            AddMapping(new MappingInfoViewModel(_textAreaWindowFactory)
            {
                RequestHttpMethod = HttpMethod.Get,
                ResponseStatusCode = HttpStatusCode.OK
            });
        }

        private void AddMapping(MappingInfoViewModel newMapping)
        {
            Mappings.Add(newMapping);
            newMapping.OnDeleteMapping += OnDeleteMapping;
        }

        private void ExecuteApplyCommand()
        {
            _mockServer.UpdateMappings(Mappings);
            _mappingsProvider.SaveMappings(ToPersistableMappings(Mappings));
        }

        private void ExecuteClearCommand()
        {
            while (Mappings.Count != 0)
            {
                Mappings.RemoveAt(0);
            }
        }

        private void OnNewRequest(NewRequestEventArgs e)
        {
            Logs += $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{e.HttpMethod}] Path: {{{e.Path}}} Request body: {{{e.Body}}}\n";
        }

        private void OnDeleteMapping(MappingInfoViewModel mapping)
        {
            Mappings.Remove(mapping);
        }

        private MappingInfoViewModel ToMappingInfoViewModel(PersistableMappingInfo mapping)
        {
            return new MappingInfoViewModel(_textAreaWindowFactory)
            {
                Path = mapping.Path,
                RequestHttpMethod = mapping.RequestHttpMethod,
                ResponseStatusCode = mapping.ResponseStatusCode,
                ResponseBody = mapping.ResponseBody,
                ResponseCacheControlMaxAge = mapping.ResponseCacheControlMaxAge
            };
        }

        private static IEnumerable<PersistableMappingInfo> ToPersistableMappings(IEnumerable<MappingInfoViewModel> mappings)
        {
            return mappings.Select(ToPersistableMappingInfo);
        }

        private static PersistableMappingInfo ToPersistableMappingInfo(MappingInfoViewModel mapping)
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

        #endregion
    }
}