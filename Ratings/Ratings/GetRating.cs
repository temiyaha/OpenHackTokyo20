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
                //databaseName: "openhack20",
                databaseName: "miyaharahack20",
                collectionName: "rating",
                //PartitionKey= "/productId",
                ConnectionStringSetting = "CosmosDBConnection2",
                Id = "{ratingId}")] RatingItem ratingItem,
        ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            if (ratingItem == null)
            {
                log.LogInformation("Rating item not found");
            }
            else
            {
                log.LogInformation("Found rating item");
            }
            return ratingItem != null
                ? (ActionResult)new OkObjectResult($"{JsonConvert.SerializeObject(ratingItem)}")
            : new NotFoundObjectResult("404 Not Found");
        
        }
    }

    public class RatingItem
    { 
        public string id { get; set; }
        public string userId { get; set; }
        public string productId { get; set; }
        public string timestamp { get; set; }
        public string locationName { get; set; }
        public string rating { get; set; }
        public string userNotes { get; set; }

    }
}
