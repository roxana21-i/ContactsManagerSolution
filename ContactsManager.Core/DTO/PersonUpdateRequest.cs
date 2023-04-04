using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class that contains the Person details to update
    /// </summary>
    public class PersonUpdateRequest
    {
        [Required(ErrorMessage = "Person ID cannot be blank")]
        public Guid PersonID { get; set; }
        [Required(ErrorMessage = "Person name cannot be blank")]
        public string? PersonName { get; set; }
        [Required(ErrorMessage = "Email cannot be blank")]
        [EmailAddress(ErrorMessage = "Email address is not valid")]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsletter { get; set; }


        /// <summary>
        /// Converts the current PersonUpdateRequest object into a Person object
        /// </summary>
        /// <returns>A Person object with the same data</returns>
        public Person ToPerson()
        {
            return new Person()
            {
                PersonID = PersonID,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender.ToString(),
                CountryID = CountryID,
                Address = Address,
                ReceiveNewsletter = ReceiveNewsletter
            };
        }
    }
}
