using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Length
{
    public class CalculationItem
    {
        static Regex _Regex = new Regex(@"(?<number>[^\s]+)\s+(?<unit>[^\s]+)", RegexOptions.Singleline);

        public double Number { get; set; }
        public string Unit { get; set; }

        /// <summary>
        /// Convert to 'm'
        /// </summary>
        /// <param name="standardList"></param>
        /// <returns></returns>
        public double ToMetricM(IEnumerable<Standard> standardList)
        {
            Standard standard = StandardFactory.GetStandard(standardList, this.Unit);
            return standard.Factor * this.Number;
        }

        public static CalculationItem LoadFromString(string expression)
        {
            Match match = _Regex.Match(expression);
            if (!match.Success)
            {
                throw new InvalidOperationException(string.Format("Input string was not recognized as valid express", expression));
            }
            string strNumber = match.Groups["number"].Value;
            double number = 0;
            bool isNumber = double.TryParse(strNumber, out number);
            if (!isNumber)
            {
                throw new InvalidOperationException(string.Format("Input number was not valid", strNumber));
            }
            return new CalculationItem()
            {
                Number = number,
                Unit = match.Groups["unit"].Value
            };
        }
    }
}
