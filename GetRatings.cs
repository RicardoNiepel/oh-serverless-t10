using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace T10Company.Function
{
    public static class GetRatings
    {
        [FunctionName("GetRatings")]
        public static Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "teamtendatabase",
                collectionName: "RatingItems",
                ConnectionStringSetting = "CosmosDBConnection"
                )] DocumentClient client,
        ILogger log)
    {
      var userId = req.Query["userId"];
      if (string.IsNullOrWhiteSpace(userId))
      {
        return (ActionResult)new NotFoundResult();
      }

      Uri collectionUri = UriFactory.CreateDocumentCollectionUri("teamtendatabase", "RatingItems");

      log.LogInformation($"Searching for: {userId}");

      var options = new FeedOptions { EnableCrossPartitionQuery = true };

      IDocumentQuery<IDictionary<string, object>> query = client.CreateDocumentQuery<IDictionary<string, object>>(collectionUri, options)
          .Where(r => r["userId"].ToString() == userId.ToString())
          .AsDocumentQuery();

      return new OkObjectResult(query);
    }
  }
}
