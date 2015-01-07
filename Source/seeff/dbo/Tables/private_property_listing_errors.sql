CREATE TABLE [dbo].[private_property_listing_errors] (
    [error_id]              INT           IDENTITY (1, 1) NOT NULL,
    [error]                 VARCHAR (MAX) NOT NULL,
    [fk_property_reference] INT           NOT NULL,
    [date_added]            DATETIME      DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_private_property_listing_errors] PRIMARY KEY CLUSTERED ([error_id] ASC)
);

