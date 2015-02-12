using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace ProspectingProject
{
    public class TracepsTestService : ITracepsService
    {
        public XDocument GetResponseXML(string idNumber)
        {
            string test = @"<traceps>
<parameters>
<param name=""lastUpdate"">2012-05-25</param>
<param name=""code"">11222122</param>
<param name=""type"">idnLookup</param>
</parameters>
<response>
<category name=""person"" version=""1"">
<field name=""id_number"">8501275158086</field>
<field name=""surname"">van der merwe</field>
<field name=""name1"">danil</field>
<field name=""name2"">dendy</field>
<field name=""date_of_birth"">1985-01-27</field>
<field name=""age"">30</field>
<field name=""citizenship"">south african</field>
<field name=""gender"">male</field>
<field name=""pop_group"">other</field>
<field name=""age_group"">Early Thirties</field>
<field name=""marital_status""/>
<field name=""deceased_status"">alive</field>
<field name=""deceased_date""/>
<field name=""deceased_cause""/>
<field name=""location""/>
<field name=""contactability"">gold</field>
</category>
<category name=""contact"" version=""1"">
<row>
<field name=""phone"">0218528160</field>
<field name=""type"">home</field>
<field name=""date"">2011-09-13</field>
</row>
<row>
<field name=""phone"">0217994230</field>
<field name=""type"">work</field>
<field name=""date"">2011-09-13</field>
</row>
<row>
<field name=""phone"">0214607419</field>
<field name=""type"">work</field>
<field name=""date"">2013-05-05</field>
</row>
<row>
<field name=""phone"">0112594700</field>
<field name=""type"">work</field>
<field name=""date"">2013-05-02</field>
</row>
<row>
<field name=""phone"">0848009229</field>
<field name=""type"">cell</field>
<field name=""date"">2011-12-09</field>
</row>
<row>
<field name=""phone"">0769561578</field>
<field name=""type"">cell</field>
<field name=""date"">2013-05-02</field>
</row>
<row>
<field name=""phone"">0724707471</field>
<field name=""type"">cell</field>
<field name=""date"">2014-01-04</field>
</row>
</category>
<category name=""physical_address"" version=""2"">
<row>
<field name=""complex""/>
<field name=""address"">43 buitenkant street</field>
<field name=""township""/>
<field name=""city"">cape town</field>
<field name=""postal_code"">8001</field>
<field name=""province"">western cape</field>
<field name=""date"">2013-05-05</field>
</row>
<row>
<field name=""complex""/>
<field name=""address"">c o 23 mcleod street</field>
<field name=""township""/>
<field name=""city"">plaas firlands</field>
<field name=""postal_code"">7130</field>
<field name=""province"">western cape</field>
<field name=""date"">2013-05-05</field>
</row>
<row>
<field name=""complex"">23 mc leod street</field>
<field name=""address"">somerset west</field>
<field name=""township""/>
<field name=""city"">plaas firlands</field>
<field name=""postal_code"">7130</field>
<field name=""province"">western cape</field>
<field name=""date"">2011-05-16</field>
</row>
</category>
<category name=""postal_address"" version=""2"">
<row>
<field name=""line1"">po box 825</field>
<field name=""line2"">somerset west</field>
<field name=""township""/>
<field name=""city""/>
<field name=""postal_code"">7129</field>
<field name=""province"">western cape</field>
<field name=""date"">2006-09-24</field>
</row>
</category>
<category name=""directorship"" version=""2""/>
<category name=""occupation"" version=""1"">
<row>
<field name=""occupation""/>
<field name=""employer"">kaplan</field>
<field name=""date"">2011-05-17</field>
</row>
<row>
<field name=""occupation""/>
<field name=""employer"">truworths</field>
<field name=""date"">2013-05-02</field>
</row>
</category>
<category name=""ownership"" version=""2""/>
<category name=""xtrace"" version=""2"">
<field name=""description"">extracted</field>
</category>
</response>
</traceps>";

            return XDocument.Parse(test);
        }
    }
}