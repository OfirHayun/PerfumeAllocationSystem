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
        // Loads perfume data from a CSV file
        public List<Perfume> LoadPerfumesFromCsv(string filePath)
        {
            List<Perfume> perfumes = new List<Perfume>();
            if (!File.Exists(filePath))
            {
                ShowFileNotFoundError(filePath);
                return perfumes;
            }

            try
            {
                ProcessCsvFile(filePath, perfumes);
            }
            catch (Exception ex)
            {
                ShowLoadError(ex);
            }

            return perfumes;
        }

        // Shows an error when file is not found
        private void ShowFileNotFoundError(string filePath)
        {
            MessageBox.Show($"File not found: {filePath}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Processes CSV file and creates perfume objects
        private void ProcessCsvFile(string filePath, List<Perfume> perfumes)
        {
            string[] lines = File.ReadAllLines(filePath);

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if (!string.IsNullOrWhiteSpace(line))
                {
                    AddPerfumeFromCsvLine(line, perfumes);
                }
            }
        }

        // Creates a perfume from a CSV line
        private void AddPerfumeFromCsvLine(string line, List<Perfume> perfumes)
        {
            string[] parts = line.Split(';');
            if (parts.Length >= 11)
            {
                Perfume perfume = CreatePerfumeFromParts(parts);
                perfumes.Add(perfume);
            }
        }

        // Creates a perfume object from CSV parts
        private Perfume CreatePerfumeFromParts(string[] parts)
        {
            return new Perfume
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
        }

        // Shows an error when loading fails
        private void ShowLoadError(Exception ex)
        {
            MessageBox.Show($"Error loading perfumes: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Saves allocation results to a CSV file
        public bool SaveAllocationResultsToCsv(List<StoreRequirement> results, string filePath)
        {
            try
            {
                WriteResultsToCsv(results, filePath);
                return true;
            }
            catch (Exception ex)
            {
                ShowSaveError(ex);
                return false;
            }
        }

        // Writes results to a CSV file
        private void WriteResultsToCsv(List<StoreRequirement> results, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                WriteResultsHeader(writer);
                WriteResultsData(writer, results);
            }
        }

        // Writes CSV header
        private void WriteResultsHeader(StreamWriter writer)
        {
            writer.WriteLine("Store,Budget,QuantityNeeded,SatisfactionPercentage," +
                            "TotalSpent,RemainingBudget,AllocatedPerfumes");
        }

        // Writes results data rows
        private void WriteResultsData(StreamWriter writer, List<StoreRequirement> results)
        {
            foreach (var store in results)
            {
                string allocatedPerfumes = FormatAllocatedPerfumes(store);
                writer.WriteLine($"{store.StoreName},{store.Budget},{store.QuantityNeeded}," +
                                $"{store.SatisfactionPercentage:F2}%,{store.TotalSpent}," +
                                $"{store.RemainingBudget},{allocatedPerfumes}");
            }
        }

        // Formats allocated perfumes for CSV
        private string FormatAllocatedPerfumes(StoreRequirement store)
        {
            return string.Join("; ", store.AllocatedPerfumes.Select(p => $"{p.Brand} {p.Name}"));
        }

        // Shows an error when saving fails
        private void ShowSaveError(Exception ex)
        {
            MessageBox.Show($"Error saving results: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}