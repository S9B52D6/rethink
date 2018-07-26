using Newtonsoft.Json;
using RethinkDb.Driver.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RethinkDBTest
{
    class RethinkQuery
    {

        public const string Hostname = "localhost";
        public const int Timeout = 60;
        public const string DatabaseName = "fba_test";
        public const string TableName = "profiler10000";

        public static List<string> GetIds(RethinkDb.Driver.RethinkDB R, RethinkDb.Driver.Net.Connection connection)
        {
            Cursor<object> cursor = R.Db(DatabaseName).Table(TableName).Pluck("id").RunCursor<object>(connection);
            String json = "";
            cursor.BufferedItems.ForEach(item => json += item + ",\n");
            json = json.TrimEnd(',', '\n');
            json = "[" + json + "]";

            List<ID> ids = new List<ID>(JsonConvert.DeserializeObject<IEnumerable<ID>>(json));
            List<string> _ids = new List<string>();
            ids.ForEach(id => _ids.Add(id.id));
            return _ids;
        }

        public static void RunQuery(Func<RethinkDb.Driver.RethinkDB, RethinkDb.Driver.Net.Connection, dynamic> query)
        {

            var R = RethinkDb.Driver.RethinkDB.R;
            Connection connection;

            try
            {
                connection = R.Connection().Hostname(Hostname).Timeout(Timeout).Connect();

                try
                {
                    dynamic result = query(R, connection);
                    Console.WriteLine(result);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public class ID
        {
            public string id;

            public ID()
            {
                id = string.Empty;
            }
        }
    }
}
