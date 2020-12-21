using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows.Input;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
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
        private readonly ILogger<MainWindowViewModel> _logger;
        private readonly IEditResponseWindowFactory _textAreaWindowFactory;
        private string _serverUrl;
        private bool _isServerStarted;
        private string _logs;

        #endregion

        #region Initialization

        public MainWindowViewModel(IMockServer mockServer, IMappingsProvider mappingsProvider)
        {
            _mockServer = mockServer;
            _mockServer.OnNewRequest += OnNewRequest;
            _mockServer.OnServerStatusChange += OnServerStatusChange;
            _mappingsProvider = mappingsProvider;
            _logger = new Logger<MainWindowViewModel>(new NLogLoggerFactory());
            _textAreaWindowFactory = new TextAreaWindowFactory();

            StartServerCommand = new RelayCommand(o => ExecuteStartServerCommand(), o => true, this);
            StopServerCommand = new RelayCommand(o => ExecuteStopServerCommand(), o => true, this);
            AddCommand = new RelayCommand(o => ExecuteAddCommand(), o => true, this);
            ApplyCommand = new RelayCommand(o => ExecuteApplyCommand(), o => true, this);
            ClearCommand = new RelayCommand(o => ExecuteClearCommand(), o => true, this);

            InitMappings();
            _serverUrl = _mockServer.Url;
            _isServerStarted = true;
            _logs = string.Empty;
        }

        #endregion

        #region Commands

        public ICommand StartServerCommand { get; }
        public ICommand StopServerCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand ApplyCommand { get; }
        public ICommand ClearCommand { get; }

        #endregion

        #region Properties

        public string ServerUrl
        {
            get => _serverUrl;
            set
            {
                _serverUrl = value;
                OnPropertyChanged(nameof(ServerUrl));
            }
        }

        public bool IsServerStarted
        {
            get => _isServerStarted;
            private set
            {
                _isServerStarted = value;
                OnPropertyChanged(nameof(IsServerStarted));
            }
        }

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

        private void ExecuteStartServerCommand()
        {
            _mockServer.Url = ServerUrl;
            _mockServer.Start();
        }

        private void ExecuteStopServerCommand()
        {
            _mockServer.Stop();
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
            _logger.LogInformation($"[{e.HttpMethod}] Path: {{{e.Path}}} Request body: {{{e.Body}}}");
            Logs += $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{e.HttpMethod}] Path: {{{e.Path}}} Request body: {{{e.Body}}}\n";
        }

        private void OnServerStatusChange(ServerStatusChangeEventArgs e)
        {
            IsServerStarted = e.IsStarted;
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
                ResponseHeaders = mapping.Headers
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
                Headers = mapping.ResponseHeaders
            };
        }

        #endregion
    }
}