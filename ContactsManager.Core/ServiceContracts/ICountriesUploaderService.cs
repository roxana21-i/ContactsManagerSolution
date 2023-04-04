using ServiceContracts.DTO;
using Microsoft.AspNetCore.Http;

namespace ServiceContracts
{
    /// <summary>
    /// Bussiness logic for manipulating Country entity
    /// </summary>
    public interface ICountriesUploaderService
    {
        /// <summary>
        /// Uploads countries from excel file into database
        /// </summary>
        /// <param name="formFile">Excel file with list of countries</param>
        /// <returns>Returns number of countries added</returns>
        Task<int> UploadCountriesFromExcelFiles(IFormFile formFile);
    }
}