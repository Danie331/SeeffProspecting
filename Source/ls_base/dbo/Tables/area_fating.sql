CREATE TABLE [dbo].[area_fating] (
    [area_fating_id] INT IDENTITY (1, 1) NOT NULL,
    [area_id]        INT NOT NULL,
    [fated]          INT NOT NULL,
    [unfated]        INT NOT NULL,
    PRIMARY KEY CLUSTERED ([area_fating_id] ASC)
);

