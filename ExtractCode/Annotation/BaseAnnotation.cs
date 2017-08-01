using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExtractCode.Annotation
{
    /// <summary>
    /// 注释处理
    /// </summary>
    public class BaseAnnotation
    {
        public StringBuilder Dislodge(FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("文件不存在[{0}]", fileInfo.FullName));
                Console.ForegroundColor = ConsoleColor.White;
                return null;
            }

            string result = File.ReadAllText(fileInfo.FullName, Encoding.UTF8);
            return this.Dislodge(result);
        }

        /// <summary>
        /// 对以下三种情况做了处理
        /// 1. 删除行，如：//{这里是注释的}
        /// 2. 删除行内注释，如： {代码}//{行内注释}
        /// 3. 删除多行，如： /* {这里是多行注释} */
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public virtual StringBuilder Dislodge(String content)
        {
            StringBuilder result = new StringBuilder();

            String[] lines = this.getLines(content);
            foreach (string line in lines)
            {
                if (null == this.removeBlankLine(line))
                {
                    continue;
                }

                result.AppendLine(line);
            }

            lines = this.getLines(result.ToString());

            return this.removeMutliNoteLine(lines);
        }

        protected String[] getLines(string content)
        {
            return content.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        protected String removeBlankLine(String line)
        {
            if (Regex.IsMatch(line, PATTER_SINGLE))
            {
                return null;
            }

            return line;
        }


        private const String PATTER_MUTLI_START = @"^\s*/\*";
        private const String PATTER_MUTLI_END = @"\*/\s*$";
        private const String PATTER_SINGLE = @"^\s*//";
        private const String PATTER_SINGLE_END = "//[^\"]*$";

        /// <summary>
        /// 删除多行注释，如 /* ........  */
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        protected StringBuilder removeMutliNoteLine(String[] lines)
        {
            StringBuilder result = new StringBuilder();
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
                    if (Regex.IsMatch(line, PATTER_MUTLI_END))
                    {
                        isMutliAnnotation = false;
                    }
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
