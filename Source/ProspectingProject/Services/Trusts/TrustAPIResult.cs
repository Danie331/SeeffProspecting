using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProspectingProject.Trusts
{

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class APIResponse
    {

        private APIResponseSearchInformation searchInformationField;

        private APIResponseTrust trustField;

        /// <remarks/>
        public APIResponseSearchInformation SearchInformation
        {
            get
            {
                return this.searchInformationField;
            }
            set
            {
                this.searchInformationField = value;
            }
        }

        /// <remarks/>
        public APIResponseTrust Trust
        {
            get
            {
                return this.trustField;
            }
            set
            {
                this.trustField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class APIResponseSearchInformation
    {

        private string searchUserNameField;

        private string reportDateField;

        private string referenceField;

        private string searchTokenField;

        private object searchTypeDescriptionField;

        private string searchDescriptionField;

        private string callerModuleField;

        private ushort searchIDField;

        private string dataSupplierField;

        private string searchTypeField;

        private object extendedSectionsField;

        private string responseTypeField;

        private string responseObjectTypeField;

        /// <remarks/>
        public string SearchUserName
        {
            get
            {
                return this.searchUserNameField;
            }
            set
            {
                this.searchUserNameField = value;
            }
        }

        /// <remarks/>
        public string ReportDate
        {
            get
            {
                return this.reportDateField;
            }
            set
            {
                this.reportDateField = value;
            }
        }

        /// <remarks/>
        public string Reference
        {
            get
            {
                return this.referenceField;
            }
            set
            {
                this.referenceField = value;
            }
        }

        /// <remarks/>
        public string SearchToken
        {
            get
            {
                return this.searchTokenField;
            }
            set
            {
                this.searchTokenField = value;
            }
        }

        /// <remarks/>
        public object SearchTypeDescription
        {
            get
            {
                return this.searchTypeDescriptionField;
            }
            set
            {
                this.searchTypeDescriptionField = value;
            }
        }

        /// <remarks/>
        public string SearchDescription
        {
            get
            {
                return this.searchDescriptionField;
            }
            set
            {
                this.searchDescriptionField = value;
            }
        }

        /// <remarks/>
        public string CallerModule
        {
            get
            {
                return this.callerModuleField;
            }
            set
            {
                this.callerModuleField = value;
            }
        }

        /// <remarks/>
        public ushort SearchID
        {
            get
            {
                return this.searchIDField;
            }
            set
            {
                this.searchIDField = value;
            }
        }

        /// <remarks/>
        public string DataSupplier
        {
            get
            {
                return this.dataSupplierField;
            }
            set
            {
                this.dataSupplierField = value;
            }
        }

        /// <remarks/>
        public string SearchType
        {
            get
            {
                return this.searchTypeField;
            }
            set
            {
                this.searchTypeField = value;
            }
        }

        /// <remarks/>
        public object ExtendedSections
        {
            get
            {
                return this.extendedSectionsField;
            }
            set
            {
                this.extendedSectionsField = value;
            }
        }

        /// <remarks/>
        public string ResponseType
        {
            get
            {
                return this.responseTypeField;
            }
            set
            {
                this.responseTypeField = value;
            }
        }

        /// <remarks/>
        public string ResponseObjectType
        {
            get
            {
                return this.responseObjectTypeField;
            }
            set
            {
                this.responseObjectTypeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class APIResponseTrust
    {

        private APIResponseTrustTrustInformation trustInformationField;

        private APIResponseTrustTrustees trusteesField;

        private APIResponseTrustCrossDeedsDetail crossDeedsDetailField;

        private object deedsOfficeContractsField;

        private object deedsOfficeBondsField;

        private APIResponseTrustPropertyInformation propertyInformationField;

        private object notarialBondsField;

        private object internalEnquiryHistoryField;

        /// <remarks/>
        public APIResponseTrustTrustInformation TrustInformation
        {
            get
            {
                return this.trustInformationField;
            }
            set
            {
                this.trustInformationField = value;
            }
        }

        /// <remarks/>
        public APIResponseTrustTrustees Trustees
        {
            get
            {
                return this.trusteesField;
            }
            set
            {
                this.trusteesField = value;
            }
        }

        /// <remarks/>
        public APIResponseTrustCrossDeedsDetail CrossDeedsDetail
        {
            get
            {
                return this.crossDeedsDetailField;
            }
            set
            {
                this.crossDeedsDetailField = value;
            }
        }

        /// <remarks/>
        public object DeedsOfficeContracts
        {
            get
            {
                return this.deedsOfficeContractsField;
            }
            set
            {
                this.deedsOfficeContractsField = value;
            }
        }

        /// <remarks/>
        public object DeedsOfficeBonds
        {
            get
            {
                return this.deedsOfficeBondsField;
            }
            set
            {
                this.deedsOfficeBondsField = value;
            }
        }

        /// <remarks/>
        public APIResponseTrustPropertyInformation PropertyInformation
        {
            get
            {
                return this.propertyInformationField;
            }
            set
            {
                this.propertyInformationField = value;
            }
        }

        /// <remarks/>
        public object NotarialBonds
        {
            get
            {
                return this.notarialBondsField;
            }
            set
            {
                this.notarialBondsField = value;
            }
        }

        /// <remarks/>
        public object InternalEnquiryHistory
        {
            get
            {
                return this.internalEnquiryHistoryField;
            }
            set
            {
                this.internalEnquiryHistoryField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class APIResponseTrustTrustInformation
    {

        private object trustIDField;

        private object deedsOfficeIDField;

        private object deedsOfficeNameField;

        private object trustTypeField;

        private object trustTypeShortenedField;

        private object trustNumberField;

        private object trustNameField;

        private object dOTSBarcodeField;

        private object shortPropertyDescriptionField;

        /// <remarks/>
        public object TrustID
        {
            get
            {
                return this.trustIDField;
            }
            set
            {
                this.trustIDField = value;
            }
        }

        /// <remarks/>
        public object DeedsOfficeID
        {
            get
            {
                return this.deedsOfficeIDField;
            }
            set
            {
                this.deedsOfficeIDField = value;
            }
        }

        /// <remarks/>
        public object DeedsOfficeName
        {
            get
            {
                return this.deedsOfficeNameField;
            }
            set
            {
                this.deedsOfficeNameField = value;
            }
        }

        /// <remarks/>
        public object TrustType
        {
            get
            {
                return this.trustTypeField;
            }
            set
            {
                this.trustTypeField = value;
            }
        }

        /// <remarks/>
        public object TrustTypeShortened
        {
            get
            {
                return this.trustTypeShortenedField;
            }
            set
            {
                this.trustTypeShortenedField = value;
            }
        }

        /// <remarks/>
        public object TrustNumber
        {
            get
            {
                return this.trustNumberField;
            }
            set
            {
                this.trustNumberField = value;
            }
        }

        /// <remarks/>
        public object TrustName
        {
            get
            {
                return this.trustNameField;
            }
            set
            {
                this.trustNameField = value;
            }
        }

        /// <remarks/>
        public object DOTSBarcode
        {
            get
            {
                return this.dOTSBarcodeField;
            }
            set
            {
                this.dOTSBarcodeField = value;
            }
        }

        /// <remarks/>
        public object ShortPropertyDescription
        {
            get
            {
                return this.shortPropertyDescriptionField;
            }
            set
            {
                this.shortPropertyDescriptionField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class APIResponseTrustTrustees
    {

        private APIResponseTrustTrusteesTrusteeDetail trusteeDetailField;

        /// <remarks/>
        public APIResponseTrustTrusteesTrusteeDetail TrusteeDetail
        {
            get
            {
                return this.trusteeDetailField;
            }
            set
            {
                this.trusteeDetailField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class APIResponseTrustTrusteesTrusteeDetail
    {

        private string entityTypeField;

        private string trustNumberField;

        private string trustNameField;

        private string highCourtField;

        private System.DateTime effectiveDateField;

        private string inferredIDNumberField;

        private string givenNamesField;

        private string firstNameField;

        private string surnameField;

        private string personalDataField;

        private string ageField;

        private string dateOfBirthField;

        private string genderField;

        /// <remarks/>
        public string EntityType
        {
            get
            {
                return this.entityTypeField;
            }
            set
            {
                this.entityTypeField = value;
            }
        }

        /// <remarks/>
        public string TrustNumber
        {
            get
            {
                return this.trustNumberField;
            }
            set
            {
                this.trustNumberField = value;
            }
        }

        /// <remarks/>
        public string TrustName
        {
            get
            {
                return this.trustNameField;
            }
            set
            {
                this.trustNameField = value;
            }
        }

        /// <remarks/>
        public string HighCourt
        {
            get
            {
                return this.highCourtField;
            }
            set
            {
                this.highCourtField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime EffectiveDate
        {
            get
            {
                return this.effectiveDateField;
            }
            set
            {
                this.effectiveDateField = value;
            }
        }

        /// <remarks/>
        public string InferredIDNumber
        {
            get
            {
                return this.inferredIDNumberField;
            }
            set
            {
                this.inferredIDNumberField = value;
            }
        }

        /// <remarks/>
        public string GivenNames
        {
            get
            {
                return this.givenNamesField;
            }
            set
            {
                this.givenNamesField = value;
            }
        }

        /// <remarks/>
        public string FirstName
        {
            get
            {
                return this.firstNameField;
            }
            set
            {
                this.firstNameField = value;
            }
        }

        /// <remarks/>
        public string Surname
        {
            get
            {
                return this.surnameField;
            }
            set
            {
                this.surnameField = value;
            }
        }

        /// <remarks/>
        public string PersonalData
        {
            get
            {
                return this.personalDataField;
            }
            set
            {
                this.personalDataField = value;
            }
        }

        /// <remarks/>
        public string Age
        {
            get
            {
                return this.ageField;
            }
            set
            {
                this.ageField = value;
            }
        }

        /// <remarks/>
        public string DateOfBirth
        {
            get
            {
                return this.dateOfBirthField;
            }
            set
            {
                this.dateOfBirthField = value;
            }
        }

        /// <remarks/>
        public string Gender
        {
            get
            {
                return this.genderField;
            }
            set
            {
                this.genderField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class APIResponseTrustCrossDeedsDetail
    {

        private object crossDeedsOfficeIDsField;

        private object trustNameField;

        private object trustNumberField;

        private object bloemfonteinField;

        private object capeTownField;

        private object johannesburgField;

        private object kimberleyField;

        private object kingWilliamsTownField;

        private object pietermaritzburgField;

        private object pretoriaField;

        private object vryburgField;

        private object umtataField;

        private object mpumalangaField;

        private object bloemfonteinDataField;

        private object capeTownDataField;

        private object johannesburgDataField;

        private object kimberleyDataField;

        private object kingWilliamsTownDataField;

        private object mpumalangaDataField;

        private object pietermaritzburgDataField;

        private object pretoriaDataField;

        private object umtataDataField;

        private object vryburgDataField;

        /// <remarks/>
        public object CrossDeedsOfficeIDs
        {
            get
            {
                return this.crossDeedsOfficeIDsField;
            }
            set
            {
                this.crossDeedsOfficeIDsField = value;
            }
        }

        /// <remarks/>
        public object TrustName
        {
            get
            {
                return this.trustNameField;
            }
            set
            {
                this.trustNameField = value;
            }
        }

        /// <remarks/>
        public object TrustNumber
        {
            get
            {
                return this.trustNumberField;
            }
            set
            {
                this.trustNumberField = value;
            }
        }

        /// <remarks/>
        public object Bloemfontein
        {
            get
            {
                return this.bloemfonteinField;
            }
            set
            {
                this.bloemfonteinField = value;
            }
        }

        /// <remarks/>
        public object CapeTown
        {
            get
            {
                return this.capeTownField;
            }
            set
            {
                this.capeTownField = value;
            }
        }

        /// <remarks/>
        public object Johannesburg
        {
            get
            {
                return this.johannesburgField;
            }
            set
            {
                this.johannesburgField = value;
            }
        }

        /// <remarks/>
        public object Kimberley
        {
            get
            {
                return this.kimberleyField;
            }
            set
            {
                this.kimberleyField = value;
            }
        }

        /// <remarks/>
        public object KingWilliamsTown
        {
            get
            {
                return this.kingWilliamsTownField;
            }
            set
            {
                this.kingWilliamsTownField = value;
            }
        }

        /// <remarks/>
        public object Pietermaritzburg
        {
            get
            {
                return this.pietermaritzburgField;
            }
            set
            {
                this.pietermaritzburgField = value;
            }
        }

        /// <remarks/>
        public object Pretoria
        {
            get
            {
                return this.pretoriaField;
            }
            set
            {
                this.pretoriaField = value;
            }
        }

        /// <remarks/>
        public object Vryburg
        {
            get
            {
                return this.vryburgField;
            }
            set
            {
                this.vryburgField = value;
            }
        }

        /// <remarks/>
        public object Umtata
        {
            get
            {
                return this.umtataField;
            }
            set
            {
                this.umtataField = value;
            }
        }

        /// <remarks/>
        public object Mpumalanga
        {
            get
            {
                return this.mpumalangaField;
            }
            set
            {
                this.mpumalangaField = value;
            }
        }

        /// <remarks/>
        public object BloemfonteinData
        {
            get
            {
                return this.bloemfonteinDataField;
            }
            set
            {
                this.bloemfonteinDataField = value;
            }
        }

        /// <remarks/>
        public object CapeTownData
        {
            get
            {
                return this.capeTownDataField;
            }
            set
            {
                this.capeTownDataField = value;
            }
        }

        /// <remarks/>
        public object JohannesburgData
        {
            get
            {
                return this.johannesburgDataField;
            }
            set
            {
                this.johannesburgDataField = value;
            }
        }

        /// <remarks/>
        public object KimberleyData
        {
            get
            {
                return this.kimberleyDataField;
            }
            set
            {
                this.kimberleyDataField = value;
            }
        }

        /// <remarks/>
        public object KingWilliamsTownData
        {
            get
            {
                return this.kingWilliamsTownDataField;
            }
            set
            {
                this.kingWilliamsTownDataField = value;
            }
        }

        /// <remarks/>
        public object MpumalangaData
        {
            get
            {
                return this.mpumalangaDataField;
            }
            set
            {
                this.mpumalangaDataField = value;
            }
        }

        /// <remarks/>
        public object PietermaritzburgData
        {
            get
            {
                return this.pietermaritzburgDataField;
            }
            set
            {
                this.pietermaritzburgDataField = value;
            }
        }

        /// <remarks/>
        public object PretoriaData
        {
            get
            {
                return this.pretoriaDataField;
            }
            set
            {
                this.pretoriaDataField = value;
            }
        }

        /// <remarks/>
        public object UmtataData
        {
            get
            {
                return this.umtataDataField;
            }
            set
            {
                this.umtataDataField = value;
            }
        }

        /// <remarks/>
        public object VryburgData
        {
            get
            {
                return this.vryburgDataField;
            }
            set
            {
                this.vryburgDataField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class APIResponseTrustPropertyInformation
    {

        private object propertiesField;

        /// <remarks/>
        public object Properties
        {
            get
            {
                return this.propertiesField;
            }
            set
            {
                this.propertiesField = value;
            }
        }
    }
}