CREATE TABLE [dbo].[documents] (
    [documentId]        INT           IDENTITY (1, 1) NOT NULL,
    [documentName]      VARCHAR (255) NULL,
    [documentShortDesc] TEXT          NULL,
    [fkCategoryId]      INT           NULL,
    [documentFile]      VARCHAR (255) NULL,
    [documentActive]    TINYINT       NULL,
    [documentDateAdded] DATETIME      NULL,
    CONSTRAINT [PK_documents] PRIMARY KEY CLUSTERED ([documentId] ASC) WITH (FILLFACTOR = 90)
);

