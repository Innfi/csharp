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
        public static AmazonDynamoDBConfig DBConfig;
        public static AmazonDynamoDBClient Client;
        public static string TestTableName = "InnfisTable";
        public static string AttrUserId = "UserId";
        public string AttrUserName = "UserName";
        public string AttrUserScore = "UserScore";

        [ClassInitialize]
        public static void ClassSetUp(TestContext context) {
            DBConfig = new AmazonDynamoDBConfig { ServiceURL = "http://127.0.0.1:8000" };
            Client = new AmazonDynamoDBClient(DBConfig);

            CreateTable();
        }

        protected static void CreateTable() {
            var request = new CreateTableRequest(TestTableName,
                new List<KeySchemaElement> {
                    new KeySchemaElement(AttrUserId, KeyType.HASH)
                },
                new List<AttributeDefinition> {
                    new AttributeDefinition(AttrUserId, ScalarAttributeType.N)
                },
                new ProvisionedThroughput(1, 1));

            var response = Client.CreateTable(request);
        }

        [ClassCleanup]
        public static void ClassTearDown() {
            var request = new DeleteTableRequest(TestTableName);

            Client.DeleteTable(request);
        }

        [TestMethod]
        public void Test1InsertNGetRecord() {
            var dummyUserId = 3;

            var request = ToDummyUpdateItemRequest(dummyUserId);
            var response = Client.UpdateItem(request);

            Assert.AreEqual(response.HttpStatusCode, HttpStatusCode.OK);

            var queryRequest = ToDummyQueryRequest(dummyUserId);
            var queryResponse = Client.Query(queryRequest);

            Assert.AreEqual(queryResponse.HttpStatusCode, HttpStatusCode.OK);
            var items = queryResponse.Items[0];

            Assert.AreEqual(items[AttrUserName].S, "Ennfi");
            Assert.AreEqual(items[AttrUserScore].N, "100");
        }

        protected UpdateItemRequest ToDummyUpdateItemRequest(int userId) {
            var dict = new Dictionary<string, AttributeValueUpdate>();
            dict[AttrUserName] = new AttributeValueUpdate
            {
                Action = "PUT",
                Value = new AttributeValue
                {
                    S = "Ennfi"
                }
            };
            dict[AttrUserScore] = new AttributeValueUpdate
            {
                Action = "PUT",
                Value = new AttributeValue
                {
                    N = "100"
                }
            };

            return new UpdateItemRequest
            {
                TableName = TestTableName,
                Key = new Dictionary<string, AttributeValue> {
                    { AttrUserId, new AttributeValue { N = userId.ToString() } }
                },
                AttributeUpdates = dict
            };
        }

        protected QueryRequest ToDummyQueryRequest(int userId) {
            var keyCondExpr = AttrUserId + " = :v_userId";
            var projExpr = AttrUserName + ", " + AttrUserScore;

            return new QueryRequest
            {
                TableName = TestTableName,
                Select = "SPECIFIC_ATTRIBUTES",
                ProjectionExpression = projExpr,
                KeyConditionExpression = keyCondExpr,
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    { ":v_userId", new AttributeValue { N = userId.ToString() } }
                }
            };
        }
    }
}