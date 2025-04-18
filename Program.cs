using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Task3
{
    public interface ILogParsed
    {
       bool TryParse(string line, out LogEntry entry);
    }

    public class LogFormat1 : ILogParsed
    {
        public const string Pattern = @"^(?<dd>\d{2})\.(?<mm>\d{2})\.(?<yyyy>\d{2,4})\s+" +
                                       @"(?<time>\d{2}:\d{2}:\d{2}\.\d{1,4})\s+" +
                                       @"(?<logLevel>INFO|WARN|ERROR|DEBUG)\s*" +
                                       @"(?<message>.*)$";
        public bool TryParse(string line, out LogEntry entry)
        {
            entry = null;
            Regex rx = new Regex(Pattern);
            Match match = rx.Match(line);

            if (!match.Success)
                return false;

            entry = new LogEntry
            {
                Date = $"{match.Groups["yyyy"].Value}-{match.Groups["mm"].Value}-{match.Groups["dd"].Value}",
                Time = match.Groups["time"].Value,
                LogLevel = match.Groups["logLevel"].Value,
                ErrorMethod = "DEFAULT",
                Message = match.Groups["message"].Value
            };
            return true;
        }
    }

    public class LogFormat2 : ILogParsed
    {
        public const string Pattern = @"^(?<yyyy>\d{2,4})-(?<mm>\d{2})-(?<dd>\d{2})\s+" +
                                       @"(?<time>\d{2}:\d{2}:\d{2}\.\d{1,4})\s*[\|]?\s*" +
                                       @"(?<logLevel>INFO|WARN|ERROR|DEBUG)\s*[\|]?\s*" +
                                       @"\d+\s*[\|]?\s*" +
                                       @"(?<errorMethod>(.*?)(?=\|))\s*[\|]?\s*" +
                                       @"(?<message>.*)$";
        public bool TryParse(string line, out LogEntry entry)
        {
            entry = null;
            Regex rx = new Regex(Pattern);
            Match match = rx.Match(line);

            if (!match.Success)
                return false;

            entry = new LogEntry
            {
                Date = $"{match.Groups["yyyy"].Value}-{match.Groups["mm"].Value}-{match.Groups["dd"].Value}",
                Time = match.Groups["time"].Value,
                LogLevel = match.Groups["logLevel"].Value,
                ErrorMethod = match.Groups["errorMethod"].Value,
                Message = match.Groups["message"].Value
            };
            return true;
        }
    }

    public class LogEntry
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string LogLevel { get; set; }
        public string ErrorMethod { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $"{Date}\t{Time}\t{LogLevel}\t{ErrorMethod}\t{Message}";
        }

    }

    public class LogProcessor
    {
        private readonly List<ILogParsed> _formats;
        public LogProcessor(List<ILogParsed> Formats) 
        {
            _formats = Formats;
        }

        public bool TryProcessLine(string input, out string parsed)
        {
            for (int i = 0; i < _formats.Count; ++i)
            {
                if (_formats[i].TryParse(input, out LogEntry entry))
                {
                    parsed = entry.ToString();
                    return true;
                }
            }
            parsed = input;
            return false;
        }
    }

    internal class Program
    {
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

            List<ILogParsed> Formats = new List<ILogParsed>
            {
                new LogFormat1(),
                new LogFormat2()
            };

            LogProcessor processor = new LogProcessor(Formats);

            while (!inputFile.EndOfStream)
            {
                string input = SetupLine(inputFile.ReadLine());

                if (processor.TryProcessLine(input, out string processedLine))
                    outputFile.WriteLine(processedLine);
                else
                    problemFile.WriteLine(processedLine);
            }

            inputFile.Close();
            outputFile.Close();
            problemFile.Close();
        }
    }
}
