CREATE TABLE [dbo].[license_sub] (
    [license_sub_id]   INT           IDENTITY (1, 1) NOT NULL,
    [license_id]       INT           NOT NULL,
    [sub_license_name] VARCHAR (150) NOT NULL,
    [Status]           CHAR (1)      DEFAULT ('A') NULL,
    [datasheet_link]   VARCHAR (150) NULL,
    [percentage_quota] INT           NULL,
    [cluster_group]    INT           NOT NULL
);

