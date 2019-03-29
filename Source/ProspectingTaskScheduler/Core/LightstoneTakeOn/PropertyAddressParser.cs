using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core.LightstoneTakeOn
{
    public class PropertyAddressParser
    {
        public static string GetAddress(DataSet owner, out string streetOrUnitNo)
        {
            if (owner.Tables["Properties"].Rows == null || owner.Tables["Properties"].Rows.Count == 0)
            {
                streetOrUnitNo = "n/a";
                return null;
            }
            DataRow dr = owner.Tables["Properties"].Rows[0];
            string property_type = dr["property_type"].ToString();
            switch (property_type)
            {
                case "SS":
                    dr["property_type"] = "Sectional Scheme";
                    return GetAddressForSS(dr, out streetOrUnitNo);
                case "FH":
                    dr["property_type"] = "Freehold";
                    return GetAddressForFH(dr, out streetOrUnitNo);
                case "FRM":
                    dr["property_type"] = "Farm";
                    return GetAddressForFarm(dr, out streetOrUnitNo);
                default:
                    dr["property_type"] = "Other";
                    return GetAddressForOther(dr, out streetOrUnitNo);
            }
        }

        private static string GetAddressForFH(DataRow dr, out string streetNo)
        {
            var fullAddress = "";
            streetNo = "n/a";
            string STREET_NAME = dr["STREET_NAME"].ToString().Trim();
            if (!string.IsNullOrEmpty(STREET_NAME))
            {
                fullAddress += STREET_NAME;
                string streetType = dr["STREET_TYPE"].ToString();
                if (!string.IsNullOrEmpty(streetType))
                {
                    fullAddress += " " + streetType;
                }
                else
                {
                    fullAddress += " STREET";
                }
                string SUBURB = dr["SUBURB"].ToString().Trim();
                if (!string.IsNullOrEmpty(SUBURB))
                {
                    fullAddress += ", " + SUBURB;
                    string PO_CODE = dr["PO_CODE"].ToString().Trim();
                    if (!string.IsNullOrEmpty(PO_CODE))
                    {
                        fullAddress += ", " + PO_CODE;
                    }
                }
            }

            string testStreetNo = dr["STREET_NUMBER"].ToString().Trim();
            if (!string.IsNullOrEmpty(testStreetNo))
            {
                streetNo = testStreetNo;
            }
            return fullAddress;
        }

        private static string GetAddressForSS(DataRow dr, out string unitNo)
        {
            unitNo = "n/a";
            string fullAddress = "";
            string SECTIONAL_TITLE = dr["SECTIONAL_TITLE"].ToString().Trim();
            if (!string.IsNullOrEmpty(SECTIONAL_TITLE))
            {
                fullAddress += SECTIONAL_TITLE;
                string STREET_NAME = dr["STREET_NAME"].ToString().Trim();
                if (!string.IsNullOrEmpty(STREET_NAME))
                {
                    fullAddress += ", " + STREET_NAME;
                    string streetType = dr["STREET_TYPE"].ToString();
                    if (!string.IsNullOrEmpty(streetType))
                    {
                        fullAddress += " " + streetType;
                    }
                    else
                    {
                        fullAddress += " STREET";
                    }

                    string SUBURB = dr["SUBURB"].ToString().Trim();
                    if (!string.IsNullOrEmpty(SUBURB))
                    {
                        fullAddress += ", " + SUBURB;
                        string PO_CODE = dr["PO_CODE"].ToString().Trim();
                        if (!string.IsNullOrEmpty(PO_CODE))
                        {
                            fullAddress += ", " + PO_CODE;
                        }
                    }
                }
            }

            string testUnitNo = dr["UNIT"].ToString().Trim();
            if (!string.IsNullOrEmpty(testUnitNo))
            {
                unitNo = testUnitNo;
            }
            
            return fullAddress;
        }

        private static string GetAddressForFarm(DataRow dr, out string streetNo)
        {
            string fullAddress = "";
            streetNo = "n/a";
            string FARMNAME = dr["FARMNAME"].ToString().Trim();
            if (!string.IsNullOrEmpty(FARMNAME))
            {
                fullAddress += " Farm: " + FARMNAME;

                string STREET_NAME = dr["STREET_NAME"].ToString().Trim();
                if (!string.IsNullOrEmpty(STREET_NAME))
                {
                    fullAddress += ", " + STREET_NAME;
                    string streetType = dr["STREET_TYPE"].ToString();
                    if (!string.IsNullOrEmpty(streetType))
                    {
                        fullAddress += " " + streetType;
                    }
                    else
                    {
                        fullAddress += " STREET";
                    }
                    string SUBURB = dr["SUBURB"].ToString().Trim();
                    if (!string.IsNullOrEmpty(SUBURB))
                    {
                        fullAddress += ", " + SUBURB;
                        string PO_CODE = dr["PO_CODE"].ToString().Trim();
                        if (!string.IsNullOrEmpty(PO_CODE))
                        {
                            fullAddress += ", " + PO_CODE;
                        }
                    }
                }
            }

            string testStreetNo = dr["STREET_NUMBER"].ToString().Trim();
            if (!string.IsNullOrEmpty(testStreetNo))
            {
                streetNo = testStreetNo;
            }

            return fullAddress;
        }

        private static string GetAddressForOther(DataRow dr, out string no)
        {
            string testStreetNo = dr["STREET_NUMBER"].ToString().Trim();
            if (!string.IsNullOrEmpty(testStreetNo))
            {
                no = testStreetNo;
            } else
            {
                no = "n/a";
            }
                
            return "n/a";
        }

        public static int?[] GetErfAndPortion(string erfKey)
        {
            var result = new int?[2] { null, null };
            try
            {
                if (string.IsNullOrEmpty(erfKey))
                    return result;

                var parts = erfKey.Split(new[] { '~' });
                if (parts.Length != 3)
                    return result;

                int erf = int.Parse(parts[1]);
                int portion = int.Parse(parts[2].TrimStart(new[] { '0' }));
                result[0] = erf;
                result[1] = portion;

                return result;
            }
            catch
            {
                return result;
            }
        }
    }
}