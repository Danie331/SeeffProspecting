CREATE TABLE [dbo].[deal] (
    [deal_id]        INT           IDENTITY (1, 1) NOT NULL,
    [deal_desc]      VARCHAR (500) NOT NULL,
    [contact_person] VARCHAR (150) NOT NULL,
    [contact_email]  VARCHAR (250) NOT NULL,
    [deal_visible]   BIT           NOT NULL
);

