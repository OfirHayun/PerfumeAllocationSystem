using System;
using System.Collections.Generic;
using System.Linq;
using PerfumeAllocationSystem.Models;

namespace PerfumeAllocationSystem.Core
{
    public class AllocationEngine
    {
        // Our inventory of perfumes
        private Dictionary<string, Perfume> _perfumeInventory = new Dictionary<string, Perfume>();

        // Minimum satisfaction percentage required (70% as specified in the project)
        private const double MinimumSatisfactionRequired = 70.0;

        // Store our allocation results
        private List<StoreRequirement> _allocationResults = new List<StoreRequirement>();

        // Track total factory profit
        private decimal _totalProfit = 0;

        public AllocationEngine(List<Perfume> perfumes)
        {
            // Initialize our inventory with the perfumes
            foreach (var perfume in perfumes)
            {
                string key = $"{perfume.Brand}_{perfume.Name}";
                _perfumeInventory[key] = perfume;
            }
        }

        // Main allocation method
        public List<StoreRequirement> AllocatePerfumes(List<StoreRequirement> storeRequirements)
        {
            _allocationResults.Clear();
            _totalProfit = 0;

            // Create a working copy of our inventory to modify during allocation
            Dictionary<string, Perfume> workingInventory = new Dictionary<string, Perfume>();
            foreach (var kvp in _perfumeInventory)
            {
                workingInventory[kvp.Key] = kvp.Value.Clone();
            }

            // Sort stores by budget (priority to stores with highest budget)
            var sortedStores = storeRequirements.OrderByDescending(s => s.Budget).ToList();

            foreach (var store in sortedStores)
            {
                // Create a clone of the store requirements to work with
                var workingStore = store.Clone();

                // Allocate perfumes to this store
                AllocateToStore(workingStore, workingInventory);

                // Calculate satisfaction percentage
                CalculateSatisfaction(workingStore);

                // Add to results
                _allocationResults.Add(workingStore);
            }

            return _allocationResults;
        }

        private void AllocateToStore(StoreRequirement store, Dictionary<string, Perfume> inventory)
        {
            // Reset allocation
            store.AllocatedPerfumes.Clear();
            store.TotalSpent = 0;

            // Create a priority queue (Heap) of perfumes sorted by match percentage
            var perfumeHeap = new List<Tuple<double, Perfume>>();

            foreach (var kvp in inventory)
            {
                Perfume perfume = kvp.Value;

                // Skip if no stock
                if (perfume.Stock <= 0)
                    continue;

                // Skip if price exceeds budget per item
                if (perfume.AveragePrice > store.MaxPrice)
                    continue;

                // Calculate match percentage
                double matchPercentage = perfume.CalculateMatchPercentage(store);

                // Only consider if it meets minimum satisfaction
                if (matchPercentage >= MinimumSatisfactionRequired)
                {
                    perfumeHeap.Add(new Tuple<double, Perfume>(matchPercentage, perfume));
                }
            }

            // Sort by match percentage (descending)
            perfumeHeap.Sort((a, b) => b.Item1.CompareTo(a.Item1));

            // Allocate perfumes
            while (store.RemainingQuantity > 0 && perfumeHeap.Count > 0)
            {
                // Get the best matching perfume
                var bestMatch = perfumeHeap[0];
                perfumeHeap.RemoveAt(0);

                Perfume perfume = bestMatch.Item2;

                // Check if we can afford it
                if (perfume.AveragePrice <= store.RemainingBudget)
                {
                    // Allocate this perfume
                    store.AllocatedPerfumes.Add(perfume.Clone());
                    store.TotalSpent += perfume.AveragePrice;

                    // Reduce stock in inventory
                    string key = $"{perfume.Brand}_{perfume.Name}";
                    inventory[key].Stock--;

                    // Remove from heap if stock depleted
                    if (inventory[key].Stock <= 0)
                    {
                        // Already removed from heap, just update inventory
                    }
                    else
                    {
                        // Re-add to heap with updated stock
                        perfumeHeap.Add(new Tuple<double, Perfume>(bestMatch.Item1, inventory[key]));
                        perfumeHeap.Sort((a, b) => b.Item1.CompareTo(a.Item1));
                    }

                    // Update profit
                    // Assuming 30% profit margin on average price
                    _totalProfit += perfume.AveragePrice * 0.3m;
                }
            }

            // If we couldn't allocate enough perfumes, try backtracking to find alternatives
            if (store.RemainingQuantity > 0)
            {
                BacktrackAllocate(store, inventory);
            }
        }

        private void BacktrackAllocate(StoreRequirement store, Dictionary<string, Perfume> inventory)
        {
            // This is a simplified backtracking approach
            // In a full implementation, you'd explore multiple allocation possibilities

            // Find perfumes that might not meet minimum satisfaction but are close
            var alternativePerfumes = new List<Tuple<double, Perfume>>();

            foreach (var kvp in inventory)
            {
                Perfume perfume = kvp.Value;

                // Skip if no stock
                if (perfume.Stock <= 0)
                    continue;

                // Skip if price exceeds budget per item
                if (perfume.AveragePrice > store.RemainingBudget)
                    continue;

                // Calculate match percentage
                double matchPercentage = perfume.CalculateMatchPercentage(store);

                // Consider alternatives that are at least 50% match
                if (matchPercentage >= 50.0 && matchPercentage < MinimumSatisfactionRequired)
                {
                    alternativePerfumes.Add(new Tuple<double, Perfume>(matchPercentage, perfume));
                }
            }

            // Sort by match percentage (descending)
            alternativePerfumes.Sort((a, b) => b.Item1.CompareTo(a.Item1));

            // Allocate alternative perfumes
            foreach (var altMatch in alternativePerfumes)
            {
                // Stop if we've allocated enough
                if (store.RemainingQuantity <= 0)
                    break;

                Perfume perfume = altMatch.Item2;

                // Check if we can afford it
                if (perfume.AveragePrice <= store.RemainingBudget)
                {
                    // Allocate this perfume
                    store.AllocatedPerfumes.Add(perfume.Clone());
                    store.TotalSpent += perfume.AveragePrice;

                    // Reduce stock in inventory
                    string key = $"{perfume.Brand}_{perfume.Name}";
                    inventory[key].Stock--;

                    // Update profit
                    // Assuming 30% profit margin on average price
                    _totalProfit += perfume.AveragePrice * 0.3m;
                }
            }
        }

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
                totalSatisfaction += perfume.CalculateMatchPercentage(store);
            }

            store.SatisfactionPercentage = totalSatisfaction / store.AllocatedPerfumes.Count;
        }

        // Get our total profit from the allocation
        public decimal GetTotalProfit()
        {
            return _totalProfit;
        }

        // Reset inventory to original state
        public void ResetInventory(List<Perfume> perfumes)
        {
            _perfumeInventory.Clear();
            foreach (var perfume in perfumes)
            {
                string key = $"{perfume.Brand}_{perfume.Name}";
                _perfumeInventory[key] = perfume;
            }
        }
    }
}