-- use Generate_area_layer_tbl to populate:
use ls_base
go

CREATE TABLE [dbo].[area_layer](
	[area_layer_id] [int] IDENTITY(1,1) NOT NULL,
	[area_id] [int] NOT NULL,
	[area_type] [varchar](1) NOT NULL,
	[province_id] [int] NULL,
	[formatted_poly_coords] [varchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[area_layer_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO