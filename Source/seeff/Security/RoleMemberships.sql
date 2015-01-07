ALTER ROLE [db_owner] ADD MEMBER [seeff_com];


GO
ALTER ROLE [db_datareader] ADD MEMBER [IIS APPPOOL\m.seeff.com];


GO
ALTER ROLE [db_datareader] ADD MEMBER [IIS APPPOOL\mymail.seeff.com];


GO
ALTER ROLE [db_datareader] ADD MEMBER [IIS APPPOOL\integration.seeff.com];


GO
ALTER ROLE [db_datareader] ADD MEMBER [IIS APPPOOL\smartadmin.seeff.com];


GO
ALTER ROLE [db_datareader] ADD MEMBER [IIS APPPOOL\staging.seeff.com];


GO
ALTER ROLE [db_datawriter] ADD MEMBER [IIS APPPOOL\m.seeff.com];


GO
ALTER ROLE [db_datawriter] ADD MEMBER [IIS APPPOOL\mymail.seeff.com];


GO
ALTER ROLE [db_datawriter] ADD MEMBER [IIS APPPOOL\integration.seeff.com];


GO
ALTER ROLE [db_datawriter] ADD MEMBER [IIS APPPOOL\smartadmin.seeff.com];


GO
ALTER ROLE [db_datawriter] ADD MEMBER [IIS APPPOOL\staging.seeff.com];

