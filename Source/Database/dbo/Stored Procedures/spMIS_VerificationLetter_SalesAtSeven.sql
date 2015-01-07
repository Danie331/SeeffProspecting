-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE spMIS_VerificationLetter_SalesAtSeven 
	-- Add the parameters for the stored procedure here
	@BeginDate NVARCHAR(20) = '2011-11-01'
	,@EndDate NVARCHAR(20) = '2012-10-31'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT   license.license_name AS License
, license.region as Region
, license.license_name + ' - ' + license.Status AS License_Name_Status
, user_registration.user_name [OwnerName]
, user_registration.user_name + ' ' + user_registration.user_surname AS [Owner]
, SUM(dbo.sales_comm_at_seven(sps_transaction.sps_refferal_type
           , sps_transaction.sps_referral_comm
           , sps_transaction.sps_comm_amount)) AS [Sale At Seven Perc]
FROM sps_transaction
INNER Join license_branches ON sps_transaction.branch_id = license_branches.branch_id
INNER Join license ON license_branches.license_id = license.license_id
INNER Join licensee ON license.license_id = licensee.license_id
INNER Join user_registration ON licensee.registration_id = user_registration.registration_id
WHERE
(sps_transaction.sps_cancelled = 0) AND
(sps_transaction.sps_transaction_type = 'Sale') AND
(ISNULL(sps_transaction.sps_comm_amount, 0) > 0) AND
(ISNULL(sps_transaction.sps_selling_price, 0) > 0) AND
(sps_transaction.branch_id NOT IN (260, 263, 277)) AND
(sps_transaction.sps_reporting_date >= @BeginDate)
AND
(sps_transaction.sps_reporting_date <= @EndDate)
Group BY
license.license_name
, user_registration.user_name 
, user_registration.user_surname
, license.region
, license.status
, user_registration.user_surname
END
