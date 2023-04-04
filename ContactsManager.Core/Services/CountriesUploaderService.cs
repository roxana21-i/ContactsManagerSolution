using Entities;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesUploaderService : ICountriesUploaderService
    {
        private readonly ICountriesRepository _countriesRepository;

        public CountriesUploaderService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }

        public async Task<int> UploadCountriesFromExcelFiles(IFormFile formFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);

            int countriesInserted = 0;

            using (ExcelPackage package = new ExcelPackage(memoryStream)) 
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Countries"];

                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    string? cellValue = Convert.ToString(worksheet.Cells[row, 1].Value);
                    
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string? countryName = cellValue;
                        if (_countriesRepository.GetCountryByCountryName(countryName) == null)
                        {
                            Country country = new Country()
                            {
                                CountryName = countryName,
                            };

                            await _countriesRepository.AddCountry(country);

                            countriesInserted++;
                        }
                    }
                }
            }
            return countriesInserted;
        }
    }
}