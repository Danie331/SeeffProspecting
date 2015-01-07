CREATE TABLE [dbo].[delete_old_property] (
    [propDeleteId]      INT      IDENTITY (1, 1) NOT NULL,
    [propertyReference] INT      NOT NULL,
    [fkBranchId]        INT      NULL,
    [dateAdded]         DATETIME DEFAULT (getdate()) NOT NULL,
    [dateDeleted]       DATETIME NULL,
    PRIMARY KEY CLUSTERED ([propDeleteId] ASC)
);

