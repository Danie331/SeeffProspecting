using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ProspectingTaskScheduler.Core.ProspectingTrainingDB
{
    public class TrainingDatabase
    {
        public static void PurgeAndResetProspectingStaging()
        {
            try
            {
                using (var staging = new prospecting_stagingEntities())
                {
                    var connection = staging.Database.Connection as SqlConnection;
                    connection.Open();

                    string cmdText = CreateScript;
                    SqlCommand cmd = new SqlCommand(cmdText, connection);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Utils.LogException(ex);
            }
        }


        public static string CreateScript
        {
            // Remember to update this for changes in the Prospecting DB
            // Also give the staging database the necessary permission for the task scheduler.
            get
            {
                return @"USE [prospecting_staging];

DECLARE @sql NVARCHAR(MAX);
SET @sql = N'';

SELECT @sql = @sql + N'
  ALTER TABLE ' + QUOTENAME(s.name) + N'.'
  + QUOTENAME(t.name) + N' DROP CONSTRAINT '
  + QUOTENAME(c.name) + ';'
FROM sys.objects AS c
INNER JOIN sys.tables AS t
ON c.parent_object_id = t.[object_id]
INNER JOIN sys.schemas AS s 
ON t.[schema_id] = s.[schema_id]
WHERE c.[type] IN ('D','C','F','PK','UQ') and  t.type = 'U' 
ORDER BY c.[type];
EXEC sys.sp_executesql @sql;
SET @sql = N'';
SELECT @sql += ' Drop table ' + QUOTENAME(s.NAME) + '.' + QUOTENAME(t.NAME) + '; '
FROM   sys.tables t
       JOIN sys.schemas s
         ON t.[schema_id] = s.[schema_id]
WHERE  t.type = 'U'
Exec sp_executesql @sql;

/****** Object:  Table [dbo].[activity_followup_type]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
CREATE TABLE [dbo].[activity_followup_type](
	[activity_followup_type_id] [int] IDENTITY(1,1) NOT NULL,
	[activity_name] [varchar](255) NOT NULL,
	[active] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[activity_followup_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

;
/****** Object:  Table [dbo].[activity_log]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[activity_log](
	[activity_log_id] [bigint] IDENTITY(1,1) NOT NULL,
	[lightstone_property_id] [int] NOT NULL,
	[followup_date] [datetime] NULL,
	[allocated_to] [uniqueidentifier] NULL,
	[activity_type_id] [int] NOT NULL,
	[comment] [varchar](max) NULL,
	[created_by] [uniqueidentifier] NOT NULL,
	[created_date] [datetime] NOT NULL,
	[contact_person_id] [int] NULL,
	[deleted_by] [uniqueidentifier] NULL,
	[delete_date] [datetime] NULL,
	[deleted] [bit] NULL,
	[parent_activity_id] [bigint] NULL,
	[sms_template_id] [int] NULL,
	[sms_sent] [bit] NULL,
	[email_template_id] [int] NULL,
	[email_sent] [bit] NULL,
	[phone_call] [bit] NULL,
	[visit] [bit] NULL,
	[activity_followup_type_id] [int] NULL,
 CONSTRAINT [PK_activity_log] PRIMARY KEY CLUSTERED 
(
	[activity_log_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

;
/****** Object:  Table [dbo].[activity_type]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[activity_type](
	[activity_type_id] [int] IDENTITY(1,1) NOT NULL,
	[activity_name] [varchar](255) NOT NULL,
	[active] [bit] NOT NULL,
	[is_system_type] [bit] NULL,
 CONSTRAINT [PK_activity_type] PRIMARY KEY CLUSTERED 
(
	[activity_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

;
/****** Object:  Table [dbo].[communications_status]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[communications_status](
	[communications_status_id] [int] IDENTITY(1,1) NOT NULL,
	[status_desc] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[communications_status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

;
/****** Object:  Table [dbo].[email_communications_log]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[email_communications_log](
	[email_communications_log_id] [int] IDENTITY(1,1) NOT NULL,
	[batch_id] [uniqueidentifier] NOT NULL,
	[batch_friendly_name] [varchar](max) NULL,
	[batch_activity_type_id] [int] NOT NULL,
	[activity_log_id] [bigint] NULL,
	[followup_activity_id] [bigint] NULL,
	[created_by_user_guid] [uniqueidentifier] NOT NULL,
	[created_by_user_name] [varchar](255) NOT NULL,
	[created_by_user_email_address] [varchar](255) NOT NULL,
	[created_datetime] [datetime] NOT NULL,
	[updated_datetime] [datetime] NULL,
	[target_contact_person_id] [int] NOT NULL,
	[target_email_address] [varchar](255) NOT NULL,
	[target_lightstone_property_id] [int] NOT NULL,
	[status] [int] NOT NULL,
	[email_body_or_link_id] [nvarchar](max) NULL,
	[email_subject_or_link_id] [nvarchar](max) NULL,
	[error_msg] [nvarchar](max) NULL,
	[attachment1_content] [nvarchar](max) NULL,
	[attachment1_type] [nvarchar](255) NULL,
	[attachment1_name] [nvarchar](255) NULL,
	[user_business_unit_id] [int] NULL,
	[api_tracking_id] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[email_communications_log_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

;
/****** Object:  Table [dbo].[exception_log]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[exception_log](
	[exception_log_id] [int] IDENTITY(1,1) NOT NULL,
	[friendly_error_msg] [varchar](max) NOT NULL,
	[exception_string] [varchar](max) NOT NULL,
	[user] [uniqueidentifier] NOT NULL,
	[date_time] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[exception_log_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

;
/****** Object:  Table [dbo].[property_valuation]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[property_valuation](
	[property_valuation_id] [int] IDENTITY(1,1) NOT NULL,
	[prospecting_property_id] [int] NOT NULL,
	[created_date] [datetime] NOT NULL,
	[updated_date] [datetime] NULL,
	[created_by_user_guid] [uniqueidentifier] NOT NULL,
	[activity_log_id] [bigint] NULL,
	[value_estimate] [decimal](18, 2) NOT NULL,
	[date_valued] [datetime] NOT NULL,
	[current_value] [bit] NOT NULL,
	[deleted] [bit] NOT NULL,
	[date_deleted] [datetime] NULL,
	[deleted_by] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[property_valuation_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

;
/****** Object:  Table [dbo].[prospecting_area_dialing_code]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[prospecting_area_dialing_code](
	[prospecting_area_dialing_code_id] [int] IDENTITY(1,1) NOT NULL,
	[dialing_code_id] [int] NOT NULL,
	[code_desc] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[prospecting_area_dialing_code_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

;
/****** Object:  Table [dbo].[prospecting_company_property_relationship]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[prospecting_company_property_relationship](
	[company_property_relationship_id] [int] IDENTITY(1,1) NOT NULL,
	[contact_company_id] [int] NOT NULL,
	[prospecting_property_id] [int] NOT NULL,
	[relationship_to_property] [int] NOT NULL,
	[created_date] [datetime] NULL,
	[updated_date] [datetime] NULL,
	[created_by] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[company_property_relationship_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

;
/****** Object:  Table [dbo].[prospecting_company_property_relationship_type]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[prospecting_company_property_relationship_type](
	[company_property_relationship_type_id] [int] IDENTITY(1,1) NOT NULL,
	[relationship_desc] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[company_property_relationship_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

;
/****** Object:  Table [dbo].[prospecting_contact_company]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[prospecting_contact_company](
	[contact_company_id] [int] IDENTITY(1,1) NOT NULL,
	[company_name] [varchar](255) NOT NULL,
	[CK_number] [varchar](255) NULL,
	[created_date] [datetime] NULL,
	[updated_date] [datetime] NULL,
	[created_by] [uniqueidentifier] NULL,
	[company_type] [varchar](10) NULL,
PRIMARY KEY CLUSTERED 
(
	[contact_company_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

;
/****** Object:  Table [dbo].[prospecting_contact_detail]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[prospecting_contact_detail](
	[prospecting_contact_detail_id] [int] IDENTITY(1,1) NOT NULL,
	[contact_detail_type] [int] NOT NULL,
	[contact_person_id] [int] NOT NULL,
	[contact_detail] [varchar](255) NOT NULL,
	[intl_dialing_code_id] [int] NULL,
	[eleventh_digit] [int] NULL,
	[is_primary_contact] [bit] NOT NULL,
	[deleted] [bit] NOT NULL,
	[deleted_by] [uniqueidentifier] NULL,
	[deleted_date] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[prospecting_contact_detail_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

;
/****** Object:  Table [dbo].[prospecting_contact_detail_type]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[prospecting_contact_detail_type](
	[contact_detail_type_id] [int] IDENTITY(1,1) NOT NULL,
	[type_desc] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[contact_detail_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

;
/****** Object:  Table [dbo].[prospecting_contact_person]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[prospecting_contact_person](
	[contact_person_id] [int] IDENTITY(1,1) NOT NULL,
	[person_title] [int] NULL,
	[person_gender] [varchar](1) NOT NULL,
	[id_number] [varchar](14) NOT NULL,
	[firstname] [varchar](255) NOT NULL,
	[surname] [varchar](255) NOT NULL,
	[job_title] [varchar](255) NOT NULL,
	[propcntrl_buyer_id] [int] NULL,
	[referral_network] [bit] NULL,
	[investor] [bit] NULL,
	[is_popi_restricted] [bit] NOT NULL,
	[created_date] [datetime] NULL,
	[updated_date] [datetime] NULL,
	[created_by] [uniqueidentifier] NULL,
	[comments_notes] [varchar](max) NULL,
	[deceased_status] [nvarchar](255) NULL,
	[age_group] [nvarchar](255) NULL,
	[location] [nvarchar](255) NULL,
	[marital_status] [nvarchar](255) NULL,
	[home_ownership] [nvarchar](255) NULL,
	[directorship] [nvarchar](255) NULL,
	[physical_address] [nvarchar](255) NULL,
	[employer] [nvarchar](255) NULL,
	[occupation] [nvarchar](255) NULL,
	[bureau_adverse_indicator] [nvarchar](255) NULL,
	[citizenship] [nvarchar](255) NULL,
	[optout_emails] [bit] NOT NULL,
	[optout_sms] [bit] NOT NULL,
	[do_not_contact] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[contact_person_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

;
/****** Object:  Table [dbo].[prospecting_person_company_relationship]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[prospecting_person_company_relationship](
	[person_company_relationship_id] [int] IDENTITY(1,1) NOT NULL,
	[contact_person_id] [int] NOT NULL,
	[contact_company_id] [int] NULL,
	[relationship_to_company] [int] NULL,
	[created_date] [datetime] NULL,
	[updated_date] [datetime] NULL,
	[created_by] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[person_company_relationship_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

;
/****** Object:  Table [dbo].[prospecting_person_company_relationship_type]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[prospecting_person_company_relationship_type](
	[person_company_relationship_type_id] [int] IDENTITY(1,1) NOT NULL,
	[relationship_desc] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[person_company_relationship_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

;
/****** Object:  Table [dbo].[prospecting_person_person_relationship]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[prospecting_person_person_relationship](
	[person_person_relationship_id] [int] IDENTITY(1,1) NOT NULL,
	[contact_person_id] [int] NOT NULL,
	[related_contacted_person_id] [int] NOT NULL,
	[relationship_to_person] [int] NOT NULL,
	[created_date] [datetime] NULL,
	[updated_date] [datetime] NULL,
	[created_by] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[person_person_relationship_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

;
/****** Object:  Table [dbo].[prospecting_person_person_relationship_type]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[prospecting_person_person_relationship_type](
	[person_person_relationship_type_id] [int] IDENTITY(1,1) NOT NULL,
	[relationship_desc] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[person_person_relationship_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

;
/****** Object:  Table [dbo].[prospecting_person_property_relationship]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[prospecting_person_property_relationship](
	[person_property_relationship_id] [int] IDENTITY(1,1) NOT NULL,
	[contact_person_id] [int] NOT NULL,
	[prospecting_property_id] [int] NOT NULL,
	[relationship_to_property] [int] NOT NULL,
	[created_date] [datetime] NULL,
	[updated_date] [datetime] NULL,
	[created_by] [uniqueidentifier] NULL,
	[from_date] [datetime] NULL,
	[to_date] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[person_property_relationship_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

;
/****** Object:  Table [dbo].[prospecting_person_property_relationship_type]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[prospecting_person_property_relationship_type](
	[person_property_relationship_type_id] [int] IDENTITY(1,1) NOT NULL,
	[relationship_desc] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[person_property_relationship_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

;
/****** Object:  Table [dbo].[prospecting_person_title]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[prospecting_person_title](
	[prospecting_person_title_id] [int] IDENTITY(1,1) NOT NULL,
	[person_title] [varchar](5) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[prospecting_person_title_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

;
/****** Object:  Table [dbo].[prospecting_property]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[prospecting_property](
	[prospecting_property_id] [int] IDENTITY(1,1) NOT NULL,
	[licence_id] [int] NULL,
	[seeff_area_id] [int] NULL,
	[development_id] [int] NULL,
	[lightstone_property_id] [int] NOT NULL,
	[propstats_id] [int] NULL,
	[windeed_id] [int] NULL,
	[erf_no] [int] NULL,
	[portion_no] [int] NULL,
	[property_address] [varchar](255) NULL,
	[street_or_unit_no] [varchar](255) NULL,
	[photo_url] [nvarchar](256) NULL,
	[latitude] [decimal](13, 8) NULL,
	[longitude] [decimal](13, 8) NULL,
	[age] [datetime] NULL,
	[erf_size] [int] NULL,
	[dwell_size] [int] NULL,
	[condition] [varchar](255) NULL,
	[beds] [int] NULL,
	[baths] [int] NULL,
	[receptions] [int] NULL,
	[studies] [int] NULL,
	[garages] [int] NULL,
	[parking_bays] [int] NULL,
	[pool] [bit] NULL,
	[staff_accomodation] [bit] NULL,
	[created_date] [datetime] NULL,
	[updated_date] [datetime] NULL,
	[created_by] [uniqueidentifier] NULL,
	[lightstone_id_or_ck_no] [varchar](255) NULL,
	[lightstone_reg_date] [varchar](8) NULL,
	[comments] [varchar](max) NULL,
	[ss_name] [varchar](255) NULL,
	[ss_number] [varchar](50) NULL,
	[unit] [varchar](50) NULL,
	[ss_fh] [varchar](3) NULL,
	[last_purch_price] [decimal](18, 2) NULL,
	[ss_id] [varchar](10) NULL,
	[ss_door_number] [varchar](50) NULL,
	[prospected] [bit] NULL,
	[farm_name] [nvarchar](255) NULL,
	[lightstone_suburb] [nvarchar](255) NULL,
	[ss_unique_identifier] [nvarchar](max) NULL,
	[locked_by_guid] [uniqueidentifier] NULL,
	[locked_datetime] [datetime] NULL,
	[latest_reg_date] [varchar](8) NULL,
	[is_short_term_rental] [bit] NULL,
	[is_long_term_rental] [bit] NULL,
	[is_commercial] [bit] NULL,
	[is_agricultural] [bit] NULL,
	[is_investment] [bit] NULL,
	[has_email] [bit] NULL,
	[has_cell] [bit] NULL,
	[has_landline] [bit] NULL,
	[has_primary_email] [bit] NULL,
	[has_primary_cell] [bit] NULL,
	[has_primary_landline] [bit] NULL,
 CONSTRAINT [PK__prospect__8E5D8F4FDB723D77] PRIMARY KEY CLUSTERED 
(
	[prospecting_property_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

;
/****** Object:  Table [dbo].[service_enquiry_log]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[service_enquiry_log](
	[service_enquiry_log_id] [int] IDENTITY(1,1) NOT NULL,
	[prospecting_property_id] [int] NOT NULL,
	[user] [uniqueidentifier] NOT NULL,
	[date_of_enquiry] [datetime] NOT NULL,
	[successful] [bit] NOT NULL,
	[id_number] [varchar](13) NOT NULL,
	[HWCE_indicator] [varchar](4) NULL,
	[service_type_name] [varchar](20) NULL,
	[enquiry_cost] [decimal](19, 4) NULL,
	[status_message] [varchar](255) NULL,
	[exception] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[service_enquiry_log_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

;
/****** Object:  Table [dbo].[sms_communications_log]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[sms_communications_log](
	[sms_communications_log_id] [int] IDENTITY(1,1) NOT NULL,
	[batch_id] [uniqueidentifier] NOT NULL,
	[batch_friendly_name] [varchar](max) NULL,
	[batch_activity_type_id] [int] NOT NULL,
	[activity_log_id] [bigint] NULL,
	[followup_activity_id] [bigint] NULL,
	[created_by_user_guid] [uniqueidentifier] NOT NULL,
	[created_datetime] [datetime] NOT NULL,
	[updated_datetime] [datetime] NULL,
	[target_contact_person_id] [int] NOT NULL,
	[target_cellphone_no] [varchar](13) NOT NULL,
	[target_lightstone_property_id] [int] NOT NULL,
	[status] [int] NOT NULL,
	[api_tracking_id] [varchar](max) NULL,
	[api_delivery_status] [varchar](50) NULL,
	[msg_body_or_link_id] [nvarchar](max) NULL,
	[reply] [nvarchar](max) NULL,
	[error_msg] [nvarchar](max) NULL,
	[user_business_unit_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[sms_communications_log_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

;
/****** Object:  Table [dbo].[system_communication_template]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[system_communication_template](
	[system_communication_template_id] [int] IDENTITY(1,1) NOT NULL,
	[activity_type_id] [int] NOT NULL,
	[template_content] [nvarchar](max) NOT NULL,
	[template_name] [varchar](1000) NOT NULL,
	[communication_type] [varchar](5) NOT NULL,
	[active] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[system_communication_template_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

;
/****** Object:  Table [dbo].[user_communication_template]    Script Date: 2017/05/08 1:07:17 PM ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[user_communication_template](
	[user_communication_template_id] [int] IDENTITY(1,1) NOT NULL,
	[created_by] [uniqueidentifier] NOT NULL,
	[created_date] [datetime] NOT NULL,
	[updated_date] [datetime] NULL,
	[template_content] [nvarchar](max) NOT NULL,
	[template_name] [varchar](1000) NOT NULL,
	[communication_type] [varchar](5) NOT NULL,
	[deleted] [bit] NOT NULL,
	[activity_type_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[user_communication_template_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

;
SET IDENTITY_INSERT [dbo].[activity_followup_type] ON 

;
INSERT [dbo].[activity_followup_type] ([activity_followup_type_id], [activity_name], [active]) select [activity_followup_type_id], [activity_name], [active] from seeff_prospecting.dbo.activity_followup_type
;

SET IDENTITY_INSERT [dbo].[activity_followup_type] OFF
;
SET IDENTITY_INSERT [dbo].[activity_type] ON 

;
INSERT [dbo].[activity_type] ([activity_type_id], [activity_name], [active], [is_system_type]) select [activity_type_id], [activity_name], [active], [is_system_type] from seeff_prospecting.dbo.[activity_type] 

SET IDENTITY_INSERT [dbo].[activity_type] OFF
;
SET IDENTITY_INSERT [dbo].[communications_status] ON 

;
INSERT [dbo].[communications_status] ([communications_status_id], [status_desc]) select [communications_status_id], [status_desc] from seeff_prospecting.dbo.[communications_status] 
;

SET IDENTITY_INSERT [dbo].[communications_status] OFF
;
SET IDENTITY_INSERT [dbo].[prospecting_area_dialing_code] ON 

;
INSERT [dbo].[prospecting_area_dialing_code] ([prospecting_area_dialing_code_id], [dialing_code_id], [code_desc]) select [prospecting_area_dialing_code_id], [dialing_code_id], [code_desc] from seeff_prospecting.dbo.[prospecting_area_dialing_code] 
;

SET IDENTITY_INSERT [dbo].[prospecting_area_dialing_code] OFF
;
SET IDENTITY_INSERT [dbo].[prospecting_company_property_relationship_type] ON 

;
INSERT [dbo].[prospecting_company_property_relationship_type] ([company_property_relationship_type_id], [relationship_desc]) select [company_property_relationship_type_id], [relationship_desc] from seeff_prospecting.dbo.[prospecting_company_property_relationship_type] 
;
SET IDENTITY_INSERT [dbo].[prospecting_company_property_relationship_type] OFF
;
SET IDENTITY_INSERT [dbo].[prospecting_contact_detail_type] ON 

;
INSERT [dbo].[prospecting_contact_detail_type] ([contact_detail_type_id], [type_desc]) select [contact_detail_type_id], [type_desc] from seeff_prospecting.dbo.[prospecting_contact_detail_type]
;

SET IDENTITY_INSERT [dbo].[prospecting_contact_detail_type] OFF
;
SET IDENTITY_INSERT [dbo].[prospecting_person_company_relationship_type] ON 

;
INSERT [dbo].[prospecting_person_company_relationship_type] ([person_company_relationship_type_id], [relationship_desc]) select [person_company_relationship_type_id], [relationship_desc] from seeff_prospecting.dbo.[prospecting_person_company_relationship_type]
;

SET IDENTITY_INSERT [dbo].[prospecting_person_company_relationship_type] OFF
;
SET IDENTITY_INSERT [dbo].[prospecting_person_property_relationship_type] ON 

;
INSERT [dbo].[prospecting_person_property_relationship_type] ([person_property_relationship_type_id], [relationship_desc]) select [person_property_relationship_type_id], [relationship_desc] from seeff_prospecting.dbo.[prospecting_person_property_relationship_type]  
;

SET IDENTITY_INSERT [dbo].[prospecting_person_property_relationship_type] OFF
;
SET IDENTITY_INSERT [dbo].[prospecting_person_title] ON 

;
INSERT [dbo].[prospecting_person_title] ([prospecting_person_title_id], [person_title]) select [prospecting_person_title_id], [person_title] from seeff_prospecting.dbo.[prospecting_person_title]    
;

SET IDENTITY_INSERT [dbo].[prospecting_person_title] OFF
;
SET IDENTITY_INSERT [dbo].[system_communication_template] ON 

;
INSERT [dbo].[system_communication_template] ([system_communication_template_id], [activity_type_id], [template_content], [template_name], [communication_type], [active]) select [system_communication_template_id], [activity_type_id], [template_content], [template_name], [communication_type], [active] from seeff_prospecting.dbo.[system_communication_template] 
;

SET IDENTITY_INSERT [dbo].[system_communication_template] OFF
;
/****** Object:  Index [ls_prop_id_unique]    Script Date: 2017/05/08 1:07:18 PM ******/
ALTER TABLE [dbo].[prospecting_property] ADD  CONSTRAINT [ls_prop_id_unique] UNIQUE NONCLUSTERED 
(
	[lightstone_property_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
;
ALTER TABLE [dbo].[property_valuation] ADD  DEFAULT ((0)) FOR [deleted]
;
ALTER TABLE [dbo].[prospecting_contact_detail] ADD  DEFAULT ((0)) FOR [deleted]
;
ALTER TABLE [dbo].[prospecting_contact_person] ADD  DEFAULT ((0)) FOR [is_popi_restricted]
;
ALTER TABLE [dbo].[prospecting_contact_person] ADD  DEFAULT ((0)) FOR [optout_emails]
;
ALTER TABLE [dbo].[prospecting_contact_person] ADD  DEFAULT ((0)) FOR [optout_sms]
;
ALTER TABLE [dbo].[prospecting_contact_person] ADD  DEFAULT ((0)) FOR [do_not_contact]
;
ALTER TABLE [dbo].[prospecting_property] ADD  CONSTRAINT [DF_prospecting_property_created_date]  DEFAULT (getdate()) FOR [created_date]
;
ALTER TABLE [dbo].[prospecting_property] ADD  CONSTRAINT [DF_prospecting_property_prospected]  DEFAULT ((0)) FOR [prospected]
;
ALTER TABLE [dbo].[prospecting_property] ADD  DEFAULT ((0)) FOR [is_short_term_rental]
;
ALTER TABLE [dbo].[prospecting_property] ADD  DEFAULT ((0)) FOR [is_long_term_rental]
;
ALTER TABLE [dbo].[prospecting_property] ADD  DEFAULT ((0)) FOR [is_commercial]
;
ALTER TABLE [dbo].[prospecting_property] ADD  DEFAULT ((0)) FOR [is_agricultural]
;
ALTER TABLE [dbo].[prospecting_property] ADD  DEFAULT ((0)) FOR [is_investment]
;
ALTER TABLE [dbo].[prospecting_property] ADD  DEFAULT ((0)) FOR [has_email]
;
ALTER TABLE [dbo].[prospecting_property] ADD  DEFAULT ((0)) FOR [has_cell]
;
ALTER TABLE [dbo].[prospecting_property] ADD  DEFAULT ((0)) FOR [has_landline]
;
ALTER TABLE [dbo].[prospecting_property] ADD  DEFAULT ((0)) FOR [has_primary_email]
;
ALTER TABLE [dbo].[prospecting_property] ADD  DEFAULT ((0)) FOR [has_primary_cell]
;
ALTER TABLE [dbo].[prospecting_property] ADD  DEFAULT ((0)) FOR [has_primary_landline]
;
ALTER TABLE [dbo].[service_enquiry_log] ADD  DEFAULT ('xxxxxxxxxxxxx') FOR [id_number]
;
ALTER TABLE [dbo].[system_communication_template] ADD  DEFAULT ((1)) FOR [active]
;
ALTER TABLE [dbo].[user_communication_template] ADD  DEFAULT ((0)) FOR [deleted]
;
ALTER TABLE [dbo].[activity_log]  WITH NOCHECK ADD FOREIGN KEY([activity_followup_type_id])
REFERENCES [dbo].[activity_followup_type] ([activity_followup_type_id])
;
ALTER TABLE [dbo].[activity_log]  WITH NOCHECK ADD  CONSTRAINT [FK_activity_type_id] FOREIGN KEY([activity_type_id])
REFERENCES [dbo].[activity_type] ([activity_type_id])
;
ALTER TABLE [dbo].[activity_log] CHECK CONSTRAINT [FK_activity_type_id]
;
ALTER TABLE [dbo].[activity_log]  WITH NOCHECK ADD  CONSTRAINT [FK_contact_person_id] FOREIGN KEY([contact_person_id])
REFERENCES [dbo].[prospecting_contact_person] ([contact_person_id])
;
ALTER TABLE [dbo].[activity_log] CHECK CONSTRAINT [FK_contact_person_id]
;
ALTER TABLE [dbo].[activity_log]  WITH NOCHECK ADD  CONSTRAINT [FK_lightstone_property_id] FOREIGN KEY([lightstone_property_id])
REFERENCES [dbo].[prospecting_property] ([lightstone_property_id])
;
ALTER TABLE [dbo].[activity_log] CHECK CONSTRAINT [FK_lightstone_property_id]
;
ALTER TABLE [dbo].[email_communications_log]  WITH CHECK ADD FOREIGN KEY([activity_log_id])
REFERENCES [dbo].[activity_log] ([activity_log_id])
;
ALTER TABLE [dbo].[email_communications_log]  WITH CHECK ADD FOREIGN KEY([batch_activity_type_id])
REFERENCES [dbo].[activity_type] ([activity_type_id])
;
ALTER TABLE [dbo].[email_communications_log]  WITH CHECK ADD FOREIGN KEY([followup_activity_id])
REFERENCES [dbo].[activity_log] ([activity_log_id])
;
ALTER TABLE [dbo].[email_communications_log]  WITH CHECK ADD FOREIGN KEY([status])
REFERENCES [dbo].[communications_status] ([communications_status_id])
;
ALTER TABLE [dbo].[email_communications_log]  WITH CHECK ADD FOREIGN KEY([target_contact_person_id])
REFERENCES [dbo].[prospecting_contact_person] ([contact_person_id])
;
ALTER TABLE [dbo].[email_communications_log]  WITH NOCHECK ADD FOREIGN KEY([target_lightstone_property_id])
REFERENCES [dbo].[prospecting_property] ([lightstone_property_id])
;
ALTER TABLE [dbo].[property_valuation]  WITH CHECK ADD FOREIGN KEY([activity_log_id])
REFERENCES [dbo].[activity_log] ([activity_log_id])
;
ALTER TABLE [dbo].[property_valuation]  WITH CHECK ADD FOREIGN KEY([prospecting_property_id])
REFERENCES [dbo].[prospecting_property] ([prospecting_property_id])
;
ALTER TABLE [dbo].[prospecting_company_property_relationship]  WITH NOCHECK ADD FOREIGN KEY([contact_company_id])
REFERENCES [dbo].[prospecting_contact_company] ([contact_company_id])
;
ALTER TABLE [dbo].[prospecting_company_property_relationship]  WITH NOCHECK ADD  CONSTRAINT [FK__prospecti__prosp__398D8EEE] FOREIGN KEY([prospecting_property_id])
REFERENCES [dbo].[prospecting_property] ([prospecting_property_id])
;
ALTER TABLE [dbo].[prospecting_company_property_relationship] CHECK CONSTRAINT [FK__prospecti__prosp__398D8EEE]
;
ALTER TABLE [dbo].[prospecting_company_property_relationship]  WITH NOCHECK ADD FOREIGN KEY([relationship_to_property])
REFERENCES [dbo].[prospecting_company_property_relationship_type] ([company_property_relationship_type_id])
;
ALTER TABLE [dbo].[prospecting_contact_detail]  WITH CHECK ADD FOREIGN KEY([contact_detail_type])
REFERENCES [dbo].[prospecting_contact_detail_type] ([contact_detail_type_id])
;
ALTER TABLE [dbo].[prospecting_contact_detail]  WITH CHECK ADD FOREIGN KEY([contact_person_id])
REFERENCES [dbo].[prospecting_contact_person] ([contact_person_id])
;
ALTER TABLE [dbo].[prospecting_contact_detail]  WITH CHECK ADD FOREIGN KEY([intl_dialing_code_id])
REFERENCES [dbo].[prospecting_area_dialing_code] ([prospecting_area_dialing_code_id])
;
ALTER TABLE [dbo].[prospecting_contact_person]  WITH CHECK ADD FOREIGN KEY([person_title])
REFERENCES [dbo].[prospecting_person_title] ([prospecting_person_title_id])
;
ALTER TABLE [dbo].[prospecting_person_company_relationship]  WITH CHECK ADD FOREIGN KEY([contact_company_id])
REFERENCES [dbo].[prospecting_contact_company] ([contact_company_id])
;
ALTER TABLE [dbo].[prospecting_person_company_relationship]  WITH CHECK ADD FOREIGN KEY([contact_person_id])
REFERENCES [dbo].[prospecting_contact_person] ([contact_person_id])
;
ALTER TABLE [dbo].[prospecting_person_company_relationship]  WITH CHECK ADD FOREIGN KEY([relationship_to_company])
REFERENCES [dbo].[prospecting_person_company_relationship_type] ([person_company_relationship_type_id])
;
ALTER TABLE [dbo].[prospecting_person_person_relationship]  WITH CHECK ADD FOREIGN KEY([contact_person_id])
REFERENCES [dbo].[prospecting_contact_person] ([contact_person_id])
;
ALTER TABLE [dbo].[prospecting_person_person_relationship]  WITH CHECK ADD FOREIGN KEY([relationship_to_person])
REFERENCES [dbo].[prospecting_person_person_relationship_type] ([person_person_relationship_type_id])
;
ALTER TABLE [dbo].[prospecting_person_person_relationship]  WITH CHECK ADD FOREIGN KEY([related_contacted_person_id])
REFERENCES [dbo].[prospecting_contact_person] ([contact_person_id])
;
ALTER TABLE [dbo].[prospecting_person_property_relationship]  WITH NOCHECK ADD FOREIGN KEY([contact_person_id])
REFERENCES [dbo].[prospecting_contact_person] ([contact_person_id])
;
ALTER TABLE [dbo].[prospecting_person_property_relationship]  WITH NOCHECK ADD  CONSTRAINT [FK__prospecti__prosp__46E78A0C] FOREIGN KEY([prospecting_property_id])
REFERENCES [dbo].[prospecting_property] ([prospecting_property_id])
;
ALTER TABLE [dbo].[prospecting_person_property_relationship] CHECK CONSTRAINT [FK__prospecti__prosp__46E78A0C]
;
ALTER TABLE [dbo].[prospecting_person_property_relationship]  WITH NOCHECK ADD FOREIGN KEY([relationship_to_property])
REFERENCES [dbo].[prospecting_person_property_relationship_type] ([person_property_relationship_type_id])
;
ALTER TABLE [dbo].[service_enquiry_log]  WITH NOCHECK ADD  CONSTRAINT [FK__prospecti__prosp__49C3F6B7] FOREIGN KEY([prospecting_property_id])
REFERENCES [dbo].[prospecting_property] ([prospecting_property_id])
;
ALTER TABLE [dbo].[service_enquiry_log] CHECK CONSTRAINT [FK__prospecti__prosp__49C3F6B7]
;
ALTER TABLE [dbo].[sms_communications_log]  WITH CHECK ADD FOREIGN KEY([activity_log_id])
REFERENCES [dbo].[activity_log] ([activity_log_id])
;
ALTER TABLE [dbo].[sms_communications_log]  WITH CHECK ADD FOREIGN KEY([batch_activity_type_id])
REFERENCES [dbo].[activity_type] ([activity_type_id])
;
ALTER TABLE [dbo].[sms_communications_log]  WITH CHECK ADD FOREIGN KEY([followup_activity_id])
REFERENCES [dbo].[activity_log] ([activity_log_id])
;
ALTER TABLE [dbo].[sms_communications_log]  WITH CHECK ADD FOREIGN KEY([status])
REFERENCES [dbo].[communications_status] ([communications_status_id])
;
ALTER TABLE [dbo].[sms_communications_log]  WITH CHECK ADD FOREIGN KEY([target_contact_person_id])
REFERENCES [dbo].[prospecting_contact_person] ([contact_person_id])
;
ALTER TABLE [dbo].[sms_communications_log]  WITH CHECK ADD FOREIGN KEY([target_lightstone_property_id])
REFERENCES [dbo].[prospecting_property] ([lightstone_property_id])
;
ALTER TABLE [dbo].[system_communication_template]  WITH CHECK ADD FOREIGN KEY([activity_type_id])
REFERENCES [dbo].[activity_type] ([activity_type_id])
;
ALTER TABLE [dbo].[user_communication_template]  WITH CHECK ADD FOREIGN KEY([activity_type_id])
REFERENCES [dbo].[activity_type] ([activity_type_id])
;
";
            }
        }
    }
}