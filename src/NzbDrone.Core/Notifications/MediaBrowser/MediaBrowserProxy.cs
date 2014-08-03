using System;
using NLog;
using NzbDrone.Core.Rest;
using RestSharp;

namespace NzbDrone.Core.Notifications.MediaBrowser
{
    public class MediaBrowserProxy
    {
        private readonly Logger _logger;

        public MediaBrowserProxy(Logger logger)
        {
            _logger = logger;
        }

        public void Notify(MediaBrowserSettings settings, String title, String message)
        {
            var request = new RestRequest("Notifications/Admin");

            request.RequestFormat = DataFormat.Json;

            request.AddBody(new
                            {
                                Name = title,
                                Description = message,
                                ImageUrl = "https://raw.github.com/NzbDrone/NzbDrone/develop/Logo/64.png",
                            });

            ProcessRequest(request, settings);
        }

        public void Update(MediaBrowserSettings settings, Int32 tvdbId)
        {
            var request = new RestRequest("Library/Series/Updated");

            request.AddParameter("tvdbid", tvdbId, ParameterType.QueryString);

            ProcessRequest(request, settings);
        }

        private String ProcessRequest(IRestRequest request, MediaBrowserSettings settings)
        {
            var client = BuildClient(settings);

            request.Method = Method.POST;
            request.AddHeader("X-MediaBrowser-Token", settings.ApiKey);

            var response = client.ExecuteAndValidate(request);
            _logger.Trace("Response: {0}", response.Content);

            CheckForError(response);

            return response.Content;
        }

        private IRestClient BuildClient(MediaBrowserSettings settings)
        {
            var url = String.Format(@"http://{0}/mediabrowser", settings.Address);
            
            return RestClientFactory.BuildClient(url);
        }

        private void CheckForError(IRestResponse response)
        {
            _logger.Debug("Looking for error in response: {0}", response);

            //TODO: actually check for the error
        }
    }
}
