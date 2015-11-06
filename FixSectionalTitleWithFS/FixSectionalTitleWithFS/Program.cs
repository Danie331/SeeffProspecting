using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixSectionalTitleWithFS
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new seeff_prospectingEntities())
            {
                Console.WriteLine("Enter the ERF no.");
                int erf = int.Parse(Console.ReadLine());
                Console.WriteLine("------- PROCESSING --------");
                var allRelatedProperties = (from pp in db.prospecting_property
                                           where pp.erf_no == erf
                                           select pp).ToList();
                bool changeMade = false;
                if (allRelatedProperties.Count > 1)
                {
                    var ssGroup = allRelatedProperties.GroupBy(pp => pp.ss_unique_identifier);
                    if (ssGroup.Count() == 2)
                    {
                        var fsCandidate = ssGroup.FirstOrDefault(gr => gr.All(pp => pp.ss_unique_identifier == null));
                        if (fsCandidate != null && fsCandidate.Count() == 1) 
                        {
                            var group2 = ssGroup.FirstOrDefault(gr => gr.Any(pp => pp.ss_unique_identifier != null)).First();
                            if (fsCandidate.First().property_address == group2.property_address)
                            {
                                string ssName = ssGroup.FirstOrDefault(gr => gr.Any(pp => pp.ss_unique_identifier != null)).First().ss_name;
                                string ssUniqueIdentifier = group2.ss_unique_identifier;
                                var targetPP = fsCandidate.First();
                                targetPP.ss_name = ssName;
                                targetPP.ss_fh = "FS";
                                targetPP.ss_unique_identifier = ssUniqueIdentifier;
                                db.SaveChanges();
                                changeMade = true;
                            }
                        }
                    }
                }
                if (changeMade)
                {
                    Console.WriteLine("Record is fixed.");
                }
                else
                {
                    Console.WriteLine("No suitable record found.");
                }
                Console.ReadKey();
            }
        }
    }
}
