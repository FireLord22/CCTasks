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

        static RealtimeTask SearchCriticalTask(List<RealtimeTask> tasksList)
        {
            RealtimeTask cTask = null;

            for (int i = 0; i < tasksList.Count; i++)
            {
                if (tasksList[i].Hardness != "жесткое")
                {
                    continue;
                }

                if (cTask == null || tasksList[i].L < cTask.L)
                {
                    cTask = tasksList[i];
                }
            }

            return cTask;
        }

        static double RequiredReactionTime(List<RealtimeTask> tasksList)
        {
            if (tasksList.Count == 0)
            {
                throw new ArgumentException("Список задач пуст.");
            }

            double minDeadline = double.PositiveInfinity;
            bool hasHardTasks = false;

            for (int i = 0; i < tasksList.Count; i++)
            {
                if (tasksList[i].Hardness == "жесткое")
                {
                    hasHardTasks = true;

                    if (tasksList[i].D < minDeadline)
                    {
                        minDeadline = tasksList[i].D;
                    }
                }
            }

            if (!hasHardTasks)
            {
                for (int i = 0; i < tasksList.Count; i++)
                {
                    if (tasksList[i].D < minDeadline)
                    {
                        minDeadline = tasksList[i].D;
                    }
                }
            }

            return minDeadline;
        }


        static double Utilization(List<RealtimeTask> tasksList)
        {
            double utilization = 0;

            for (int i = 0; i < tasksList.Count; i++)
            {
                utilization += tasksList[i].C / tasksList[i].T;
            }

            return utilization;
        }

        static string ClassifyArchitecture(int nCpu, bool hasNetwork)
        {
            if (nCpu < 1)
            {
                throw new ArgumentException(
                    "Количество процессоров должно быть не меньше 1.");
            }

            if (hasNetwork)
            {
                return "распределенная";
            }

            if (nCpu == 1)
            {
                return "однопроцессорная";
            }

            return "многопроцессорная";
        }

    }
}
