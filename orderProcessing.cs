using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace OH.Challenge6
{
    public class orderProcessing
    {
        [FunctionName("orderProcessing")]
        public void Run([BlobTrigger("orders/{name}", Connection = "team10ohchallenge6orders_STORAGE")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
