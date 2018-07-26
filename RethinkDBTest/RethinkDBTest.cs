using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Threading;
using RethinkDb.Driver.Net;
using RethinkDb.Driver.Model;

namespace RethinkDBTest
{
    [TestClass]
    public class ReThinkDBTest
    {

        static readonly string Hostname = RethinkQuery.Hostname;
        static readonly int Timeout = RethinkQuery.Timeout;
        static readonly string DatabaseName = RethinkQuery.DatabaseName;
        static readonly string TableName = RethinkQuery.TableName;

        [TestMethod]
        public void TestConnection()
        {
            var R = RethinkDb.Driver.RethinkDB.R;

            try
            {
                var connection = R.Connection().Hostname(Hostname).Timeout(Timeout).Connect();
                var result = R.Now().Run<DateTimeOffset>(connection);
                connection.Close();
                Console.WriteLine(result);
                Assert.IsNotNull(result);
            }
            catch(Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void CreateDatabase()
        {
            RethinkQuery.RunQuery((R, connection) =>
            {
                if (R.DbList().Contains(DatabaseName).Run(connection))
                {
                    R.DbDrop(DatabaseName).Run(connection);
                }
                var result = R.DbCreate(DatabaseName).Run(connection);
                return result;
            });
        }

        [TestMethod]
        public void DeleteDatabase()
        {
            RethinkQuery.RunQuery((R, connection) =>
            {
                dynamic result = null;
                if (R.DbList().Contains(DatabaseName).Run(connection))
                {
                    result = R.DbDrop(DatabaseName).Run(connection);
                }
                return result;
            });
        }

        [TestMethod]
        public void CreateTable()
        {
            RethinkQuery.RunQuery((R, connection) =>
            {
                if (R.Db(DatabaseName).TableList().Contains(TableName).Run(connection))
                {
                    R.Db(DatabaseName).TableDrop(TableName).Run(connection);
                }
                var result = R.Db(DatabaseName).TableCreate(TableName).Run(connection);
                return result;
            });   
            
        }

        [TestMethod]
        public void DeleteTable()
        { 
            RethinkQuery.RunQuery((R, connection) => 
            {
                if (R.Db(DatabaseName).TableList().Contains(TableName).Run(connection))
                {
                    dynamic result = R.Db(DatabaseName).TableDrop(TableName).Run(connection);
                    return result;
                }
                else return null;
            });
            
        }

        [TestMethod]
        public void LoadJson()
        //Insert into Table via JSON file
        { 
            dynamic report = null;
            using (StreamReader reader = new StreamReader("fba_base.json"))
            {
                String json = reader.ReadToEnd();
                report = JsonConvert.DeserializeObject(json);
            }

            const int NUM_SCENARIOS = 5000;
            const int NUM_THREADS = 20;
            const int BATCH_SIZE = 25;
            const int SCENARIOS_PER_THREAD = NUM_SCENARIOS / NUM_THREADS;

            List<dynamic> reports = new List<dynamic>();
            for (int i = 0; i < BATCH_SIZE; i++)
            {
                reports.Add(report);
            }

            void insert()
            {
                RethinkQuery.RunQuery((R, connection) =>
                {

                    const int NUM_BATCHES = SCENARIOS_PER_THREAD / BATCH_SIZE;

                    Result result = null;
                    for (int i = 0; i < NUM_BATCHES; i++)
                    {
                        result = R.Db(DatabaseName).Table(TableName).Insert(reports).RunResult(connection);
                    }
                    return "";
                });
            }

            List<Thread> threads = new List<Thread>(NUM_THREADS);
            for (int i = 0; i < threads.Capacity; i++)
            {
                threads.Add(new Thread(new ThreadStart(insert)));
                threads[i].Start();
            }

            bool insertFinished = false;
            while (!insertFinished)
            {
                insertFinished = true;
                foreach (Thread thread in threads)
                {
                    if (thread.IsAlive)
                    {
                        insertFinished = false;
                    }
                }
                Thread.Sleep(1000);
            }
        }

        [TestMethod]
        public void InsertScenarios()
        //Insert into Table by creating objects
        {
            const int NUM_SCENARIOS = 500;
            const int NUM_THREADS = 5;
            const int SCENARIOS_PER_THREAD = NUM_SCENARIOS / NUM_THREADS;
            const int BATCH_SIZE = 50;

            List<Scenario> scenarios = new List<Scenario>(BATCH_SIZE);
            for (int j = 0; j < BATCH_SIZE; j++)
            {
                scenarios.Add(new Scenario());
            }

            void insert()
            {
                RethinkQuery.RunQuery((R, connection) =>
                {
                    const int NUM_BATCHES = SCENARIOS_PER_THREAD / BATCH_SIZE;
                    dynamic result = null;
                    for(int i = 0; i < NUM_BATCHES; i++)
                    {
                        result = R.Db(DatabaseName).Table(TableName).Insert(scenarios).Run(connection);
                    }
                    return result;
                });
                
            }

            List<Thread> threads = new List<Thread>(NUM_THREADS);
            for(int i = 0; i < threads.Capacity; i++)
            {
                threads.Add(new Thread(new ThreadStart(insert)));
                threads[i].Start();
            }

            bool insertFinished = false;
            while(!insertFinished)
            {
                insertFinished = true;
                foreach(Thread thread in threads)
                {
                    if(thread.IsAlive)
                    {
                        insertFinished = false;
                    }
                }
                Thread.Sleep(1000);
            }
        }

        [TestMethod]
        public void RandomizeValues()
        //Randomize specific fields for use in Filter/Pluck tests
        {
            RethinkQuery.RunQuery((R, connection) =>
            {
                Random r = new Random();
                Cursor<RethinkQuery.ID> ids = R.Db(DatabaseName).Table(TableName).Pluck("id").RunCursor<RethinkQuery.ID>(connection);

                dynamic result = null;

                foreach (RethinkQuery.ID id in ids)
                {
                    String _id = id.id.ToString();
                    JObject scenario = R.Db(DatabaseName).Table(TableName).Get(_id).Run<JObject>(connection);

                    /*_reports.ForEach(x => x["OutstandingBal"] = r.NextDouble() * 100000);
                    _reports.ForEach(x => x["Current"] = r.NextDouble() * 100000);
                    _reports.ForEach(x => x["PropertyValue"] = r.NextDouble() * 100000);*/
                    scenario["PortfolioId"] = r.Next(0, 100);
                    result = R.Db(DatabaseName).Table(TableName).Get(_id)
                                      .Update(scenario)
                                      .Run(connection);
                }
                return result;
            });
        }

        [TestMethod]
        public void GetAll()
        {
            RethinkQuery.RunQuery((R, connection) =>  
            {
                Cursor<Scenario> cursor = R.Db(DatabaseName).Table(TableName).Limit(100).RunCursor<Scenario>(connection);
                List<Scenario> scenarios = new List<Scenario>();
                foreach(Scenario scenario in cursor)
                {
                    scenarios.Add(scenario);
                }
                return JsonConvert.SerializeObject(scenarios);
            });
        }

        [TestMethod]
        public void GetIds()
        {
            RethinkQuery.RunQuery((R, connection) =>
            {
                String json = "[";
                RethinkQuery.GetIds(R, connection).ForEach(item => json += item + ",");
                json = json.TrimEnd(',');
                return json + "]";
            });
        }

        [TestMethod]
        public void GetById()
        {
            RethinkQuery.RunQuery((R, connection) =>
            {
                String id = RethinkQuery.GetIds(R, connection)[0];
                dynamic result = R.Db(DatabaseName).Table(TableName).Get(id).Run(connection);
                return result;
            });
        }

        [TestMethod]
        public void GetReports()
        {
            RethinkQuery.RunQuery((R, connection) =>
            {
                String id = RethinkQuery.GetIds(R, connection)[0];
                JArray result = R.Db(DatabaseName).Table(TableName).Get(id)["_reports"]["FBA Base"]
                                 .Run<JObject>(connection);
                return result;
            });
        }

        [TestMethod]
        public void GetOrderedByOutstandingBal()
        {
            RethinkQuery.RunQuery((R, connection) =>
            {
                String id = RethinkQuery.GetIds(R, connection)[0];
                dynamic result = R.Db(DatabaseName).Table(TableName).Get(id)["_reports"]["FBA Base"]
                                  .Filter(x => x.G("OutstandingBal").Gt(0))
                                  .OrderBy(R.Desc("OutstandingBal")).Run(connection);

                return result;
            });
        }

        [TestMethod]
        public void CountReports()
        {
            const string DatabaseName = RethinkQuery.DatabaseName;
            const string TableName = RethinkQuery.TableName;

            RethinkQuery.RunQuery((R, connection) =>
            {
                String id = RethinkQuery.GetIds(R, connection)[0];
                dynamic result = R.Db(DatabaseName).Table(TableName)
                                  .Count().Run(connection);

                return result;
            });
        }

        [TestMethod]
        public void DoStatFunctions()
        {
            RethinkQuery.RunQuery((R, connection) =>
            {
                String id = RethinkQuery.GetIds(R, connection)[0];
                var balances = R.Db(DatabaseName).Table(TableName).Get(id)["_reports"]["FBA Base"]["OutstandingBal"];

                var min = balances.Min().Round().Run(connection);
                var max = balances.Max().Round().Run(connection);
                var avg = balances.Avg().Round().Run(connection);

                return "[" + min + "," + max + "," + avg + "]";
            });
        }

        [TestMethod]
        public void DoAggregation()
        {
            RethinkQuery.RunQuery((R, connection) =>
            {
                String id = RethinkQuery.GetIds(R, connection)[0];
                var result = R.Db(DatabaseName).Table(TableName).Get(id)["_reports"]["FBA Base"]
                              .Group("Period").Avg("OutstandingBal").Round().Run(connection);
                return result;
            });
        }

        [TestMethod]
        public void DropById()
        {
            RethinkQuery.RunQuery((R, connection) =>
            {
                String id = RethinkQuery.GetIds(R, connection)[0];
                dynamic result = R.Db(DatabaseName).Table(TableName).Get(id).Delete().Run(connection);
                return result;
            });
        }

        [TestMethod]
        public void SetPropVal()
        {
            RethinkQuery.RunQuery((R, connection) =>
            {
                String id = RethinkQuery.GetIds(R, connection)[0];
                JArray reports = R.Db(DatabaseName).Table(TableName).Get(id)["_reports"]["FBA Base"].Run<JObject>(connection);
                List<JObject> _reports = new List<JObject>(reports.Values<JObject>());
                _reports.ForEach(report => report["PropertyValue"] = 200000);
                dynamic result = R.Db(DatabaseName).Table(TableName).Get(id)
                                  .Update(R.HashMap("_reports", 
                                          R.HashMap("FBA Base", _reports))).Run(connection);
                return result;
            });
        }

        [TestMethod]
        public void FilterOnIndex()
        {
            RethinkQuery.RunQuery((R, connection) =>
            {
                Cursor<dynamic> results = R.Db(DatabaseName).Table(TableName).Between(0, 50).OptArg("index", "PortfolioId").Run(connection);
                string _results = string.Empty;

                using (StreamWriter writer = new StreamWriter(@"output.log"))
                {
                    foreach (dynamic result in results)
                    {
                        writer.Write(JsonConvert.SerializeObject(result));
                    }
                }
                return _results;
            });
        }
    }
}
