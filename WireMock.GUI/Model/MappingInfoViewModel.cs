using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Input;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using WireMock.GUI.Utility;
using WireMock.GUI.Window;
using WireMock.GUI.WPF;

namespace WireMock.GUI.Model
{
    public class MappingInfoViewModel : ViewModel
    {
        #region Private fields

        private readonly IEditResponseWindowFactory _textAreaWindowFactory;
        private string _path;
        private HttpMethod _requestHttpMethod;
        private HttpStatusCode _responseStatusCode;
        private string _responseBody;

        #endregion

        #region Initialization

        public MappingInfoViewModel(IEditResponseWindowFactory textAreaWindowFactory)
        {
            _textAreaWindowFactory = textAreaWindowFactory;
            EditResponseCommand = new RelayCommand(o => ExecuteEditResponse(), o => true, this);
            DeleteMappingCommand = new RelayCommand(o => ExecuteDeleteMapping(), o => true, this);
            ResponseHeaders = new Dictionary<string, string>();
        }

        #endregion

        #region Events

        public delegate void DeleteMapping(MappingInfoViewModel mapping);
        public event DeleteMapping OnDeleteMapping;

        #endregion

        #region Commands

        public ICommand EditResponseCommand { get; }
        public ICommand DeleteMappingCommand { get; }

        #endregion

        #region Properties

        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                OnPropertyChanged(nameof(Path));
            }
        }

        public HttpMethod RequestHttpMethod
        {
            get => _requestHttpMethod;
            set
            {
                _requestHttpMethod = value;
                OnPropertyChanged(nameof(RequestHttpMethod));
            }
        }

        public static IEnumerable<HttpMethod> HttpMethods => new List<HttpMethod>
        {
            HttpMethod.Get,
            HttpMethod.Post,
            HttpMethod.Put,
            HttpMethod.Patch,
            HttpMethod.Delete
        };

        public HttpStatusCode ResponseStatusCode
        {
            get => _responseStatusCode;
            set
            {
                _responseStatusCode = value;
                OnPropertyChanged(nameof(ResponseStatusCode));
            }
        }

        public static IEnumerable<HttpStatusCode> HttpStatusCodes => Enum.GetValues(typeof(HttpStatusCode)).Cast<HttpStatusCode>();

        public string ResponseBody
        {
            get => _responseBody;
            set
            {
                _responseBody = value;
                OnPropertyChanged(nameof(MinifiedResponseBody));
            }
        }

        public string MinifiedResponseBody => JsonUtilities.Minify(ResponseBody);

        public IDictionary<string, string> ResponseHeaders { get; set; }

        #endregion

        #region Utility Methods

        private void ExecuteEditResponse()
        {
            var textAreaWindow = _textAreaWindowFactory.Create();
            textAreaWindow.Body = ResponseBody;
            textAreaWindow.Headers = ResponseHeaders;

            if (textAreaWindow.ShowDialog())
            {
                ResponseBody = textAreaWindow.Body;
                ResponseHeaders = textAreaWindow.Headers;
            }
        }

        private void ExecuteDeleteMapping()
        {
            OnDeleteMapping?.Invoke(this);
        }

        #endregion
    }
}