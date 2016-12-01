#region License
// <copyright file="TaxExemptionTests.cs">
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
using SalesTax.Tests.Attributes;
using Xunit;

namespace SalesTax.Tests
{
    public class TaxExemptionTests
    {
        [Fact]
        [TraitCategory("TaxManager - tax exemption")]
        public void Should_ThrowException_When_ProductIsNull()
        {
            // Arrange
            var taxManager = TaxManager.Instance;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => taxManager.IsTaxExempt(null));
        }

        [Theory]
        [TraitCategory("TaxManager - tax exemption")]
        [InlineData(null)]
        [InlineData("")]
        public void Should_ThrowArgumentException_When_ProductHasInvalidDescription(string description)
        {
            // Arrange
            var taxManager = TaxManager.Instance;
            var product = new Product(ProductOrigin.Local, description, Decimal.Zero, 0);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => taxManager.IsTaxExempt(product));
            Assert.Equal($"{nameof(product.Description)} cannot be null or empty", exception.Message);
        }

        [Theory]
        [TraitCategory("TaxManager - tax exemption")]
        [InlineData("book", true)]
        [InlineData("music", false)]
        [InlineData("chocolate", true)]
        [InlineData("box of chocolates", true)]
        [InlineData("bottle of perfume", false)]
        [InlineData("perfume", false)]
        [InlineData("headache pills", true)]
        public void Should_ReturnExpectedValue_When_ProductHasValidDescription(string description, bool expectedResult)
        {
            // Arrange
            var taxManager = TaxManager.Instance;
            var product = new Product(ProductOrigin.Local, description, Decimal.Zero, 0);

            // Act
            var result = taxManager.IsTaxExempt(product);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
