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
    public class PersonsDeleterService : IPersonsDeleterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsDeleterService(IPersonsRepository personsRepository,
            ILogger<PersonsGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            if (personID == null)
            {
                throw new AbandonedMutexException(nameof(personID));
            }

            Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);

            if (person == null)
            {
                return false;
            }

            await _personsRepository.DeletePersonByPersonID(person.PersonID);

            return true;
        }
    }
}
