CREATE TABLE [dbo].[propctrl_error_type] (
    [error_type_id]   INT           IDENTITY (1, 1) NOT NULL,
    [error_type_name] VARCHAR (100) NOT NULL,
    PRIMARY KEY CLUSTERED ([error_type_id] ASC)
);

