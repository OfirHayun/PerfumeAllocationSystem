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
            Dictionary<string, Perfume> workingInventory = CreateWorkingInventoryCopy();
            List<StoreRequirement> sortedStores = storeRequirements.OrderByDescending(s => s.Budget).ToList();

            ProcessStoreAllocations(sortedStores, workingInventory);

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

            // Update heap if needed
            if (inventory[key].Stock > 0)
            {
                UpdatePerfumeHeap(inventory, bestMatch);
            }

            // Update profit (30% margin on average price)
            _totalProfit += perfume.AveragePrice * 0.3m;
        }

        private void UpdatePerfumeHeap(Dictionary<string, Perfume> inventory, Tuple<double, Perfume> bestMatch)
        {
            // This function would update the heap in a real implementation
            // Here we just have a placeholder since we're handling each allocation step separately
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
                    if (matchPercentage >= 50.0 && matchPercentage < MinimumSatisfactionRequired)
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