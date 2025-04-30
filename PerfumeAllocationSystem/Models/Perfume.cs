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

            CheckGenderMatch(requirement, ref totalCriteria, ref matchingCriteria);
            CheckAccordMatch(requirement, ref totalCriteria, ref matchingCriteria);
            CheckQualityMatch(requirement, ref totalCriteria, ref matchingCriteria);
            CheckPriceMatch(requirement, ref totalCriteria, ref matchingCriteria);
            CheckNotesMatch(requirement, ref totalCriteria, ref matchingCriteria);

            // If no criteria specified, return 100% match
            if (totalCriteria == 0)
                return 100.0;

            return (double)matchingCriteria / totalCriteria * 100.0;
        }

        private void CheckGenderMatch(StoreRequirement requirement, ref int totalCriteria, ref int matchingCriteria)
        {
            if (!string.IsNullOrEmpty(requirement.Gender))
            {
                totalCriteria++;
                if (Gender == requirement.Gender || Gender == "Unisex" || requirement.Gender == "Any")
                    matchingCriteria++;
            }
        }

        private void CheckAccordMatch(StoreRequirement requirement, ref int totalCriteria, ref int matchingCriteria)
        {
            if (!string.IsNullOrEmpty(requirement.PreferredAccord))
            {
                totalCriteria++;
                if (MainAccord == requirement.PreferredAccord)
                    matchingCriteria++;
            }
        }

        private void CheckQualityMatch(StoreRequirement requirement, ref int totalCriteria, ref int matchingCriteria)
        {
            if (requirement.MinLongevity > 0)
            {
                totalCriteria++;
                if (Longevity >= requirement.MinLongevity)
                    matchingCriteria++;
            }

            if (requirement.MinProjection > 0)
            {
                totalCriteria++;
                if (Projection >= requirement.MinProjection)
                    matchingCriteria++;
            }
        }

        private void CheckPriceMatch(StoreRequirement requirement, ref int totalCriteria, ref int matchingCriteria)
        {
            if (requirement.MaxPrice > 0)
            {
                totalCriteria++;
                if (AveragePrice <= requirement.MaxPrice)
                    matchingCriteria++;
            }
        }

        private void CheckNotesMatch(StoreRequirement requirement, ref int totalCriteria, ref int matchingCriteria)
        {
            CheckSingleNoteMatch(requirement.PreferredTopNotes, TopNotes, ref totalCriteria, ref matchingCriteria);
            CheckSingleNoteMatch(requirement.PreferredMiddleNotes, MiddleNotes, ref totalCriteria, ref matchingCriteria);
            CheckSingleNoteMatch(requirement.PreferredBaseNotes, BaseNotes, ref totalCriteria, ref matchingCriteria);
        }

        private void CheckSingleNoteMatch(string requiredNote, string perfumeNotes, ref int totalCriteria, ref int matchingCriteria)
        {
            if (!string.IsNullOrEmpty(requiredNote))
            {
                totalCriteria++;
                if (perfumeNotes.Contains(requiredNote))
                    matchingCriteria++;
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