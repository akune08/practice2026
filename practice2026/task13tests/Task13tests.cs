using Xunit;

public class StudentTests
{
    [Fact]
    public void SerializeAndDeserialize_ShouldMatchOriginal()
    {
        var service = new JsonService();
        var student = new Student { FirstName = "Ivan", LastName = "Ivanov", BirthDate = new DateTime(2000, 1, 1) };
        string path = "test.json";

        service.SerializeToFile(student, path);
        var result = service.DeserializeFromFile(path);

        Assert.Equal(student.FirstName, result.FirstName);
        Assert.Equal(student.BirthDate, result.BirthDate);

        File.Delete(path); 
    }
}