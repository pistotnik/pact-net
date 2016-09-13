using System;
using System.Net.Http;
using System.Threading;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Reporters.Outputters;

namespace PactNet.Mocks.MockHttpService
{
    internal class HttpClientRequestSender : IHttpRequestSender
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpRequestMessageMapper _httpRequestMessageMapper;
        private readonly IProviderServiceResponseMapper _providerServiceResponseMapper;
        private readonly IReportOutputter _reportOutputter;
        
        internal HttpClientRequestSender(
            HttpClient httpClient, 
            IHttpRequestMessageMapper httpRequestMessageMapper, 
            IProviderServiceResponseMapper providerServiceResponseMapper, IReportOutputter reportOutputter)
        {
            _httpClient = httpClient;
            _httpRequestMessageMapper = httpRequestMessageMapper;
            _providerServiceResponseMapper = providerServiceResponseMapper;
            _reportOutputter = reportOutputter;
        }

        public HttpClientRequestSender(HttpClient httpClient, IReportOutputter reportOutputter)
            : this(httpClient, new HttpRequestMessageMapper(), new ProviderServiceResponseMapper(), reportOutputter)
        {
        }

        public ProviderServiceResponse Send(ProviderServiceRequest request)
        {
            _reportOutputter.Write("Request path before: " + request.Path);

            if (_httpClient.BaseAddress != null && _httpClient.BaseAddress.OriginalString.EndsWith("/"))
            {
                request.Path = request.Path.TrimStart('/');
            }

            var httpRequest = _httpRequestMessageMapper.Convert(request);

            // TODO
            // See http://stackoverflow.com/questions/23438416/why-is-httpclient-baseaddress-not-working
            //_httpClient.BaseAddress = new Uri(@"http://webapi.orders.local/");

            _reportOutputter.Write(string.Format("BaseAddress: {0}", _httpClient.BaseAddress));
            _reportOutputter.Write(string.Format("Request path after: {0}", request.Path));
            _reportOutputter.Write(httpRequest.ToString());

            if (httpRequest.Content != null)
            {
                _reportOutputter.Write(httpRequest.Content.ReadAsStringAsync().Result);
            }

            var httpResponse = _httpClient.SendAsync(httpRequest, CancellationToken.None).Result;

            string result = httpResponse.Content.ReadAsStringAsync().Result;

            _reportOutputter.Write(string.Format("Results body: {0}", result));

            var response = _providerServiceResponseMapper.Convert(httpResponse);

            Dispose(httpRequest);
            Dispose(httpResponse);

            return response;
        }

        private static void Dispose(IDisposable disposable)
        {
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}