CREATE TABLE [dbo].[seoUrl] (
    [seoUrlId] INT            IDENTITY (1, 1) NOT NULL,
    [seoUrl]   NVARCHAR (200) NULL,
    [seoGoto]  NVARCHAR (200) NULL,
    CONSTRAINT [PK_seoUrl] PRIMARY KEY CLUSTERED ([seoUrlId] ASC) WITH (FILLFACTOR = 90)
);

