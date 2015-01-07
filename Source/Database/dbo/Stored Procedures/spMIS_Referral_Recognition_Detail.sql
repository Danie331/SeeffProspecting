-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Referral_Recognition_Detail] 
	-- Add the parameters for the stored procedure here
	@TransType NVARCHAR(50) = 'SALE'
	,@intYear INT = 2013
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	IF OBJECT_ID('reports_Referral_Recognition_Detail') IS NOT NULL
    DROP TABLE reports_Referral_Recognition_Detail


CREATE TABLE reports_Referral_Recognition_Detail

( region NVARCHAR(50)
,license_id INT
,license_name NVARCHAR(50)
,smart_pass_id INT
,transaction_division VARCHAR(50)
,month_reported INT
,sps_refferal_type NVARCHAR(50)
,Agent_Partnership NVARCHAR(50)
,registration_id INT
,sps_transaction_ref NVARCHAR(50)
,Agent_Count INT
,Recognition_Unit DECIMAL(16,2)
)

INSERT INTO reports_Referral_Recognition_Detail

SELECT		DISTINCT  license.region
			, license.license_id
			, license.license_name
			, sps_transaction.smart_pass_id
			, sps_transaction.sps_transaction_division
			, DATEPART(MM,sps_transaction.sps_reporting_date)
			, sps_transaction.sps_refferal_type
			, partnership.partnership_name
			, partnership.partnership_id
			, sps_agent_split.sps_transaction_ref
			,partner_count
			,NULL
FROM         
			sps_transaction INNER JOIN
                      branch ON sps_transaction.branch_id = branch.branchId INNER JOIN
                      license_branches ON branch.branchId = license_branches.branch_id INNER JOIN
                      license ON license_branches.license_id = license.license_id INNER JOIN
                      sps_agent_split ON sps_transaction.sps_transaction_ref = sps_agent_split.sps_transaction_ref INNER JOIN
                      user_registration ON sps_agent_split.registration_id = user_registration.registration_id
LEFT OUTER JOIN
	partnership 
INNER JOIN
   partnership_group 
ON partnership.partnership_id = partnership_group.partnership_id 
ON sps_agent_split.registration_id = partnership_group.registration_id
LEFT JOIN
		smart_pass ON sps_transaction.smart_pass_id = smart_pass.smart_pass_id
WHERE        
	(partnership.end_date IS NULL)
AND 
	(sps_transaction.sps_transaction_type LIKE @TransType)
AND 
	(partnership.section LIKE 'Sales')
AND
	(DATEPART(YYYY,[sps_transaction].[sps_reporting_date]) = @intYear)
AND
	(sps_cancelled = 0)
AND
	(sps_agent_split.recognise = 1)
AND     (sps_transaction.smart_pass_id IS NOT NULL) AND ((sps_transaction.sps_refferal_type LIKE 'External You Paid') OR (sps_transaction.sps_refferal_type LIKE 'External paid to you'))
AND user_registration.registration_id <> 1667

UNION ALL

SELECT     license.region
			, license.license_id
			, license.license_name
			, sps_transaction.smart_pass_id
			, sps_transaction.sps_transaction_division
			, DATEPART(MM,sps_transaction.sps_reporting_date)
			, sps_transaction.sps_refferal_type
			, [user_registration].[user_preferred_name] + ' ' + user_registration.user_surname AS [Agent]
            , sps_agent_split.registration_id
            , sps_agent_split.sps_transaction_ref
            , 1
			,NULL
FROM 
	   [boss].[dbo].[sps_agent_split]
JOIN
		user_registration on sps_agent_split.registration_id = user_registration.registration_id
JOIN
		sps_transaction ON sps_agent_split.sps_transaction_ref = sps_transaction.sps_transaction_ref
JOIN
		branch on sps_transaction.branch_id = branch.branchId
JOIN
		license_branches ON branch.branchId = license_branches.branch_id
JOIN
		license on license_branches.license_id = license.license_id
LEFT JOIN
		smart_pass ON sps_transaction.smart_pass_id = smart_pass.smart_pass_id
WHERE
		[sps_agent_split].[registration_id] <> 1667
AND
		sps_transaction_type = @TransType
AND
		DATEPART(YYYY,[sps_transaction].[sps_reporting_date]) = @intYear				
AND
		(sps_cancelled = 0)
AND
	(sps_agent_split.recognise = 1)
AND
user_registration.confirmation LIKE 'Y'
AND
		user_registration.registration_id NOT IN (SELECT partnership_group.registration_id from partnership_group JOIN partnership on partnership_group.partnership_id = partnership.partnership_id and partnership.end_date IS NULL)
	AND     (sps_transaction.smart_pass_id IS NOT NULL) AND ((sps_transaction.sps_refferal_type LIKE 'External You Paid') OR (sps_transaction.sps_refferal_type LIKE 'External Paid to You'))
ORDER BY sps_transaction_ref

UPDATE reports_Referral_Recognition_Detail
	SET Recognition_Unit = (SELECT [dbo].[fnMIS_ReferralRecog_Unit](sps_transaction_ref,sps_refferal_type,registration_id,Agent_Count))

SELECT * 
FROM reports_Referral_Recognition_Detail
--WHERE sps_transaction_ref LIKE '84243434-4bf4-457b-9f8f-d208d5915ee1'
ORDER BY sps_transaction_ref


END
