CREATE TABLE [dbo].[espServiceProvider] (
    [serviceProviderId]        INT           IDENTITY (1, 1) NOT NULL,
    [serviceProviderName]      VARCHAR (150) NOT NULL,
    [serviceProviderShortName] VARCHAR (100) NULL,
    [serviceProviderActive]    TINYINT       NULL
);

