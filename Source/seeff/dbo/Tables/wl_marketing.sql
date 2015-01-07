CREATE TABLE [dbo].[wl_marketing] (
    [pk_marketing_Id] INT          IDENTITY (1, 1) NOT NULL,
    [vendor]          VARCHAR (50) NOT NULL,
    [timestamp]       DATETIME     CONSTRAINT [DF_wl_marketing_timestamp] DEFAULT (getdate()) NOT NULL
);

