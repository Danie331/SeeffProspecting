CREATE TABLE [dbo].[social_media] (
    [social_media_id]       INT            IDENTITY (1, 1) NOT NULL,
    [fkBranchId]            INT            NOT NULL,
    [social_media_platform] NVARCHAR (200) NULL,
    [social_media_url]      VARCHAR (1000) NULL,
    [date_created]          DATETIME       DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([social_media_id] ASC)
);

