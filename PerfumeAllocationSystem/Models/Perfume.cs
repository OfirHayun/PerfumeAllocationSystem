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

        private const double NORMAL_WEIGHT = 1.0;

        // Calculates match percentage between perfume and store requirements
        public double CalculateMatchPercentage(StoreRequirement requirement)
        {
            double totalPoints = 0;
            double matchingPoints = 0;

            CheckGenderMatch(requirement, ref totalPoints, ref matchingPoints);
            CheckAccordMatch(requirement, ref totalPoints, ref matchingPoints);
            CheckQualityMatch(requirement, ref totalPoints, ref matchingPoints);
            CheckPriceMatch(requirement, ref totalPoints, ref matchingPoints);
            CheckNotesMatch(requirement, ref totalPoints, ref matchingPoints);

            if (totalPoints == 0)
                return 70.0 + (new Random().NextDouble() * 20.0);

            double basePercentage = (matchingPoints / totalPoints) * 100.0;
            double randomVariation = (new Random().NextDouble() * 10.0) - 5.0;

            return Math.Max(0, Math.Min(100, basePercentage + randomVariation));
        }

        // Checks if gender matches store requirements
        private void CheckGenderMatch(StoreRequirement requirement, ref double totalPoints, ref double matchingPoints)
        {
            if (!string.IsNullOrEmpty(requirement.Gender))
            {
                totalPoints += NORMAL_WEIGHT;
                if (Gender == requirement.Gender || Gender == "Unisex" || requirement.Gender == "Any")
                    matchingPoints += NORMAL_WEIGHT;
            }
        }

        // Checks if accord matches store requirements
        private void CheckAccordMatch(StoreRequirement requirement, ref double totalPoints, ref double matchingPoints)
        {
            if (!string.IsNullOrEmpty(requirement.PreferredAccord))
            {
                totalPoints += NORMAL_WEIGHT;
                if (MainAccord == requirement.PreferredAccord)
                    matchingPoints += NORMAL_WEIGHT;
            }
        }

        // Checks if quality (longevity/projection) matches store requirements
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

        // Checks if price matches store requirements
        private void CheckPriceMatch(StoreRequirement requirement, ref double totalPoints, ref double matchingPoints)
        {
            if (requirement.MaxPrice > 0)
            {
                totalPoints += NORMAL_WEIGHT;
                if (AveragePrice <= requirement.MaxPrice)
                    matchingPoints += NORMAL_WEIGHT;
            }
        }

        // Checks if notes match store requirements
        private void CheckNotesMatch(StoreRequirement requirement, ref double totalPoints, ref double matchingPoints)
        {
            CheckSingleNoteMatch(requirement.PreferredTopNotes, TopNotes, ref totalPoints, ref matchingPoints);
            CheckSingleNoteMatch(requirement.PreferredMiddleNotes, MiddleNotes, ref totalPoints, ref matchingPoints);
            CheckSingleNoteMatch(requirement.PreferredBaseNotes, BaseNotes, ref totalPoints, ref matchingPoints);
        }

        // Checks if a specific note matches requirements
        private void CheckSingleNoteMatch(string requiredNote, string perfumeNotes, ref double totalPoints, ref double matchingPoints)
        {
            if (!string.IsNullOrEmpty(requiredNote))
            {
                totalPoints += NORMAL_WEIGHT;
                if (perfumeNotes.Contains(requiredNote))
                    matchingPoints += NORMAL_WEIGHT;
            }
        }

        // Creates a clone of this perfume
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

        // Returns string representation of the perfume
        public override string ToString()
        {
            return $"{Brand} - {Name}";
        }
    }
}