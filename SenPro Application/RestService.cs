using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Savonia.Measurements.Clients.Service20;
using Newtonsoft.Json;

namespace SenPro_Application
{
    /// <summary>
    /// This is the RESTful service used to make HTTP POST requests
    /// to SaMi. Getting it to work took some time and communication
    /// with Miko, and changing it is not advised.
    /// </summary>
    class RestService
    {
        HttpClient client;
        public List<TodoItem> Items { get; private set; }
        public static string saMiUrl = "https://sami.savonia.fi/Service/2.0/MeasurementsService.svc/json";
        public static string saMiUrlTests = "https://sami.savonia.fi/Service/1.0/MeasurementsService.svc/json/measurements/save";
        public static string samiImportHandler = "https://sami.savonia.fi/Manage/Import/Push/CommonSaMiData";
        public string debugUrl = "https://sami.savonia.fi/Manage/Import/Push/DebugHandler-1";
        public static string testUrl = "http://httpbin.org/post";

        public RestService()
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
        }

        public async Task SavePackageAsync(MeasurementPackage item, bool isNewItem = true)
        {
            var uri = new Uri(string.Format(samiImportHandler, string.Empty));

            try
            {

                var json = JsonConvert.SerializeObject(item);
                Debug.WriteLine(@"      Json: " + json);

                var content = new StringContent(json, Encoding.UTF8, "application/x-www-form-urlencoded");

                HttpResponseMessage response = null;
                if (isNewItem)
                {
                    response = await client.PostAsync(uri, content);
                    if (response.IsSuccessStatusCode)
                        Debug.WriteLine(@"      TodoItem succesfully saved");
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(@"				ERROR {0}", ex.Message);
            }
        }
    }

    public class TodoItem
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public bool Done { get; set; }
    }
}