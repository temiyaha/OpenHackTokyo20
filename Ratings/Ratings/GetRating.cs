using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Ratings
{
    public static class GetRating
    {
        [FunctionName("GetRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetRating/{ratingId}")] HttpRequest req,
            [CosmosDB(
                databaseName: "openhack20",
                collectionName: "rating",
                ConnectionStringSetting = "CosmosDBConnection",
                Id = "{ratingId}"
                )] Ratings ratings,
        ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            if (ratings == null)
            {
                log.LogInformation("Rating item not found");
            }
            else
            {
                log.LogInformation("Found rating item");
            }
            return ratings != null
                ? (ActionResult)new OkObjectResult($"{JsonConvert.SerializeObject(ratings)}")
            : new NotFoundObjectResult("404 Not Found");
        }
    }
}
