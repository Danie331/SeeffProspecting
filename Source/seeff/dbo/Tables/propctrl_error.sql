CREATE TABLE [dbo].[propctrl_error] (
    [error_id]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [mandate_id]            INT            NOT NULL,
    [fk_property_reference] INT            NULL,
    [fk_error_type_id]      INT            NULL,
    [error_text]            VARCHAR (5000) NULL,
    [fk_propctrl_user_id]   INT            NULL,
    [created_date]          DATETIME       DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([error_id] ASC)
);

