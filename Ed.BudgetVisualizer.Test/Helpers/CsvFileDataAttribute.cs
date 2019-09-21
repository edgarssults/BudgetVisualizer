using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit.Sdk;

/// <summary>
/// Attribute for loading data for a file for tests.
/// </summary>
/// <remarks>
/// https://andrewlock.net/creating-a-custom-xunit-theory-test-dataattribute-to-load-data-from-json-files/
/// </remarks>
public sealed class CsvFileDataAttribute : DataAttribute
{
    private readonly string _filePath;

    /// <summary>
    /// Load data from a CSV file as the data source for a theory
    /// </summary>
    /// <param name="filePath">The absolute or relative path to the JSON file to load</param>
    public CsvFileDataAttribute(string filePath)
    {
        _filePath = filePath;
    }

    /// <inheritDoc />
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        if (testMethod == null)
        {
            throw new ArgumentNullException(nameof(testMethod));
        }

        var path = Path.IsPathRooted(_filePath)
            ? _filePath
            : Path.GetRelativePath(Directory.GetCurrentDirectory(), _filePath);

        if (!File.Exists(path))
        {
            throw new ArgumentException($"Could not find file at path: {path}");
        }

        return new List<object[]>
        {
            new object[] { File.Open(_filePath, FileMode.Open) }
        };
    }
}