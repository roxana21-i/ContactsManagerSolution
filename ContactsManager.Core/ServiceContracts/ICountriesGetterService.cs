using ServiceContracts.DTO;
using Microsoft.AspNetCore.Http;

namespace ServiceContracts
{
    /// <summary>
    /// Bussiness logic for manipulating Country entity
    /// </summary>
    public interface ICountriesGetterService
    {

        /// <summary>
        /// Gets all countries
        /// </summary>
        /// <returns>All countries from the list as a list of CountryResponse</returns>
        Task<List<CountryResponse>> GetAllCountries();

        /// <summary>
        /// Returns a Country object based on the given countryID
        /// </summary>
        /// <param name="countryID">CountryID (GUID) to match</param>
        /// <returns>The matching Country object as CountriesResponse or null if none</returns>
        Task<CountryResponse?> GetCountryByCountryID(Guid? countryID);
    }
}