CREATE TABLE [dbo].[categories] (
    [categoryId]        INT           IDENTITY (1, 1) NOT NULL,
    [categoryName]      VARCHAR (255) NULL,
    [categoryShortDesc] TEXT          NULL,
    [categoryParentId]  INT           CONSTRAINT [DF_categories_categoryParentId] DEFAULT (0) NULL,
    [categoryActive]    TINYINT       NULL,
    [categoryDateAdded] DATETIME      NULL,
    [categorySortOrder] INT           NULL,
    CONSTRAINT [PK_categories] PRIMARY KEY CLUSTERED ([categoryId] ASC) WITH (FILLFACTOR = 90)
);

