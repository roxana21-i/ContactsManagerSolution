using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace CRUD_Application.Controllers
{
    [Route("[controller]")]
    public class CountriesController : Controller
    {
        private readonly ICountriesUploaderService _countriesUploaderService;

        public CountriesController(ICountriesUploaderService countriesUploaderService)
        {
            _countriesUploaderService = countriesUploaderService;
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult UploadFromExcel()
        {
            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> UploadFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ViewBag.ErrorMessage = "Please select an .xlsx file";
                return View();
            }

            if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage = "Please select an .xlsx file";
                return View();
            }

            int countriesInserted = await _countriesUploaderService.UploadCountriesFromExcelFiles(excelFile);
            ViewBag.Message = $"{countriesInserted} countries uploaded";

            return View();
        }
    }
}
