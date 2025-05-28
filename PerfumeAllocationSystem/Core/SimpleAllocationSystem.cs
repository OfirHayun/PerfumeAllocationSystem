using System;
using System.Collections.Generic;
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
            List<StoreRequirement> results = CloneStoreRequirements(storeRequirements);

            foreach (var store in results)
            {
                store.AllocatedPerfumes.Clear();
                store.TotalSpent = 0;
                List<Perfume> availablePerfumes = GetAvailablePerfumes(store, workingInventory);
                ShufflePerfumes(availablePerfumes);
                AllocatePerfumesToStore(store, workingInventory, availablePerfumes);
                CalculateSatisfaction(store);
            }

            return results;
        }

        // Clones store requirements list
        private List<StoreRequirement> CloneStoreRequirements(List<StoreRequirement> storeRequirements)
        {
            List<StoreRequirement> results = new List<StoreRequirement>();
            foreach (var store in storeRequirements)
            {
                results.Add(store.Clone());
            }
            return results;
        }

        // Gets all perfumes that match basic price and stock requirements
        private List<Perfume> GetAvailablePerfumes(StoreRequirement store, Dictionary<string, Perfume> inventory)
        {
            List<Perfume> availablePerfumes = new List<Perfume>();
            foreach (var perfume in inventory.Values)
            {
                if (perfume.Stock > 0 && perfume.AveragePrice <= store.MaxPrice)
                {
                    availablePerfumes.Add(perfume);
                }
            }
            return availablePerfumes;
        }

        // Shuffles perfumes randomly 
        private void ShufflePerfumes(List<Perfume> perfumes)
        {
            
            for (int i = perfumes.Count - 1; i > 0; i--)
            {
                int randomIndex = _random.Next(i + 1);

                
                Perfume temp = perfumes[i];
                perfumes[i] = perfumes[randomIndex];
                perfumes[randomIndex] = temp;
            }
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
                    _totalProfit += perfume.AveragePrice * 0.27m;
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