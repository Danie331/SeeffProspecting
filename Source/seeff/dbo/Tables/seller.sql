CREATE TABLE [dbo].[seller] (
    [sellerId]   INT           IDENTITY (1, 1) NOT NULL,
    [sellerName] NVARCHAR (50) NULL,
    CONSTRAINT [PK_seller] PRIMARY KEY CLUSTERED ([sellerId] ASC) WITH (FILLFACTOR = 90)
);

