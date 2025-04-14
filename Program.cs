using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Task3
{
    internal class Program
    {
        public const string Pattern1 = @"^(?<dd>\d{2})\.(?<mm>\d{2})\.(?<yyyy>\d{2,4})\s+" +
                                       @"(?<time>\d{2}:\d{2}:\d{2}\.\d{1,4})\s+" +
                                       @"(?<logLevel>INFO|WARN|ERROR|DEBUG)\s*" +
                                       @"(?<message>.*)$";

        public const string Pattern2 = @"^(?<yyyy>\d{2,4})-(?<mm>\d{2})-(?<dd>\d{2})\s+" +
                                       @"(?<time>\d{2}:\d{2}:\d{2}\.\d{1,4})\s*[\|]?\s*" +
                                       @"(?<logLevel>INFO|WARN|ERROR|DEBUG)\s*[\|]?\s*" +
                                       @"\d+\s*[\|]?\s*" +
                                       @"(?<errorMethod>(.*?)(?=\|))\s*[\|]?\s*" +
                                       @"(?<message>.*)$";

        public static string SetupLine(string input)
        {
            while (input.IndexOf("INFORMATION") != -1)
                input = input.Replace("INFORMATION", "INFO");
            while (input.IndexOf("WARNING") != -1)
                input = input.Replace("WARNING", "WARN");
            return input;
        }

        public static void Main()
        {
            StreamReader inputFile = new StreamReader("input.txt");
            StreamWriter outputFile = new StreamWriter("output.txt");
            StreamWriter problemFile = new StreamWriter("problems.txt");

            Regex rx1 = new Regex(Pattern1);
            Regex rx2 = new Regex(Pattern2);

            while (!inputFile.EndOfStream)
            {
                string input = SetupLine(inputFile.ReadLine());
                Match match1 = rx1.Match(input);
                if (match1.Success)
                {
                    string date = $"{match1.Groups["yyyy"].Value}-{match1.Groups["mm"].Value}-{match1.Groups["dd"].Value}";
                    string time = match1.Groups["time"].Value;
                    string logLevel = match1.Groups["logLevel"].Value;
                    string message = match1.Groups["message"].Value;
                    outputFile.WriteLine($"{date}\t{time,-13}\t{logLevel}\tDEFAULT\t{message}");
                }
                else
                {
                    Match match2 = rx2.Match(input);
                    if (match2.Success)
                    {
                        string date = $"{match2.Groups["yyyy"].Value}-{match2.Groups["mm"].Value}-{match2.Groups["dd"].Value}";
                        string time = match2.Groups["time"].Value;
                        string logLevel = match2.Groups["logLevel"].Value;
                        string errorMethod = match2.Groups["errorMethod"].Value;
                        string message = match2.Groups["message"].Value;
                        outputFile.WriteLine($"{date}\t{time,-13}\t{logLevel}\t{errorMethod}\t{message}");
                    }
                    else
                    {
                        problemFile.WriteLine(input);
                    }
                }
            }

            inputFile.Close();
            outputFile.Close();
            problemFile.Close();
        }
    }
}
