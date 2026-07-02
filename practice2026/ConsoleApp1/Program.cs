using System;
using System.Reflection;
using System.IO;

namespace LibraryMetadataAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Ошибка: Укажите путь к файлу библиотеки (.dll) в аргументах командной строки.");
                return;
            }

            string path = args[0];

            if (!File.Exists(path))
            {
                Console.WriteLine($"Ошибка: Файл не найден по пути: {path}");
                return;
            }

            try
            {
                Assembly assembly = Assembly.LoadFrom(path);
                Console.WriteLine($"Анализ сборки: {assembly.FullName}\n");

                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsClass)
                    {
                        Console.WriteLine($"Класс: {type.Name}");

                        Console.WriteLine("  [Атрибуты/Поля]:");
                        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                        {
                            Console.WriteLine($"    - {field.FieldType.Name} {field.Name}");
                        }

                        Console.WriteLine("  [Конструкторы]:");
                        foreach (var ctor in type.GetConstructors())
                        {
                            PrintParameters("    - Конструктор", ctor.GetParameters());
                        }

                        Console.WriteLine("  [Методы]:");
                        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
                        {
                            PrintParameters($"    - {method.ReturnType.Name} {method.Name}", method.GetParameters());
                        }
                        Console.WriteLine(new string('-', 30));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при анализе сборки: {ex.Message}");
            }
        }

        static void PrintParameters(string title, ParameterInfo[] parameters)
        {
            string paramList = string.Join(", ", Array.ConvertAll(parameters, p => $"{p.ParameterType.Name} {p.Name}"));
            Console.WriteLine($"{title}({paramList})");
        }
    }
}
