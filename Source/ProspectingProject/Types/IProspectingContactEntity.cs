using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProspectingProject
{
    public interface IProspectingContactEntity
    {
        ContactEntityType ContactEntityType { get; set; }
        bool IsSameEntity(string idOrCkNo);
        bool IsSameEntity(IProspectingContactEntity entity);
    }

    public enum ContactEntityType { NaturalPerson = 1, JuristicEntity = 2 };
}
