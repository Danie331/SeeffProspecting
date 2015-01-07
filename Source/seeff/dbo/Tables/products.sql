CREATE TABLE [dbo].[products] (
    [productId]          INT             IDENTITY (1, 1) NOT NULL,
    [productName]        VARCHAR (255)   NULL,
    [productCode]        VARCHAR (50)    NULL,
    [productDesc]        TEXT            NULL,
    [productImage]       VARCHAR (255)   NULL,
    [productPrice]       DECIMAL (19, 2) NULL,
    [productQuantity]    DECIMAL (19, 2) NULL,
    [productOrderByDate] DATETIME        NULL,
    [productOrderForm]   VARCHAR (255)   NULL,
    [fkCategoryId]       INT             CONSTRAINT [DF_products_fkCategoryId] DEFAULT (0) NULL,
    [productActive]      TINYINT         NULL,
    [productDateAdded]   DATETIME        NULL,
    [productSortOrder]   INT             NULL,
    CONSTRAINT [PK_products] PRIMARY KEY CLUSTERED ([productId] ASC) WITH (FILLFACTOR = 90)
);

