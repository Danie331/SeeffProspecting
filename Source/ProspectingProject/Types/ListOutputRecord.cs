using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject
{
    public class ListOutputRecord
    {
        [ColumnMapping("[Title]")]
        public string Title { get; set; }
        [ColumnMapping("[First Name]")]
        public string Firstname { get; set; }
        [ColumnMapping("[Surname]")]
        public string Surname { get; set; }
        [ColumnMapping("[ID number]")]
        public string IdNumber { get; set; }
        [ColumnMapping("[Email Address]")]
        public string EmailAddress { get; set; }
        [ColumnMapping("[Home Landline]")]
        public string HomeLandline { get; set; }
        [ColumnMapping("[Work Landline]")]
        public string WorkLandline { get; set; }
        [ColumnMapping("[Cellphone]")]
        public string Cellphone { get; set; }
        [ColumnMapping("[Property Address]")]
        public string PropertyAddress { get; set; }
        public bool PopiOptOut { get; set; }
        public bool EmailOptOut { get; set; }
        public bool SmsOptOut { get; set; }
        public bool DoNotContact { get; set; }
    }

    class ColumnMappingAttribute: Attribute
    {
        private string _columnName;

        public string ColumnName
        {
            get { return _columnName; }
        }
        public ColumnMappingAttribute(string columnName)
        {
            _columnName = columnName;
        }
    }
}