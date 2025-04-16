using System;
using System.Collections.Generic;

namespace PerfumeAllocationSystem.Models
{
    public class Perfume
    {
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Gender { get; set; }
        public string TopNotes { get; set; }
        public string MiddleNotes { get; set; }
        public string BaseNotes { get; set; }
        public string MainAccord { get; set; }
        public int Longevity { get; set; }
        public int Projection { get; set; }
        public decimal AveragePrice { get; set; }
        public int Stock { get; set; }

        // Helper method to check if this perfume meets the store's requirements
        public double CalculateMatchPercentage(StoreRequirement requirement)
        {
            int totalCriteria = 0;
            int matchingCriteria = 0;

            // Check Gender
            if (!string.IsNullOrEmpty(requirement.Gender))
            {
                totalCriteria++;
                if (Gender == requirement.Gender || Gender == "Unisex" || requirement.Gender == "Any")
                    matchingCriteria++;
            }

            // Check MainAccord
            if (!string.IsNullOrEmpty(requirement.PreferredAccord))
            {
                totalCriteria++;
                if (MainAccord == requirement.PreferredAccord)
                    matchingCriteria++;
            }

            // Check Longevity
            if (requirement.MinLongevity > 0)
            {
                totalCriteria++;
                if (Longevity >= requirement.MinLongevity)
                    matchingCriteria++;
            }

            // Check Projection
            if (requirement.MinProjection > 0)
            {
                totalCriteria++;
                if (Projection >= requirement.MinProjection)
                    matchingCriteria++;
            }

            // Check Price
            if (requirement.MaxPrice > 0)
            {
                totalCriteria++;
                if (AveragePrice <= requirement.MaxPrice)
                    matchingCriteria++;
            }

            // Check Top/Middle/Base Notes
            if (!string.IsNullOrEmpty(requirement.PreferredTopNotes))
            {
                totalCriteria++;
                if (TopNotes.Contains(requirement.PreferredTopNotes))
                    matchingCriteria++;
            }

            if (!string.IsNullOrEmpty(requirement.PreferredMiddleNotes))
            {
                totalCriteria++;
                if (MiddleNotes.Contains(requirement.PreferredMiddleNotes))
                    matchingCriteria++;
            }

            if (!string.IsNullOrEmpty(requirement.PreferredBaseNotes))
            {
                totalCriteria++;
                if (BaseNotes.Contains(requirement.PreferredBaseNotes))
                    matchingCriteria++;
            }

            // If no criteria specified, return 100% match
            if (totalCriteria == 0)
                return 100.0;

            return (double)matchingCriteria / totalCriteria * 100.0;
        }

        // Clone this perfume (useful when allocating)
        public Perfume Clone()
        {
            return new Perfume
            {
                Name = this.Name,
                Brand = this.Brand,
                Gender = this.Gender,
                TopNotes = this.TopNotes,
                MiddleNotes = this.MiddleNotes,
                BaseNotes = this.BaseNotes,
                MainAccord = this.MainAccord,
                Longevity = this.Longevity,
                Projection = this.Projection,
                AveragePrice = this.AveragePrice,
                Stock = this.Stock
            };
        }

        public override string ToString()
        {
            return $"{Brand} - {Name}";
        }
    }
}