#region License
// <copyright file="ProductParserTests.cs">
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
using SalesTax.Tests.Attributes;
using Xunit;
using SalesTax.Core.Data;
using SalesTax.Core.Parsing;
using SalesTax.Core;
using System.Globalization;

namespace SalesTax.Tests
{
    public class ProductParserTests
    {
        [Theory]
        [TraitCategory("ProductParser - parse input")]
        [InlineData(null)]
        [InlineData("")]
        public void Should_ThrowException_When_InputIsNullOrEmpty(string input)
        {
            // Arrange
            var productParser = new ProductParser();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => productParser.TryParse(input, out Product _));
        }

        [Theory]
        [TraitCategory("ProductParser - parse input")]
        [InlineData("1 book 12.49")]
        [InlineData("1 book")]
        [InlineData("1 book at")]
        [InlineData("book at 12.49")]
        [InlineData("1.5 books at 12.49")]
        [InlineData("1 at 12.49")]
        public void ShouldNot_ParseProduct_When_MismatchingInput(string input)
        {
            // Arrange
            var productParser = new ProductParser();

            // Act
            var result = productParser.TryParse(input, out Product product);

            // Assert
            Assert.False(result);
            Assert.Null(product);
        }

        [Theory]
        [TraitCategory("ProductParser - parse input")]
        [InlineData("1 book at 12.49", ProductOrigin.Local, "book", 12.49, 1)]
        [InlineData("1 music CD at 14.99", ProductOrigin.Local, "music CD", 14.99, 1)]
        [InlineData("2 music CDs at 14.99", ProductOrigin.Local, "music CDs", 14.99, 2)]
        [InlineData("1 chocolate bar at 0.85", ProductOrigin.Local, "chocolate bar", 0.85, 1)]
        [InlineData("1 imported box of chocolates at 10", ProductOrigin.Imported, "box of chocolates", 10, 1)]
        [InlineData("1 imported bottle of perfume at 47.50", ProductOrigin.Imported, "bottle of perfume", 47.5, 1)]
        [InlineData("1 imported bottle of perfume at 27.99", ProductOrigin.Imported, "bottle of perfume", 27.99, 1)]
        [InlineData("1 bottle of perfume at 18.99", ProductOrigin.Local, "bottle of perfume", 18.99, 1)]
        [InlineData("1 packet of headache pills at 9.75", ProductOrigin.Local, "packet of headache pills", 9.75, 1)]
        [InlineData("1 imported box of chocolates at 11.25", ProductOrigin.Imported, "box of chocolates", 11.25, 1)]
        public void Should_ParseProductCorrectly(string input, ProductOrigin expectedOrigin, string expectedDescription, decimal expectedPrice, int expectedQuantity)
        {
            using (new CultureOverride(CultureInfo.InvariantCulture))
            {
                // Arrange
                var productParser = new ProductParser();

                // Act
                var result = productParser.TryParse(input, out Product product);

                // Assert
                Assert.True(result);
                Assert.Equal(expectedOrigin, product.Origin);
                Assert.Equal(expectedDescription, product.Description);
                Assert.Equal(expectedQuantity, product.Quantity);
                Assert.Equal(expectedPrice, product.Price);
                Assert.Equal(expectedPrice * expectedQuantity, product.Amount);
            }
        }
    }
}
