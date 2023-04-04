using Entities;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesGetterService : ICountriesGetterService
    {
        private readonly ICountriesRepository _countriesRepository;

        public CountriesGetterService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return  (await _countriesRepository.GetAllCountries())
                .Select(country => country.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
            {
                return null;
            }

            Country? response = await _countriesRepository.GetCountryByCountryID(countryID.Value);

            if (response == null)
            {
                return null;
            }
            return response.ToCountryResponse();
        }
    }
}