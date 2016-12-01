#region License
// <copyright file="Program.cs">
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
using SalesTax.Core.Engine;
using SalesTax.Core.Parsing;
using SalesTax.Core.Data;
using SalesTax.Core;
using System.Globalization;

namespace SalesTax
{
    internal class Program
    {
        private static IEnumerable<List<string>> InputOrders
        {
            get
            {
                // Input 1
                yield return new List<string>
                {
                    "1 book at 12.49",
                    "1 music CD at 14.99",
                    "1 chocolate bar at 0.85"
                };

                // Input 2
                yield return new List<string>
                {
                    "1 imported box of chocolates at 10.00",
                    "1 imported bottle of perfume at 47.50"
                };

                // Input 3
                yield return new List<string>
                {
                    "1 imported bottle of perfume at 27.99",
                    "1 bottle of perfume at 18.99",
                    "1 packet of headache pills at 9.75",
                    "1 box of imported chocolates at 11.25"
                };
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("INPUT:");

            var i = 1;

            foreach (var productList in InputOrders)
            {
                Console.WriteLine();
                Console.WriteLine($"Input {i++}:");

                foreach (var productString in productList)
                {
                    Console.WriteLine(productString);
                }
            }

            i = 1;

            Console.WriteLine();
            Console.WriteLine("OUTPUT:");

            using (new CultureOverride(CultureInfo.InvariantCulture))
            {
                foreach (var productList in InputOrders)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Output {i++}:");

                    var productParser = new ProductParser();
                    var orderInvoiceManager = new OrderInvoiceManager();

                    var orderInvoice = orderInvoiceManager.CreateInvoice(productParser.ParseAll(productList));

                    foreach (var computedProduct in orderInvoice.Products)
                    {
                        Console.WriteLine(computedProduct);
                    }

                    Console.WriteLine($"Sales Taxes: {orderInvoiceManager.GetTotalTaxes(orderInvoice).ToString("0.00")}");
                    Console.WriteLine($"Total: {orderInvoiceManager.GetTotalAmount(orderInvoice).ToString("0.00")}");
                }

                Console.WriteLine();
                Console.Write("Press any key to exit...");

                Console.ReadLine();
            }
        }
    }
}