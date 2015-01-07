CREATE TABLE [dbo].[boss_search] (
    [search_id]     INT           IDENTITY (1, 1) NOT NULL,
    [headline]      VARCHAR (64)  NOT NULL,
    [topic_desc]    VARCHAR (512) NOT NULL,
    [topic_link]    VARCHAR (256) NOT NULL,
    [view_group]    VARCHAR (64)  NOT NULL,
    [inserted_date] DATETIME      NOT NULL,
    [inserted_by]   INT           NOT NULL,
    [updated_date]  DATETIME      NOT NULL,
    [updated_by]    INT           NOT NULL
);

