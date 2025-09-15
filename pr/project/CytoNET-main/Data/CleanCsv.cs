using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

public class CsvCleaner
{
    private readonly string _seedDataDirectory;

    public CsvCleaner(string seedDataDirectory)
    {
        _seedDataDirectory = seedDataDirectory;
    }

    public void CleanCsvFiles()
    {
        string[] fileNames =
        {
            "TableA.csv",
            "TableB.csv",
            "TableC.csv",
            "TableD.csv",
            "TableE.csv",
        };

        foreach (var fileName in fileNames)
        {
            string filePath = Path.Combine(_seedDataDirectory, fileName);

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                continue;
            }

            CleanCsvFile(filePath);
        }
    }

    private void CleanCsvFile(string filePath)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            TrimOptions = TrimOptions.Trim,
            IgnoreBlankLines = true,
            Delimiter = ",",
            Quote = '"',
            Mode = CsvMode.RFC4180,
        };

        var cleanedLines = new List<string>();
        var lineCount = 0;
        var emptyRowCount = 0;

        using (var reader = new StreamReader(filePath))
        {
            string? headerLine = reader.ReadLine(); // Preserve header
            if (headerLine == null)
            {
                Console.WriteLine($"Empty file skipped: {filePath}");
                return;
            }

            cleanedLines.Add(headerLine); // Add header to cleaned file

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                lineCount++;

                if (string.IsNullOrWhiteSpace(line))
                {
                    emptyRowCount++;
                    continue; // Skip empty lines
                }

                var cleanedFields = line.Split(',').Select(field => field.Trim()).ToArray();

                // Skip rows where all fields are empty
                if (cleanedFields.All(string.IsNullOrWhiteSpace))
                {
                    emptyRowCount++;
                    continue;
                }

                cleanedLines.Add(string.Join(",", cleanedFields));
            }
        }

        File.WriteAllLines(filePath, cleanedLines);

        Console.WriteLine(
            $"Cleaned file: {filePath} | Total rows: {lineCount + 1} | Empty rows removed: {emptyRowCount}"
        );
    }
}
