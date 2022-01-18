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
    public static class CreateRating
    {
        [FunctionName("CreateRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            if(data.rating < 0 || data.rating > 5) {
                return new BadRequestObjectResult("rating field needs to be an integer from 0 to 5");
            }
            
            data.id = Guid.NewGuid();
            data.timestamp = DateTime.UtcNow;

            // TODO: Validate both userId and productId by calling the existing API endpoints. You can find a user id to test with from the sample payload above
            // TODO: Validate both userId and productId by calling the existing API endpoints. You can find a user id to test with from the sample payload above

            return new OkObjectResult(data);
        }
    }
}
