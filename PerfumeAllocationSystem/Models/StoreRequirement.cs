using System;
using System.Collections.Generic;

namespace PerfumeAllocationSystem.Models
{
    public class StoreRequirement
    {
        public string StoreName { get; set; }
        public decimal Budget { get; set; }
        public int QuantityNeeded { get; set; }

        // Perfume attributes requirements
        public string Gender { get; set; }  // Male, Female, Unisex, Any
        public string PreferredAccord { get; set; } // Main accord type (e.g., Woody, Floral, etc.)
        public string PreferredTopNotes { get; set; }
        public string PreferredMiddleNotes { get; set; }
        public string PreferredBaseNotes { get; set; }
        public int MinLongevity { get; set; }
        public int MinProjection { get; set; }
        public decimal MaxPrice { get; set; }

        // Track allocated perfumes
        public List<Perfume> AllocatedPerfumes { get; set; } = new List<Perfume>();

        // Track allocation metrics
        public double SatisfactionPercentage { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal RemainingBudget => Budget - TotalSpent;
        public int RemainingQuantity => QuantityNeeded - AllocatedPerfumes.Count;

        // Clone this requirement (useful for backtracking algorithm)
        public StoreRequirement Clone()
        {
            StoreRequirement clone = CreateClonedStore();
            CloneAllocatedPerfumes(clone);
            return clone;
        }

        private StoreRequirement CreateClonedStore()
        {
            return new StoreRequirement
            {
                StoreName = this.StoreName,
                Budget = this.Budget,
                QuantityNeeded = this.QuantityNeeded,
                Gender = this.Gender,
                PreferredAccord = this.PreferredAccord,
                PreferredTopNotes = this.PreferredTopNotes,
                PreferredMiddleNotes = this.PreferredMiddleNotes,
                PreferredBaseNotes = this.PreferredBaseNotes,
                MinLongevity = this.MinLongevity,
                MinProjection = this.MinProjection,
                MaxPrice = this.MaxPrice,
                SatisfactionPercentage = this.SatisfactionPercentage,
                TotalSpent = this.TotalSpent
            };
        }

        private void CloneAllocatedPerfumes(StoreRequirement clone)
        {
            foreach (var perfume in this.AllocatedPerfumes)
            {
                clone.AllocatedPerfumes.Add(perfume.Clone());
            }
        }
    }
}