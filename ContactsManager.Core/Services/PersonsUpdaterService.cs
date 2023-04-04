using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using OfficeOpenXml;
using RepositoryContracts;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogTimings;
using Exceptions;

namespace Services
{
    public class PersonsUpdaterService : IPersonsUpdaterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsUpdaterService(IPersonsRepository personsRepository,
            ILogger<PersonsGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }
 
        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(personUpdateRequest));
            }

            //Validation
            ValidationHelper.ModelValidation(personUpdateRequest);

            //Get matching Person
            Person? updatedPersonData = personUpdateRequest.ToPerson();

            Person? matchingPerson = await _personsRepository.GetPersonByPersonID(updatedPersonData.PersonID);

            if (matchingPerson == null)
            {
                throw new InvalidPersonIDException("Given PersonID doens't exist");
            }

            await _personsRepository.UpdatePerson(updatedPersonData);

            return updatedPersonData.ToPersonResponse();
        }
    }
}
