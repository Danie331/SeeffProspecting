ALTER ROLE [db_owner] ADD MEMBER [prospecting_user];


GO
ALTER ROLE [db_datareader] ADD MEMBER [prospecting_user];


GO
ALTER ROLE [db_datawriter] ADD MEMBER [prospecting_user];

