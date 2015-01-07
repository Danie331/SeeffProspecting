CREATE TABLE [dbo].[wl_fnb_property_address] (
    [pk_fnb_prop_id] INT           IDENTITY (1, 1) NOT NULL,
    [fk_propertyId]  INT           NOT NULL,
    [unit_number]    VARCHAR (50)  NULL,
    [complex_name]   VARCHAR (150) NULL,
    [street_number]  VARCHAR (50)  NOT NULL,
    [street_name]    VARCHAR (150) NOT NULL,
    [suburb]         VARCHAR (150) NOT NULL,
    [town]           VARCHAR (150) NOT NULL,
    [province]       VARCHAR (150) NOT NULL,
    [Active]         CHAR (1)      CONSTRAINT [DF_wl_fnb_property_address_Active] DEFAULT ('Y') NOT NULL,
    [AddedDate]      DATETIME      CONSTRAINT [DF_wl_fnb_property_address_AddedDate] DEFAULT (getdate()) NOT NULL
);

