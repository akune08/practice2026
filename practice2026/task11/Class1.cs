using System;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Task11
{
    public interface ICalculator
    {
        int Add(int a, int b);
        int Minus(int a, int b);
        int Mul(int a, int b);
        int Div(int a, int b);
    }

    public static class CalculatorEngine
    {
        public static ICalculator CreateCalculator()
        {
            string classCode = @"
            using Task11;
            public class Calculator : ICalculator
            {
                public int Add(int a, int b) => a + b;
                public int Minus(int a, int b) => a - b;
                public int Mul(int a, int b) => a * b;
                public int Div(int a, int b) => a / b;
            }";

            var tree = SyntaxFactory.ParseSyntaxTree(classCode);

            var references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ICalculator).Assembly.Location)
            };

            var compilation = CSharpCompilation.Create("DynamicCalculatorAssembly")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(references)
                .AddSyntaxTrees(tree);

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (!result.Success)
                {
                    string errors = string.Join("\n", result.Diagnostics);
                    throw new Exception($"Ошибка компиляции:\n{errors}");
                }

                var assembly = Assembly.Load(ms.ToArray());

                var type = assembly.GetType("Calculator");
                return (ICalculator)Activator.CreateInstance(type);
            }
        }
    }
}