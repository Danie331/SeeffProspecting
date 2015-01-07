CREATE TABLE [dbo].[news-2009oct23] (
    [newsId]         INT            IDENTITY (1, 1) NOT NULL,
    [newsTitle]      NVARCHAR (250) NULL,
    [newsDate]       SMALLDATETIME  NULL,
    [newsIntro]      TEXT           NULL,
    [newsCopy]       TEXT           NULL,
    [newsInsertDate] SMALLDATETIME  NOT NULL
);

