#region License
// <copyright file="TraitCategoryAttribute.cs">
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
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace SalesTax.Tests.Attributes
{
    public class CategoryDiscoverer : ITraitDiscoverer
    {
        public const string Key = "Category";

        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        {
            var ctorArgs = traitAttribute.GetConstructorArguments().ToList();
            yield return new KeyValuePair<string, string>(Key, ctorArgs[0].ToString());
        }
    }

    [TraitDiscoverer("SalesTax.Tests.Attributes.CategoryDiscoverer", "SalesTax.Tests")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class TraitCategoryAttribute : Attribute, ITraitAttribute
    {
        public TraitCategoryAttribute(string category)
        {
            Category = category;
        }

        public string Category { get; }
    }
}
