using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sabio.Web.Models.Requests
{
    public class PersonAddRequest
    {
        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string JobTitle { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public int? StateProvinceId { get; set; }
        public string PostalCode { get; set; }
        public int? CountryId { get; set; }
        public List<string> LanguageProficiencyKeys { get; set; }
        public List<int> SkillIds { get; set; }
        public List<int> MilitaryBaseIds { get; set; }
        public string AspNetUserId { get; set; }
        public bool IsVeteran { get; set; }
        public bool IsEmployer { get; set; }
        public bool IsFamilyMember { get; set; }
        public string Description { get; set; }
        public string EmploymentStatus { get; set; }
        public PersonNotificationPreferenceAddRequest[] Preferences { get; set; }
        public bool IsEmployed { get; set; }
        public bool OpCodeEmployAssist { get; set; }
        public int EducationLevelId { get; set; }
    }
}