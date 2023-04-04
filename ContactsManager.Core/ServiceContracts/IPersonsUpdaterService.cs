using System;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Business logic for manipulating Person entity
    /// </summary>
    public interface IPersonsUpdaterService
    {
        /// <summary>
        /// Updates the specified person details based on the given PersonID
        /// </summary>
        /// <param name="personUpdateRequest">HTe details to update, including the PersonID to match the person</param>
        /// <returns>Returns the PersonResponse with the updated data</returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);
    }
}
