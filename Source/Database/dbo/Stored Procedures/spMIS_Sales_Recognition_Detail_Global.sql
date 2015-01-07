
-- ================================================================================================
-- Author:		Gustav Swanepoel
-- Create date: 2013-10-04
-- Description:	Essentially the same stored proc
--				as spMIS_Sales_Recognition_Detail
--				this one provides for "stripping"
--				out the partnerships and returns
--				only individualised (?) agents in
--				the result set
--				
--	-- ,partnership.partnership_name AS [Agent] --//USE PARTNERSHIP NAME HERE
--	   ,user_registration.user_preferred_name + ' ' + user_registration.user_surname AS [Agent]
-- ================================================================================================


CREATE PROCEDURE [dbo].[spMIS_Sales_Recognition_Detail_Global] 
	-- Add the parameters for the stored procedure here
	@TransType NVARCHAR(50) = 'SALE'
	,@intYear INT = 2013
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	IF OBJECT_ID('dbo.reports_Sales_Recognition_Detail_Global','U') IS NOT NULL DROP TABLE reports_Sales_Recognition_Detail_Global

CREATE TABLE reports_Sales_Recognition_Detail_Global
(
 Region VARCHAR(50)
,License_ID INT
,License NVARCHAR(50)
,[Agent] NVARCHAR(100)
,[Recognition Unit] DECIMAL(18,6)
,[Recognition Amount] DECIMAL(18,6)
,[SPS Seeff Referral] BIT
,[SPS Referral Comm] DECIMAL(18,2)
,[Comm Paid] DECIMAL(18,2)
,[SPS Comm Amount] DECIMAL(18,2)
,[SPS Percentage Split] DECIMAL(18,2)
,[SPS Transaction Type] NVARCHAR(100)
,[SPS Transaction Division] NVARCHAR(100)
,[Reg ID] INT
,[Month] INT
,[Partner Count] INT
,[SPS Referral Type] NVARCHAR(50)
,[SPS Transaction ID] NVARCHAR(50)
,[SPS Transaction Ref] NVARCHAR(50)
,[SPS License Deal No] NVARCHAR(100)
,[SPS Sold Date]DATETIME
,[SPS Selling Price] DECIMAL(18,2)
,[SPS Property ID] BIGINT
,[SPS Property Address] NVARCHAR(500)
,[SPS Created Date] DATETIME
,[SPS Reporting Date] DATETIME
,[Recognise] BIT
,[Smart Pass ID] INT
,[Smart Pass License ID To] INT
,[Smart Pass License ID From] INT
,[InsertOn] DATETIME
,[Sale Comm @ 7%] DECIMAL(18,6)
)
	
CREATE NONCLUSTERED INDEX Index01
ON [dbo].[sps_transaction] ([sps_cancelled],[sps_comm_amount])
INCLUDE ([sps_transaction_id],[sps_transaction_ref],[sps_lic_deal_no],[sps_refferal_type],[sps_referral_comm],[sps_sold_date],[sps_selling_price],[sps_percentage_split],[sps_transaction_type],[sps_transaction_division],[sps_property_id],[sps_property_address],[sps_created_date],[branch_id],[sps_reporting_date],[sps_seeff_referral],[smart_pass_id])

CREATE NONCLUSTERED INDEX Index02
ON [dbo].[sps_transaction] ([sps_cancelled],[sps_comm_amount])
INCLUDE ([sps_transaction_id],[sps_transaction_ref],[sps_lic_deal_no],[sps_refferal_type],[sps_referral_comm],[sps_sold_date],[sps_selling_price],[sps_percentage_split],[sps_transaction_type],[sps_transaction_division],[sps_property_id],[sps_property_address],[sps_created_date],[branch_id],[sps_reporting_date],[sps_seeff_referral],[smart_pass_id])	

INSERT INTO reports_Sales_Recognition_Detail_Global

SELECT DISTINCT
	  license.region
	  ,license.license_id
	  ,[license].license_name AS [License]
	  -- ,partnership.partnership_name AS [Agent] --//USE PARTNERSHIP NAME HERE --GW: 2013-10-04
	   ,user_registration.user_preferred_name + ' ' + user_registration.user_surname AS [Agent]
	   ,0 AS [Recognition Unit]						
      , 0 AS [Recognition Amount]
	 ,[sps_seeff_referral]  AS [SPS Seeff Referral]
	 ,[sps_referral_comm]  AS [SPS Referral Comm]
	 ,[sps_agent_split].[comm_paid]  AS [Comm Paid]
	 ,[sps_comm_amount]  AS [SPS Comm Amount]
	 ,[sps_percentage_split] AS [SPS Percentage Split]
     ,[sps_transaction_type] AS [SPS Transaction Type]
     ,[sps_transaction_division]  AS [SPS Transaction Division]
	 ,[sps_agent_split].[registration_id]  AS [Reg ID]
	 , DATEPART(M,[sps_reporting_date]) AS Month
     , partner_count AS [Partner Count]
     ,[sps_refferal_type]  AS [SPS Referral Type]
	  ,[sps_transaction_id] AS [SPS Transaction ID]
      ,[sps_transaction].[sps_transaction_ref]  AS [SPS Transaction Ref]
      ,[sps_lic_deal_no]  AS [SPS License Deal No]    
      ,[sps_sold_date]AS [SPS Sold Date]
      ,[sps_selling_price] AS [SPS Selling Price]
      ,[sps_property_id]  AS [SPS Property ID]
      ,[sps_property_address]  AS [SPS Property Address]
      ,[sps_created_date]AS [SPS Created Date]
      ,[sps_reporting_date] AS [SPS Reporting Date]
      ,[sps_agent_split].[recognise] AS [Recognise]
      ,[sps_transaction].[smart_pass_id] AS [Smart Pass ID]
      ,[smart_pass].[license_id_to] AS [Smart Pass License ID To]
      ,[smart_pass].[license_id_from] AS [Smart Pass License ID From]
      ,GETDATE()
	  ,dbo.sales_comm_at_seven ([sps_refferal_type],[sps_referral_comm],[sps_comm_amount]) AS [Sale @ 7]          
FROM  
		user_registration 
INNER JOIN
		sps_transaction 
INNER JOIN
		sps_agent_split 
ON  
		sps_transaction.sps_transaction_ref = sps_agent_split.sps_transaction_ref
JOIN
		branch on sps_transaction.branch_id = branch.branchId
JOIN
		license_branches ON branch.branchId = license_branches.branch_id
JOIN
		license on license_branches.license_id = license.license_id 
ON		
		user_registration.registration_id = sps_agent_split.registration_id 
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
AND
	(user_registration.confirmation = 'Y')	
AND
	(sps_Agent_split.comm_paid <> 0)

		
INSERT reports_Sales_Recognition_Detail_Global (
Region
,LICENSE_ID
,License
,[Agent]
,[Recognition Unit]
,[Recognition Amount]
,[SPS Seeff Referral]
,[SPS Referral Comm]
,[Comm Paid]
,[SPS Comm Amount]
,[SPS Percentage Split]
,[SPS Transaction Type]
,[SPS Transaction Division]
,[Reg ID]
,[Month]
,[Partner Count]
,[SPS Referral Type]
,[SPS Transaction ID]
,[SPS Transaction Ref]
,[SPS License Deal No]
,[SPS Sold Date]
,[SPS Selling Price]
,[SPS Property ID]
,[SPS Property Address]
,[SPS Created Date]
,[SPS Reporting Date]
,[Recognise]
,[Smart Pass ID]
,[Smart Pass License ID To]
,[Smart Pass License ID From]
,[InsertOn]
,[Sale Comm @ 7%] 
)

SELECT 
	  license.region
	  , license.license_id
	  ,[license].license_name AS [License]
	  ,[user_registration].[user_preferred_name] + ' ' + user_registration.user_surname AS [Agent] 
	  ,0 AS [Recognition Unit]
      ,0 AS [Recognition Amount]
	 ,[sps_seeff_referral]  AS [SPS Seeff Referral]	
	 ,[sps_referral_comm]  AS [SPS Referral Comm]
     ,[sps_agent_split].[comm_paid]  AS [Comm Paid]
     ,[sps_comm_amount]  AS [SPS Comm Amount]
     ,[sps_percentage_split] AS [SPS Percentage Split]
     ,[sps_transaction_type] AS [SPS Transaction Type]
     ,[sps_transaction_division]  AS [SPS Transaction Division]
     ,[sps_agent_split].[registration_id]  AS [Reg ID]
     , DATEPART(M,[sps_reporting_date]) AS Month
     , 1 AS [Partner Count]
     ,[sps_refferal_type]  AS [SPS Referral Type]
	 ,[sps_transaction_id] AS [SPS Transaction ID]
     ,[sps_transaction].[sps_transaction_ref]  AS [SPS Transaction Ref]
     ,[sps_lic_deal_no]  AS [SPS License Deal No]
     ,[sps_sold_date] AS [SPS Sold Date]
     ,[sps_selling_price] AS [SPS Selling Price]   
     ,[sps_property_id]  AS [SPS Property ID]
     ,[sps_property_address]  AS [SPS Property Address]
     ,[sps_created_date]AS [SPS Created Date]
     ,[sps_reporting_date] AS [SPS Reporting Date]
     ,[sps_agent_split].[recognise] AS [Recognise]  
     ,[sps_transaction].[smart_pass_id] AS [Smart Pass ID]
     ,[smart_pass].[license_id_to] AS [Smart Pass License ID To]
     ,[smart_pass].[license_id_from] AS [Smart Pass License ID From]  
     ,GETDATE()
	 ,dbo.sales_comm_at_seven ([sps_refferal_type],[sps_referral_comm],[sps_comm_amount]) AS [Sale @ 7%]   
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
	(sps_Agent_split.comm_paid <> 0)
AND
	(sps_agent_split.recognise = 1)
--GW: 2013-09-10
--GW: 2014-01-17 initially commented out, but returned
-- all transactions of agents - only interested in those where
--confirmation is still a  go 				
AND
user_registration.confirmation LIKE 'Y'
AND
		user_registration.registration_id NOT IN (SELECT partnership_group.registration_id from partnership_group JOIN partnership on partnership_group.partnership_id = partnership.partnership_id and partnership.end_date IS NULL)	
ORDER BY [SPS Transaction Ref] 

UPDATE reports_Sales_Recognition_Detail_Global 
		SET [Recognition Unit] = (SELECT dbo.[fnMIS_SalesRecognition]([SPS Transaction Ref],[Reg ID],[Comm Paid],[Sale Comm @ 7%],1,[SPS Referral Type]))
		,[Recognition Amount] = (SELECT dbo.[fnMIS_SalesRecognition]([SPS Transaction Ref],[Reg ID],[Comm Paid],[Sale Comm @ 7%],0,[SPS Referral Type]))
	



DROP INDEX Index01
ON [dbo].[sps_transaction]

DROP INDEX Index02
ON [dbo].[sps_transaction]

END

