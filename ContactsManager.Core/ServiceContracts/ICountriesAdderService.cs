using ServiceContracts.DTO;
using Microsoft.AspNetCore.Http;

namespace ServiceContracts
{
    /// <summary>
    /// Bussiness logic for manipulating Country entity
    /// </summary>
    public interface ICountriesAdderService
    {
        /// <summary>
        /// Adds a country to the list of countries
        /// </summary>
        /// <param name="countryAddRequest">Country object to add</param>
        /// <returns>Returns the country object after adding it (with newly generated CountryID)</returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);
    }
}