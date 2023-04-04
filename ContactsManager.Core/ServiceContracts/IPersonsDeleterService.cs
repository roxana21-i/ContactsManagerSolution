using System;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Business logic for manipulating Person entity
    /// </summary>
    public interface IPersonsDeleterService
    {
        /// <summary>
        /// Delete the Person matching the given PersonID
        /// </summary>
        /// <param name="personID">PersonID to match</param>
        /// <returns>Returns true if person was deleted succesfully, false otherwise</returns>
        Task<bool> DeletePerson(Guid? personID);
    }
}
