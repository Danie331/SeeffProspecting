CREATE TABLE [dbo].[news] (
    [newsId]         INT            IDENTITY (1, 1) NOT NULL,
    [newsTitle]      NVARCHAR (250) NULL,
    [newsDate]       SMALLDATETIME  NULL,
    [newsIntro]      TEXT           NULL,
    [newsCopy]       TEXT           NULL,
    [newsInsertDate] SMALLDATETIME  CONSTRAINT [DF_news_newsInsertDate] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_news] PRIMARY KEY CLUSTERED ([newsId] ASC) WITH (FILLFACTOR = 90)
);

