using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Length
{
    class Program
    {
        /// <summary>
        /// Regular expression to get unit conversion standard
        /// </summary>
        static Regex _StandardRegex = new Regex("1\\s+(?<sourceUnit>[^\\s]+)\\s+=\\s+(?<factor>[^\\s]+)\\s+m", RegexOptions.Singleline);

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                ShowHelp();
                Console.ReadLine();
                return;
            }
            //Load file
            string filePath = args[0];
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Input file is not existing, please specify and try again");
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
                return;
            }
            //Process file
            try
            {
                ProcessFile(filePath);
                Console.WriteLine("File process success. Please refer output.txt for result");
                Console.WriteLine("Press enter to exit");
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Process file failed. Please ensure file format. Message: {0}\r\n. Stack: {1}", ex.Message, ex.StackTrace));
                Console.WriteLine("Press enter to exit");
            }
            Console.ReadLine();
        }

        /// <summary>
        /// Process all file and output
        /// </summary>
        /// <param name="filePath"></param>
        private static void ProcessFile(string filePath)
        {
            //Load standard
            StreamReader reader = new StreamReader(filePath);
            List<Standard> standardList = LoadStandardList(reader);
            //Load calibration request, get result and output to file: $currentDirectory\output.txt
            ProcessAndOutput(standardList, reader);
            reader.Close();
            reader.Dispose();
        }

        /// <summary>
        /// Load conversion standard
        /// </summary>
        /// <param name="filePath">Input file</param>
        /// <returns></returns>
        private static List<Standard> LoadStandardList(StreamReader reader)
        {
            List<Standard> result = new List<Standard>();
            string line = reader.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                Standard standard = GetOneStandard(line.Trim());
                if (standard != null)
                {
                    result.Add(standard);
                }
                line = reader.ReadLine();
            }
            return result;
        }

        /// <summary>
        /// Get calculation request and output result
        /// </summary>
        /// <param name="standardList"></param>
        /// <param name="reader"></param>
        private static void ProcessAndOutput(List<Standard> standardList, StreamReader reader)
        {
            List<string> result = new List<string>();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine().Trim();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                string lineResult = CalcLineResult(line, standardList);
                if (!string.IsNullOrEmpty(lineResult))
                {
                    result.Add(lineResult);
                }
            }
            //Out put to file
            string outputFilePath = Path.Combine(Environment.CurrentDirectory, "output.txt");
            if (File.Exists(outputFilePath))
            {
                File.Delete(outputFilePath);
            }
            AddResultToOutPut(result, outputFilePath);
        }

        /// <summary>
        /// Output result to file
        /// </summary>
        /// <param name="result"></param>
        /// <param name="outputFilePath"></param>
        private static void AddResultToOutPut(List<string> list, string outputFilePath)
        {
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                writer.WriteLine("yiwangqishi@sina.com");
                writer.WriteLine();
                foreach (string calcResult in list)
                {
                    writer.WriteLine(calcResult);
                }
            }
        }

        /// <summary>
        /// Get a calculation request from one line and output result
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static string CalcLineResult(string line, List<Standard> standardList)
        {
            string restLine = line;
            string itemString = string.Empty;
            double currentResult = 0;
            bool isAdding = true;
            while (restLine.Length > 0)
            {
                char c = restLine[0];
                if (c == '+' || c == '-')
                {
                    //Add previous item to result
                    itemString = itemString.Trim();
                    if (!string.IsNullOrEmpty(itemString))
                    {
                        CalculationItem calcItem = CalculationItem.LoadFromString(itemString);
                        currentResult += GetNewCalResult(calcItem, isAdding, standardList);
                    }
                    //change calculation status
                    isAdding = c == '+' ? true : false;
                    itemString = string.Empty;
                }
                else
                {
                    itemString += c;
                }
                restLine = restLine.Substring(1);
            }
            itemString = itemString.Trim();
            if (!string.IsNullOrEmpty(itemString))
            {
                CalculationItem calcItem = CalculationItem.LoadFromString(itemString);
                currentResult += GetNewCalResult(calcItem, isAdding, standardList);
            }
            return Math.Round(currentResult, 2, MidpointRounding.AwayFromZero).ToString() + " m";
        }

        /// <summary>
        /// Get calculation result
        /// </summary>
        /// <param name="calcItem"></param>
        /// <param name="isAdding"></param>
        /// <returns></returns>
        private static double GetNewCalResult(CalculationItem calcItem, bool isAdding, List<Standard> standardList)
        {
            double value = calcItem.ToMetricM(standardList);
            if (isAdding)
            {
                return value;
            }
            else
            {
                return 0 - value;
            }
        }

        /// <summary>
        /// Get one standard from a line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static Standard GetOneStandard(string line)
        {
            //Use regex to match and get information
            Match match = _StandardRegex.Match(line);
            if (!match.Success)
            {
                return null;
            }
            string sourceUnit = match.Groups["sourceUnit"].Value;
            string strFactor = match.Groups["factor"].Value;
            double factor = 0;
            bool isDouble = double.TryParse(strFactor, out factor);
            if (!isDouble)
            {
                return null;
            }
            string complexUnit = GetSpecificComplexUnit(sourceUnit);
            return new Standard()
            {
                Factor = factor,
                SourceUnit = sourceUnit,
                SpecificComplexUnit = complexUnit
            };
        }

        private static string GetSpecificComplexUnit(string sourceUnit)
        {
            if (sourceUnit.EndsWith("ch"))
            {
                return sourceUnit + "es";
            }
            else if (sourceUnit == "foot")
            {
                return "feet";
            }
            else
            {
                return sourceUnit;
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine(@"Please input file path as only one input parameter. For example: Length.exe c:\Temp\input.txt");
            Console.WriteLine("Press enter to exit");
        }
    }
}
