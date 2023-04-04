using Entities;
using System.Runtime.CompilerServices;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Country entity
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// Adds a new country object to the data store
        /// </summary>
        /// <param name="country">Country object to add</param>
        /// <returns>Returns the country object after adding it to the data store</returns>
        Task<Country> AddCountry(Country country);

        /// <summary>
        /// Returns all countries in the data store
        /// </summary>
        /// <returns>Returns all countries in the data store</returns>
        Task<List<Country>> GetAllCountries();

        /// <summary>
        /// Returns a Country object based on given countryID; otherwise returs null
        /// </summary>
        /// <param name="countryID">Country ID to search by</param>
        /// <returns>The matching Country object; otherwise null</returns>
        Task<Country?> GetCountryByCountryID(Guid countryID);

        /// <summary>
        /// Returns a Country object based on given countryName; otherwise returs null
        /// </summary>
        /// <param name="countryName">Country Name to search by</param>
        /// <returns>The matching Country object; otherwise null</returns>
        Task<Country?> GetCountryByCountryName(string countryName);
    }
}