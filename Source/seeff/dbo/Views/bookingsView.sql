CREATE VIEW dbo.bookingsView
AS
SELECT     TOP 100 PERCENT dbo.branch.branchId, dbo.branch.branchName, dbo.property.propertyAddress, dbo.agent.fkAgentTypeId, dbo.agent.agentFirstName, 
                      dbo.agent.agentSurname, dbo.property.propertyReference, dbo.property.fkAreaId, dbo.property.propertyIsPPPN, dbo.property.propertySleeps, 
                      dbo.property.propertyContactPerson, dbo.property.propertyContactTelephone, dbo.property.propertyContactEmail, dbo.property.propertyPrice, 
                      dbo.bookings.bookingName, dbo.bookings.bookingContactTel, dbo.bookings.bookingEmail, dbo.bookings.bookingNumGuests, 
                      dbo.bookings.bookingLanguage, dbo.bookings.bookingReserved, dbo.bookings.bookingDepositPaid, dbo.bookings.bookingDepositPaidAmount, 
                      dbo.bookings.bookingDepositDueDate, dbo.bookings.bookingDepositPaidDate, dbo.bookings.bookingSecondPaymentPaid, 
                      dbo.bookings.bookingSecondPaymentAmount, dbo.bookings.bookingSecondPaymentDueDate, dbo.bookings.bookingSecondPaymentPaidDate, 
                      dbo.bookings.bookingFinalPaymentPaid, dbo.bookings.bookingFinalPaymentAmount, dbo.bookings.bookingFinalPaymentDueDate, 
                      dbo.bookings.bookingFinalPaymentPaidDate, dbo.bookings.bookingBreakageDepositAmount, dbo.bookings.bookingBreakageDepositRepaid, 
                      dbo.bookings.bookingAdded, dbo.bookings.bookingDeleted, dbo.bookings.bookingDeletedBy, dbo.bookings.bookingDeletedDate, 
                      dbo.bookingDates.bookingDateIn, dbo.bookingDates.bookingDateOut, dbo.agent.agentId
FROM         dbo.bookings INNER JOIN
                      dbo.bookingDates ON dbo.bookings.bookingId = dbo.bookingDates.fkBookingId INNER JOIN
                      dbo.property ON dbo.bookings.fkPropertyReference = dbo.property.propertyReference INNER JOIN
                      dbo.agent ON dbo.property.fkAgentId = dbo.agent.agentId INNER JOIN
                      dbo.branch ON dbo.agent.fkBranchId = dbo.branch.branchId
ORDER BY dbo.branch.branchName, dbo.agent.agentId, dbo.property.propertyReference, dbo.bookingDates.bookingDateIn

GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[43] 4[19] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1[50] 2[25] 3) )"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1 [56] 4 [18] 2))"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "bookings"
            Begin Extent = 
               Top = 16
               Left = 340
               Bottom = 217
               Right = 577
            End
            DisplayFlags = 280
            TopColumn = 16
         End
         Begin Table = "bookingDates"
            Begin Extent = 
               Top = 14
               Left = 623
               Bottom = 122
               Right = 804
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "property"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 114
               Right = 290
            End
            DisplayFlags = 280
            TopColumn = 9
         End
         Begin Table = "agent"
            Begin Extent = 
               Top = 189
               Left = 111
               Bottom = 297
               Right = 285
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "branch"
            Begin Extent = 
               Top = 226
               Left = 329
               Bottom = 334
               Right = 563
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      RowHeights = 220
      Begin ColumnWidths = 42
         Width = 284
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
       ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'bookingsView';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N'  Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
         Width = 1440
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'bookingsView';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'bookingsView';

