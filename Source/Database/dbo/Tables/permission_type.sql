CREATE TABLE [dbo].[permission_type] (
    [permission_type_id]   INT          IDENTITY (1, 1) NOT NULL,
    [permission_type_name] VARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([permission_type_id] ASC)
);

