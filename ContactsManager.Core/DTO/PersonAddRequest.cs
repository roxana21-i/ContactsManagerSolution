using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Acts as DTO for adding a new person
    /// </summary>
    public class PersonAddRequest
    {
        [Required(ErrorMessage = "Person name cannot be blank")]
        public string? PersonName { get; set; }
        [Required(ErrorMessage = "Email cannot be blank")]
        [EmailAddress(ErrorMessage = "Email address is not valid")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage = "Gender cannot be blank")]
        public GenderOptions? Gender { get; set; }
        [Required(ErrorMessage = "Please select a country")]
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsletter { get; set; }


        /// <summary>
        /// Converts the current PersonAddRequest object into a Person object
        /// </summary>
        /// <returns>A Person object with the same data</returns>
        public Person ToPerson()
        {
            return new Person()
            {
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
