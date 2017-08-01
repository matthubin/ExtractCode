using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtractCode.Annotation;

namespace ExtractCode
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine(string.Format("对目录 {0} 进行提炼代码：", args[0]));

            DirectoryInfo directoryInfo = new DirectoryInfo(args[0]);
            if (!directoryInfo.Exists)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("目录 {0} 不存在！", args[0]));
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            StringBuilder builder = new StringBuilder();
            Queue<DirectoryInfo> queue = new Queue<DirectoryInfo>();
            queue.Enqueue(directoryInfo);
            while (queue.Count > 0)
            {
                directoryInfo = queue.Dequeue();

                foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
                {
                    if (directory.FullName.ToLower().Contains("Packages")
                        || directory.FullName.ToLower().Contains(".git")
                        || directory.FullName.ToLower().Contains(".idea")
                        || directory.FullName.ToLower().Contains("node_modules")
                        || directory.FullName.ToLower().Contains("bower_components")
                        || directory.FullName.ToLower().Contains("build")
                        || directory.FullName.ToLower().Contains("dist"))
                    {
                        continue;
                    }
                    queue.Enqueue(directory);
                }

                foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                {
                    BaseAnnotation annotation = null;
                    switch (fileInfo.Extension.ToLower())
                    {
                        case ".java":
                            annotation = new JavaAnnotation();
                            break;
                        case ".js":
                            annotation = new JsAnnotation();
                            break;
                    }

                    if (null == annotation)
                    {
                        Console.WriteLine(string.Format("无提取器：[{0}]", fileInfo.FullName));
                        continue;
                    }

                    StringBuilder fileBuilder = annotation.Dislodge(fileInfo);
                    if (null == fileBuilder)
                    {
                        Console.WriteLine(string.Format("无内容：[{0}]", fileInfo.FullName));
                        continue;
                    }

                    Console.WriteLine(string.Format("提取成功：[{0}]", fileInfo.FullName));
                    String[] lines = fileBuilder.ToString()
                        .Split(new string[] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);
                    if (lines.Length == 0 || lines.Any(x => x.Length > 500))
                    {
                        continue;
                    }
                    builder.AppendLine(fileBuilder.ToString());
                }
            }

            builder = builder.Replace("\r\n\r\n", "\r\n");
            builder = builder.Replace("\n\n", "\n");
            File.WriteAllText("out.txt", builder.ToString(), Encoding.UTF8);
        }
    }
}
