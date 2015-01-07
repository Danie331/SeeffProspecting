CREATE TABLE [dbo].[license_intranet] (
    [intranet_id]   INT           IDENTITY (1, 1) NOT NULL,
    [license_id]    INT           NOT NULL,
    [intranet_desc] VARCHAR (250) NOT NULL,
    [intranet_url]  VARCHAR (250) NOT NULL
);

