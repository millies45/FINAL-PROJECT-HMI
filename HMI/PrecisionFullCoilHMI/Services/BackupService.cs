using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PrecisionFullCoilHMI.Data;
using PrecisionFullCoilHMI.Models;
using PrecisionFullCoilHMI.Services;

public class BackupService : IBackupService
{
    private readonly ApplicationDbContext _context;

    public BackupService(ApplicationDbContext context)
    {
        _context = context;
    }

    public void ExportData(string filePath)
    {
        // Retrieve all data from the database
        var data = new
        {
            Tags = _context.Tags.ToList()
            //Recipes = _context.Recipes.Include(r => r.Jobs).ToList(),
            // Add more entities as needed
        };

        // Serialize the data to JSON
        var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);

        // Write the JSON data to a file
        File.WriteAllText(filePath, jsonData);
    }

    public void RestoreData(string filePath)
    {
        // Read the JSON data from the file
        var jsonData = File.ReadAllText(filePath);

        // Deserialize the JSON data
        var data = JsonConvert.DeserializeObject<BackupData>(jsonData);

        if (data == null) throw new Exception("Failed to deserialize backup data.");

        // Clear existing data
        _context.Tags.RemoveRange(_context.Tags);
        //_context.Recipes.RemoveRange(_context.Recipes);

        // Insert the deserialized data
        _context.Tags.AddRange(data.Tags);
        //_context.Recipes.AddRange(data.Recipes);

        // Save changes to the database
        _context.SaveChanges();
    }

    private class BackupData
    {
        public List<Tag> Tags { get; set; }
        //public List<Recipe> Recipes { get; set; }
    }
}
