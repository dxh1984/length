using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Length
{
    public static class StandardFactory
    {
        public static Standard GetStandard(IEnumerable<Standard> standardList, string unit)
        {
            if (standardList == null || string.IsNullOrEmpty(unit))
            {
                return null;
            }
            //Check matched unit under single or multiple style
            foreach (Standard standard in standardList)
            {
                if (standard.SourceUnit == unit)
                {
                    return standard;
                }
                if (standard.SourceUnit + "s" == unit || standard.SourceUnit + "es" == unit)
                {
                    return standard;
                }
                if (standard.SpecificComplexUnit == unit)
                {
                    return standard;
                }
            }
            return null;
        }
    }
}
