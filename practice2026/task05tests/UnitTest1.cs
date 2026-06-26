using System;
using System.Collections.Generic;
using Xunit;

public class TestClass
{
    public int PublicField;
    private string _privateField;
    public int Property { get; set; }

    public void Method() { }
    public string ComplexMethod(int id, string name) => null;
}

[Serializable]
public class AttributedClass { }

public class ClassAnalyzerTests
{
    [Fact]
    public void GetPublicMethods_ReturnsCorrectMethods()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var methods = analyzer.GetPublicMethods();

        Assert.Contains("Method", methods);
        Assert.Contains("ComplexMethod", methods);
    }

    [Fact]
    public void GetAllFields_IncludesPrivateFields()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var fields = analyzer.GetAllFields();

        Assert.Contains("PublicField", fields);
        Assert.Contains("_privateField", fields);
    }

    [Fact]
    public void GetProperties_ReturnsCorrectProperties()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var properties = analyzer.GetProperties();

        Assert.Contains("Property", properties);
    }

    [Fact]
    public void HasAttribute_ReturnsTrue_WhenAttributeExists()
    {
        var analyzer = new ClassAnalyzer(typeof(AttributedClass));
        var hasAttr = analyzer.HasAttribute<SerializableAttribute>();

        Assert.True(hasAttr);
    }

    [Fact]
    public void HasAttribute_ReturnsFalse_WhenAttributeDoesNotExist()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var hasAttr = analyzer.HasAttribute<SerializableAttribute>();

        Assert.False(hasAttr);
    }

    [Fact]
    public void GetMethodParams_ReturnsCorrectParameters()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var parameters = analyzer.GetMethodParams("ComplexMethod");

        var paramList = new List<string>(parameters);

        Assert.Equal(2, paramList.Count);
        Assert.Contains("Int32 id", paramList);
        Assert.Contains("String name", paramList);
    }
}