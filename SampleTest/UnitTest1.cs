using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;


namespace SampleTest
{
    [TestClass]
    public class SampleTestDynamoDB
    {
        [TestMethod]
        public void Test0CallClient() {
            var serviceUrl = "http://127.0.0.1:8000";
            var ddbConfig = new AmazonDynamoDBConfig {
                ServiceURL = serviceUrl
            };
            var client = new AmazonDynamoDBClient(ddbConfig);
        }

        [TestMethod]
        public void Test1CreateTable() {
            var serviceUrl = "http://127.0.0.1:8000";
            var ddbConfig = new AmazonDynamoDBConfig {
                ServiceURL = serviceUrl
            };
            var client = new AmazonDynamoDBClient(ddbConfig);

            var request = new CreateTableRequest("DummyTable",
                new List<KeySchemaElement> {
                    new KeySchemaElement("UserId", KeyType.HASH)
                },
                new List<AttributeDefinition> {
                    new AttributeDefinition("UserId", ScalarAttributeType.N)
                },
                new ProvisionedThroughput(1, 1));

            var response = client.CreateTable(request);

            Assert.AreEqual(response.HttpStatusCode, HttpStatusCode.OK);
        }
    }
}
