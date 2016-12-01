#region License
// <copyright file="OrderInvoiceTests.cs">
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
using System.Collections.Generic;
using SalesTax.Core.Data;
using SalesTax.Core.Engine;
using SalesTax.Tests.Attributes;
using Xunit;
using System.Linq;
using SalesTax.Core.Parsing;
using SalesTax.Core;
using System.Globalization;

namespace SalesTax.Tests
{
    public class OrderInvoiceTests
    {
        public static IEnumerable<object[]> ProductSource
        {
            get
            {
                yield return new object[]
                {
                    new Product(ProductOrigin.Local, "book", 12.49M, 1),
                    0,
                    12.49
                };

                yield return new object[]
                {
                    new Product(ProductOrigin.Local, "music CD", 14.99M, 1),
                    1.5,
                    16.49
                };

                yield return new object[]
                {
                    new Product(ProductOrigin.Local, "music CDs", 14.99M, 2),
                    3,
                    32.98
                };

                yield return new object[]
                {
                    new Product(ProductOrigin.Local, "chocolate bar", 0.85M, 1),
                    0,
                    0.85
                };

                yield return new object[]
                {
                    new Product(ProductOrigin.Imported, "box of chocolates", 10, 1),
                    0.5,
                    10.5
                };

                yield return new object[]
                {
                    new Product(ProductOrigin.Imported, "bottle of perfume", 47.5M, 1),
                    7.15,
                    54.65
                };

                yield return new object[]
                {
                    new Product(ProductOrigin.Imported, "bottle of perfume", 27.99M, 1),
                    4.2,
                    32.19
                };

                yield return new object[]
                {
                    new Product(ProductOrigin.Local, "bottle of perfume", 18.99M, 1),
                    1.9,
                    20.89
                };

                yield return new object[]
                {
                    new Product(ProductOrigin.Local, "packet of headache pills", 9.75M, 1),
                    0,
                    9.75
                };

                yield return new object[]
                {
                    new Product(ProductOrigin.Imported, "box of chocolates", 11.25M, 1),
                    0.6,
                    11.85
                };
            }
        }

        public static IEnumerable<object[]> OrderInvoiceSource
        {
            get
            {
                yield return new object[]
                {
                    new List<Product>
                    {
                        new Product(ProductOrigin.Local, "book", 12.49M, 1),
                        new Product(ProductOrigin.Local, "music CD", 14.99M, 1),
                        new Product(ProductOrigin.Local, "chocolate bar", 0.85M, 1)
                    },
                    1.5,
                    29.83
                };

                yield return new object[]
                {
                    new List<Product>
                    {
                        new Product(ProductOrigin.Imported, "box of chocolates", 10, 1),
                        new Product(ProductOrigin.Imported, "bottle of perfume", 47.5M, 1)
                    },
                    7.65,
                    65.15
                };

                yield return new object[]
                {
                    new List<Product>
                    {
                        new Product(ProductOrigin.Imported, "bottle of perfume", 27.99M, 1),
                        new Product(ProductOrigin.Local, "bottle of perfume", 18.99M, 1),
                        new Product(ProductOrigin.Local, "packet of headache pills", 9.75M, 1),
                        new Product(ProductOrigin.Imported, "box of chocolates", 11.25M, 1)
                    },
                    6.7,
                    74.68
                };
            }
        }

        [Fact]
        [TraitCategory("OrderInvoiceManager - orderInvoice creation")]
        public void Should_ThrowException_When_CreatingInvoiceWithNullProductList()
        {
            // Arrange
            var orderInvoiceManager = new OrderInvoiceManager();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => orderInvoiceManager.CreateInvoice(null));
        }

        [Fact]
        [TraitCategory("OrderInvoiceManager - orderInvoice creation")]
        public void Should_ThrowException_When_CreatingInvoiceWithEmptyProductList()
        {
            // Arrange
            var orderInvoiceManager = new OrderInvoiceManager();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => orderInvoiceManager.CreateInvoice(new List<Product>(0)));
            Assert.Equal("Value cannot be an empty collection.\r\nParameter name: products", exception.Message);
        }

        [Theory]
        [TraitCategory("OrderInvoiceManager - orderInvoice amounts")]
        [MemberData(nameof(ProductSource))]
        public void Should_ReturnExpectedAmounts_When_CalculatingTaxes(Product product, decimal expectedTaxAmount, decimal expectedAmount)
        {
            // Arrange
            var orderInvoiceManager = new OrderInvoiceManager();

            // Act
            var orderInvoice = orderInvoiceManager.CreateInvoice(new List<Product> { product });

            // Assert
            var computedProduct = orderInvoice.Products.Single();
            Assert.Equal(expectedTaxAmount, computedProduct.TaxAmount);
            Assert.Equal(expectedAmount, computedProduct.Amount);
        }

        [Theory]
        [TraitCategory("OrderInvoiceManager - orderInvoice amounts")]
        [MemberData(nameof(OrderInvoiceSource))]
        public void Should_ReturnExpectedAmounts_When_CalculatingTaxes(List<Product> products, decimal expectedTaxAmount, decimal expectedTotalAmount)
        {
            // Arrange
            var orderInvoiceManager = new OrderInvoiceManager();
            var orderInvoice = orderInvoiceManager.CreateInvoice(products);

            // Act
            var totalTaxes = orderInvoiceManager.GetTotalTaxes(orderInvoice);
            var totalAmount = orderInvoiceManager.GetTotalAmount(orderInvoice);

            // Assert
            Assert.Equal(expectedTaxAmount, totalTaxes);
            Assert.Equal(expectedTotalAmount, totalAmount);
        }

        [Theory]
        [TraitCategory("OrderInvoiceManager - print computed product")]
        [InlineData("1 book at 12.49", "1 book: 12.49")]
        [InlineData("1 music CD at 14.99", "1 music CD: 16.49")]
        [InlineData("1 chocolate bar at 0.85", "1 chocolate bar: 0.85")]
        [InlineData("1 imported box of chocolates at 11.25", "1 imported box of chocolates: 11.85")]
        public void Should_ReturnExpectedString_When_PrintingComputedProduct(string productInput, string expectedOutput)
        {
            using (new CultureOverride(CultureInfo.InvariantCulture))
            {
                // Arrange
                var productParser = new ProductParser();
                var orderInvoiceManager = new OrderInvoiceManager();

                // Act
                var orderInvoice = orderInvoiceManager.CreateInvoice(productParser.ParseAll(new List<string> { productInput }));

                // Assert
                Assert.Equal(1, orderInvoice.Products.Count);
                Assert.Equal(expectedOutput, orderInvoice.Products.Single().ToString());
            }
        }
    }
}
