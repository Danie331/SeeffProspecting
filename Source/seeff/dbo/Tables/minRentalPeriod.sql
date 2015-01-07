CREATE TABLE [dbo].[minRentalPeriod] (
    [minRentalPeriodId]   NUMERIC (18) IDENTITY (1, 1) NOT NULL,
    [minRentalPeriodName] VARCHAR (50) NULL,
    [MinRentalPeriodDays] SMALLINT     NULL,
    CONSTRAINT [PK_minRentalPeriod] PRIMARY KEY CLUSTERED ([minRentalPeriodId] ASC)
);

