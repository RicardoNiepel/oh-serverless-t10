using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace T10Company.Function
{
    public static class GetRatings
    {
        [FunctionName("GetRatings")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "teamtendatabase",
                collectionName: "RatingItems",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "SELECT top 100 * FROM r ORDER BY r._ts DESC")]
                IEnumerable<dynamic> ratingItems,
            ILogger log)
        {
            return new OkObjectResult(ratingItems);
        }
    }
}
