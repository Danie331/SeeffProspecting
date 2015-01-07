-- =============================================
-- Author:		GW Swanepoel
-- Create date: 
-- Description:	Updates table reports_rental_recognition
--				with Seeff Centurion and Seeff Pinelands
--				rental recognition records
-- =============================================
Create PROCEDURE [dbo].[spMIS_UPDATE_rental_recognition_Centurion] 
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
--First delete the all the records for Centurion and Pinelands...
--Centurion = 7
	DELETE FROM reports_rental_recognition WHERE license_id IN (7)

--Before updating all the records again records
INSERT reports_rental_recognition 
	(
	   [license_id]
      ,[deal_number]
      ,[contract_start_date]
      ,[contract_end_date]
      ,[comm]
      ,[agent]
      ,[entity_type]
      ,[agent_registration_id]
      ,[partnership_registration_id]
      ,[created_by]
      ,[created_date]
      ,[units]     
      )

SELECT     
		  license.license_id as [license_id]
		, sps_lic_deal_no[deal_number]
		, sps_listed_date AS [contract_start_date]
		, sps_sold_date AS [contract_end_date]
		, CAST((ISNULL(sps_tran.sps_rental_management_fee,0)* (dbo.fnMIS_MonthsApart (sps_listed_date,sps_sold_date))/0.07) +  (ISNULL(sps_tran.sps_comm_amount,0)/0.07)AS DECIMAL(16,2)) AS [Recognition Amount]	
		, dbo.fnMIS_AgentName (sps_tran.sps_transaction_ref) AS Agent 
		, recognition_agent.entity_type
		, [dbo].[fnMIS_AgentRegID] (sps_tran.sps_transaction_ref)  AS Agent_Registration_ID
		, NULL  AS Parternship_Registration_ID
		, user_registration.registration_id AS [Captured By]
		, GETDATE() AS [created_date]
		,1
FROM    
	      sps_transaction sps_tran
INNER JOIN
        license_branches ON sps_tran.branch_id = license_branches.branch_id 
INNER JOIN
        license ON license_branches.license_id = license.license_id 
INNER JOIN
        branch ON sps_tran.branch_id = branch.branchId 
INNER JOIN
        user_registration ON sps_tran.created_by = user_registration.registration_id
JOIN
		recognition_agent ON sps_tran.sps_transaction_ref = recognition_agent.sps_transaction_ref	
WHERE     
				(sps_tran.sps_cancelled = 0) 
AND 
				(sps_tran.sps_transaction_type = 'Rental') 

--good for testing purposes / to pinpoint specific month(s)
--but for now unnecessary
--AND 
--				(YEAR(sps_tran.sps_listed_date) = 2012) 
--AND 
--				(MONTH(sps_tran.sps_listed_date) BETWEEN 1 AND 12)

AND
				license.license_id IN (7)
AND 				
				[dbo].[fnMIS_AgentRegID] (sps_tran.sps_transaction_ref) <> 1667
--1667 = "No Agent" which we don't want to insert in to the rental recognition stats
				
GROUP BY				
				license.license_id
				,license.license_name
				,RIGHT(CONVERT(VARCHAR(10), sps_tran.sps_listed_date, 105), 7)
				,user_registration.user_preferred_name
				,user_registration.user_surname
				,sps_tran.sps_transaction_type
				,dbo.fnMIS_AgentName(sps_tran.sps_transaction_ref)
				,sps_listed_date 
				,sps_sold_date
				,sps_lic_deal_no  
				,sps_tran.sps_refferal_type
				,recognition_agent.entity_type
				,user_registration.user_name + ' ' + user_registration.user_surname
				,user_registration.registration_id
				,sps_tran.sps_transaction_ref
				,sps_tran.sps_rental_management_fee
				,sps_tran.sps_comm_amount
				
ORDER BY  Agent, deal_number

END
