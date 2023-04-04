using Entities;
using System;
using System.Collections.Generic;

namespace ServiceContracts.DTO
{
    public class CountryAddRequest
    {
        /// <summary>
        /// DTO class for adding a new country
        /// </summary>
        public string? CountryName { get; set; }

        public Country ToCountry()
        {
            return new Country()
            {
                CountryName = CountryName,
            };
        }
    }
}
