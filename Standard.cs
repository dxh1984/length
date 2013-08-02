using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Length
{
    public class Standard
    {
        /// <summary>
        /// Unit under single style
        /// </summary>
        public string SourceUnit { get; set; }
        /// <summary>
        /// Specific complex unit under multiple style if it has
        /// </summary>
        public string SpecificComplexUnit { get; set; }
        /// <summary>
        /// factor to convert to 'm'
        /// </summary>
        public double Factor { get; set; }
    }
}
