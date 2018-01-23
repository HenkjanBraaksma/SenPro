using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Savonia.Measurements.Clients.Service20
{
    /// <summary>
    /// A soap client for Savonia Measurements System (SaMi). Use <see cref="MeasurementsServiceClient.GetClient(string)"/> to get the client.
    /// </summary>
    public partial class MeasurementsServiceClient
    {
        public static MeasurementsServiceClient GetClient(string serviceUrl, EndpointConfiguration endpointConfiguration)
        {
            if (string.IsNullOrEmpty(serviceUrl))
            {
                return new MeasurementsServiceClient(endpointConfiguration);
            }
            else
            {
                return new MeasurementsServiceClient(endpointConfiguration, serviceUrl);
            }
        }

        /// <summary>
        /// <see cref="MeasurementsServiceClient"/> configure implementation
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(ServiceEndpoint serviceEndpoint, ClientCredentials clientCredentials)
        {
            if (serviceEndpoint.Binding.Scheme != serviceEndpoint.Address.Uri.Scheme)
            {
                serviceEndpoint.Binding = GetBinding(serviceEndpoint.Address.Uri.Scheme);
            }
        }

        private static Binding GetBinding(string scheme)
        {
            if ("https".Equals(scheme, StringComparison.InvariantCultureIgnoreCase))
            {
                System.ServiceModel.BasicHttpsBinding result = new System.ServiceModel.BasicHttpsBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            else
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
        }
    }
}
