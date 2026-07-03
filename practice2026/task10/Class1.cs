using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PluginSystemApp
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginLoadAttribute : Attribute { }

    public interface IPlugin
    {
        string Name { get; }
        string[] Dependencies { get; }
        void Execute();
    }

    class Program
    {
        static void Main(string[] args)
        {
            string pluginsPath = AppDomain.CurrentDomain.BaseDirectory; 

            Console.WriteLine("Загрузка плагинов...");
            var loader = new PluginLoader();
            loader.LoadAndExecutePlugins(pluginsPath);

            Console.WriteLine("\nРабота завершена. Нажмите любую клавишу.");
            Console.ReadKey();
        }
    }

    public class PluginLoader
    {
        public void LoadAndExecutePlugins(string path)
        {
            var pluginInstances = new List<IPlugin>();

            var dllFiles = Directory.GetFiles(path, "*.dll");

            foreach (var file in dllFiles)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(file);
                    var types = assembly.GetTypes()
                        .Where(t => t.GetCustomAttribute<PluginLoadAttribute>() != null
                                 && typeof(IPlugin).IsAssignableFrom(t));

                    foreach (var type in types)
                    {
                        pluginInstances.Add((IPlugin)Activator.CreateInstance(type));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при загрузке {file}: {ex.Message}");
                }
            }

            var graph = pluginInstances.ToDictionary(p => p.Name, p => p.Dependencies);
            var executionOrder = TopologicalSort(graph);

            foreach (var name in executionOrder)
            {
                var plugin = pluginInstances.First(p => p.Name == name);
                Console.WriteLine($"Запуск плагина: {name}");
                plugin.Execute();
            }
        }

        private List<string> TopologicalSort(Dictionary<string, string[]> graph)
        {
            var visited = new HashSet<string>(); 
            var temp = new HashSet<string>();    
            var result = new List<string>();

            void Visit(string node)
            {
                if (temp.Contains(node)) throw new Exception($"Обнаружена циклическая зависимость: {node}");
                if (!visited.Contains(node))
                {
                    temp.Add(node);
                    if (graph.ContainsKey(node))
                    {
                        foreach (var dep in graph[node]) Visit(dep);
                    }
                    temp.Remove(node);
                    visited.Add(node);
                    result.Add(node);
                }
            }

            foreach (var node in graph.Keys) Visit(node);
            return result;
        }
    }
}