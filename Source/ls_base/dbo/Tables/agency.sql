CREATE TABLE [dbo].[agency] (
    [agency_name] VARCHAR (MAX) NULL,
    [agency_id]   INT           IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_agency] PRIMARY KEY CLUSTERED ([agency_id] ASC)
);

