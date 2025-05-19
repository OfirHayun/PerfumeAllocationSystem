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
        private Random _random = new Random();


        // Initializes the simple allocation engine with perfume inventory
        public SimpleAllocationEngine(List<Perfume> perfumes)
        {
            foreach (var perfume in perfumes)
            {
                string key = $"{perfume.Brand}_{perfume.Name}";
                _perfumeInventory[key] = perfume;
            }
        }

        // Allocates perfumes randomly without optimization strategies
        public List<StoreRequirement> AllocatePerfumes(List<StoreRequirement> storeRequirements)
        {
            _totalProfit = 0;
            Dictionary<string, Perfume> workingInventory = CreateWorkingInventoryCopy();
            List<StoreRequirement> results = storeRequirements.Select(s => s.Clone()).ToList();

            foreach (var store in results)
            {
                store.AllocatedPerfumes.Clear();
                store.TotalSpent = 0;
                List<Perfume> availablePerfumes = GetAvailablePerfumes(store, workingInventory);
                availablePerfumes = availablePerfumes.OrderBy(p => _random.Next()).ToList();
                AllocatePerfumesToStore(store, workingInventory, availablePerfumes);
                CalculateSatisfaction(store);
            }

            return results;
        }

        // Gets all perfumes that match basic price and stock requirements
        private List<Perfume> GetAvailablePerfumes(StoreRequirement store, Dictionary<string, Perfume> inventory)
        {
            return inventory.Values
                .Where(p => p.Stock > 0 && p.AveragePrice <= store.MaxPrice)
                .ToList();
        }

        // Allocates perfumes to a store in random order
        private void AllocatePerfumesToStore(StoreRequirement store, Dictionary<string, Perfume> inventory, List<Perfume> perfumes)
        {
            foreach (var perfume in perfumes)
            {
                if (store.RemainingQuantity <= 0)
                {
                    return;
                }

                if (perfume.AveragePrice <= store.RemainingBudget)
                {
                    store.AllocatedPerfumes.Add(perfume.Clone());
                    store.TotalSpent += perfume.AveragePrice;
                    string key = $"{perfume.Brand}_{perfume.Name}";
                    inventory[key].Stock--;
                    _totalProfit += perfume.AveragePrice * 0.28m;
                }
            }
        }

        // Creates a deep copy of the inventory
        private Dictionary<string, Perfume> CreateWorkingInventoryCopy()
        {
            Dictionary<string, Perfume> workingInventory = new Dictionary<string, Perfume>();
            foreach (var kvp in _perfumeInventory)
            {
                workingInventory[kvp.Key] = kvp.Value.Clone();
            }
            return workingInventory;
        }

        // Calculates satisfaction for a store based on allocated perfumes
        private void CalculateSatisfaction(StoreRequirement store)
        {
            if (store.AllocatedPerfumes.Count == 0)
            {
                store.SatisfactionPercentage = 0;
                return;
            }

            double totalSatisfaction = 0;
            foreach (var perfume in store.AllocatedPerfumes)
            {
                double perfumeSatisfaction = Math.Min(perfume.CalculateMatchPercentage(store), 100.0);
                totalSatisfaction += perfumeSatisfaction;
            }

            double avgSatisfaction = totalSatisfaction / store.AllocatedPerfumes.Count;
            store.SatisfactionPercentage = Math.Min(avgSatisfaction, 100.0);
        }

        // Returns the total profit from the current allocation
        public decimal GetTotalProfit()
        {
            return _totalProfit;
        }
    }
}