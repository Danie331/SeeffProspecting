CREATE TABLE [dbo].[category_action] (
    [category_actionId] INT IDENTITY (1, 1) NOT NULL,
    [fkCategoryId]      INT NOT NULL,
    [fkActionId]        INT NOT NULL,
    CONSTRAINT [PK_category_action] PRIMARY KEY CLUSTERED ([category_actionId] ASC) WITH (FILLFACTOR = 90)
);

