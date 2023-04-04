using AutoFixture;
using Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using RepositoryContracts;
using Serilog;
using Serilog.Extensions.Hosting;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;
using Xunit.Abstractions;


namespace CRUD_Tests
{
    public class PersonServiceTest
    {
        private readonly IPersonsGetterService _personsGetterService;
		private readonly IPersonsAdderService _personsAdderService;
		private readonly IPersonsUpdaterService _personsUpdaterService;
		private readonly IPersonsDeleterService _personsDeleterService;
		private readonly IPersonsSorterService _personsSorterService;
		private readonly Mock<IPersonsRepository> _personsRepositoryMock;
        private readonly IPersonsRepository _personsRepository;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;

        public PersonServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();

            _personsRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personsRepositoryMock.Object;
            var diagnosticContextMock = new Mock<IDiagnosticContext>();
            var loggerMock = new Mock<ILogger<PersonsGetterService>>();

            _personsGetterService = new PersonsGetterService(_personsRepository, loggerMock.Object, diagnosticContextMock.Object);
            _personsAdderService = new PersonsAdderService(_personsRepository, loggerMock.Object, diagnosticContextMock.Object);
			_personsUpdaterService = new PersonsUpdaterService(_personsRepository, loggerMock.Object, diagnosticContextMock.Object);
			_personsDeleterService = new PersonsDeleterService(_personsRepository, loggerMock.Object, diagnosticContextMock.Object);
			_personsSorterService = new PersonsSorterService(_personsRepository, loggerMock.Object, diagnosticContextMock.Object);
			_testOutputHelper = testOutputHelper;
        }

        #region AddPerson
        //When PersonAddRequest is null, throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
        {
            //Arrange
            PersonAddRequest request = null;

            Func<Task> action = async () =>
            {
                await _personsAdderService.AddPerson(request);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        //When PersonName is null, throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPersonName_ToBeArgumentException()
        {
            //Arrange
            PersonAddRequest request = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, null as string)
                .Create();

            Person person = request.ToPerson();

            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            Func<Task> action = async () =>
            {
                await _personsAdderService.AddPerson(request);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        //When we supply proper person details, it should insert the person into the persons list and return an object of
        //PersonResponse, which includes the newly generated PersonID
        [Fact]
        public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
        {
            //Arrange
            PersonAddRequest request = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone@example.com")
                .Create();

            Person person = request.ToPerson();
            PersonResponse personResponseExpected = person.ToPersonResponse();

            //If we supply any argument value, it should return the same value
            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            //Act
            PersonResponse responseFromAdd = await _personsAdderService.AddPerson(request);
            personResponseExpected.PersonID = responseFromAdd.PersonID;

            responseFromAdd.PersonID.Should().NotBe(Guid.Empty);
            responseFromAdd.Should().Be(personResponseExpected);
        }
        #endregion

        #region GetPersonByPersonID
        //If PersonID is null, return null
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
        {
            //Arrange 
            Guid? personID = null;

            //Act
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonID(personID);

            //Assert
            personResponse.Should().BeNull();
        }

        //If you supply a valid PersonID, return a valid PersonResponse object
        [Fact]
        public async Task GetPersonByPersonID_ProperPersonID_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "mike@gmail.com")
                .With(temp => temp.Country, null as Country)
                .Create();
            PersonResponse personResponseExpected = person.ToPersonResponse();

            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            PersonResponse? personResponseFromGet = await _personsGetterService.GetPersonByPersonID(person.PersonID);

            personResponseFromGet.Should().Be(personResponseExpected);
        }
        #endregion

        #region GetAllPersons
        //Return an empty list by default
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            var persons = new List<Person>();

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(persons);

            List<PersonResponse> personsFromGet = await _personsGetterService.GetAllPersons();

            personsFromGet.Should().BeEmpty();
        }

        //Should return the same persons that were previously added to the list
        [Fact]
        public async Task GetAllPersons_AddAndCheckPersons_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone@example.eu")
                .With(temp => temp.Country, null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone2@example.eu")
                .With(temp => temp.Country, null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone3@example.eu")
                .With(temp => temp.Country, null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone4@example.eu")
                .With(temp => temp.Country, null as Country)
                .Create(),
            };

            List<PersonResponse> personResponseListExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _testOutputHelper.WriteLine("Expected:");
            foreach(PersonResponse personResponse in personResponseListExpected)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(persons);

            //Act
            List<PersonResponse> personResponseListFromGet = await _personsGetterService.GetAllPersons();

            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse personResponse in personResponseListFromGet)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            personResponseListFromGet.Should().BeEquivalentTo(personResponseListExpected);
        }
        #endregion

        #region GetFilteredPersons
        //If the search text is empty and search is PersonName, it should return all persons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone@example.eu")
                .With(temp => temp.Country, null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone2@example.eu")
                .With(temp => temp.Country, null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone3@example.eu")
                .With(temp => temp.Country, null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone4@example.eu")
                .With(temp => temp.Country, null as Country)
                .Create(),
            };

            List<PersonResponse> personResponseListExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _testOutputHelper.WriteLine("All persons:");
            foreach (PersonResponse personResponse in personResponseListExpected)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);

            //Act
            List<PersonResponse> personResponseListFromSearch = await _personsGetterService.GetFilteredPersons(nameof(Person.PersonName), "");

            _testOutputHelper.WriteLine("Persons matching with empty string:");
            foreach (PersonResponse personResponse in personResponseListFromSearch)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Assert
            personResponseListFromSearch.Should().BeEquivalentTo(personResponseListExpected);
        }

        //If searched based on PersonName and some search string, should return matching Person
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.PersonName, "Marius")
                .With(temp => temp.Email, "someone@example.eu")
                .With(temp => temp.Country, null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.PersonName, "Mauritius")
                .With(temp => temp.Email, "someone2@example.eu")
                .With(temp => temp.Country, null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.PersonName, "mary")
                .With(temp => temp.Email, "someone3@example.eu")
                .With(temp => temp.Country, null as Country)
                .Create(),
            };

            List<PersonResponse> personResponseListExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();
            string checkString = "ma";

            _testOutputHelper.WriteLine("All persons:");
            foreach (PersonResponse personResponse in personResponseListExpected)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);

            //Act
            List<PersonResponse> personResponseListFromSearch = 
                await _personsGetterService.GetFilteredPersons(nameof(Person.PersonName), checkString);

            _testOutputHelper.WriteLine($"Matching with {checkString}:");
            foreach (PersonResponse personResponse in personResponseListFromSearch)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Assert
            //foreach (PersonResponse personResponseAdd in personResponseListFromAdd)
            //{
            //    if (personResponseAdd.PersonName != null)
            //    {
            //        if (personResponseAdd.PersonName.Contains(checkString, StringComparison.OrdinalIgnoreCase))
            //        {
            //            Assert.Contains(personResponseAdd, personResponseListFromSearch);
            //        }
            //    }
            //}

            personResponseListFromSearch.Should().OnlyContain(
                temp => temp.PersonName.Contains(checkString, StringComparison.OrdinalIgnoreCase));
            //personResponseListFromSearch.Should().BeEquivalentTo(personResponseListExpected);
        }
        #endregion

        #region GetSoterdPersons
        //When we sort based on PersonName in DESC, it should return a person list in descending order
        [Fact]
        public async Task GetSortedPersons_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone@example.eu")
                .With(temp => temp.Country, null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone2@example.eu")
                .With(temp => temp.Country, null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone3@example.eu")
                .With(temp => temp.Country, null as Country)
                .Create(),
                _fixture.Build<Person>()
                .With(temp => temp.Email, "someone4@example.eu")
                .With(temp => temp.Country, null as Country)
                .Create(),
            };

            List<PersonResponse> personResponseListExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            _testOutputHelper.WriteLine("All persons:");
            foreach (PersonResponse personResponse in personResponseListExpected)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            List<PersonResponse> allPersons = await _personsGetterService.GetAllPersons();

            //Act
            List<PersonResponse> personResponseListFromSort =
                await _personsSorterService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            _testOutputHelper.WriteLine("Sorted list:");
            foreach (PersonResponse personResponse in personResponseListFromSort)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //allPersons = allPersons.OrderByDescending(temp => temp.PersonName).ToList();

            //Assert
            //for (int i = 0; i< allPersons.Count; i++)
            //{
            //    Assert.Equal(allPersons[i], personResponseListFromSort[i]);
            //}

            personResponseListFromSort.Should().BeInDescendingOrder(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase);
        }
        #endregion

        #region UpdatePerson
        //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;

            Func<Task> action = async () =>
            {
                await _personsUpdaterService.UpdatePerson(personUpdateRequest);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        //If PersonID is invalid, throw ArgumentException
        [Fact]
        public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = _fixture.Build<PersonUpdateRequest>().Create();

            //Assert
            Func<Task> action = async () =>
            {
                await _personsUpdaterService.UpdatePerson(personUpdateRequest);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //If PersonName is null, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_NullPersonName_ToBeArgumentException()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.PersonName, null as string)
                .With(temp => temp.Email, "example@google.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, "Male")
                .Create();

            PersonResponse personResponseFromAdd = person.ToPersonResponse();

            PersonUpdateRequest? personUpdateRequest = personResponseFromAdd.ToPersonUpdateRequest();

            //Assert
            Func<Task> action = async () =>
            {
                await _personsUpdaterService.UpdatePerson(personUpdateRequest);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //Add a new person and update name and email
        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdate_ToBeSuccesful()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "example@google.com")
                .With(temp => temp.Gender, "Male")
                .With(temp => temp.Country, null as Country)
                .Create();

            PersonResponse personResponseExpected = person.ToPersonResponse();

            PersonUpdateRequest? personUpdateRequest = personResponseExpected.ToPersonUpdateRequest();

            _personsRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
                .ReturnsAsync(person);
            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            //Act
            PersonResponse personResponseFromUpdate = await _personsUpdaterService.UpdatePerson(personUpdateRequest);

            //Assert
            personResponseFromUpdate.Should().Be(personResponseExpected);
        }
        #endregion

        #region DeletePerson
        //If PersonID is valid, return true
        [Fact]
        public async Task DeletePerson_ValidPersonID_ToBeSuccessful()
        {
            Person person = _fixture.Build<Person>()
                .With(temp => temp.PersonName, null as string)
                .With(temp => temp.Email, "example@google.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, "Male")
                .Create();

            _personsRepositoryMock.Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(true);
            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            bool isDeleted = await _personsDeleterService.DeletePerson(person.PersonID);

            isDeleted.Should().BeTrue();
        }

        //If PersonID is invalid, return false
        [Fact]
        public async Task DeletePerson_InvalidPersonID_ToBeFalse()
        {
            bool isDeleted = await _personsDeleterService.DeletePerson(Guid.NewGuid());

            isDeleted.Should().BeFalse();
        }

        #endregion
    }
}
