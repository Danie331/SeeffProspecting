CREATE TABLE [dbo].[smart_pass] (
    [smart_pass_id]     INT             IDENTITY (1, 1) NOT NULL,
    [referral_type]     VARCHAR (20)    NOT NULL,
    [department]        CHAR (1)        NOT NULL,
    [license_id_to]     INT             NOT NULL,
    [license_id_from]   INT             NOT NULL,
    [expiry_date]       DATETIME        NOT NULL,
    [current_status]    VARCHAR (50)    NOT NULL,
    [created_by]        INT             CONSTRAINT [DF_smart_pass_created_by] DEFAULT ((0)) NOT NULL,
    [created_date]      DATETIME        CONSTRAINT [DF_smart_pass_created_date] DEFAULT (getdate()) NOT NULL,
    [updated_by]        INT             CONSTRAINT [DF_smart_pass_updated_by] DEFAULT ((0)) NOT NULL,
    [updated_date]      DATETIME        CONSTRAINT [DF_smart_pass_updated_date] DEFAULT (getdate()) NOT NULL,
    [value_from]        DECIMAL (18, 2) NULL,
    [value_to]          DECIMAL (18, 2) NULL,
    [no_beds]           INT             NULL,
    [no_baths]          INT             NULL,
    [property_id]       INT             NULL,
    [property_desc]     VARCHAR (350)   NULL,
    [property_geo_code] VARCHAR (50)    NULL,
    [division]          VARCHAR (50)    NULL,
    [category]          VARCHAR (50)    NULL,
    [areaId]            INT             NULL,
    [area_desc]         VARCHAR (250)   NULL,
    [registration_id]   INT             NOT NULL,
    [notification_sent] DATETIME        NULL,
    [sms_sent]          DATETIME        NULL,
    [score]             INT             NULL,
    [entity_type]       CHAR (1)        NULL,
    [entity_id]         INT             NULL,
    [from_entity_type]  CHAR (1)        NULL,
    [from_entity_id]    INT             NULL,
    [source]            CHAR (1)        NULL,
    [access_id]         VARCHAR (50)    NULL,
    [lead_id]           INT             NULL,
    [extend_expiry]     VARCHAR (50)    NULL,
    CONSTRAINT [PK_smart_pass] PRIMARY KEY CLUSTERED ([smart_pass_id] ASC)
);


GO
-- =============================================
-- Author:		Adam roberts
-- Create date: 2011-11-30
-- Description:	Insert a referral audit
-- =============================================
CREATE TRIGGER [dbo].[InsertReferralAudit] 
   ON dbo.smart_pass
   AFTER INSERT
AS 
BEGIN
	SET NOCOUNT ON;
    
INSERT INTO [boss].[dbo].[smart_pass_actual]
           ([smart_pass_id]
           ,[referral_type]
           ,[created_by]
           ,[created_date]
           ,[updated_by]
           ,[updated_date]
           ,[value_from]
           ,[value_to]
           ,[no_beds]
           ,[no_baths]
           ,[property_id]
           ,[property_desc]
           ,[property_geo_code]
           ,[division]
           ,[category]
           ,[area]
           ,[score])
     SELECT [smart_pass_id]
           ,[referral_type]
           ,[created_by]
           ,[created_date]
           ,[updated_by]
           ,[updated_date]
           ,[value_from]
           ,[value_to]
           ,[no_beds]
           ,[no_baths]
           ,[property_id]
           ,[property_desc]
           ,[property_geo_code]
           ,[division]
           ,[category]
           ,[area_desc]
           ,[score] 
      FROM inserted AS i
END
