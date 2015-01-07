CREATE TABLE [dbo].[propctrl_suburbs] (
    [propCtrlSeeffSuburbId] INT            IDENTITY (1, 1) NOT NULL,
    [propCtrlAreaId]        INT            NOT NULL,
    [seeffAreaId]           INT            DEFAULT (0) NOT NULL,
    [propCtrlCountry]       NVARCHAR (100) NOT NULL,
    [propCtrlProvince]      NVARCHAR (100) NOT NULL,
    [propCtrlCity]          NVARCHAR (100) NOT NULL,
    [propCtrlSuburb]        NVARCHAR (100) NOT NULL,
    [lastUpdated]           DATETIME       DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_propctrl_suburbs] PRIMARY KEY CLUSTERED ([propCtrlSeeffSuburbId] ASC)
);

