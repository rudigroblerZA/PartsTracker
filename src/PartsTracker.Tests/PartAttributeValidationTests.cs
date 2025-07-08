using PartsTracker.WebApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace PartsTracker.Tests
{
    public class PartAttributeValidationTests
    {
        private IList<ValidationResult> ValidateModel(Part part)
        {
            var context = new ValidationContext(part, null, null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(part, context, results, true);
            return results;
        }

        [Fact]
        public void PartNumber_Required_And_MaxLength()
        {
            var part = new Part { PartNumber = null! };
            var results = ValidateModel(part);
            Assert.Contains(results, r => r.MemberNames.Contains("PartNumber") && r.ErrorMessage!.Contains("required"));

            part.PartNumber = new string('A', 51);
            results = ValidateModel(part);
            Assert.Contains(results, r => r.MemberNames.Contains("PartNumber") && r.ErrorMessage!.Contains("maximum length"));
        }

        [Fact]
        public void Description_MaxLength()
        {
            var part = new Part { PartNumber = "P1", Description = new string('D', 201) };
            var results = ValidateModel(part);
            Assert.Contains(results, r => r.MemberNames.Contains("Description") && r.ErrorMessage!.Contains("maximum length"));
        }

        [Fact]
        public void QuantityOnHand_Range()
        {
            var part = new Part { PartNumber = "P1", QuantityOnHand = -1 };
            var results = ValidateModel(part);
            Assert.Contains(results, r => r.MemberNames.Contains("QuantityOnHand") && r.ErrorMessage!.Contains("zero or more"));
        }

        [Fact]
        public void LocationCode_MaxLength()
        {
            var part = new Part { PartNumber = "P1", LocationCode = new string('L', 51) };
            var results = ValidateModel(part);
            Assert.Contains(results, r => r.MemberNames.Contains("LocationCode") && r.ErrorMessage!.Contains("maximum length"));
        }

        [Fact]
        public void LastStockTake_DefaultsToUtcNow()
        {
            var part = new Part { PartNumber = "P1" };
            Assert.True((DateTime.UtcNow - part.LastStockTake).TotalSeconds < 5);
        }
    }
}
