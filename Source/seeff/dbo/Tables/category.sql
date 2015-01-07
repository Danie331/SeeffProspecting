CREATE TABLE [dbo].[category] (
    [categoryId]           INT           IDENTITY (1, 1) NOT NULL,
    [categoryName]         NVARCHAR (50) NULL,
    [categoryDisplayOrder] INT           CONSTRAINT [DF_category_categoryDisplayOrder] DEFAULT (99) NULL,
    [categoryCanRent]      BIT           CONSTRAINT [DF_category_categoryCanRent] DEFAULT (0) NULL,
    CONSTRAINT [PK_category] PRIMARY KEY CLUSTERED ([categoryId] ASC) WITH (FILLFACTOR = 90)
);

