CREATE TABLE [dbo].[news_archive] (
    [newsArchiveId]         INT            IDENTITY (1, 1) NOT NULL,
    [newsArchiveTitle]      NVARCHAR (250) NULL,
    [newsArchiveDate]       SMALLDATETIME  NULL,
    [newsArchiveIntro]      TEXT           NULL,
    [newsArchiveCopy]       TEXT           NULL,
    [newsArchiveInsertDate] SMALLDATETIME  NOT NULL,
    PRIMARY KEY CLUSTERED ([newsArchiveId] ASC)
);

