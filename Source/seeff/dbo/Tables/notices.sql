CREATE TABLE [dbo].[notices] (
    [noticeId]        INT           IDENTITY (1, 1) NOT NULL,
    [noticeTitle]     VARCHAR (255) NULL,
    [noticePostDate]  DATETIME      NULL,
    [noticeText]      TEXT          NULL,
    [noticeFile]      VARCHAR (255) NULL,
    [noticeDateAdded] DATETIME      NOT NULL,
    [noticeActive]    SMALLINT      NULL
);

