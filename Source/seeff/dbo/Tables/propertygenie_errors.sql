CREATE TABLE [dbo].[propertygenie_errors] (
    [error_id]              INT      IDENTITY (1, 1) NOT NULL,
    [fk_property_reference] INT      NOT NULL,
    [created_date]          DATETIME DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([error_id] ASC)
);

