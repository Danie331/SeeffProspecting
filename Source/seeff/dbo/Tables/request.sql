CREATE TABLE [dbo].[request] (
    [requestId]            INT           IDENTITY (1, 1) NOT NULL,
    [requestIdent]         BIGINT        NULL,
    [requestTitle]         VARCHAR (200) NULL,
    [requestCopy]          NTEXT         NULL,
    [fkBranchId]           INT           NULL,
    [fkAgentId]            INT           NULL,
    [requestDateLogged]    DATETIME      NULL,
    [requestDateUpdated]   DATETIME      NULL,
    [requestDateClosed]    DATETIME      NULL,
    [requestBugUError]     VARCHAR (10)  NULL,
    [requestHours]         FLOAT (53)    CONSTRAINT [DF_request_requestHours] DEFAULT (0.00) NULL,
    [requestLastEdittedBy] INT           CONSTRAINT [DF_request_requestLastEdittedBy] DEFAULT (0) NULL,
    [requestActive]        BIT           CONSTRAINT [DF_request_requestActive] DEFAULT (1) NULL,
    [requestVersion]       INT           CONSTRAINT [DF_request_requestVersion] DEFAULT (0) NULL,
    [requestLatestVersion] BIT           CONSTRAINT [DF_request_requestLatestVersion] DEFAULT (1) NULL,
    [requestClosed]        BIT           CONSTRAINT [DF_request_requestClosed] DEFAULT (0) NULL,
    CONSTRAINT [PK_request] PRIMARY KEY CLUSTERED ([requestId] ASC) WITH (FILLFACTOR = 90)
);

