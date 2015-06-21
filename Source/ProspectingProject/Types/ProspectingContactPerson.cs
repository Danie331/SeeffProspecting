using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    // Update DB diagram + S
    public class ProspectingContactPerson : IProspectingContactEntity
    {
        public ProspectingContactPerson()
        {
            PersonPropertyRelationships = new List<KeyValuePair<int, int>>();
        }

        private ContactEntityType entityType;

        public int? ContactPersonId { get; set; }
        public string IdNumber { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Company { get; set; }
        public int? Title { get; set; }
        public string Gender { get; set; }
        public List<KeyValuePair<int, int>> PersonPropertyRelationships { get; set; }
        //public int? PersonPropertyRelationshipType { get; set; }
        public int? PersonCompanyRelationshipType { get; set; } // TODO: same as for PersonPropertyRelationshipType 
        public string Comments { get; set; }
        public bool IsPOPIrestricted { get; set; }
        public int? ContactCompanyId { get; set; }

        // New fields from Dracore
        public string DeceasedStatus { get; set; }
        public string AgeGroup { get; set; }
        public string Location { get; set; }
        public string MaritalStatus { get; set; }
        public string HomeOwnership { get; set; }
        public string Directorship { get; set; }
        public string PhysicalAddress { get; set; }
        public string Employer { get; set; }
        public string Occupation { get; set; }
        public string BureauAdverseIndicator { get; set; }
        public string Citizenship { get; set; }

        public bool EmailOptout { get; set; }
        public bool SMSOptout { get; set; }

        public int? TargetLightstonePropertyIdForComms { get; set; }

        public IEnumerable<ProspectingContactDetail> PhoneNumbers { get; set; }
        public IEnumerable<ProspectingContactDetail> EmailAddresses { get; set; }

        public List<ProspectingProperty> PropertiesOwned { get; set; }

        public string Fullname
        {
            get { return Firstname + " " + Surname; }
        }

        public ContactEntityType ContactEntityType
        {
            get { return entityType; }
            set { entityType = value; }
        }


        public bool IsSameEntity(string idOrCkNo)
        {
            return string.Equals(IdNumber, idOrCkNo);
        }


        public bool IsSameEntity(IProspectingContactEntity entity)
        {
            ProspectingContactPerson cp = entity as ProspectingContactPerson;
            return cp != null ? string.Equals(this.IdNumber, cp.IdNumber) : false;
        }

        public bool ContactIsCompromised { get; set; } // special

        public string TargetContactEmailAddress { get; set; }
        public string TargetContactCellphoneNumber { get; set; }
    }
}