using CRUD_Application.Filters;
using CRUD_Application.Filters.ActionFilters;
using CRUD_Application.Filters.AuthorizationFilters;
using CRUD_Application.Filters.ExceptionFilters;
using CRUD_Application.Filters.ResourceFilters;
using CRUD_Application.Filters.ResultFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System.Runtime.CompilerServices;

namespace CRUD_Application.Controllers
{
	[Route("[controller]")]
	//[TypeFilter(typeof(ResponseHeaderActionFilter),
	//	Arguments = new object[] { "X-ControllerKey", "Controller-Value", 3 },
	//	Order = 3)]
	[ResponseHeaderFilterFactory("X-ControllerKey", "Controller-Value", 3)]
	//[TypeFilter(typeof(HandleExceptionFilter))]
	[TypeFilter(typeof(PersonsAlwaysRunResultFilter))]
	public class PersonsController : Controller
	{
		private readonly IPersonsGetterService _personGetterService;
		private readonly IPersonsAdderService _personAdderService;
		private readonly IPersonsUpdaterService _personsUpdaterService;
		private readonly IPersonsDeleterService _personsDeleterService;
		private readonly IPersonsSorterService _personsSorterService;
		private readonly ICountriesGetterService _countriesGetterService;
		private readonly ILogger<PersonsController> _logger;

		public PersonsController(IPersonsGetterService personGetterService,
			IPersonsAdderService personsAdderService,
			IPersonsUpdaterService personsUpdaterService,
			IPersonsDeleterService personsDeleterService,
			IPersonsSorterService personsSorterService,
			ICountriesGetterService countriesGetterService,
			ILogger<PersonsController> logger)
		{
			_personGetterService = personGetterService;
			_personAdderService = personsAdderService;
			_personsUpdaterService = personsUpdaterService;
			_personsDeleterService = personsDeleterService;
			_personsSorterService = personsSorterService;
			_countriesGetterService = countriesGetterService;
			_logger = logger;
		}

		[Route("[action]")]
		[Route("/")]
		[ServiceFilter(typeof(PersonsListActionFilter), Order = 4)]
		//[TypeFilter(typeof(ResponseHeaderActionFilter),
		//	Arguments = new object[] { "X-MyCustomKey", "Custom-Value", 1 },
		//	Order = 1)]
		[ResponseHeaderFilterFactory("X-MyCustomKey", "Custom-Value", 1)]
		[TypeFilter(typeof(PersonsListResultFilter))]
		[SkipFilter]
		public async Task<IActionResult> Index(string searchBy, string? searchString,
			string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
		{
			_logger.LogInformation("Index action of PersonsController");
			_logger.LogDebug($"searchBy: {searchBy}, searchString: {searchString}, sortBy: {sortBy}, sortOrder: {sortOrder}");

			List<PersonResponse> persons = await _personGetterService.GetFilteredPersons(searchBy, searchString);
			//ViewBag.CurrentSearchBy = searchBy;
			//ViewBag.CurrentSearchString = searchString;

			//Sorting
			List<PersonResponse> sortedPersons = await _personsSorterService.GetSortedPersons(persons, sortBy, sortOrder);
			//ViewBag.CurrentSortBy = sortBy;
			//ViewBag.CurrentSortOrder = sortOrder.ToString();

			return View(sortedPersons);
		}

		[Route("[action]")]
		[HttpGet]
		//[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "X-CreateKey", "Create-Value", 4 })]
		[ResponseHeaderFilterFactory("X-CreateKey", "Create-Value", 4)]
		public async Task<IActionResult> Create()
		{
			List<CountryResponse> countries = await _countriesGetterService.GetAllCountries();
			ViewBag.Countries = countries.Select(temp => new SelectListItem()
			{
				Text = temp.CountryName,
				Value = temp.CountryID.ToString()
			});

			return View();
		}

		[Route("[action]")]
		[HttpPost]
		[TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
		//[TypeFilter(typeof(FeatureDisabledResourceFilter))]
		public async Task<IActionResult> Create(PersonAddRequest personRequest)
		{
			PersonResponse person = await _personAdderService.AddPerson(personRequest);
			return RedirectToAction("Index", "Persons");
		}

		[Route("[action]/{personID}")]
		[HttpGet]
		[TypeFilter(typeof(TokenResultFilter))]
		public async Task<IActionResult> Edit(Guid personID)
		{
			PersonResponse? personResponse = await _personGetterService.GetPersonByPersonID(personID);
			if (personResponse == null)
			{
				return RedirectToAction("Index");
			}

			List<CountryResponse> countries = await _countriesGetterService.GetAllCountries();
			ViewBag.Countries = countries.Select(temp => new SelectListItem()
			{
				Text = temp.CountryName,
				Value = temp.CountryID.ToString()
			});

			PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

			return View(personUpdateRequest);
		}

		[Route("[action]/{personID}")]
		[HttpPost]
		[TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
		[TypeFilter(typeof(TokenAuthorizationFilter))]
		public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
		{
			PersonResponse? personResponse = await _personGetterService.GetPersonByPersonID(personRequest.PersonID);
			if (personResponse == null)
			{
				return RedirectToAction("Index");
			}

			PersonResponse updatedPerson = await _personsUpdaterService.UpdatePerson(personRequest);
			return RedirectToAction("Index");
		}

		[Route("[action]/{personID}")]
		[HttpGet]
		public async Task<IActionResult> Delete(Guid? personID)
		{
			PersonResponse? personResponse = await _personGetterService.GetPersonByPersonID(personID);

			if (personResponse == null)
			{
				return RedirectToAction("Persons", "Index");
			}

			return View(personResponse);
		}

		[Route("[action]/{personID}")]
		[HttpPost]
		public async Task<IActionResult> Delete(PersonResponse person)
		{
			PersonResponse? personResponse = await _personGetterService.GetPersonByPersonID(person.PersonID);

			if (personResponse == null)
			{
				return RedirectToAction("Index");
			}

			await _personsDeleterService.DeletePerson(person.PersonID);
			return RedirectToAction("Index");
		}

		[Route("[action]")]
		public async Task<IActionResult> PersonsPDF()
		{
			//Get list of persons
			List<PersonResponse> persons = await _personGetterService.GetAllPersons();

			//Return view as PDF
			return new ViewAsPdf("PersonsPDF", persons, ViewData)
			{
				PageMargins = new Rotativa.AspNetCore.Options.Margins()
				{
					Top = 20,
					Right = 20,
					Bottom = 20,
					Left = 20
				},
				PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
			};
		}

		[Route("[action]")]
		public async Task<IActionResult> PersonsCSV()
		{
			MemoryStream memoryStream = await _personGetterService.GetPersonsCSV();
			return File(memoryStream, "applicaiton/octet-stream", "persons.csv");
		}

		[Route("[action]")]
		public async Task<IActionResult> PersonsExcel()
		{
			MemoryStream memoryStream = await _personGetterService.GetPersonsExcel();
			return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
		}
	}
}
