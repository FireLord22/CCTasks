using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CCTasks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");

            List<RealtimeTask> allTasks = ReadingTask();

            if (allTasks.Count == 0)
            {
                Console.WriteLine("В файле нет задач.");
                Console.ReadKey();
                return;
            }

            ReadSystemParameters(out int nCpu, out bool hasNetwork);

            DeterminingTypeDeadline(allTasks);
            CalculatingTaskStock(allTasks);
        }

        static void ReadSystemParameters(out int nCpu, out bool hasNetwork)
        {
            Console.Write("Количество процессоров: ");

            while (!int.TryParse(Console.ReadLine(), out nCpu) || nCpu < 1)
            {
                Console.Write("Введите целое число больше нуля: ");
            }

            Console.Write("Есть сеть? (да/нет): ");
            string answer = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

            while (answer != "да" && answer != "нет")
            {
                Console.Write("Введите «да» или «нет»: ");
                answer = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
            }

            hasNetwork = answer == "да";
            Console.WriteLine();
        }

        static List<RealtimeTask> ReadingTask()
        {
            List<RealtimeTask> tasks = new List<RealtimeTask>();

            string[] lines = File.ReadAllLines("tasks.csv");

            CultureInfo culture = CultureInfo.GetCultureInfo("ru-RU");

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    continue;
                }

                string[] values = lines[i].Split(';');

                RealtimeTask task = new RealtimeTask
                {
                    Name = values[0].Trim(),
                    Hardness = values[1].Trim(),
                    C = double.Parse(values[2], culture),
                    D = double.Parse(values[3], culture),
                    T = double.Parse(values[4], culture)
                };

                tasks.Add(task);
            }

            return tasks;
        }

        static string TaskClassification(List<RealtimeTask> tasksList)
        {
            for (int i = 0; i < tasksList.Count; i++)
            {
                if (tasksList[i].Hardness == "жесткое")
                {
                    return "жесткого";
                }
            }

            return "мягкого";
        }

        static List<RealtimeTask> DeterminingTypeDeadline(List<RealtimeTask> tasksList)
        {
            for (int i = 0; i < tasksList.Count; i++)
            {
                if (tasksList[i].D == tasksList[i].T)
                {
                    tasksList[i].deadlineType = "неявный";
                    continue;
                }
                if (tasksList[i].D <= tasksList[i].T)
                {
                    tasksList[i].deadlineType = "ограниченный";
                    continue;
                }
                if (tasksList[i].D >= tasksList[i].T)
                {
                    tasksList[i].deadlineType = "произвольный";
                    continue;
                }
            }

            return tasksList;
        }

        static List<RealtimeTask> CalculatingTaskStock(List<RealtimeTask> tasksList)
        {
            for (int i = 0; i < tasksList.Count; i++)
            {
                tasksList[i].L = tasksList[i].D - tasksList[i].C;
            }

            return tasksList;
        }


    }
}
