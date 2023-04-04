using CRUD_Application.Controllers;
using Entities;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;

namespace CRUD_Application.Filters.ActionFilters
{
    public class PersonCreateAndEditPostActionFilter : IAsyncActionFilter
    {
        private readonly ICountriesGetterService _countriesGetterService;

        public PersonCreateAndEditPostActionFilter(ICountriesGetterService countriesGetterService)
        {
            _countriesGetterService = countriesGetterService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //before logic

            if (context.Controller is PersonsController)
            {
                PersonsController personsController = (PersonsController)context.Controller;

                if (!personsController.ModelState.IsValid)
                {
                    List<CountryResponse> countries = await _countriesGetterService.GetAllCountries();
                    personsController.ViewBag.Countries = countries.Select(temp => new SelectListItem()
                    {
                        Text = temp.CountryName,
                        Value = temp.CountryID.ToString()
                    });
                    personsController.ViewBag.Errors = personsController.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                    var personRequest = context.ActionArguments["personRequest"];
                    context.Result = personsController.View(personRequest); //short-circuits (or skips) the subsequent action filters and methods
                }
                else
                {
                    await next();
                }
            }
            else
            {
                await next();
            }
            //after logic
        }
    }
}
