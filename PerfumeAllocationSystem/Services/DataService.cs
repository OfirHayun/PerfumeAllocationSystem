using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PerfumeAllocationSystem.Models;

namespace PerfumeAllocationSystem.Services
{
    public class DataService
    {
        // Load perfumes from CSV file
        public List<Perfume> LoadPerfumesFromCsv(string filePath)
        {
            List<Perfume> perfumes = new List<Perfume>();

            try
            {
                // Check if file exists
                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"File not found: {filePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return perfumes;
                }

                // Read all lines
                string[] lines = File.ReadAllLines(filePath);

                // Skip header
                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i];

                    // Skip empty lines
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    // Split by semicolon
                    string[] parts = line.Split(';');

                    // Ensure we have all required fields
                    if (parts.Length >= 11)
                    {
                        Perfume perfume = new Perfume
                        {
                            Name = parts[0],
                            Brand = parts[1],
                            Gender = parts[2],
                            TopNotes = parts[3],
                            MiddleNotes = parts[4],
                            BaseNotes = parts[5],
                            MainAccord = parts[6],
                            Longevity = int.Parse(parts[7]),
                            Projection = int.Parse(parts[8]),
                            AveragePrice = decimal.Parse(parts[9]),
                            Stock = int.Parse(parts[10])
                        };

                        perfumes.Add(perfume);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading perfumes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return perfumes;
        }

        // Save allocation results to CSV
        public bool SaveAllocationResultsToCsv(List<StoreRequirement> results, string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    // Write header
                    writer.WriteLine("Store,Budget,QuantityNeeded,SatisfactionPercentage,TotalSpent,RemainingBudget,AllocatedPerfumes");

                    // Write data for each store
                    foreach (var store in results)
                    {
                        // Combine allocated perfumes into a single string
                        string allocatedPerfumes = string.Join("; ", store.AllocatedPerfumes.Select(p => $"{p.Brand} {p.Name}"));

                        writer.WriteLine($"{store.StoreName},{store.Budget},{store.QuantityNeeded},{store.SatisfactionPercentage:F2}%,{store.TotalSpent},{store.RemainingBudget},{allocatedPerfumes}");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving results: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}