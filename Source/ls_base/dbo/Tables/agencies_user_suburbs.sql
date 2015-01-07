CREATE TABLE [dbo].[agencies_user_suburbs] (
    [agencies_user_suburbs_id] INT           IDENTITY (1, 1) NOT NULL,
    [suburb_id]                INT           NOT NULL,
    [agency_id]                INT           NULL,
    [updated_by]               VARCHAR (MAX) NOT NULL,
    [updated_date]             ROWVERSION    NOT NULL,
    CONSTRAINT [PK_agencies_user_suburbs] PRIMARY KEY CLUSTERED ([agencies_user_suburbs_id] ASC),
    CONSTRAINT [FK_agency_id] FOREIGN KEY ([agency_id]) REFERENCES [dbo].[agency] ([agency_id])
);

