using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;


namespace SampleCSharp
{
    public class SampleDynamoDB
    {
        protected readonly AmazonDynamoDBClient client;

        public SampleDynamoDB()
        {
            var serviceUrl = "http://127.0.0.1:8000";

            var ddbConfig = new AmazonDynamoDBConfig
            {
                ServiceURL = serviceUrl
            };

            client = new AmazonDynamoDBClient(ddbConfig);
        }
    }
}
