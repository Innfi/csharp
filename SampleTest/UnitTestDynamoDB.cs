using System.Collections.Generic;
using System.Linq;
using System.Net;
using System;
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

            var listTableResponse = Client.ListTables();
            if (listTableResponse.TableNames.Contains(TestTableName)) return;
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
            dict[AttrUserName] = new AttributeValueUpdate {
                Action = "PUT",
                Value = new AttributeValue { S = "Ennfi" }
            };
            dict[AttrUserScore] = new AttributeValueUpdate {
                Action = "PUT",
                Value = new AttributeValue { N = "100" }
            };

            return new UpdateItemRequest {
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

            return new QueryRequest {
                TableName = TestTableName,
                Select = "SPECIFIC_ATTRIBUTES",
                ProjectionExpression = projExpr,
                KeyConditionExpression = keyCondExpr,
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    { ":v_userId", new AttributeValue { N = userId.ToString() } }
                }
            };
        }

        [TestMethod]
        public void Test2InsertNumberSet() {
            var tableName = "GameHistory";
            CreateGameHistoryTable(tableName);

            var userIds = GenerateDummyUserIds();

            var dict = new Dictionary<string, AttributeValueUpdate>();
            dict["Userids"] = new AttributeValueUpdate {
                Action = "PUT",
                Value = new AttributeValue {
                    NS = userIds.ToList()
                }
            };

            var request = new UpdateItemRequest {
                TableName = tableName,
                Key = new Dictionary<string, AttributeValue>() {
                    { "GameId", new AttributeValue { N = "12345" } }
                },
                AttributeUpdates = dict
            };

            var response = Client.UpdateItem(request);

            Assert.AreEqual(response.HttpStatusCode, HttpStatusCode.OK);
        }

        protected void CreateGameHistoryTable(string tableName)
        {
            var createTableRequest = new CreateTableRequest(tableName,
                new List<KeySchemaElement> {
                    new KeySchemaElement("GameId", KeyType.HASH)
                },
                new List<AttributeDefinition> {
                    new AttributeDefinition("GameId", ScalarAttributeType.N)
                },
                new ProvisionedThroughput(1, 1));

            Client.CreateTable(createTableRequest);
        }

        protected HashSet<string> GenerateDummyUserIds()
        {
            var userIds = new HashSet<string>();
            var random = new Random();

            for (int i = 0; i < 20; i++) userIds.Add(random.Next(1, 200).ToString());

            return userIds;
        }

        [TestMethod]
        public void Test3UpdateNumberSet() {
            var tableName = "GameHistory";
            DeleteTable(tableName);

            CreateGameHistoryTable(tableName);
            var userIds = GenerateDummyUserIds();
            InsertUserIds(tableName, userIds);

            var newUserIds = new List<string>();
            newUserIds.Add("100");
            newUserIds.Add("101");
            newUserIds.Add("102");

            var dict = new Dictionary<string, AttributeValueUpdate>();
            dict["UserIds"] = new AttributeValueUpdate {
                Action = "ADD",
                Value = new AttributeValue { NS = newUserIds }
            };

            var request = new UpdateItemRequest {
                TableName = tableName,
                Key = new Dictionary<string, AttributeValue>() {
                    { "GameId", new AttributeValue { N = "12345" } }
                },
                AttributeUpdates = dict
            };

            var response = Client.UpdateItem(request);
            Assert.AreEqual(response.HttpStatusCode, HttpStatusCode.OK);

            DeleteTable(tableName);
        }

        protected void InsertUserIds(string tableName, HashSet<string> userIds)
        {
            var dict = new Dictionary<string, AttributeValueUpdate>();
            dict["UserIds"] = new AttributeValueUpdate {
                Action = "PUT",
                Value = new AttributeValue { NS = userIds.ToList() }
            };

            var request = new UpdateItemRequest {
                TableName = tableName,
                Key = new Dictionary<string, AttributeValue>() {
                    { "GameId", new AttributeValue { N = "12345" } }
                },
                AttributeUpdates = dict
            };

            var response = Client.UpdateItem(request);
        }

        protected void DeleteTable(string tableName)
        {
            var response = Client.ListTables();
            if (!response.TableNames.Contains(tableName)) return;

            Client.DeleteTable(new DeleteTableRequest { TableName = tableName });
        }

        [TestMethod]
        public void Test4QueryItems() {
            var tableName = "GameHistory";
            DeleteTable(tableName);
            CreateGameHistoryTable(tableName);
            var userIds = GenerateDummyUserIds();
            InsertUserIds(tableName, userIds);

            var queryRequest = new QueryRequest {
                TableName = tableName,
                Select = "SPECIFIC_ATTRIBUTES",
                ProjectionExpression = "UserIds",
                KeyConditionExpression = "GameId = :gameId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    { ":gameId", new AttributeValue { N = "12345" } }
                }
            };

            var response = Client.Query(queryRequest);
            var resultUserIds = response.Items[0]["UserIds"].NS;

            Assert.AreEqual(userIds.Count, resultUserIds.Count);
            foreach (var resultUserId in resultUserIds) {
                Assert.AreEqual(userIds.Contains(resultUserId), true);
            }

            DeleteTable(tableName);
        }

        [TestMethod]
        public void Test5LocalSeconaryIndex() {
            var tableName = "GameHistory";
            DeleteTable(tableName);

            var createTableRequest = new CreateTableRequest(tableName,
                new List<KeySchemaElement> {
                    new KeySchemaElement("GameId", KeyType.HASH),
                    new KeySchemaElement("GameType", KeyType.RANGE)
                },
                new List<AttributeDefinition> {
                    new AttributeDefinition("GameId", ScalarAttributeType.N),
                    new AttributeDefinition("GameType", ScalarAttributeType.N),
                    new AttributeDefinition("SeasonId", ScalarAttributeType.N)
                },
                new ProvisionedThroughput(1, 1));

            var lsiGameSeason = new LocalSecondaryIndex {
                IndexName = "GameSeasonIndex",
                KeySchema = new List<KeySchemaElement> {
                    new KeySchemaElement("GameId", KeyType.HASH),
                    new KeySchemaElement("SeasonId", KeyType.RANGE)
                },
                Projection = new Projection {
                    ProjectionType = "INCLUDE",
                    NonKeyAttributes = new List<string> {
                        "UserIds"
                    }
                }
            };

            createTableRequest.LocalSecondaryIndexes.Add(lsiGameSeason);

            var response = Client.CreateTable(createTableRequest);

            Assert.AreEqual(response.HttpStatusCode, HttpStatusCode.OK);

            PutDummyItems(tableName);

            var queryRequest = new QueryRequest(tableName);
            queryRequest.IndexName = "GameSeasonIndex";
            queryRequest.KeyConditionExpression =
                "GameId = :v_gameId and SeasonId = :v_seasonId";
            queryRequest.ExpressionAttributeValues[":v_gameId"] =
                new AttributeValue { N = "5" };
            queryRequest.ExpressionAttributeValues[":v_seasonId"] =
                new AttributeValue { N = "15" };
            queryRequest.ProjectionExpression = "GameId, GameType, SeasonId";
            var queryResponse = Client.Query(queryRequest);

            Assert.AreEqual(queryResponse.HttpStatusCode, HttpStatusCode.OK);

            DeleteTable(tableName);
        }

        protected void PutDummyItems(string tableName) {
            var random = new Random();
            for (int i = 0; i < 10; i++) {
                string dummyGameType = random.Next(5).ToString();
                string seasonid = (i + 10).ToString();

                var itemsDict = new Dictionary<string, AttributeValue>();
                itemsDict["GameId"] = new AttributeValue { N = i.ToString() };
                itemsDict["GameType"] = new AttributeValue { N = dummyGameType };
                itemsDict["SeasonId"] = new AttributeValue { N = seasonid };

                Client.PutItem(new PutItemRequest(tableName, itemsDict));
            }
        }

        [TestMethod]
        public void Test6GlobalSeconaryIndex() {
            var tableName = "GameHistory";
            DeleteTable(tableName);

            var gsiSeaonalKeyPlayer = new GlobalSecondaryIndex {
                IndexName = "SeasonalKeyPlayerIndex",
                ProvisionedThroughput = new ProvisionedThroughput {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 1
                },
                Projection = new Projection { ProjectionType = "ALL" },
                KeySchema = new List<KeySchemaElement> {
                    { new KeySchemaElement { AttributeName = "SeasonId", KeyType = "HASH"} },
                    { new KeySchemaElement { AttributeName = "KeyPlayerId", KeyType = "RANGE" } }
                }
            };

            var createTableRequest = new CreateTableRequest {
                TableName = tableName,
                KeySchema = new List<KeySchemaElement> {
                    new KeySchemaElement("GameId", KeyType.HASH),
                    new KeySchemaElement("GameType", KeyType.RANGE)
                },
                AttributeDefinitions = new List<AttributeDefinition> {
                    new AttributeDefinition("GameId", ScalarAttributeType.N),
                    new AttributeDefinition("GameType", ScalarAttributeType.N),
                    new AttributeDefinition("SeasonId", ScalarAttributeType.N),
                    new AttributeDefinition("KeyPlayerId", ScalarAttributeType.N)
                },
                ProvisionedThroughput = new ProvisionedThroughput(1, 1),
                GlobalSecondaryIndexes = { gsiSeaonalKeyPlayer }
            };

            var response = Client.CreateTable(createTableRequest);
            Assert.AreEqual(response.HttpStatusCode, HttpStatusCode.OK);

            var queryRequest = new QueryRequest {
                TableName = tableName,
                IndexName = gsiSeaonalKeyPlayer.IndexName,
                KeyConditionExpression = "SeasonId = :season_id and KeyPlayerId = :player_id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    { ":season_id", new AttributeValue { N = "1" } },
                    { ":player_id", new AttributeValue { N = "1234" } }
                },
                ScanIndexForward = true
            };

            var queryResponse = Client.Query(queryRequest);
        }

        [TestMethod]
        public void Test7ScanItems()
        {
            var tableName = "GameHistory";
            DeleteTable(tableName);
            CreateTableWithLSI(tableName);
            PutDummyItems(tableName);

            var scanRequest = new ScanRequest {
                TableName = tableName,
                Limit = 5,
                FilterExpression = "SeasonId < :season_id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    { ":season_id", new AttributeValue { N = "15" } }
                }
            };

            var scanResponse = Client.Scan(scanRequest);
            Assert.AreEqual(scanResponse.HttpStatusCode, HttpStatusCode.OK);
        }

        protected CreateTableResponse CreateTableWithLSI(string tableName) {

            var lsiGameSeason = new LocalSecondaryIndex
            {
                IndexName = "GameSeasonIndex",
                KeySchema = new List<KeySchemaElement> {
                    new KeySchemaElement("GameId", KeyType.HASH),
                    new KeySchemaElement("SeasonId", KeyType.RANGE)
                },
                Projection = new Projection { ProjectionType = "ALL" }
            };

            var createTableRequest = new CreateTableRequest
            {
                TableName = tableName,
                KeySchema = new List<KeySchemaElement> {
                    new KeySchemaElement("GameId", KeyType.HASH),
                    new KeySchemaElement("GameType", KeyType.RANGE)
                },
                AttributeDefinitions = new List<AttributeDefinition> {
                    new AttributeDefinition("GameId", ScalarAttributeType.N),
                    new AttributeDefinition("GameType", ScalarAttributeType.N),
                    new AttributeDefinition("SeasonId", ScalarAttributeType.N)
                },
                ProvisionedThroughput = new ProvisionedThroughput(1, 1)
            };
            createTableRequest.LocalSecondaryIndexes.Add(lsiGameSeason);

            return Client.CreateTable(createTableRequest);
        }
    }
}