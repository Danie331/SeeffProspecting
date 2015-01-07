CREATE VIEW dbo.residentialRentalPropertiesView
AS
SELECT     dbo.property.propertyId, dbo.property.propertyReference, dbo.property.propertyActive,
                          (SELECT     COUNT(property_imagesId)
                            FROM          property_images
                            WHERE      property_images.propertyReference = property.propertyReference) AS imageCount,
                          (SELECT     TOP 1 property_imagesName
                            FROM          property_images
                            WHERE      property_images.propertyReference = property.propertyReference AND property_imagesDefault = 1) AS primaryImage, 
                      dbo.property.fkAgentId, dbo.property.fkBranchId, dbo.property.fkAreaId, dbo.property.fkCategoryId, dbo.property.fkPropertyTypeId, 
                      dbo.property.fkActionId, dbo.property.propertyPrice, dbo.property.propertyAddress, dbo.property.propertyRentalTerm, 
                      dbo.agent.agentFirstName + ' ' + dbo.agent.agentSurname AS lastEditedBy, agent_1.agentFirstName + ' ' + agent_1.agentSurname AS agentName, 
                      dbo.property.propertyIsPPPN, dbo.property.propertyBookingLink, dbo.property.propertyBookingLinkText
FROM         dbo.property LEFT OUTER JOIN
                      dbo.agent ON dbo.property.propertLastEditedBy = dbo.agent.agentId INNER JOIN
                      dbo.agent agent_1 ON dbo.property.fkAgentId = agent_1.agentId
WHERE     (dbo.property.fkActionId = 3) AND (dbo.property.fkCategoryId = 1)

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
         Begin Table = "property"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 114
               Right = 290
            End
            DisplayFlags = 280
            TopColumn = 63
         End
         Begin Table = "agent"
            Begin Extent = 
               Top = 114
               Left = 38
               Bottom = 222
               Right = 212
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "agent_1"
            Begin Extent = 
               Top = 114
               Left = 250
               Bottom = 222
               Right = 424
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
      Begin ColumnWidths = 20
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
         Or = 13', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'residentialRentalPropertiesView';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N'50
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'residentialRentalPropertiesView';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'residentialRentalPropertiesView';

