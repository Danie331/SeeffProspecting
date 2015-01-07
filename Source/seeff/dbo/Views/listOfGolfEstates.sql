CREATE VIEW dbo.listOfGolfEstates
AS
SELECT     TOP 100 PERCENT dbo.agentType.agentTypeName, dbo.property.propertyReference, dbo.propertyType.propertyTypeName, dbo.agent.agentFirstName,
                       dbo.agent.agentSurname, dbo.agent.agentId, dbo.agent.agentEmail
FROM         dbo.agent INNER JOIN
                      dbo.agentType ON dbo.agent.fkAgentTypeId = dbo.agentType.agentTypeId AND dbo.agentType.agentTypeId = 2 INNER JOIN
                      dbo.property ON dbo.agent.agentId = dbo.property.fkAgentId INNER JOIN
                      dbo.propertyType ON dbo.property.fkPropertyTypeId = dbo.propertyType.propertyTypeId AND dbo.propertyType.propertyTypeId = 65
ORDER BY dbo.property.propertyReference

GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
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
         Configuration = "(H (4[30] 2[40] 3) )"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2[22] 3) )"
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
      ActivePaneConfig = 5
   End
   Begin DiagramPane = 
      PaneHidden = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "agent"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 114
               Right = 212
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "agentType"
            Begin Extent = 
               Top = 6
               Left = 250
               Bottom = 84
               Right = 409
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "property"
            Begin Extent = 
               Top = 6
               Left = 447
               Bottom = 114
               Right = 695
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "propertyType"
            Begin Extent = 
               Top = 6
               Left = 733
               Bottom = 114
               Right = 921
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
      Begin ColumnWidths = 8
         Width = 284
         Width = 1335
         Width = 1530
         Width = 1545
         Width = 1290
         Width = 1200
         Width = 720
         Width = 1770
      End
   End
   Begin CriteriaPane = 
      PaneHidden = 
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
         O', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'listOfGolfEstates';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N'r = 1350
         Or = 1350
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'listOfGolfEstates';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'listOfGolfEstates';

