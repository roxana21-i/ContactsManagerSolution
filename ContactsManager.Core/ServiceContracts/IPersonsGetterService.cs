using System;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Business logic for manipulating Person entity
    /// </summary>
    public interface IPersonsGetterService
    {
        /// <summary>
        /// Returns all persons
        /// </summary>
        /// <returns>Returns a list of objects of Personresponse type</returns>
        Task<List<PersonResponse>> GetAllPersons();

        /// <summary>
        /// Returns the matching person based on PersonID
        /// </summary>
        /// <param name="personID">PersonID to search</param>
        /// <returns>Returns matching person as PersonResponse</returns>
        Task<PersonResponse?> GetPersonByPersonID(Guid? personID);

        /// <summary>
        /// Returns matching persons based on search criteria
        /// </summary>
        /// <param name="searchBy">Can be any parameter of the Person entity</param>
        /// <param name="searchString">Actual string value to search </param>
        /// <returns>Returns matching persons based on search criteria</returns>
        Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);

        /// <summary>
        /// Returns the persons as CSV
        /// </summary>
        /// <returns>Returns the memory stream with CSV values</returns>
        Task<MemoryStream> GetPersonsCSV();

        /// <summary>
        /// Returns persons as Excel
        /// </summary>
        /// <returns>Returns the memory stream with Excel values</returns>
        Task<MemoryStream> GetPersonsExcel();
    }
}
