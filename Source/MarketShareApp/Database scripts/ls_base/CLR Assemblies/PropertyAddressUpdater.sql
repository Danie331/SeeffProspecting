-- ensure that the database has the schema shown below, when generating the new records ensure that they too contain this schema.
use master
GO

EXEC sp_configure 'clr enabled', '1'
reconfigure
GO

CREATE ASYMMETRIC KEY SEEFFGLOBAL_ASM_KEY FROM EXECUTABLE FILE = 'C:\Users\Seeff\Google Drive\Adam VB\C# Converted projects - 2014\SeeffGlobal\SeeffGlobal\bin\Debug\SeeffGlobal.dll'; -- DLL must be signed
CREATE LOGIN SEEFFGLOBAL_ASM_LOGIN FROM ASYMMETRIC KEY SEEFFGLOBAL_ASM_KEY;
GRANT EXTERNAL ACCESS ASSEMBLY TO SEEFFGLOBAL_ASM_LOGIN;

CREATE ASYMMETRIC KEY PROP_ADDRESSES_ASM_KEY FROM EXECUTABLE FILE = 'C:\Users\Seeff\Google Drive\Dev\GetPropertyAddress\GetPropertyAddress\bin\Debug\GetPropertyAddress.dll'; -- DLL must be signed
CREATE LOGIN PROP_ADDRESSES_ASM_LOGIN FROM ASYMMETRIC KEY PROP_ADDRESSES_ASM_KEY;
GRANT UNSAFE ASSEMBLY TO PROP_ADDRESSES_ASM_LOGIN;

use ls_base
GO

CREATE ASSEMBLY PropertyAddressUpdater from 'C:\Users\Seeff\Google Drive\Dev\GetPropertyAddress\GetPropertyAddress\bin\Debug\GetPropertyAddress.dll'
WITH PERMISSION_SET = UNSAFE
GO