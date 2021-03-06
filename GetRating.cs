using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace T10Company.Function
{
    public static class GetRating
    {
        [FunctionName("GetRating")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "rating/{ratingId}")] HttpRequest req,
            [CosmosDB(
                databaseName: "teamtendatabase",
                collectionName: "RatingItems",
                ConnectionStringSetting = "CosmosDBConnection",
                Id = "{ratingId}",
                PartitionKey = "{ratingId}")] dynamic ratingItem,
        ILogger log)
    {
      if (ratingItem == null)
      {
        return new NotFoundObjectResult($"No rating found with id {req.Query["ratingId"]}");
      }
      else
      {
        return new OkObjectResult(ratingItem);
      }
    }
  }
}
