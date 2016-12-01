#region License
// <copyright file="ProductParser.cs">
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
using System.Text.RegularExpressions;
using SalesTax.Core.Data;

namespace SalesTax.Core.Parsing
{
    public class ProductParser
    {
        private const string QuantityRegexGroup = "QUANTITY";
        private const string ImportedOriginRegexGroup = "ORIGIN";
        private const string DescriptionRegexGroup = "DESCRIPTION";
        private const string PriceRegexGroup = "PRICE";

        private static readonly Regex ProductRegex = new Regex($@"^(?<{QuantityRegexGroup}>\d+)(?<{ImportedOriginRegexGroup}>\s+imported)?\s+(?<{DescriptionRegexGroup}>.*)\s+at\s+(?<{PriceRegexGroup}>\d+(\.\d+)?)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public bool TryParse(string input, out Product product)
        {
            if (string.IsNullOrEmpty(input)) throw new ArgumentException(nameof(input));

            product = null;

            var regexMatch = ProductRegex.Match(input);

            if (!regexMatch.Success)
            {
                return false;
            }

            var quantity = Convert.ToInt32(regexMatch.Groups[QuantityRegexGroup].Value);
            var origin = regexMatch.Groups[ImportedOriginRegexGroup].Success ? ProductOrigin.Imported : ProductOrigin.Local;
            var description = regexMatch.Groups[DescriptionRegexGroup].Value;
            var price = Convert.ToDecimal(regexMatch.Groups[PriceRegexGroup].Value);

            product = new Product(origin, description, price, quantity);

            return true;
        }
    }
}
