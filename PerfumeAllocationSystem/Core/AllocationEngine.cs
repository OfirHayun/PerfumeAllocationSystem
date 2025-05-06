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

        // Reduced minimum satisfaction required to find good matches more easily
        private const double MinimumSatisfactionRequired = 60.0; // Was 70.0

        // Still aim for 70% as target but allow lower to start with
        private const double TargetSatisfactionGoal = 70.0;

        // Store our allocation results
        private List<StoreRequirement> _allocationResults = new List<StoreRequirement>();

        // Track total factory profit
        private decimal _totalProfit = 0;

        public AllocationEngine(List<Perfume> perfumes)
        {
            InitializeInventory(perfumes);
        }

        private void InitializeInventory(List<Perfume> perfumes)
        {
            foreach (var perfume in perfumes)
            {
                string key = $"{perfume.Brand}_{perfume.Name}";
                _perfumeInventory[key] = perfume;
            }
        }

        // Main allocation method
        public List<StoreRequirement> AllocatePerfumes(List<StoreRequirement> storeRequirements)
        {
            ResetAllocationData();

            // Create deep copy of inventory to preserve original
            Dictionary<string, Perfume> workingInventory = CreateWorkingInventoryCopy();

            // Sort stores by budget to prioritize larger stores (giving them first pick)
            List<StoreRequirement> sortedStores = storeRequirements.OrderByDescending(s => s.Budget).ToList();

            ProcessStoreAllocations(sortedStores, workingInventory);

            // Rebalance allocations to improve satisfaction
            RebalanceAllocations(sortedStores, workingInventory);

            return _allocationResults;
        }

        private void ResetAllocationData()
        {
            _allocationResults.Clear();
            _totalProfit = 0;
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

        private void ProcessStoreAllocations(List<StoreRequirement> sortedStores, Dictionary<string, Perfume> workingInventory)
        {
            foreach (var store in sortedStores)
            {
                var workingStore = store.Clone();
                AllocateToStore(workingStore, workingInventory);
                CalculateSatisfaction(workingStore);
                _allocationResults.Add(workingStore);
            }
        }

        private void AllocateToStore(StoreRequirement store, Dictionary<string, Perfume> inventory)
        {
            InitializeStoreAllocation(store);
            List<Tuple<double, Perfume>> perfumeHeap = BuildPerfumeHeap(store, inventory);

            AllocateBestMatches(store, inventory, perfumeHeap);

            if (store.RemainingQuantity > 0)
            {
                BacktrackAllocate(store, inventory);
            }
        }

        private void InitializeStoreAllocation(StoreRequirement store)
        {
            store.AllocatedPerfumes.Clear();
            store.TotalSpent = 0;
        }

        private List<Tuple<double, Perfume>> BuildPerfumeHeap(StoreRequirement store, Dictionary<string, Perfume> inventory)
        {
            List<Tuple<double, Perfume>> perfumeHeap = new List<Tuple<double, Perfume>>();

            foreach (var kvp in inventory)
            {
                Perfume perfume = kvp.Value;
                if (IsPerfumeEligible(perfume, store))
                {
                    double matchPercentage = perfume.CalculateMatchPercentage(store);
                    if (matchPercentage >= MinimumSatisfactionRequired)
                    {
                        perfumeHeap.Add(new Tuple<double, Perfume>(matchPercentage, perfume));
                    }
                }
            }

            perfumeHeap.Sort((a, b) => b.Item1.CompareTo(a.Item1));
            return perfumeHeap;
        }

        private bool IsPerfumeEligible(Perfume perfume, StoreRequirement store)
        {
            return perfume.Stock > 0 && perfume.AveragePrice <= store.MaxPrice;
        }

        private void AllocateBestMatches(StoreRequirement store, Dictionary<string, Perfume> inventory, List<Tuple<double, Perfume>> perfumeHeap)
        {
            while (store.RemainingQuantity > 0 && perfumeHeap.Count > 0)
            {
                var bestMatch = perfumeHeap[0];
                perfumeHeap.RemoveAt(0);

                Perfume perfume = bestMatch.Item2;
                if (perfume.AveragePrice <= store.RemainingBudget)
                {
                    AllocatePerfumeToStore(store, inventory, bestMatch, perfume);
                }
            }
        }

        private void AllocatePerfumeToStore(StoreRequirement store, Dictionary<string, Perfume> inventory, Tuple<double, Perfume> bestMatch, Perfume perfume)
        {
            // Add perfume to store allocation
            store.AllocatedPerfumes.Add(perfume.Clone());
            store.TotalSpent += perfume.AveragePrice;

            // Update inventory
            string key = $"{perfume.Brand}_{perfume.Name}";
            inventory[key].Stock--;

            // Update profit (30% margin on average price)
            _totalProfit += perfume.AveragePrice * 0.3m;
        }

        private void BacktrackAllocate(StoreRequirement store, Dictionary<string, Perfume> inventory)
        {
            List<Tuple<double, Perfume>> alternativePerfumes = FindAlternativePerfumes(store, inventory);
            AllocateAlternatives(store, inventory, alternativePerfumes);
        }

        private List<Tuple<double, Perfume>> FindAlternativePerfumes(StoreRequirement store, Dictionary<string, Perfume> inventory)
        {
            List<Tuple<double, Perfume>> alternativePerfumes = new List<Tuple<double, Perfume>>();

            foreach (var kvp in inventory)
            {
                Perfume perfume = kvp.Value;
                if (IsPerfumeAlternativeEligible(perfume, store))
                {
                    double matchPercentage = perfume.CalculateMatchPercentage(store);
                    // Lower threshold to find more options
                    if (matchPercentage >= 40.0 && matchPercentage < MinimumSatisfactionRequired)
                    {
                        alternativePerfumes.Add(new Tuple<double, Perfume>(matchPercentage, perfume));
                    }
                }
            }

            alternativePerfumes.Sort((a, b) => b.Item1.CompareTo(a.Item1));
            return alternativePerfumes;
        }

        private bool IsPerfumeAlternativeEligible(Perfume perfume, StoreRequirement store)
        {
            return perfume.Stock > 0 && perfume.AveragePrice <= store.RemainingBudget;
        }

        private void AllocateAlternatives(StoreRequirement store, Dictionary<string, Perfume> inventory, List<Tuple<double, Perfume>> alternativePerfumes)
        {
            foreach (var altMatch in alternativePerfumes)
            {
                if (store.RemainingQuantity <= 0)
                    break;

                Perfume perfume = altMatch.Item2;
                if (perfume.AveragePrice <= store.RemainingBudget)
                {
                    AllocateAlternativePerfume(store, inventory, perfume);
                }
            }
        }

        private void AllocateAlternativePerfume(StoreRequirement store, Dictionary<string, Perfume> inventory, Perfume perfume)
        {
            store.AllocatedPerfumes.Add(perfume.Clone());
            store.TotalSpent += perfume.AveragePrice;

            string key = $"{perfume.Brand}_{perfume.Name}";
            inventory[key].Stock--;

            _totalProfit += perfume.AveragePrice * 0.3m;
        }

        // Rebalance allocations to improve stores below target satisfaction
        private void RebalanceAllocations(List<StoreRequirement> storeRequirements, Dictionary<string, Perfume> inventory)
        {
            // Try to improve satisfaction for stores below target
            var storesBelowTarget = _allocationResults.Where(s => s.SatisfactionPercentage < TargetSatisfactionGoal).ToList();

            foreach (var store in storesBelowTarget)
            {
                // Try to replace worst-matching perfumes with better ones
                TryImproveStoreSatisfaction(store, inventory);
            }
        }

        private void TryImproveStoreSatisfaction(StoreRequirement store, Dictionary<string, Perfume> inventory)
        {
            if (store.AllocatedPerfumes.Count == 0) return;

            // Find worst performing perfumes in allocation
            var allocatedByPerformance = store.AllocatedPerfumes
                .Select(p => new Tuple<double, Perfume>(p.CalculateMatchPercentage(store), p))
                .OrderBy(t => t.Item1)
                .ToList();

            // Try to replace bottom 25% of perfumes
            int replacementCount = Math.Max(1, allocatedByPerformance.Count / 4);

            for (int i = 0; i < replacementCount && i < allocatedByPerformance.Count; i++)
            {
                var worstMatch = allocatedByPerformance[i];

                // Find a better replacement
                var betterOption = FindBetterReplacement(store, inventory, worstMatch);

                if (betterOption != null)
                {
                    // Remove worst match
                    string key = $"{worstMatch.Item2.Brand}_{worstMatch.Item2.Name}";
                    if (inventory.ContainsKey(key))
                    {
                        // Return to inventory
                        inventory[key].Stock++;
                        _totalProfit -= worstMatch.Item2.AveragePrice * 0.3m;
                    }

                    // Replace with better option
                    store.AllocatedPerfumes.Remove(worstMatch.Item2);
                    store.TotalSpent -= worstMatch.Item2.AveragePrice;

                    // Add better option
                    store.AllocatedPerfumes.Add(betterOption.Clone());
                    store.TotalSpent += betterOption.AveragePrice;

                    // Update inventory
                    string betterKey = $"{betterOption.Brand}_{betterOption.Name}";
                    inventory[betterKey].Stock--;
                    _totalProfit += betterOption.AveragePrice * 0.3m;
                }
            }

            // Recalculate satisfaction
            CalculateSatisfaction(store);
        }

        private Perfume FindBetterReplacement(StoreRequirement store, Dictionary<string, Perfume> inventory, Tuple<double, Perfume> currentMatch)
        {
            Perfume bestReplacement = null;
            double bestScore = currentMatch.Item1;

            foreach (var kvp in inventory)
            {
                Perfume perfume = kvp.Value;

                if (perfume.Stock > 0 &&
                    perfume.AveragePrice <= store.RemainingBudget + currentMatch.Item2.AveragePrice &&
                    perfume.AveragePrice <= store.MaxPrice)
                {
                    double score = perfume.CalculateMatchPercentage(store);

                    // Require improvement of at least 10%
                    if (score > bestScore + 10.0)
                    {
                        bestScore = score;
                        bestReplacement = perfume;
                    }
                }
            }

            return bestReplacement;
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
            InitializeInventory(perfumes);
        }
    }
}