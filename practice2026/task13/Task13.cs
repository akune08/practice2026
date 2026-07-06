using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Task13
{
    public class Subject
    {
        public string Name { get; set; }
        public int Grade { get; set; }
    }

    public class Student
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Subject> Grades { get; set; }
    }

    public class StudentJsonService
    {
        private readonly JsonSerializerOptions _options;

        public StudentJsonService()
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = true, 
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = true 
            };
        }

        public void SaveToFile(Student student, string filePath)
        {
            string json = JsonSerializer.Serialize(student, _options);
            File.WriteAllText(filePath, json);
        }

        public Student LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден.");

            string json = File.ReadAllText(filePath);
            var student = JsonSerializer.Deserialize<Student>(json, _options);

            if (student == null || string.IsNullOrWhiteSpace(student.FirstName))
            {
                throw new JsonException("Десериализованный объект пуст или не содержит обязательных данных.");
            }

            return student;
        }
    }
}