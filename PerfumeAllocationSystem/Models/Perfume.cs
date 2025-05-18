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

        // Modified weight system - notes get more weight
    
        private const double NORMAL_WEIGHT = 1.0; // Regular weight for other criteria

        // Helper method to check if this perfume meets the store's requirements
        public double CalculateMatchPercentage(StoreRequirement requirement)
        {
            double totalPoints = 0;
            double matchingPoints = 0;

            CheckGenderMatch(requirement, ref totalPoints, ref matchingPoints);
            CheckAccordMatch(requirement, ref totalPoints, ref matchingPoints);
            CheckQualityMatch(requirement, ref totalPoints, ref matchingPoints);
            CheckPriceMatch(requirement, ref totalPoints, ref matchingPoints);
            CheckNotesMatch(requirement, ref totalPoints, ref matchingPoints);

            // If no criteria specified, return a more realistic value instead of 100%
            // This makes it more realistic - even with no requirements, not every perfume is perfect
            if (totalPoints == 0)
                return 70.0 + (new Random().NextDouble() * 20.0); // Random between 70-90%

            // Apply a small random factor to make satisfaction values more varied
            double basePercentage = (matchingPoints / totalPoints) * 100.0;
            double randomVariation = (new Random().NextDouble() * 10.0) - 5.0; // Random between -5 and +5

            // Ensure we stay within 0-100% range after applying random variation
            return Math.Max(0, Math.Min(100, basePercentage + randomVariation));
        }

        private void CheckGenderMatch(StoreRequirement requirement, ref double totalPoints, ref double matchingPoints)
        {
            if (!string.IsNullOrEmpty(requirement.Gender))
            {
                totalPoints += NORMAL_WEIGHT;
                if (Gender == requirement.Gender || Gender == "Unisex" || requirement.Gender == "Any")
                    matchingPoints += NORMAL_WEIGHT;
            }
        }

        private void CheckAccordMatch(StoreRequirement requirement, ref double totalPoints, ref double matchingPoints)
        {
            if (!string.IsNullOrEmpty(requirement.PreferredAccord))
            {
                totalPoints += NORMAL_WEIGHT;
                if (MainAccord == requirement.PreferredAccord)
                    matchingPoints += NORMAL_WEIGHT;
            }
        }

        private void CheckQualityMatch(StoreRequirement requirement, ref double totalPoints, ref double matchingPoints)
        {
            if (requirement.MinLongevity > 0)
            {
                totalPoints += NORMAL_WEIGHT;
                if (Longevity >= requirement.MinLongevity)
                    matchingPoints += NORMAL_WEIGHT;
            }

            if (requirement.MinProjection > 0)
            {
                totalPoints += NORMAL_WEIGHT;
                if (Projection >= requirement.MinProjection)
                    matchingPoints += NORMAL_WEIGHT;
            }
        }

        private void CheckPriceMatch(StoreRequirement requirement, ref double totalPoints, ref double matchingPoints)
        {
            if (requirement.MaxPrice > 0)
            {
                totalPoints += NORMAL_WEIGHT;
                if (AveragePrice <= requirement.MaxPrice)
                    matchingPoints += NORMAL_WEIGHT;
            }
        }

        private void CheckNotesMatch(StoreRequirement requirement, ref double totalPoints, ref double matchingPoints)
        {
            // Use weighted system for notes matching
            CheckSingleNoteMatch(requirement.PreferredTopNotes, TopNotes, ref totalPoints, ref matchingPoints);
            CheckSingleNoteMatch(requirement.PreferredMiddleNotes, MiddleNotes, ref totalPoints, ref matchingPoints);
            CheckSingleNoteMatch(requirement.PreferredBaseNotes, BaseNotes, ref totalPoints, ref matchingPoints);
        }

        private void CheckSingleNoteMatch(string requiredNote, string perfumeNotes, ref double totalPoints, ref double matchingPoints)
        {
            if (!string.IsNullOrEmpty(requiredNote))
            {
                totalPoints += NORMAL_WEIGHT;  // Add weighted points for notes
                if (perfumeNotes.Contains(requiredNote))
                    matchingPoints += NORMAL_WEIGHT;  // Add weighted points for match
            }
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