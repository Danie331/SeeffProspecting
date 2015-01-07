CREATE TABLE [dbo].[competitor] (
    [competitorId]   INT           IDENTITY (1, 1) NOT NULL,
    [competitorName] NVARCHAR (50) NULL,
    CONSTRAINT [PK_competitor] PRIMARY KEY CLUSTERED ([competitorId] ASC) WITH (FILLFACTOR = 90)
);

