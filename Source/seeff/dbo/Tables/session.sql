CREATE TABLE [dbo].[session] (
    [sessionId]         NUMERIC (18) IDENTITY (1, 1) NOT NULL,
    [stateId]           NUMERIC (18) NULL,
    [propertyReference] NUMERIC (18) NULL,
    [sessionDate]       DATETIME     CONSTRAINT [DF_session_sessionDate] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_session] PRIMARY KEY CLUSTERED ([sessionId] ASC) WITH (FILLFACTOR = 90)
);

