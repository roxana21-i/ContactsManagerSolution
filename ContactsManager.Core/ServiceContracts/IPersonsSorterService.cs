using System;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Business logic for manipulating Person entity
    /// </summary>
    public interface IPersonsSorterService
    {
        /// <summary>
        /// Returns a sorted list of persons based on the sort criteria
        /// </summary>
        /// <param name="allPersons">List of persons to sort</param>
        /// <param name="sortBy">Name of the property based on which we sort the list</param>
        /// <param name="sortOrder">Can be ascending or descending</param>
        /// <returns>Returns a sorted list of persons based on the sort criteria</returns>
        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);
    }
}
