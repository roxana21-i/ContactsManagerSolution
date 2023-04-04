using AutoFixture;
using Castle.Core.Logging;
using CRUD_Application.Controllers;
using Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CRUD_Tests
{
    public class PersonsControllerTest
    {
        private readonly IPersonsGetterService _personsGetterService;
		private readonly IPersonsAdderService _personsAdderService;
		private readonly IPersonsUpdaterService _personsUpdaterService;
		private readonly IPersonsDeleterService _personsDeleterService;
		private readonly IPersonsSorterService _personsSorterService;
		private readonly Mock<IPersonsGetterService> _personsGetterServiceMock;
        private readonly Mock<IPersonsAdderService> _personsAdderServiceMock;
        private readonly Mock<IPersonsUpdaterService> _personsUpdaterServiceMock;
        private readonly Mock<IPersonsDeleterService> _personsDeleterServiceMock;
        private readonly Mock<IPersonsSorterService> _personsSorterServiceMock;

        private readonly ICountriesGetterService _countriesGetterService;
        private readonly Mock<ICountriesGetterService> _countriesGetterServiceMock;

        private readonly IFixture _fixture;

        public PersonsControllerTest() 
        {
            _fixture = new Fixture();

            _countriesGetterServiceMock = new Mock<ICountriesGetterService>();
            _personsGetterServiceMock = new Mock<IPersonsGetterService>();
            _personsAdderServiceMock = new Mock<IPersonsAdderService>();
            _personsUpdaterServiceMock = new Mock<IPersonsUpdaterService>();
            _personsDeleterServiceMock = new Mock<IPersonsDeleterService>();
            _personsSorterServiceMock = new Mock<IPersonsSorterService>();

            _countriesGetterService = _countriesGetterServiceMock.Object;
            _personsGetterService = _personsGetterServiceMock.Object;
            _personsAdderService = _personsAdderServiceMock.Object;
            _personsUpdaterService = _personsUpdaterServiceMock.Object;
            _personsDeleterService = _personsDeleterServiceMock.Object;
            _personsSorterService = _personsSorterServiceMock.Object;
        }

        #region Index
        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            //Arrange
            List<PersonResponse> personResponseList = _fixture.Create<List<PersonResponse>>();

            var loggerMock = new Mock<ILogger<PersonsController>>();

            PersonsController personsController = new PersonsController(
                _personsGetterService, _personsAdderService, _personsUpdaterService,
                _personsDeleterService, _personsSorterService, _countriesGetterService, loggerMock.Object);

            _personsGetterServiceMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(personResponseList);
            _personsSorterServiceMock.Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), 
                It.IsAny<string>(), It.IsAny<SortOrderOptions>()))
                .ReturnsAsync(personResponseList);

            //Act
            IActionResult result = await personsController.Index(_fixture.Create<string>(),
                _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<SortOrderOptions>());

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
            viewResult.ViewData.Model.Should().BeEquivalentTo(personResponseList);
        }
        #endregion

        #region Create
        //[Fact] -> became obsolete since Model Validation functionality was shifted to the filter
        //public async void Create_IfModelErrors_ToReturnCreateView()
        //{
        //    //Arrange
        //    PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
        //        .Create();

        //    PersonResponse personResponse = _fixture.Create<PersonResponse>();

        //    List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

        //    _countriesServiceMock.Setup(temp => temp.GetAllCountries())
        //        .ReturnsAsync(countries);

        //    _personServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
        //        .ReturnsAsync(personResponse);

        //    var loggerMock = new Mock<ILogger<PersonsController>>();

        //    PersonsController personsController = new PersonsController(_personService, _countriesService, loggerMock.Object);

        //    //Act
        //    personsController.ModelState.AddModelError("PersonName", "Person name cannot be blank");

        //    IActionResult result = await personsController.Create(personAddRequest);

        //    //Assert
        //    ViewResult viewResult = Assert.IsType<ViewResult>(result);

        //    viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();
        //    viewResult.ViewData.Model.Should().Be(personAddRequest);
        //}

        [Fact]
        public async void Create_IfNoModelErrors_ToReturnIndexView()
        {
            //Arrange
            PersonAddRequest personAddRequest = _fixture.Create<PersonAddRequest>();

            PersonResponse personResponse = _fixture.Create<PersonResponse>();

            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _countriesGetterServiceMock.Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(countries);

            _personsAdderServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(personResponse);

            var loggerMock = new Mock<ILogger<PersonsController>>();

			PersonsController personsController = new PersonsController(
				_personsGetterService, _personsAdderService, _personsUpdaterService,
				_personsDeleterService, _personsSorterService, _countriesGetterService, loggerMock.Object);

			//Act
			IActionResult result = await personsController.Create(personAddRequest);

            //Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            redirectResult.ActionName.Should().Be("Index");
        }
        #endregion
    }
}
