//using System;
//using System.IO;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Extensions.Http;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;

//namespace Ratings
//{
//    public static class CreateRating
//    {
//        [FunctionName("CreateRating")]
//        public static void Run(
//            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
//            [CosmosDB(
//                databaseName: "openhack20",
//                collectionName: "rating",
//                PartitionKey = "/productID",
//                ConnectionStringSetting = "CosmosDBConnection")]out dynamic document,
//            ILogger log)
//        {
//            log.LogInformation("C# HTTP trigger function processed a request.");

//            string productID = req.Query["productID"];

//            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
//            //dynamic data = JsonConvert.DeserializeObject(requestBody);
//            //name = name ?? data?.name;

//            //return new OkResult();

//            document = new { productID, id = Guid.NewGuid() };

//        }
//    }
//}

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

            bool userIdChkFlg = false;
            bool productIdChkFlg = false;
            bool DbInsertErrorFlg = false;

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
            ratings.rating = rating ?? data?.rating;
            ratings.userNotes = userNotes ?? data?.userNotes;

            if (!GetProduct(ratings.productId).Result.IsSuccessStatusCode)
                return new BadRequestObjectResult("Product is not exist.");

            if (!GetUser(ratings.userId).Result.IsSuccessStatusCode)
                return new BadRequestObjectResult("User is not exist.");

            

            //userId notFound check

            //if(userIdChkFlg) return new NotFoundObjectResult("Please ...");

            //productId notFound check

            //if(productIdChkFlg) return new NotFoundObjectResult("Please ...");

            //rating Check? 整数かどうか、もしくは整数化(全角数字等)


            //create id
            ratings.id = Guid.NewGuid().ToString();

            //create timestamp
            ratings.timestamp = DateTime.UtcNow;

            //DBInsert

            //Insert失敗時
            //if (DbInsertErrorFlg) return new UnprocessableEntityObjectResult("Please ....");

            document = ratings;

            //TODO:return check
            return new OkObjectResult(document);
        }

        /// <summary>
        /// Productの取得
        /// </summary>
        /// <param name="userIDID"></param>
        /// <returns></returns>
        private static async Task<HttpResponseMessage> GetProduct(string productId)
        {
            var azureFunctionUrl = $"https://serverlessohlondonproduct.azurewebsites.net/api/GetProduct?productId={productId}";
            return await new HttpClient().GetAsync(azureFunctionUrl);
        }

        /// <summary>
        /// ユーザーの取得
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