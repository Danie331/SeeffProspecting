CREATE TABLE [dbo].[smart_pass_lead] (
    [lead_id]    INT           IDENTITY (1, 1) NOT NULL,
    [license_id] INT           NOT NULL,
    [lead_desc]  VARCHAR (250) NOT NULL,
    [lead_order] INT           NOT NULL
);

