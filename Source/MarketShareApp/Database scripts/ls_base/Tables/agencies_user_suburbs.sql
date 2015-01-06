use ls_base
go

-- agency tables
-- create [agencies_user_suburbs]
-- create [agencies_user_suburbs]
CREATE TABLE [dbo].[agencies_user_suburbs](
	[agencies_user_suburbs_id] [int] IDENTITY(1,1) NOT NULL,
	[suburb_id] [int] NOT NULL,
	[agency_id] [int]  NULL,
	[updated_by] [varchar](max) NOT NULL,
	[updated_date] [timestamp] NOT NULL,
 CONSTRAINT [PK_agencies_user_suburbs] PRIMARY KEY CLUSTERED 
(
	[agencies_user_suburbs_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


ALTER TABLE [dbo].[agencies_user_suburbs]  WITH CHECK ADD  CONSTRAINT [FK_agency_id] FOREIGN KEY([agency_id])
REFERENCES [dbo].[agency] ([agency_id])
GO

ALTER TABLE [dbo].[agencies_user_suburbs] CHECK CONSTRAINT [FK_agency_id]
GO