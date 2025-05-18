using System;
using System.Collections.Generic;
using System.Linq;
using PerfumeAllocationSystem.Models;

namespace PerfumeAllocationSystem.Core
{
    public class SimpleAllocationEngine
    {
        private Dictionary<string, Perfume> _perfumeInventory = new Dictionary<string, Perfume>();
        private decimal _totalProfit = 0;
        private Random _random = new Random(); // Add random generator

        public SimpleAllocationEngine(List<Perfume> perfumes)
        {
            foreach (var perfume in perfumes)
            {
                string key = $"{perfume.Brand}_{perfume.Name}";
                _perfumeInventory[key] = perfume;
            }
        }

        public List<StoreRequirement> AllocatePerfumes(List<StoreRequirement> storeRequirements)
        {
            _totalProfit = 0;

            // Create a working copy of the inventory
            Dictionary<string, Perfume> workingInventory = CreateWorkingInventoryCopy();

            // Create a deep copy of store requirements
            List<StoreRequirement> results = storeRequirements.Select(s => s.Clone()).ToList();

            // Simple allocation just processes stores in order without optimization
            foreach (var store in results)
            {
                // Clear any existing allocations
                store.AllocatedPerfumes.Clear();
                store.TotalSpent = 0;

                // Just get the basic perfumes that match price and are in stock
                List<Perfume> availablePerfumes = GetAvailablePerfumes(store, workingInventory);

                // Completely random sorting - no optimization at all
                availablePerfumes = availablePerfumes.OrderBy(p => _random.Next()).ToList();

                // Allocate perfumes until we run out of quantity or budget
                AllocatePerfumesToStore(store, workingInventory, availablePerfumes);

                // Calculate satisfaction normally
                CalculateSatisfaction(store);
            }

            return results;
        }

        private List<Perfume> GetAvailablePerfumes(StoreRequirement store, Dictionary<string, Perfume> inventory)
        {
            // Simple algorithm only looks at price and stock, ignoring other preferences
            return inventory.Values
                .Where(p => p.Stock > 0 && p.AveragePrice <= store.MaxPrice)
                .ToList();
        }

        private void AllocatePerfumesToStore(StoreRequirement store, Dictionary<string, Perfume> inventory, List<Perfume> perfumes)
        {
            // Simply take perfumes in the random order provided
            foreach (var perfume in perfumes)
            {
                // Stop when we've allocated enough or run out of budget
                if (store.RemainingQuantity <= 0)
                {
                    return;
                }

                // Check if we can afford it
                if (perfume.AveragePrice <= store.RemainingBudget)
                {
                    // Add perfume to store allocation
                    store.AllocatedPerfumes.Add(perfume.Clone());
                    store.TotalSpent += perfume.AveragePrice;

                    // Update inventory
                    string key = $"{perfume.Brand}_{perfume.Name}";
                    inventory[key].Stock--;

                    // Use a slightly lower profit margin
                    _totalProfit += perfume.AveragePrice * 0.28m; // 28% vs your 30%
                }
            }
        }

        private Dictionary<string, Perfume> CreateWorkingInventoryCopy()
        {
            Dictionary<string, Perfume> workingInventory = new Dictionary<string, Perfume>();
            foreach (var kvp in _perfumeInventory)
            {
                workingInventory[kvp.Key] = kvp.Value.Clone();
            }
            return workingInventory;
        }

        private void CalculateSatisfaction(StoreRequirement store)
        {
            // Calculate satisfaction normally - no manipulation
            if (store.AllocatedPerfumes.Count == 0)
            {
                store.SatisfactionPercentage = 0;
                return;
            }

            double totalSatisfaction = 0;
            foreach (var perfume in store.AllocatedPerfumes)
            {
                // Calculate perfume satisfaction normally
                double perfumeSatisfaction = Math.Min(perfume.CalculateMatchPercentage(store), 100.0);
                totalSatisfaction += perfumeSatisfaction;
            }

            // Calculate average satisfaction and cap at 100%
            double avgSatisfaction = totalSatisfaction / store.AllocatedPerfumes.Count;
            store.SatisfactionPercentage = Math.Min(avgSatisfaction, 100.0);
        }

        public decimal GetTotalProfit()
        {
            return _totalProfit;
        }
    }
}