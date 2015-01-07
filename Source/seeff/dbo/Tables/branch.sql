CREATE TABLE [dbo].[branch] (
    [branchId]                        INT             IDENTITY (1, 1) NOT NULL,
    [branchName]                      NVARCHAR (100)  NULL,
    [branchURL]                       NVARCHAR (255)  NULL,
    [branchDialCode]                  CHAR (50)       NULL,
    [branchTelephone]                 CHAR (50)       NULL,
    [branchFax]                       CHAR (50)       NULL,
    [branchCell]                      CHAR (50)       NULL,
    [branchPostalAddress]             NVARCHAR (500)  NULL,
    [branchPhysicalAddress]           NVARCHAR (500)  NULL,
    [branchEmail]                     NVARCHAR (150)  NULL,
    [branchContact]                   INT             NULL,
    [branchVax]                       VARCHAR (20)    NULL,
    [fkBranchTypeId]                  INT             NULL,
    [fkAreaId]                        INT             NULL,
    [fkCountryId]                     TINYINT         CONSTRAINT [DF_branch_fkCountryId] DEFAULT (0) NULL,
    [branchActive]                    BIT             CONSTRAINT [DF_branch_branchActive] DEFAULT (0) NULL,
    [branchParentId]                  INT             CONSTRAINT [DF_branch_branchParentId] DEFAULT (0) NOT NULL,
    [branchCopy]                      NVARCHAR (4000) NULL,
    [branchSpeciality]                NVARCHAR (4000) NULL,
    [branchOpeningHours]              VARCHAR (200)   NULL,
    [branchLicenseeInformation]       NVARCHAR (4000) NULL,
    [branchSubscribedPropertyGenie]   TINYINT         CONSTRAINT [DF_branch_branchSubscribedPropertyGenie] DEFAULT (0) NULL,
    [propCtrlId]                      INT             NULL,
    [virtualTour_width]               VARCHAR (100)   NULL,
    [virtualTour_height]              VARCHAR (100)   NULL,
    [branchViewOnly]                  INT             CONSTRAINT [DF__branch__branchVi__3805392F] DEFAULT (0) NOT NULL,
    [updatedByUserId]                 INT             NULL,
    [lastUpdated]                     DATETIME        NULL,
    [branchLatitude]                  VARCHAR (255)   NULL,
    [branchLongitude]                 VARCHAR (255)   NULL,
    [propertyGenieBranchId]           INT             NULL,
    [branchSubscribedPrivateProperty] BIT             DEFAULT ((0)) NOT NULL,
    [privatePropertyBranchId]         VARCHAR (100)   NULL,
    [my_mail_subscribed]              BIT             DEFAULT ((0)) NOT NULL,
    [branch_image]                    VARCHAR (100)   NULL,
    [fk_license_id]                   INT             NULL,
    [mymail_agent_client_assoc]       BIT             DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_branch] PRIMARY KEY CLUSTERED ([branchId] ASC) WITH (FILLFACTOR = 90)
);


GO
CREATE NONCLUSTERED INDEX [foreignKeys]
    ON [dbo].[branch]([fkBranchTypeId] ASC, [fkAreaId] ASC, [branchParentId] ASC) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [toggleFlags]
    ON [dbo].[branch]([branchActive] ASC, [branchSubscribedPropertyGenie] ASC) WITH (FILLFACTOR = 90);

