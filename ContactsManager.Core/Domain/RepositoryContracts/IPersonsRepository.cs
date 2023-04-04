using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Person entity
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        /// Adds a Person object to the data store
        /// </summary>
        /// <param name="person">Person object to add</param>
        /// <returns>Returns the person object after adding it to the data store</returns>
        Task<Person> AddPerson(Person person);

        /// <summary>
        /// Gets all persons from the data store
        /// </summary>
        /// <returns>Returns all persons from the data store as list</returns>
        Task<List<Person>> GetAllPersons();

        /// <summary>
        /// Returns a Person object based on the given personID; otherwise null
        /// </summary>
        /// <param name="personID">PersonID to search</param>
        /// <returns>Matching Person object or null</returns>
        Task<Person?> GetPersonByPersonID(Guid personID);

        /// <summary>
        /// Returns all Person objects based on the given expression
        /// </summary>
        /// <param name="predicate">LINQ expression to check</param>
        /// <returns>All Person objects matching with the expression</returns>
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);

        /// <summary>
        /// Delete a Person object based on personID
        /// </summary>
        /// <param name="personID">personID to search</param>
        /// <returns>True if successfully deleted, false otherwise</returns>
        Task<bool> DeletePersonByPersonID(Guid personID);

        /// <summary>
        /// Updates a Person object based on the given personID
        /// </summary>
        /// <param name="person">Object to update</param>
        /// <returns>Updated Person object</returns>
        Task<Person> UpdatePerson(Person person);
    }
}
