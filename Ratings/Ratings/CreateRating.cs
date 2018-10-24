using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.WebJobs.Extensions.CosmosDB;
using System.Net.Http;

namespace Ratings
{
    public static class CreateRating
    {
        [FunctionName("CreateRating")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [CosmosDB(
              databaseName: "openhack20",
              collectionName: "rating",
               ConnectionStringSetting = "CosmosDBConnection")]out dynamic document,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            document = null;

            string userId = req.Query["userId"];
            string productId = req.Query["productId"];
            string locationName = req.Query["locationName"];
            string rating = req.Query["rating"];
            string userNotes = req.Query["userNotes"];

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            Ratings ratings = new Ratings();

            ratings.userId = userId ?? data?.userId;
            ratings.productId = productId ?? data?.productId;
            ratings.locationName = locationName ?? data?.locationName;
            ratings.userNotes = userNotes ?? data?.userNotes;
            var ratingString = rating ?? data?.rating;

            if (!GetProduct(ratings.productId).Result.IsSuccessStatusCode)
                return new NotFoundObjectResult("Product is not exist.");

            if (!GetUser(ratings.userId).Result.IsSuccessStatusCode)
                return new NotFoundObjectResult("User is not exist.");

            int rating_int = 0;
            if (!int.TryParse(ratingString.ToString(), out rating_int))
                return new BadRequestObjectResult("Rating is not integer.");

            if (rating_int<0 || rating_int>5)
                return new BadRequestObjectResult("Rating is out of range.");


            ratings.rating = rating_int;

            //create id
            ratings.id = Guid.NewGuid().ToString();

            //create timestamp
            ratings.timestamp = DateTime.UtcNow;

            document = ratings;
            return new OkObjectResult(document);
        }

        /// <summary>
        /// ProductÇÃéÊìæ
        /// </summary>
        /// <param name="userIDID"></param>
        /// <returns></returns>
        private static async Task<HttpResponseMessage> GetProduct(string productId)
        {
            var azureFunctionUrl = $"https://serverlessohlondonproduct.azurewebsites.net/api/GetProduct?productId={productId}";
            return await new HttpClient().GetAsync(azureFunctionUrl);
        }

        /// <summary>
        /// ÉÜÅ[ÉUÅ[ÇÃéÊìæ
        /// </summary>
        /// <param name="userIDID"></param>
        /// <returns></returns>
        private static async Task<HttpResponseMessage> GetUser(string userID)
        {
            var azureFunctionUrl = $"https://serverlessohlondonuser.azurewebsites.net/api/GetUser?userId={userID}";
            return await new HttpClient().GetAsync(azureFunctionUrl);
        }

    }

    public class Ratings
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string productId { get; set; }
        public DateTime timestamp { get; set; }
        public string locationName { get; set; }
        public int rating { get; set; }
        public string userNotes { get; set; }
    }
}