CREATE TABLE [dbo].[reports_Referral_Recognition_Detail] (
    [region]               NVARCHAR (50)   NULL,
    [license_id]           INT             NULL,
    [license_name]         NVARCHAR (50)   NULL,
    [smart_pass_id]        INT             NULL,
    [transaction_division] VARCHAR (50)    NULL,
    [month_reported]       INT             NULL,
    [sps_refferal_type]    NVARCHAR (50)   NULL,
    [Agent_Partnership]    NVARCHAR (50)   NULL,
    [registration_id]      INT             NULL,
    [sps_transaction_ref]  NVARCHAR (50)   NULL,
    [Agent_Count]          INT             NULL,
    [Recognition_Unit]     DECIMAL (16, 2) NULL
);

