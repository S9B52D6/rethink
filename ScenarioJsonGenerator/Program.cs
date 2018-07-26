using System;
using RethinkDBTest;
using Newtonsoft.Json;
using System.IO;

namespace ScenarioJsonGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            // WriteScenarioFileBatch(1000, @"C:\Users\maneiro\Scenarios");
            WriteReportFile(@".\reports.json", 30000);
        }

        static void WriteScenarioFile(string filepath)
        {
            Scenario scenario = new Scenario();
            String json = JsonConvert.SerializeObject(scenario);
            using (StreamWriter writer = new StreamWriter(filepath))
            {
                writer.WriteLine(json);
            }
        }

        static void WriteReportFile(string filepath, int amount)
        {
            using (StreamWriter writer = new StreamWriter(filepath))
            {
                writer.Write('[');
                for (int i = 0; i < amount; i++)
                {
                    Report report = new Report();
                    String json = JsonConvert.SerializeObject(report) + ',';
                    writer.Write(json);
                }
                writer.Write(']');
            }
        }

        static void WriteScenarioFileBatch(int amount, string path)
        {
            const string BASE_FILE_NAME = "Scenario";
            const string FILE_EXTENSION = ".json";
            for(int i = 0; i < amount; i++)
            {
                string filepath = path + '\\' + BASE_FILE_NAME + '_' + i + FILE_EXTENSION;
                Console.WriteLine("Writing to " + filepath);
                WriteScenarioFile(filepath);
            }
        }
    }
}
