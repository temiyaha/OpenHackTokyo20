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

namespace Ratings
{
    public static class GetRatings
    {
        [FunctionName("GetRatings")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "GetRatings/{userId}")]HttpRequest req,
            [CosmosDB(
                databaseName: "openhack20",
                collectionName: "rating",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "SELECT * FROM c WHERE c.userId = {userId}")] IEnumerable<Ratings> items,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            foreach (var item in items)
            {
                log.LogInformation($"id={item.id} userId={item.userId} productId={item.productId}");
            }

            return new OkObjectResult(items);

        }
    }
}
