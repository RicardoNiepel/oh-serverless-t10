using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace OH.Challenge6
{
    public class orderProcessing
    {
        [FunctionName("orderProcessing")]
        public async Task Run(
            [BlobTrigger("orders/{id}-{type}.csv", Connection = "team10ohchallenge6orders_STORAGE")]Stream myBlob,
            [Blob("orders/{id}-OrderHeaderDetails.csv", FileAccess.ReadWrite, Connection = "team10ohchallenge6orders_STORAGE")] ICloudBlob OrderHeaderDetails,
            [Blob("orders/{id}-OrderLineItems.csv", FileAccess.ReadWrite, Connection = "team10ohchallenge6orders_STORAGE")] ICloudBlob OrderLineItems,
            [Blob("orders/{id}-ProductInformation.csv", FileAccess.ReadWrite, Connection = "team10ohchallenge6orders_STORAGE")] ICloudBlob ProductInformation,
            string id, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{id}");

            if (OrderHeaderDetails != null && OrderLineItems != null && ProductInformation != null)
            {
                log.LogInformation($"All files there for {id}");

                var request = new
                {
                    orderHeaderDetailsCSVUrl = OrderHeaderDetails.StorageUri.PrimaryUri.ToString(),
                    orderLineItemsCSVUrl =  OrderLineItems.StorageUri.PrimaryUri.ToString(),
                    productInformationCSVUrl =  ProductInformation.StorageUri.PrimaryUri.ToString()
                };

                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("team10oh", "1.0.0"));

                var response = await client.PostAsync("https://serverlessohmanagementapi.trafficmanager.net/api/order/combineOrderContent", new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    log.LogError(await response.Content.ReadAsStringAsync());
                    response.EnsureSuccessStatusCode();
                }

                log.LogInformation($"Combined order for {id}");
            }
        }
    }
}
