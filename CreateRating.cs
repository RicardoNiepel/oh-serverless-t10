using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;

namespace T10Company.Function
{
  public static class CreateRating
  {
    [FunctionName("CreateRating")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
        [CosmosDB(
                databaseName: "teamtendatabase",
                collectionName: "RatingItems",
                ConnectionStringSetting = "CosmosDBConnection")]
                IAsyncCollector<dynamic> ratingItemsOut,
        ILogger log)
    {
      string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
      dynamic data = JsonConvert.DeserializeObject(requestBody);

      if (data.rating < 0 || data.rating > 5)
      {
        return new BadRequestObjectResult("rating field needs to be an integer from 0 to 5");
      }

      data.id = Guid.NewGuid();
      data.timestamp = DateTime.UtcNow;

      var client = new HttpClient();
      var respUserId = await client.GetAsync($"https://serverlessohapi.azurewebsites.net/api/GetUser?userId={data.userId}");
      if (!respUserId.IsSuccessStatusCode)
      {
        return new NotFoundObjectResult($"No user found with id {data.userId}");
      }
      var respProductId = await client.GetAsync($"https://serverlessohapi.azurewebsites.net/api/GetProduct?productId={data.productId}");
      if (!respProductId.IsSuccessStatusCode)
      {
        return new NotFoundObjectResult($"No product found with id {data.productId}");
      }

      await ratingItemsOut.AddAsync(data);

      return new OkObjectResult(data);
    }
  }
}
