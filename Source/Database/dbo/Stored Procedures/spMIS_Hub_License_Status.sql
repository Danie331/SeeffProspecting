




-- =============================================
-- Author:		Gustav Swanepoel
-- Create date: 2012-07-16
-- Description:	Returns listing of the various
--				statuses for the Hub transactions
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Hub_License_Status]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DISTINCT
	
					CASE sps_hub_tran.transaction_status
					WHEN 'Rev Ref Request' THEN 2 
					WHEN 'Loaded' THEN 3
					WHEN 'Accepted' THEN 4
					WHEN 'Property Active' THEN 5
					WHEN 'OTP Received' THEN 6
					WHEN 'OTP Accepted' THEN 7
					WHEN 'OTP Conditions Met' THEN 8
					WHEN 'Sold' THEN 9
					WHEN 'Invoiced' THEN 10
					WHEN 'Registered' THEN 11
					WHEN 'Declined' THEN 12
					WHEN 'Mandate Period Expired' THEN 13
					WHEN 'Withdrawn' THEN 14
					WHEN 'Expired' THEN 15
					WHEN 'Escalated - Lynne' THEN 16
					WHEN 'Escalated - Tracy' THEN 17
					WHEN 'Sale Fell Through' THEN 18
					ELSE 99
					END AS [Process Order]	 
					,sps_hub_tran.transaction_status 
	FROM 
					sps_hub_transaction AS sps_hub_tran
	WHERE
					sps_hub_tran.transaction_status NOT LIKE 'Archived' 				
					
	UNION
	
	SELECT 1, '_ALL' 
	
	ORDER BY
					[Process Order] ASC				
					
END	





