using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MongoDB.Driver;
using AzureFunction_MongoDB_Demo.Models;
using MongoDB.Bson;

namespace AzureFunction_MongoDB_Demo
{
    public class ProductAPI
    {
       
        public static IMongoDatabase GetMongoDatabase()
        {
            MongoClient mongoClient = new MongoClient(Environment.GetEnvironmentVariable("mongodb"));
            return mongoClient.GetDatabase("E-Commerce");
        }
        [FunctionName("AddProduct")]
        public async Task<IActionResult> AddProduct(
           [HttpTrigger(AuthorizationLevel.Function, "post", Route = "AddProduct")] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {

                IMongoDatabase database = GetMongoDatabase();

                IMongoCollection<Product> collection = database.GetCollection<Product>("products");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var product = JsonConvert.DeserializeObject<Product>(requestBody);
                await collection.InsertOneAsync(product);
                log.LogInformation("Product added in MongoDB");
                return new OkObjectResult(product);
            }
            catch (Exception ex)
            {
                log.LogError($"Error occurred: {ex.Message} ");
                return new BadRequestObjectResult(ex.Message);
            }
        }
        [FunctionName("GetAllProduct")]
        public async Task<IActionResult> GetAllProduct(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetAllProduct")] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {

                IMongoDatabase database = GetMongoDatabase();

                IMongoCollection<Product> collection = database.GetCollection<Product>("products");

                var products = await collection.Find(new BsonDocument()).ToListAsync();

                return new OkObjectResult(products);
            }
            catch (Exception ex)
            {
                log.LogError($"Error occurred: {ex.Message} ");
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
