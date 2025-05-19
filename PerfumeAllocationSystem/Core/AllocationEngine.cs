using System;
using System.Collections.Generic;
using System.Linq;
using PerfumeAllocationSystem.Models;

namespace PerfumeAllocationSystem.Core
{
    public class AllocationEngine
    {
        private Dictionary<string, Perfume> _perfumeInventory = new Dictionary<string, Perfume>();
        private const double MinimumSatisfactionRequired = 60.0;
        private const double TargetSatisfactionGoal = 70.0;
        private List<StoreRequirement> _allocationResults = new List<StoreRequirement>();
        private decimal _totalProfit = 0;

        // Initializes the allocation engine with a list of perfumes
        public AllocationEngine(List<Perfume> perfumes)
        {
            InitializeInventory(perfumes);
        }

        // Adds perfumes to the inventory dictionary
        private void InitializeInventory(List<Perfume> perfumes)
        {
            foreach (var perfume in perfumes)
            {
                string key = $"{perfume.Brand}_{perfume.Name}";
                _perfumeInventory[key] = perfume;
            }
        }

        // Allocates perfumes to stores based on requirements and returns results
        public List<StoreRequirement> AllocatePerfumes(List<StoreRequirement> storeRequirements)
        {
            ResetAllocationData();
            Dictionary<string, Perfume> workingInventory = CreateWorkingInventoryCopy();
            List<StoreRequirement> sortedStores = storeRequirements.OrderByDescending(s => s.Budget).ToList();
            ProcessStoreAllocations(sortedStores, workingInventory);
            RebalanceAllocations(sortedStores, workingInventory);
            return _allocationResults;
        }

        // Resets allocation data for a new allocation run
        private void ResetAllocationData()
        {
            _allocationResults.Clear();
            _totalProfit = 0;
        }

        // Creates a deep copy of the perfume inventory
        private Dictionary<string, Perfume> CreateWorkingInventoryCopy()
        {
            Dictionary<string, Perfume> workingInventory = new Dictionary<string, Perfume>();
            foreach (var kvp in _perfumeInventory)
            {
                workingInventory[kvp.Key] = kvp.Value.Clone();
            }
            return workingInventory;
        }

        // Processes all store allocations in priority order
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

        // Allocates perfumes to a single store based on its requirements
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

        // Initializes store allocation data
        private void InitializeStoreAllocation(StoreRequirement store)
        {
            store.AllocatedPerfumes.Clear();
            store.TotalSpent = 0;
        }

        // Creates a priority list of perfumes matching store requirements
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

        // Checks if a perfume meets basic requirements for a store
        private bool IsPerfumeEligible(Perfume perfume, StoreRequirement store)
        {
            return perfume.Stock > 0 && perfume.AveragePrice <= store.MaxPrice;
        }

        // Allocates best matching perfumes to a store
        private void AllocateBestMatches(StoreRequirement store, Dictionary<string, Perfume> inventory, List<Tuple<double, Perfume>> perfumeHeap)
        {
            var eligibleMatches = perfumeHeap
                .Where(match => store.RemainingQuantity > 0)
                .ToList();

            foreach (var bestMatch in eligibleMatches)
            {
                Perfume perfume = bestMatch.Item2;
                if (store.RemainingQuantity <= 0)
                {
                    return;
                }

                if (perfume.AveragePrice <= store.RemainingBudget)
                {
                    AllocatePerfumeToStore(store, inventory, bestMatch, perfume);
                }
            }
        }

        // Allocates a single perfume to a store and updates inventory
        private void AllocatePerfumeToStore(StoreRequirement store, Dictionary<string, Perfume> inventory, Tuple<double, Perfume> bestMatch, Perfume perfume)
        {
            store.AllocatedPerfumes.Add(perfume.Clone());
            store.TotalSpent += perfume.AveragePrice;
            string key = $"{perfume.Brand}_{perfume.Name}";
            inventory[key].Stock--;
            _totalProfit += perfume.AveragePrice * 0.3m;
        }

        // Uses backtracking to allocate alternative perfumes when primary matches are insufficient
        private void BacktrackAllocate(StoreRequirement store, Dictionary<string, Perfume> inventory)
        {
            List<Tuple<double, Perfume>> alternativePerfumes = FindAlternativePerfumes(store, inventory);
            AllocateAlternatives(store, inventory, alternativePerfumes);
        }

        // Finds alternative perfumes with lower match requirements
        private List<Tuple<double, Perfume>> FindAlternativePerfumes(StoreRequirement store, Dictionary<string, Perfume> inventory)
        {
            List<Tuple<double, Perfume>> alternativePerfumes = new List<Tuple<double, Perfume>>();

            foreach (var kvp in inventory)
            {
                Perfume perfume = kvp.Value;
                if (IsPerfumeAlternativeEligible(perfume, store))
                {
                    double matchPercentage = perfume.CalculateMatchPercentage(store);
                    if (matchPercentage >= 40.0 && matchPercentage < MinimumSatisfactionRequired)
                    {
                        alternativePerfumes.Add(new Tuple<double, Perfume>(matchPercentage, perfume));
                    }
                }
            }

            alternativePerfumes.Sort((a, b) => b.Item1.CompareTo(a.Item1));
            return alternativePerfumes;
        }

        // Checks if a perfume is eligible as an alternative allocation
        private bool IsPerfumeAlternativeEligible(Perfume perfume, StoreRequirement store)
        {
            return perfume.Stock > 0 && perfume.AveragePrice <= store.RemainingBudget;
        }

        // Allocates alternative perfumes to a store
        private void AllocateAlternatives(StoreRequirement store, Dictionary<string, Perfume> inventory, List<Tuple<double, Perfume>> alternativePerfumes)
        {
            var eligibleAlternatives = alternativePerfumes
                .TakeWhile(_ => store.RemainingQuantity > 0)
                .Where(altMatch => altMatch.Item2.AveragePrice <= store.RemainingBudget);

            foreach (var altMatch in eligibleAlternatives)
            {
                AllocateAlternativePerfume(store, inventory, altMatch.Item2);
            }
        }

        // Allocates a single alternative perfume to a store
        private void AllocateAlternativePerfume(StoreRequirement store, Dictionary<string, Perfume> inventory, Perfume perfume)
        {
            store.AllocatedPerfumes.Add(perfume.Clone());
            store.TotalSpent += perfume.AveragePrice;
            string key = $"{perfume.Brand}_{perfume.Name}";
            inventory[key].Stock--;
            _totalProfit += perfume.AveragePrice * 0.3m;
        }

        // Improves store allocations by replacing poor matches with better ones
        private void RebalanceAllocations(List<StoreRequirement> storeRequirements, Dictionary<string, Perfume> inventory)
        {
            var storesBelowTarget = _allocationResults.Where(s => s.SatisfactionPercentage < TargetSatisfactionGoal).ToList();
            foreach (var store in storesBelowTarget)
            {
                TryImproveStoreSatisfaction(store, inventory);
            }
        }

        // Attempts to improve a store's satisfaction by replacing worst-matching perfumes
        private void TryImproveStoreSatisfaction(StoreRequirement store, Dictionary<string, Perfume> inventory)
        {
            if (store.AllocatedPerfumes.Count == 0) return;

            var allocatedByPerformance = store.AllocatedPerfumes
                .Select(p => new Tuple<double, Perfume>(p.CalculateMatchPercentage(store), p))
                .OrderBy(t => t.Item1)
                .ToList();

            int replacementCount = Math.Max(1, allocatedByPerformance.Count / 4);

            for (int i = 0; i < replacementCount && i < allocatedByPerformance.Count; i++)
            {
                var worstMatch = allocatedByPerformance[i];
                var betterOption = FindBetterReplacement(store, inventory, worstMatch);

                if (betterOption != null)
                {
                    string key = $"{worstMatch.Item2.Brand}_{worstMatch.Item2.Name}";
                    if (inventory.ContainsKey(key))
                    {
                        inventory[key].Stock++;
                        _totalProfit -= worstMatch.Item2.AveragePrice * 0.3m;
                    }

                    store.AllocatedPerfumes.Remove(worstMatch.Item2);
                    store.TotalSpent -= worstMatch.Item2.AveragePrice;

                    store.AllocatedPerfumes.Add(betterOption.Clone());
                    store.TotalSpent += betterOption.AveragePrice;

                    string betterKey = $"{betterOption.Brand}_{betterOption.Name}";
                    inventory[betterKey].Stock--;
                    _totalProfit += betterOption.AveragePrice * 0.3m;
                }
            }

            CalculateSatisfaction(store);
        }

        // Finds a better replacement perfume with at least 10% higher match score
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

                    if (score > bestScore + 10.0)
                    {
                        bestScore = score;
                        bestReplacement = perfume;
                    }
                }
            }

            return bestReplacement;
        }

        // Calculates the satisfaction percentage for a store based on allocated perfumes
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

        // Resets the inventory to its original state
        public void ResetInventory(List<Perfume> perfumes)
        {
            _perfumeInventory.Clear();
            InitializeInventory(perfumes);
        }
    }
}