using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // connect to local host
            MongoClient client = new MongoClient("mongodb://127.0.0.1:27017/test");
            MongoServer server = client.GetServer();

            // "test" is the name of the database.

            MongoDatabase database = server.GetDatabase("test");
            MongoDatabase testDatabase = server.GetDatabase("newtest");
            MongoCollection<BsonDocument> bankData =
                database.GetCollection<BsonDocument>("bank_data");

            BsonDocument person = new BsonDocument
            {
                { "first_name", "Stevenb"},
                { "last_name", "Edouard"},
                { "accounts", new BsonArray {
                    new BsonDocument {
                        { "account_balance",     50000000},
                        {"account_type"   , "Investment"},
                        {"currency"       ,        "USD"}
                    }}
                }
            };

            bankData.Insert(person);
            Console.WriteLine(person["_id"]);
            //increment this person's balance by 100000
            person["accounts"][0]["account_balance"] = person["accounts"][0]["account_balance"].AsInt32 + 100000;
            // Console.Read();

            bankData.Save(person);
            Console.WriteLine("Successfully updated one document.");

            //retrieve the inserted collection from MongoDB
            //should be the updated object
            BsonDocument newperson = bankData.FindOneById(person["_id"]);
            //check if account balance was updated
            Console.WriteLine(newperson["_id"]);
            Console.WriteLine(newperson["accounts"][0]["account_balance"]);

            bankData.FindOneByIdAs<BsonDocument>("No_123");
            bankData.FindOneAs<BsonDocument>(Query.EQ("_id", newperson["_id"]));


            //delete the account we just inserted
            var query = Query.EQ("_id", newperson["_id"]);
            Console.WriteLine(query);

            WriteConcernResult result = bankData.Remove(query);
            Console.WriteLine("Number of documents removed: " + result.DocumentsAffected);

        }
    }
}
