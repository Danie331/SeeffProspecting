CREATE TABLE [dbo].[tmpPropertyImages] (
    [tmpImagesId]       INT            IDENTITY (1, 1) NOT NULL,
    [tmpImagesName]     NVARCHAR (500) NULL,
    [propertyReference] NUMERIC (18)   NULL,
    [tmpImagesWidth]    INT            NULL,
    [tmpImagesHeight]   INT            NULL,
    [tmpImagesValid]    BIT            CONSTRAINT [DF_tmpPropertyImages_tmpImagesValid] DEFAULT (0) NULL,
    CONSTRAINT [PK_tmpPropertyImages] PRIMARY KEY CLUSTERED ([tmpImagesId] ASC) WITH (FILLFACTOR = 90)
);

