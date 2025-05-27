using System;
using System.Collections.Generic;
using PerfumeAllocationSystem.Models;

namespace PerfumeAllocationSystem.Core
{
    
    public class PerfumeMatch
    {
        public double MatchPercentage { get; set; }
        public Perfume Perfume { get; set; }

        public PerfumeMatch(double matchPercentage, Perfume perfume)
        {
            MatchPercentage = matchPercentage;
            Perfume = perfume;
        }
    }

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
            List<StoreRequirement> sortedStores = SortStoresByBudgetDescending(storeRequirements);
            ProcessStoreAllocations(sortedStores, workingInventory);
            RebalanceAllocations(sortedStores, workingInventory);
            return _allocationResults;
        }

        // Sorts stores by budget in descending order
        private List<StoreRequirement> SortStoresByBudgetDescending(List<StoreRequirement> storeRequirements)
        {
            List<StoreRequirement> sortedStores = new List<StoreRequirement>(storeRequirements);
            sortedStores.Sort(CompareBudgetDescending);
            return sortedStores;
        }

        // Comparer for sorting stores by budget (descending)
        private int CompareBudgetDescending(StoreRequirement store1, StoreRequirement store2)
        {
            return store2.Budget.CompareTo(store1.Budget);
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
            List<PerfumeMatch> perfumeHeap = BuildPerfumeHeap(store, inventory);
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
        private List<PerfumeMatch> BuildPerfumeHeap(StoreRequirement store, Dictionary<string, Perfume> inventory)
        {
            List<PerfumeMatch> perfumeHeap = new List<PerfumeMatch>();

            foreach (var kvp in inventory)
            {
                Perfume perfume = kvp.Value;
                if (IsPerfumeEligible(perfume, store))
                {
                    double matchPercentage = perfume.CalculateMatchPercentage(store);
                    if (matchPercentage >= MinimumSatisfactionRequired)
                    {
                        perfumeHeap.Add(new PerfumeMatch(matchPercentage, perfume));
                    }
                }
            }

            SortPerfumeMatchesByScoreDescending(perfumeHeap);
            return perfumeHeap;
        }

        // Sorts perfume matches by score in descending order
        private void SortPerfumeMatchesByScoreDescending(List<PerfumeMatch> matches)
        {
            matches.Sort(ComparePerfumeMatchDescending);
        }

        // Comparer for sorting perfume matches by score (descending)
        private int ComparePerfumeMatchDescending(PerfumeMatch match1, PerfumeMatch match2)
        {
            return match2.MatchPercentage.CompareTo(match1.MatchPercentage);
        }

        // Checks if a perfume meets basic requirements for a store
        private bool IsPerfumeEligible(Perfume perfume, StoreRequirement store)
        {
            return perfume.Stock > 0 && perfume.AveragePrice <= store.MaxPrice;
        }

        // Allocates best matching perfumes to a store
        private void AllocateBestMatches(StoreRequirement store, Dictionary<string, Perfume> inventory, List<PerfumeMatch> perfumeHeap)
        {
            List<PerfumeMatch> eligibleMatches = FilterEligibleMatches(store, perfumeHeap);

            foreach (var bestMatch in eligibleMatches)
            {
                Perfume perfume = bestMatch.Perfume;
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

        // Filters eligible matches 
        private List<PerfumeMatch> FilterEligibleMatches(StoreRequirement store, List<PerfumeMatch> perfumeHeap)
        {
            List<PerfumeMatch> eligibleMatches = new List<PerfumeMatch>();
            foreach (var match in perfumeHeap)
            {
                if (store.RemainingQuantity > 0)
                {
                    eligibleMatches.Add(match);
                }
            }
            return eligibleMatches;
        }

        // Allocates a single perfume to a store and updates inventory
        private void AllocatePerfumeToStore(StoreRequirement store, Dictionary<string, Perfume> inventory, PerfumeMatch bestMatch, Perfume perfume)
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
            List<PerfumeMatch> alternativePerfumes = FindAlternativePerfumes(store, inventory);
            AllocateAlternatives(store, inventory, alternativePerfumes);
        }

        // Finds alternative perfumes with lower match requirements
        private List<PerfumeMatch> FindAlternativePerfumes(StoreRequirement store, Dictionary<string, Perfume> inventory)
        {
            List<PerfumeMatch> alternativePerfumes = new List<PerfumeMatch>();

            foreach (var kvp in inventory)
            {
                Perfume perfume = kvp.Value;
                if (IsPerfumeAlternativeEligible(perfume, store))
                {
                    double matchPercentage = perfume.CalculateMatchPercentage(store);
                    if (matchPercentage >= 40.0 && matchPercentage < MinimumSatisfactionRequired)
                    {
                        alternativePerfumes.Add(new PerfumeMatch(matchPercentage, perfume));
                    }
                }
            }

            SortPerfumeMatchesByScoreDescending(alternativePerfumes);
            return alternativePerfumes;
        }

        // Checks if a perfume is eligible as an alternative allocation
        private bool IsPerfumeAlternativeEligible(Perfume perfume, StoreRequirement store)
        {
            return perfume.Stock > 0 && perfume.AveragePrice <= store.RemainingBudget;
        }

        // Allocates alternative perfumes to a store 
        private void AllocateAlternatives(StoreRequirement store, Dictionary<string, Perfume> inventory, List<PerfumeMatch> alternativePerfumes)
        {
            List<PerfumeMatch> eligibleAlternatives = FilterEligibleAlternatives(store, alternativePerfumes);

            foreach (var altMatch in eligibleAlternatives)
            {
                AllocateAlternativePerfume(store, inventory, altMatch.Perfume);
            }
        }

        // Filters eligible alternatives 
        private List<PerfumeMatch> FilterEligibleAlternatives(StoreRequirement store, List<PerfumeMatch> alternativePerfumes)
        {
            List<PerfumeMatch> eligibleAlternatives = new List<PerfumeMatch>();
            foreach (var altMatch in alternativePerfumes)
            {
                if (store.RemainingQuantity <= 0)
                {
                    return eligibleAlternatives; 
                }

                if (altMatch.Perfume.AveragePrice <= store.RemainingBudget)
                {
                    eligibleAlternatives.Add(altMatch);
                }
            }
            return eligibleAlternatives;
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
            List<StoreRequirement> storesBelowTarget = FindStoresBelowTarget(_allocationResults);
            foreach (var store in storesBelowTarget)
            {
                TryImproveStoreSatisfaction(store, inventory);
            }
        }

        // Finds stores below target satisfaction 
        private List<StoreRequirement> FindStoresBelowTarget(List<StoreRequirement> allocationResults)
        {
            List<StoreRequirement> storesBelowTarget = new List<StoreRequirement>();
            foreach (var store in allocationResults)
            {
                if (store.SatisfactionPercentage < TargetSatisfactionGoal)
                {
                    storesBelowTarget.Add(store);
                }
            }
            return storesBelowTarget;
        }

        // Attempts to improve a store's satisfaction by replacing worst-matching perfumes 
        private void TryImproveStoreSatisfaction(StoreRequirement store, Dictionary<string, Perfume> inventory)
        {
            if (store.AllocatedPerfumes.Count == 0) return;

            List<PerfumeMatch> allocatedByPerformance = GetAllocatedPerfumesByPerformance(store);

            int replacementCount = Math.Max(1, allocatedByPerformance.Count / 4);

            for (int i = 0; i < replacementCount && i < allocatedByPerformance.Count; i++)
            {
                var worstMatch = allocatedByPerformance[i];
                var betterOption = FindBetterReplacement(store, inventory, worstMatch);

                if (betterOption != null)
                {
                    string key = $"{worstMatch.Perfume.Brand}_{worstMatch.Perfume.Name}";
                    if (inventory.ContainsKey(key))
                    {
                        inventory[key].Stock++;
                        _totalProfit -= worstMatch.Perfume.AveragePrice * 0.3m;
                    }

                    store.AllocatedPerfumes.Remove(worstMatch.Perfume);
                    store.TotalSpent -= worstMatch.Perfume.AveragePrice;

                    store.AllocatedPerfumes.Add(betterOption.Clone());
                    store.TotalSpent += betterOption.AveragePrice;

                    string betterKey = $"{betterOption.Brand}_{betterOption.Name}";
                    inventory[betterKey].Stock--;
                    _totalProfit += betterOption.AveragePrice * 0.3m;
                }
            }

            CalculateSatisfaction(store);
        }

        // Gets allocated perfumes sorted by performance
        private List<PerfumeMatch> GetAllocatedPerfumesByPerformance(StoreRequirement store)
        {
            List<PerfumeMatch> allocatedByPerformance = new List<PerfumeMatch>();

            // Step 1: Transform to PerfumeMatch objects 
            foreach (var perfume in store.AllocatedPerfumes)
            {
                double matchPercentage = perfume.CalculateMatchPercentage(store);
                allocatedByPerformance.Add(new PerfumeMatch(matchPercentage, perfume));
            }

            // Step 2: Sort by performance ascending
            allocatedByPerformance.Sort(ComparePerfumeMatchAscending);

            return allocatedByPerformance;
        }

        // Comparer for sorting perfume matches by score (ascending - worst first)
        private int ComparePerfumeMatchAscending(PerfumeMatch match1, PerfumeMatch match2)
        {
            return match1.MatchPercentage.CompareTo(match2.MatchPercentage);
        }

        // Finds a better replacement perfume with at least 10% higher match score
        private Perfume FindBetterReplacement(StoreRequirement store, Dictionary<string, Perfume> inventory, PerfumeMatch currentMatch)
        {
            Perfume bestReplacement = null;
            double bestScore = currentMatch.MatchPercentage;

            foreach (var kvp in inventory)
            {
                Perfume perfume = kvp.Value;

                if (perfume.Stock > 0 &&
                    perfume.AveragePrice <= store.RemainingBudget + currentMatch.Perfume.AveragePrice &&
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