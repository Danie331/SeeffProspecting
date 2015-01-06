using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class ProspectingContactCompany : IProspectingContactEntity
    {
        public ProspectingContactCompany()
        {
            CompanyContacts = new List<ProspectingContactPerson>();
            ProspectingProperties = new List<ProspectingProperty>();
        }

        private ProspectingProject.ContactEntityType entityType;

        public int? ContactCompanyId { get; set; }
        public string CKNumber { get; set; }
        public string CompanyName { get; set; }
        public string CompanyType { get; set; }
        public List<ProspectingContactPerson> CompanyContacts { get; set; }
        public List<ProspectingProperty> ProspectingProperties { get; set; }

        public ContactEntityType ContactEntityType
        {
            get { return entityType; }
            set { entityType = value; }
        }

        public bool IsSameEntity(string idOrCkNo)
        {
            return string.Equals(CKNumber, idOrCkNo);
        }


        public bool IsSameEntity(IProspectingContactEntity entity)
        {
            ProspectingContactCompany cc = entity as ProspectingContactCompany;
            return cc != null ? string.Equals(this.CKNumber, cc.CKNumber) : false;
        }

    }
}