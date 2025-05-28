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
        private int _maxBacktrackingIterations = 10000;
        private int _currentBacktrackingIterations = 0;

        // ========================= INITIALIZATION PHASE =========================
        // Functions needed: InitializeInventory()

        // Initializes allocation engine with perfume inventory
        public AllocationEngine(List<Perfume> perfumes)
        {
            InitializeInventory(perfumes); // PHASE 1: Build HashMap
        }

        // Builds perfume inventory dictionary with brand_name keys
        private void InitializeInventory(List<Perfume> perfumes)
        {
            foreach (var perfume in perfumes)
            {
                string key = $"{perfume.Brand}_{perfume.Name}";
                _perfumeInventory[key] = perfume;
            }
        }

        // ========================= MAIN CONTROL FLOW =========================
        // Orchestrates all phases in sequence

        // Main allocation method that processes all stores and returns results
        public List<StoreRequirement> AllocatePerfumes(List<StoreRequirement> storeRequirements)
        {
            // PHASE 1: INITIALIZATION
            ResetAllocationData();                                    // Clear previous results
            Dictionary<string, Perfume> workingInventory = CreateWorkingInventoryCopy(); // Clone inventory
            List<StoreRequirement> sortedStores = SortStoresByBudgetDescending(storeRequirements); // Priority order

            // PHASE 2: STORE PROCESSING  
            ProcessStoreAllocations(sortedStores, workingInventory);  // Main allocation logic

            // PHASE 3: OPTIMIZATION
            RebalanceAllocations(sortedStores, workingInventory);     // Backtracking improvement

            // PHASE 4: RESULTS
            return _allocationResults;                                // Return final results
        }

        // ========================= PHASE 1: INITIALIZATION FUNCTIONS =========================

        // Clears previous allocation results and profit tracking
        private void ResetAllocationData()
        {
            _allocationResults.Clear();
            _totalProfit = 0;
        }

        // Creates deep copy of inventory for allocation operations
        private Dictionary<string, Perfume> CreateWorkingInventoryCopy()
        {
            Dictionary<string, Perfume> workingInventory = new Dictionary<string, Perfume>();
            foreach (var kvp in _perfumeInventory)
            {
                workingInventory[kvp.Key] = kvp.Value.Clone();
            }
            return workingInventory;
        }

        // Sorts stores by budget in descending order for priority allocation
        private List<StoreRequirement> SortStoresByBudgetDescending(List<StoreRequirement> storeRequirements)
        {
            List<StoreRequirement> sortedStores = new List<StoreRequirement>(storeRequirements);
            sortedStores.Sort(CompareBudgetDescending);
            return sortedStores;
        }

        // Compares two stores by budget for sorting
        private int CompareBudgetDescending(StoreRequirement store1, StoreRequirement store2)
        {
            return store2.Budget.CompareTo(store1.Budget);
        }

        // ========================= PHASE 2: STORE PROCESSING FUNCTIONS =========================

        // Processes allocation for each store in priority order
        private void ProcessStoreAllocations(List<StoreRequirement> sortedStores, Dictionary<string, Perfume> workingInventory)
        {
            foreach (var store in sortedStores)
            {
                var workingStore = store.Clone();                    // Work on copy
                AllocateToStore(workingStore, workingInventory);     // CORE ALLOCATION LOGIC
                CalculateSatisfaction(workingStore);                 // Calculate final satisfaction
                _allocationResults.Add(workingStore);                // Save result
            }
        }

        // Allocates perfumes to a single store using two-phase approach
        private void AllocateToStore(StoreRequirement store, Dictionary<string, Perfume> inventory)
        {
            store.AllocatedPerfumes.Clear();
            store.TotalSpent = 0;

           //60% satisfaction
            List<PerfumeMatch> bestMatches = GetPerfumeMatches(store, inventory, MinimumSatisfactionRequired);
            AllocateMatches(store, inventory, bestMatches);

         //  40-60% satisfaction
            if (store.RemainingQuantity > 0)
            {
                List<PerfumeMatch> alternatives = GetPerfumeMatches(store, inventory, 40.0, MinimumSatisfactionRequired);
                AllocateMatches(store, inventory, alternatives);
            }
        }

        // Gets perfumes matching satisfaction criteria sorted by match percentage
        private List<PerfumeMatch> GetPerfumeMatches(StoreRequirement store, Dictionary<string, Perfume> inventory,
                                                   double minSatisfaction, double maxSatisfaction = 100.0)
        {
            List<PerfumeMatch> matches = new List<PerfumeMatch>();

            // STEP 1: Find eligible perfumes
            foreach (var kvp in inventory)
            {
                Perfume perfume = kvp.Value;
                if (perfume.Stock > 0 && perfume.AveragePrice <= store.MaxPrice)
                {
                    double matchPercentage = perfume.CalculateMatchPercentage(store);
                    if (matchPercentage >= minSatisfaction && matchPercentage < maxSatisfaction)
                    {
                        matches.Add(new PerfumeMatch(matchPercentage, perfume));
                    }
                }
            }

            // STEP 2: Sort by match score (best first)
            matches.Sort(ComparePerfumeMatchDescending);
            return matches;
        }

        // Compares perfume matches by percentage for sorting
        private int ComparePerfumeMatchDescending(PerfumeMatch match1, PerfumeMatch match2)
        {
            return match2.MatchPercentage.CompareTo(match1.MatchPercentage);
        }

        // Allocates perfumes from match list to store within budget constraints
        private void AllocateMatches(StoreRequirement store, Dictionary<string, Perfume> inventory, List<PerfumeMatch> matches)
        {
            foreach (var match in matches)
            {
                if (store.RemainingQuantity <= 0) return;

                Perfume perfume = match.Perfume;
                if (perfume.AveragePrice <= store.RemainingBudget)
                {
                    // ALLOCATE: Add to store, update inventory, track profit
                    store.AllocatedPerfumes.Add(perfume.Clone());
                    store.TotalSpent += perfume.AveragePrice;
                    string key = $"{perfume.Brand}_{perfume.Name}";
                    inventory[key].Stock--;
                    _totalProfit += perfume.AveragePrice * 0.3m;
                }
            }
        }

        // Calculates satisfaction percentage based on average match scores of allocated perfumes
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

            store.SatisfactionPercentage = Math.Min(totalSatisfaction / store.AllocatedPerfumes.Count, 100.0);
        }

        // ========================= PHASE 3: OPTIMIZATION FUNCTIONS =========================

        // Improves allocations for stores below target satisfaction using backtracking
        private void RebalanceAllocations(List<StoreRequirement> storeRequirements, Dictionary<string, Perfume> inventory)
        {
            foreach (var store in _allocationResults)
            {
                if (store.SatisfactionPercentage < TargetSatisfactionGoal)
                {
                    _currentBacktrackingIterations = 0;

                    // SUB-PHASE 3A: Try simple improvement first (quick wins)
                    TrySimpleImprovement(store, inventory);

                    // SUB-PHASE 3B: Use recursive backtracking if still below target
                    if (store.SatisfactionPercentage < TargetSatisfactionGoal)
                    {
                        BacktrackingRebalance(store, inventory);
                    }
                }
            }
        }

        // ========== SUB-PHASE 3A: SIMPLE IMPROVEMENT ==========

        // Tries quick improvement by replacing worst performing perfumes
        private void TrySimpleImprovement(StoreRequirement store, Dictionary<string, Perfume> inventory)
        {
            if (store.AllocatedPerfumes.Count == 0) return;

            // STEP 1: Find worst performing perfumes
            List<PerfumeMatch> sortedPerfumes = GetAllocatedPerfumesByPerformance(store);
            int replacementCount = Math.Max(1, sortedPerfumes.Count / 4);

            // STEP 2: Try to replace worst performers
            for (int i = 0; i < replacementCount && i < sortedPerfumes.Count; i++)
            {
                var worstMatch = sortedPerfumes[i];
                var betterOption = FindBetterReplacement(store, inventory, worstMatch);

                if (betterOption != null)
                {
                    // REPLACE: Update store allocation and inventory
                    string oldKey = $"{worstMatch.Perfume.Brand}_{worstMatch.Perfume.Name}";
                    string newKey = $"{betterOption.Brand}_{betterOption.Name}";

                    store.AllocatedPerfumes.Remove(worstMatch.Perfume);
                    store.AllocatedPerfumes.Add(betterOption.Clone());
                    store.TotalSpent = store.TotalSpent - worstMatch.Perfume.AveragePrice + betterOption.AveragePrice;

                    inventory[oldKey].Stock++;
                    inventory[newKey].Stock--;

                    _totalProfit = _totalProfit - (worstMatch.Perfume.AveragePrice * 0.3m) + (betterOption.AveragePrice * 0.3m);
                }
            }

            CalculateSatisfaction(store);
        }

        // Gets allocated perfumes sorted by match performance ascending
        private List<PerfumeMatch> GetAllocatedPerfumesByPerformance(StoreRequirement store)
        {
            List<PerfumeMatch> allocatedByPerformance = new List<PerfumeMatch>();

            foreach (var perfume in store.AllocatedPerfumes)
            {
                double matchPercentage = perfume.CalculateMatchPercentage(store);
                allocatedByPerformance.Add(new PerfumeMatch(matchPercentage, perfume));
            }

            allocatedByPerformance.Sort(ComparePerfumeMatchAscending); // Worst first
            return allocatedByPerformance;
        }

        // Compares perfume matches by percentage ascending for worst-first sorting
        private int ComparePerfumeMatchAscending(PerfumeMatch match1, PerfumeMatch match2)
        {
            return match1.MatchPercentage.CompareTo(match2.MatchPercentage);
        }

        // Finds better replacement perfume with higher match score
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
                    if (score > bestScore + 10.0) // Must be significantly better
                    {
                        bestScore = score;
                        bestReplacement = perfume;
                    }
                }
            }

            return bestReplacement;
        }

        // ========== SUB-PHASE 3B: RECURSIVE BACKTRACKING ==========

        // Uses recursive backtracking to find optimal perfume combinations for store
        private void BacktrackingRebalance(StoreRequirement store, Dictionary<string, Perfume> inventory)
        {
            // STEP 1: Initialize best solution tracking
            List<Perfume> bestAllocation = new List<Perfume>();
            foreach (var perfume in store.AllocatedPerfumes)
            {
                bestAllocation.Add(perfume.Clone());
            }

            decimal bestTotalSpent = store.TotalSpent;
            double bestSatisfaction = store.SatisfactionPercentage;

            // STEP 2: Get available perfumes for swapping
            List<Perfume> availablePerfumes = GetAvailablePerfumesForRebalancing(store, inventory);

            // STEP 3: Start recursive backtracking
            if (BacktrackRebalanceRecursive(store, inventory, availablePerfumes, 0, ref bestSatisfaction, ref bestAllocation, ref bestTotalSpent))
            {
                // STEP 4: Apply the best solution found
                ApplyBestSolution(store, inventory, bestAllocation, bestTotalSpent);
            }
        }

        // Gets available perfumes suitable for rebalancing sorted by match score
        private List<Perfume> GetAvailablePerfumesForRebalancing(StoreRequirement store, Dictionary<string, Perfume> inventory)
        {
            List<(Perfume perfume, double score)> perfumeScores = new List<(Perfume, double)>();

            foreach (var kvp in inventory)
            {
                var perfume = kvp.Value;
                if (perfume.Stock > 0 && perfume.AveragePrice <= store.MaxPrice)
                {
                    double matchScore = perfume.CalculateMatchPercentage(store);
                    if (matchScore >= 50.0) // Only consider reasonable matches
                    {
                        perfumeScores.Add((perfume, matchScore));
                    }
                }
            }

            perfumeScores.Sort((a, b) => b.score.CompareTo(a.score)); // Best first

            List<Perfume> available = new List<Perfume>();
            foreach (var item in perfumeScores)
            {
                available.Add(item.perfume);
            }

            return available;
        }

        // Recursive function that explores perfume swaps to maximize satisfaction
        private bool BacktrackRebalanceRecursive(StoreRequirement store, Dictionary<string, Perfume> inventory,
                                               List<Perfume> availablePerfumes, int depth,
                                               ref double bestSatisfaction, ref List<Perfume> bestAllocation, ref decimal bestTotalSpent)
        {
            _currentBacktrackingIterations++;

            // BASE CASE: Prevent infinite recursion
            if (_currentBacktrackingIterations > _maxBacktrackingIterations || depth > 6)
                return false;

            CalculateSatisfaction(store);

            // UPDATE BEST SOLUTION: If current is better than best so far
            if (store.SatisfactionPercentage > bestSatisfaction + 1.0)
            {
                bestSatisfaction = store.SatisfactionPercentage;
                bestAllocation.Clear();
                foreach (var perfume in store.AllocatedPerfumes)
                {
                    bestAllocation.Add(perfume.Clone());
                }
                bestTotalSpent = store.TotalSpent;

                // EARLY EXIT: If we reached target, no need to continue
                if (store.SatisfactionPercentage >= TargetSatisfactionGoal)
                    return true;
            }

            // RECURSIVE EXPLORATION: Try swapping each perfume with each available alternative
            bool foundBetter = false;
            for (int i = 0; i < store.AllocatedPerfumes.Count; i++)
            {
                var currentPerfume = store.AllocatedPerfumes[i];

                foreach (var newPerfume in availablePerfumes)
                {
                    if (CanSwap(store, currentPerfume, newPerfume, inventory))
                    {
                        // MAKE CHOICE: Swap perfumes
                        SwapPerfumes(store, inventory, i, currentPerfume, newPerfume);

                        // RECURSE: Explore deeper with this swap
                        if (BacktrackRebalanceRecursive(store, inventory, availablePerfumes, depth + 1,
                                                      ref bestSatisfaction, ref bestAllocation, ref bestTotalSpent))
                        {
                            foundBetter = true;
                        }

                        // BACKTRACK: Undo the swap
                        SwapPerfumes(store, inventory, i, newPerfume, currentPerfume);
                    }
                }
            }

            return foundBetter;
        }

        // Checks if two perfumes can be swapped within budget and stock constraints
        private bool CanSwap(StoreRequirement store, Perfume current, Perfume newPerfume, Dictionary<string, Perfume> inventory)
        {
            string newKey = $"{newPerfume.Brand}_{newPerfume.Name}";
            return inventory[newKey].Stock > 0 &&
                   newPerfume.AveragePrice <= store.RemainingBudget + current.AveragePrice &&
                   newPerfume.AveragePrice <= store.MaxPrice;
        }

        // Swaps two perfumes in store allocation and updates inventory
        private void SwapPerfumes(StoreRequirement store, Dictionary<string, Perfume> inventory, int index, Perfume oldPerfume, Perfume newPerfume)
        {
            store.AllocatedPerfumes[index] = newPerfume.Clone();
            store.TotalSpent = store.TotalSpent - oldPerfume.AveragePrice + newPerfume.AveragePrice;

            string oldKey = $"{oldPerfume.Brand}_{oldPerfume.Name}";
            string newKey = $"{newPerfume.Brand}_{newPerfume.Name}";

            inventory[oldKey].Stock++;
            inventory[newKey].Stock--;

            _totalProfit = _totalProfit - (oldPerfume.AveragePrice * 0.3m) + (newPerfume.AveragePrice * 0.3m);
        }

        // Applies the best allocation solution found by backtracking to store
        private void ApplyBestSolution(StoreRequirement store, Dictionary<string, Perfume> inventory, List<Perfume> bestAllocation, decimal bestTotalSpent)
        {
            // STEP 1: Return current perfumes to inventory
            foreach (var perfume in store.AllocatedPerfumes)
            {
                string key = $"{perfume.Brand}_{perfume.Name}";
                inventory[key].Stock++;
                _totalProfit -= perfume.AveragePrice * 0.3m;
            }

            // STEP 2: Apply best allocation
            store.AllocatedPerfumes.Clear();
            store.TotalSpent = 0;

            foreach (var perfume in bestAllocation)
            {
                store.AllocatedPerfumes.Add(perfume.Clone());
                store.TotalSpent += perfume.AveragePrice;
                string key = $"{perfume.Brand}_{perfume.Name}";
                inventory[key].Stock--;
                _totalProfit += perfume.AveragePrice * 0.3m;
            }

            CalculateSatisfaction(store);
        }

        // ========================= PHASE 4: RESULTS FUNCTIONS =========================

        // Returns total profit from all allocations
        public decimal GetTotalProfit()
        {
            return _totalProfit;
        }

        // Sets maximum iterations for backtracking to control performance
        public void SetMaxBacktrackingIterations(int maxIterations)
        {
            _maxBacktrackingIterations = maxIterations;
        }

        // Resets inventory to original state for new allocation runs
        public void ResetInventory(List<Perfume> perfumes)
        {
            _perfumeInventory.Clear();
            InitializeInventory(perfumes);
        }
    }
}