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

        public virtual StringBuilder Dislodge(String content)
        {
            StringBuilder result = new StringBuilder();

            String[] lines = this.getLines(content);
            foreach (string line in lines)
            {
                if (Regex.Replace(line, "\\s*", string.Empty).Length == 0)
                {
                    continue;
                }

                result.AppendLine(line);
            }

            return result;
        }

        protected String[] getLines(string content)
        {
            return content.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
