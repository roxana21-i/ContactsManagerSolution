using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using RepositoryContracts;
using Moq;
using FluentAssertions;
using AutoFixture;

namespace CRUD_Tests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesAdderService _countriesAdderService;
        private readonly ICountriesGetterService _countriesGetterService;
        //private readonly ICountriesUploaderService _countriesUploaderService;
        private readonly ICountriesRepository _countriesRepository;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;

        private readonly IFixture _fixture;

        public CountriesServiceTest()
        {
            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;

            _countriesAdderService = new CountriesAdderService(_countriesRepository);
            _countriesGetterService = new CountriesGetterService(_countriesRepository);
            //_countriesUploaderService = new CountriesUploaderService(_countriesRepository);

            _fixture = new Fixture();
        }

        #region AddCountry
        //When CountryAddRequest is null, throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry_ToBeArgumentNullException()
        {
            //Arrange
            CountryAddRequest? request = null;

            Func<Task> action = async () =>
            {
                await _countriesAdderService.AddCountry(request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        //When CountryName is null, throw ArgumentException
        [Fact]
        public async Task AddCountry_NullCountryName_ToBeArgumentException()
        {
            //Arrange
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, null as string)
                .Create();

            Country country = request.ToCountry();

            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country);

            Func<Task> action = async () =>
            {
                await _countriesAdderService.AddCountry(request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //When CountryName is duplicate, throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName_ToBeArgumentException()
        {
            //Arrange
            CountryAddRequest? request1 = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, "Japan")
                .Create();

            CountryAddRequest? request2 = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, "Japan")
                .Create();

            Country country1 = request1.ToCountry();
            Country country2 = request2.ToCountry();

            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country1);

            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync(null as Country);

            CountryResponse reponseFromAdd1 = await _countriesAdderService.AddCountry(request1);

            Func<Task> action = async () =>
            {
                _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country1);

                _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
                    .ReturnsAsync(country1);

                await _countriesAdderService.AddCountry(request2);
            };
            //Assert

            await action.Should().ThrowAsync<ArgumentException>();
        }

        //When you supply proper CountryName, insert the Country to the list of Countries
        [Fact]
        public async Task AddCountry_ProperCountryDetails_ToBeSuccessful()
        {
            //Arrange
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
               .With(temp => temp.CountryName, "Japan")
               .Create();

            Country country = request.ToCountry();
            CountryResponse expectedResponse = country.ToCountryResponse();

            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country);

            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync(null as Country);

            //Act
            CountryResponse response = await _countriesAdderService.AddCountry(request);
            expectedResponse.CountryID = response.CountryID;

            response.CountryID.Should().NotBe(Guid.Empty);
            response.Should().Be(expectedResponse);
        }
        #endregion

        #region GetAllCountries
        //The list of countries should be empty by default
        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(new List<Country>());

            //Act
            List<CountryResponse> countryList = await _countriesGetterService.GetAllCountries();

            //Assert
            countryList.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries_ToBeSuccessful()
        {
            //Arrange
            List<Country> countries = new List<Country> {
                 _fixture.Build<Country>()
                .With(temp => temp.CountryName, "Japan")
                .With(temp => temp.Persons, null as List<Person>)
                .Create(),
                 _fixture.Build<Country>()
                .With(temp => temp.CountryName, "USA")
                .With(temp => temp.Persons, null as List<Person>)
                .Create(),
                 _fixture.Build<Country>()
                .With(temp => temp.CountryName, "Romania")
                .With(temp => temp.Persons, null as List<Person>)
                .Create(),
            };

            //Act
            List<CountryResponse> countriesReponseExpected = countries.Select(temp => temp.ToCountryResponse()).ToList();

            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(countries);

            List<CountryResponse> actualCountryResponses = await _countriesGetterService.GetAllCountries();
            //check that every country was properly added to the list

            actualCountryResponses.Should().BeEquivalentTo(countriesReponseExpected);
        }
        #endregion

        #region GetCountryByCountryID
        //If countryID is null, should return null
        [Fact]
        public async Task GetCountryByCountryID_NullCountryID_ToBeNull()
        {
            //Arange
            Guid? guid = null;

            //Act
            CountryResponse? countryResponse = await _countriesGetterService.GetCountryByCountryID(guid);

            //Assert
            countryResponse.Should().BeNull();
        }

        //If you supply a valid countryID, it should supply the matching country as a CountryResponse
        [Fact]
        public async Task GetCountryByCountryID_ValidCountryID_ToBeSuccessful()
        {
            //Arrange
            Country country = _fixture.Build<Country>()
                .With(temp => temp.Persons, null as List<Person>)
                .Create();

            CountryResponse expectedResponse = country.ToCountryResponse();

            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryID(It.IsAny<Guid>()))
                .ReturnsAsync(country);

            //Act
            CountryResponse? countryResponseFromGet = await _countriesGetterService.GetCountryByCountryID(country.CountryID);

            //Assert
            countryResponseFromGet.Should().Be(expectedResponse);
        }
        #endregion
    }
}
