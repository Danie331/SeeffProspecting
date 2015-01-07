CREATE TABLE [dbo].[erf_nos_temp] (
    [property_id] INT           NOT NULL,
    [erf_no]      VARCHAR (MAX) NULL,
    [id]          INT           IDENTITY (1, 1) NOT NULL,
    [portion]     VARCHAR (MAX) NULL,
    CONSTRAINT [PK__erf_nos___3213E83FE1DA8D60] PRIMARY KEY CLUSTERED ([id] ASC)
);

