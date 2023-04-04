using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class that is used as a return type for most methods of PersonsService
    /// </summary>
    public class PersonResponse
    {
        public Guid PersonID { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? CountryName { get; set; } 
        public string? Address { get; set; }
        public bool ReceiveNewsletter { get; set; }
        public double? Age { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(PersonResponse)) return false;

            PersonResponse person = (PersonResponse)obj;

            return this.PersonID == person.PersonID && this.PersonName == person.PersonName && this.Email == person.Email &&
                this.DateOfBirth == person.DateOfBirth && this.Gender == person.Gender && this.CountryID == person.CountryID &&
                this.Address == person.Address && this.ReceiveNewsletter == person.ReceiveNewsletter;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"Person ID: {PersonID}; Person Name: {PersonName}, Email: {Email}, " +
                $"DoB: {DateOfBirth?.ToString("dd MM yyyy")}, Gender {Gender}, " +
                $"Country ID: {CountryID}, Country Name: {CountryName}, Address: {Address}, " +
                $"Receive newsletter: {ReceiveNewsletter}";
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonID = PersonID,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender, true),
                CountryID = CountryID,
                Address = Address,
                ReceiveNewsletter = ReceiveNewsletter,
            };
        }
    }

    public static class PersonExtensions
    {
        /// <summary>
        /// An extension method to convert an object of Person class into an object of PersonResponse class
        /// </summary>
        /// <param name="person">The Person object to convert</param>
        /// <returns>Returns the converted PersonResponse object</returns>
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                CountryID = person.CountryID,
                Address = person.Address,
                ReceiveNewsletter = person.ReceiveNewsletter,
                Age = (person.DateOfBirth != null)? 
                    Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null,
                CountryName = person.Country?.CountryName,
            };
        }
    }
}
