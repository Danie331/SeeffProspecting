CREATE TABLE [dbo].[reports_rental_recognition] (
    [rental_id]                   INT             IDENTITY (1, 1) NOT NULL,
    [license_id]                  INT             NOT NULL,
    [deal_number]                 NVARCHAR (50)   NOT NULL,
    [contract_start_date]         DATETIME        NOT NULL,
    [contract_end_date]           DATETIME        NOT NULL,
    [comm]                        DECIMAL (18, 2) NOT NULL,
    [agent]                       NVARCHAR (200)  NOT NULL,
    [entity_type]                 CHAR (1)        NOT NULL,
    [agent_registration_id]       INT             NULL,
    [partnership_registration_id] INT             NULL,
    [created_by]                  INT             NOT NULL,
    [created_date]                DATETIME        CONSTRAINT [DF_reports_rental_recognition_created_date] DEFAULT (getdate()) NOT NULL,
    [units]                       INT             NULL
);

