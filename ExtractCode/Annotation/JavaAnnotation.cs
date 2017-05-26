using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExtractCode.Annotation
{
    public class JavaAnnotation : BaseAnnotation
    {
        private const String PATTER_MUTLI_START = @"^\s*/\*";
        private const String PATTER_MUTLI_END = @"\*/\s*$";
        private const String PATTER_SINGLE = @"^\s*//";
        private const String PATTER_SINGLE_END = "//[^\"]*$";

        public override StringBuilder Dislodge(string content)
        {
            content = base.Dislodge(content).ToString();
            StringBuilder result = new StringBuilder();

            String[] lines = this.getLines(content);
            Boolean isMutliAnnotation = false;//是否是多行注释
            foreach (String line in lines)
            {
                if (isMutliAnnotation)
                {
                    if (Regex.IsMatch(line, PATTER_MUTLI_END))
                    {
                        isMutliAnnotation = false;
                    }
                    continue;
                }

                if (Regex.IsMatch(line, PATTER_MUTLI_START))
                {
                    isMutliAnnotation = true;
                    continue;
                }

                if (Regex.IsMatch(line, PATTER_SINGLE))
                {
                    continue;
                }

                String temp = line;
                temp = Regex.Replace(temp, PATTER_SINGLE_END, String.Empty);

                result.AppendLine(temp);
            }

            return result;
        }
    }
}
