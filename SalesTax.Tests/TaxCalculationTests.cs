#region License
// <copyright file="TaxCalculationTests.cs">
// 
// Copyright (C) 2016
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// The SalesTax program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/agpl.txt/>.
// </copyright>
// <summary>
// Basic sales tax is applicable at a rate of 10% on all goods, except books, food, and medical products that are exempt. 
// Import duty is an additional sales tax applicable on all imported goods at a rate of 5%, with no exemptions.
// When I purchase items I receive a receipt which lists the name of all the items and their price (including tax), finishing with the total cost of the items, and the total amounts of sales taxes paid. 
// The rounding rules for sales tax are that for a tax rate of n%, a shelf price of p contains (np/100 rounded up to the nearest 0.05) amount of sales tax.
// 
// Email: simone.dalcastagne@gmail.com
// </summary>
#endregion
using System;
using SalesTax.Core.Data;
using SalesTax.Core.Engine;
using SalesTax.Core.Parsing;
using SalesTax.Tests.Attributes;
using Xunit;
using SalesTax.Core;
using System.Globalization;

namespace SalesTax.Tests
{
    public class TaxCalculationTests
    {
        [Fact]
        [TraitCategory("TaxManager - tax calculation")]
        public void Should_ThrowException_When_ProductIsNull()
        {
            // Arrange
            var taxManager = TaxManager.Instance;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => taxManager.IsTaxExempt(null));
        }

        [Theory]
        [TraitCategory("TaxManager - tax calculation")]
        [InlineData("1 book at 12.49", 0)]
        [InlineData("1 music CD at 14.99", 1.5)]
        [InlineData("2 music CDs at 14.99", 1.5)]
        [InlineData("1 chocolate bar at 0.85", 0)]
        [InlineData("1 imported box of chocolates at 10.00", 0.50)]
        [InlineData("1 imported bottle of perfume at 47.50", 7.15)]
        [InlineData("2 imported bottles of perfume at 47.50", 7.15)]
        [InlineData("1 imported bottle of perfume at 27.99", 4.2)]
        [InlineData("1 bottle of perfume at 18.99", 1.9)]
        [InlineData("1 packet of headache pills at 9.75", 0)]
        [InlineData("1 imported box of chocolates at 11.25", 0.6)]

        public void Should_CalculateTaxCorrectly(string input, decimal expectedTax)
        {
            using (new CultureOverride(CultureInfo.InvariantCulture))
            {
                // Arrange
                var taxManager = TaxManager.Instance;
                var productParser = new ProductParser();
                productParser.TryParse(input, out Product product);

                // Act
                var tax = taxManager.CalculateTax(product);

                // Assert
                Assert.Equal(expectedTax, tax);
            }
        }
    }
}
