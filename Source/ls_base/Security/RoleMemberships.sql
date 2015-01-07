ALTER ROLE [db_owner] ADD MEMBER [LIGHTSTONE\cornev];


GO
ALTER ROLE [db_datareader] ADD MEMBER [LIGHTSTONE\antonf];


GO
ALTER ROLE [db_datareader] ADD MEMBER [IIS APPPOOL\smartadmin.seeff.com];


GO
ALTER ROLE [db_datareader] ADD MEMBER [IIS APPPOOL\staging.seeff.com];


GO
ALTER ROLE [db_datawriter] ADD MEMBER [IIS APPPOOL\smartadmin.seeff.com];


GO
ALTER ROLE [db_datawriter] ADD MEMBER [IIS APPPOOL\staging.seeff.com];

