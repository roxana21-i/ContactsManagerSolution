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
    public class PersonsGetterService : IPersonsGetterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsGetterService(IPersonsRepository personsRepository,
            ILogger<PersonsGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public virtual async Task<List<PersonResponse>> GetAllPersons()
        {
            _logger.LogInformation("GetAllPersons of PersonsService");

            var persons = await _personsRepository.GetAllPersons();

            //SELECT * FROM Persons
            return persons.Select(person => person.ToPersonResponse()).ToList();
            //return _db.SP_GetAllPersons().Select(person => person.ToPersonResponse()).ToList();
        }

        public virtual async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
            {
                return null;
            }

            Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);

            if (person == null)
            {
                return null;
            }

            return person.ToPersonResponse();
        }

        public virtual async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            _logger.LogInformation("GetFilteredPersons of PersonsService");

            List<Person> persons;

            using (Operation.Time("Time for GetFilteredPersons from Database"))
            {
                persons = searchBy switch
                {

                    nameof(PersonResponse.PersonName) =>
                        await _personsRepository.GetFilteredPersons(temp => (
                        temp.PersonName.Contains(searchString))),

                    nameof(PersonResponse.Email) =>
                       await _personsRepository.GetFilteredPersons(temp => (!string.IsNullOrEmpty(temp.Email) ?
                        temp.Email.Contains(searchString) : true)),

                    nameof(PersonResponse.DateOfBirth) =>
                        await _personsRepository.GetFilteredPersons(temp => (temp.DateOfBirth != null) ?
                            temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString,
                            StringComparison.OrdinalIgnoreCase) : true),

                    nameof(PersonResponse.Gender) =>
                        await _personsRepository.GetFilteredPersons(temp => (!string.IsNullOrEmpty(temp.Gender) ?
                        temp.Gender.Equals(searchString) : true)),

                    nameof(PersonResponse.CountryID) =>
                        await _personsRepository.GetFilteredPersons(temp => (
                        temp.Country.CountryName.Contains(searchString))),

                    nameof(PersonResponse.Address) =>
                        await _personsRepository.GetFilteredPersons(temp => (!string.IsNullOrEmpty(temp.Address) ?
                        temp.Address.Contains(searchString) : true)),

                    _ => await _personsRepository.GetAllPersons()
                };
            } //end using block

            _diagnosticContext.Set("Persons", persons);

            return persons.Select(temp => temp.ToPersonResponse()).ToList();
        }

        public virtual async Task<MemoryStream> GetPersonsCSV()
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(memoryStream);

            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
            CsvWriter csvWriter = new CsvWriter(writer, csvConfiguration);

            csvWriter.WriteField(nameof(PersonResponse.PersonName));
            csvWriter.WriteField(nameof(PersonResponse.Email));
            csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
            csvWriter.WriteField(nameof(PersonResponse.Age));
            csvWriter.WriteField(nameof(PersonResponse.Gender));
            csvWriter.WriteField(nameof(PersonResponse.CountryName));
            csvWriter.WriteField(nameof(PersonResponse.Address));
            csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsletter));
            csvWriter.NextRecord();

            List<PersonResponse> persons = await GetAllPersons();

            foreach (PersonResponse person in persons)
            {
                csvWriter.WriteField(person.PersonName);
                csvWriter.WriteField(person.Email);
                if (person.DateOfBirth.HasValue)
                    csvWriter.WriteField(person.DateOfBirth.Value.ToString("yyyy-MM-dd"));
                csvWriter.WriteField(person.Age);
                csvWriter.WriteField(person.Gender);
                csvWriter.WriteField(person.CountryName);
                csvWriter.WriteField(person.Address);
                csvWriter.WriteField(person.ReceiveNewsletter);

                csvWriter.NextRecord();
                csvWriter.Flush();
            }

            memoryStream.Position = 0; //we have to go back to the beginning in order to write a file
            return memoryStream;
        }

        public virtual async Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
                worksheet.Cells["A1"].Value = "Person Name";
                worksheet.Cells["B1"].Value = "Email";
                worksheet.Cells["C1"].Value = "Date of Birth";
                worksheet.Cells["D1"].Value = "Age";
                worksheet.Cells["E1"].Value = "Gender";
                worksheet.Cells["F1"].Value = "Country";
                worksheet.Cells["G1"].Value = "Address";
                worksheet.Cells["H1"].Value = "Receive Newsletter";

                using (ExcelRange headerCells = worksheet.Cells["A1:H1"])
                {
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    headerCells.Style.Font.Bold = true;
                }

                int row = 2;
                List<PersonResponse> persons = await GetAllPersons();

                foreach (PersonResponse person in persons)
                {
                    worksheet.Cells[row, 1].Value = person.PersonName;
                    worksheet.Cells[row, 2].Value = person.Email;
                    if (person.DateOfBirth.HasValue)
                        worksheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 4].Value = person.Age;
                    worksheet.Cells[row, 5].Value = person.Gender;
                    worksheet.Cells[row, 6].Value = person.CountryName;
                    worksheet.Cells[row, 7].Value = person.Address;
                    worksheet.Cells[row, 8].Value = person.ReceiveNewsletter;

                    row++;
                }

                worksheet.Cells[$"A1:H{row}"].AutoFitColumns();

                await excelPackage.SaveAsync();
            }

            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
