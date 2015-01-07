CREATE TABLE [dbo].[feature] (
    [featureId]       INT            IDENTITY (1, 1) NOT NULL,
    [featureName]     NVARCHAR (50)  NULL,
    [featureImage]    NVARCHAR (255) NULL,
    [fkfeatureTypeId] INT            NULL,
    [featureValues]   NVARCHAR (500) NULL,
    [fkCategoryId]    INT            NULL,
    CONSTRAINT [PK_attribute] PRIMARY KEY CLUSTERED ([featureId] ASC) WITH (FILLFACTOR = 90)
);


GO
CREATE NONCLUSTERED INDEX [foreignKeys]
    ON [dbo].[feature]([fkfeatureTypeId] ASC, [fkCategoryId] ASC) WITH (FILLFACTOR = 90);

