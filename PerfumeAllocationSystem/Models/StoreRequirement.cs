using System;
using System.Collections.Generic;

namespace PerfumeAllocationSystem.Models
{
    public class StoreRequirement
    {
        public string StoreName { get; set; }
        public decimal Budget { get; set; }
        public int QuantityNeeded { get; set; }

        public string Gender { get; set; }
        public string PreferredAccord { get; set; }
        public string PreferredTopNotes { get; set; }
        public string PreferredMiddleNotes { get; set; }
        public string PreferredBaseNotes { get; set; }
        public int MinLongevity { get; set; }
        public int MinProjection { get; set; }
        public decimal MaxPrice { get; set; }

        public List<Perfume> AllocatedPerfumes { get; set; } = new List<Perfume>();

        public double SatisfactionPercentage { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal RemainingBudget => Budget - TotalSpent;
        public int RemainingQuantity => QuantityNeeded - AllocatedPerfumes.Count;

        // Creates a clone of this store requirement
        public StoreRequirement Clone()
        {
            StoreRequirement clone = CreateClonedStore();
            CloneAllocatedPerfumes(clone);
            return clone;
        }

        // Creates a new store with copied properties
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

        // Clones the allocated perfumes to the new store
        private void CloneAllocatedPerfumes(StoreRequirement clone)
        {
            foreach (var perfume in this.AllocatedPerfumes)
            {
                clone.AllocatedPerfumes.Add(perfume.Clone());
            }
        }
    }
}