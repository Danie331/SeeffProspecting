use ls_base
go

drop table [dbo].[agency]
go

-- create [agency] with default data
CREATE TABLE [dbo].[agency](
	[agency_name] [varchar](max) NULL,
	[agency_id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_agency] PRIMARY KEY CLUSTERED 
(
	[agency_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[agency] ON 

GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Seeff', 1)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Molly Kahn Estates', 2)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Mosaic Properties', 3)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'MTA Mackenzie Thomas Associates CC', 4)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'My Style Properties', 5)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Nan Roberts Estates', 6)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'National Realtors Group', 7)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Nomsa- Nene Properties', 8)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'OCC', 9)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Oreo Prop', 10)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Otter Estates', 11)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Pam Golding Properties', 12)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Pam Stein Real Estate', 13)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Park Properties', 14)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Penny Read', 15)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Prestige Homes', 16)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Primary Estates', 17)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Property Emporium', 18)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Rawson', 19)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Real Net Linden Properties', 20)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Remax', 21)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Renprop Residential', 22)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Richter Properties', 23)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'RJD Realty', 24)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Roger Wiehe', 25)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Russel Fisher Properties', 26)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Sandra Solomon Properties', 27)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Sandton Realty', 28)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Siema Katz Properties', 29)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Silver Property Group', 30)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Soukop', 31)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Stamelman Properties', 32)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Stratos', 33)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Swank Real Estate', 34)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Ted Slotar Estates', 35)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'The Proper Team ', 36)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'The Smartest Spot', 37)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Tony Larsson Country Real Estate', 38)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Tracy Roberts Properties', 39)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Tracy Smid Real Estate', 40)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Vered Estates', 41)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Vic Martin Real Estate', 42)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Woodbridge Estates', 43)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Woods Estates', 44)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'You First Properties', 45)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Zareena Kara Properties', 46)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Aarons International Property', 47)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Abaden Properties', 48)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Adrienne Hersch Properties', 49)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Aim High Properties', 50)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Allwright Properties', 51)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Amour Properties', 52)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Analia Properties', 53)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Angela Kernot Real Estate', 54)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Angor ', 55)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Anne Williamson Properties', 56)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Averil Slome Properties', 57)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Basil Elk Real Estate', 58)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Belagio Estates', 59)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Bergplatz Property Investments', 60)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Bertha Ludolphi Real Estate', 61)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Beryl Friedman Properties', 62)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Big H Properties', 63)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Blue Living Properties', 64)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Bowring Estates', 65)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Brett Cohen Estates', 66)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Brian Falconer Real Estate', 67)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'CadsPROP', 68)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Carabiner', 69)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Carol Slome Network Properties', 70)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Carr Estates', 71)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Cathy Covotsos Properties', 72)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Century 21', 73)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Charing Cross Property Brokers', 74)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Charmaine Kirchmann Property Service', 75)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Charne'' Realty', 76)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Chas Everitt', 77)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'ChernoDavis', 78)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Cherry Grove Realty', 79)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Christine Williams Properties', 80)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Corbett Investment Properties', 81)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Cornerstone Properties', 82)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Country Life Properties', 83)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'D and A Properties', 84)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Daryl Brett Properties', 85)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'David Flaum Property ', 86)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Dawn Prop', 87)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Di Tucker Properties', 88)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Eighth Avenue', 89)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Eltec Nationwide', 90)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Engel & Volkers', 91)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Ennik Estates', 92)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Erica Solomon Real Estate', 93)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Executive Homes', 94)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Fine & Country', 95)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Finest Real Estate', 96)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Firzt Realty Company', 97)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Fourways Garden Realty', 98)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Francesca Estates', 99)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Frankie Bell''s Real Esate', 100)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Gaye Cawood Realty', 101)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'GB Properties', 102)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Global Lifestyle Properties', 103)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Gloria Real Estate', 104)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Graham & Constable', 105)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Hall Real Estate', 106)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Hamilton''s Property Portfolio', 107)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Hanna Yarmarkov', 108)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Harcourts', 109)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Heather Leas Estates', 110)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Homes of Distinction', 111)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Huizemark', 112)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Ira Properties', 113)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Jacqui Van Ysendyk Estates', 114)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Jawitz', 115)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Jean Baillie Investment Properties', 116)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Jessie "k" Estates', 117)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'John Livanos Estates', 118)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Johnston Estates', 119)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Kathy Cohen Properties', 120)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Kay-Eddie Estates', 121)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Kings', 122)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Le Property Brokers', 123)
GO
--INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Leap Frog ', 124)
--GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Leigh Properties', 125)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Lennis and Anthea', 126)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Lev Mark Properties', 127)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Lew Geffen Sothebys', 128)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Lilla Shaw', 129)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Lime Stone Properties', 130)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Maine Realty', 131)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Mary Mc Gregor', 132)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'MBJ Properties', 133)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Michael Black Properties', 134)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Michael Mcknight Real Estate', 135)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'MMD Property', 136)
GO
-- Paul Kruger 25-02-2014
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Fine Living', 137)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Willows Property Group', 138)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'La Ville Eiendomme', 139)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Bez Properties', 140)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Elizma Eiendomme', 141)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Marsha Joubert Eiendomme', 142)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Prime Properties', 143)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Just Residential', 144)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'LandLink', 145)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Property Profile', 146)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Sorento', 147)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Noordbrug Eiendomme', 148)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Sukses Eiendomme', 149)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Varsity Rentals', 150)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'BIG Eiendomme', 151)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Nexor', 152)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'TG Kruger Eiendomme', 153)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Realty 1', 154)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Vogue', 155)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Potch Eiendomme', 156)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Eurika Eiendomme', 157)
GO
-- Hout Bay
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Anne Porter Knight Frank', 158)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Atteridge Real Estate', 159)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Cape Waterfront Estates', 160)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Cluttons', 161)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Dannie Kagan Properties', 162)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'HoutBay.info', 163)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Kronendal Village Real Estate', 164)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Lola Kramer Properties', 165)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Property Management & Rental Specialists CC', 166)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Rattle Walton', 167)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Re/Max Home', 168)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Signature Real Estate', 169)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Frank Holland & Associates', 170)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Private sale', 171)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Auction', 172)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Greeff', 173)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Brooks and Michaels', 174)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Tysons', 175)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Wakefields', 176)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Lyn Tayfield Properties', 177)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Max Prop', 178)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Malcom Plint Estates', 179)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'ZAK Eiendomme', 180)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Anzel Eiendomme', 181)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Hilary McClean', 182)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Blue Chip', 183)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Gibson Plumb', 184)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Regina Sauer', 185)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Front Door', 186)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Kalahari Eiendomme', 187)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Era Eiendomme', 188)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Dehari Eiendomme', 189)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Upton Properties', 190)
GO

--
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Aida', 191)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Perfektum', 192)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Re/Max Homefinders', 193)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Elite', 194)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Alfa', 195)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Jam', 196)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'WPMG', 197)
GO

--

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Exclusive Properties', 198)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Realnet', 199)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Leapfrog', 200)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Property for All', 201)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Homes-a-plenty', 202)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Ansie Marais t/a Fine & Country', 203)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'@ Home Properties', 204)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Theo Goosen', 205)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Living Large Properties', 206)
GO

--

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Gina Hart', 207)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Dunvegan Estates', 208)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Gallery', 209)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Heads', 210)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Norgarb', 211)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Vineyard Estates', 212)
GO 

--
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Infinity Homes', 213)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Margarets', 214)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'V&B Estates', 215)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Lucy Properties', 216)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Durr Estates', 217)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Constantia Valley', 218)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Helen Hoare', 219)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Pam Sorel', 220)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Constantia Estates', 221)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Icon Properties', 222)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Joy McNab', 223)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Sharon Ball', 224)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Werner Properties', 225)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'LM Properties', 226)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Mark Clench', 227)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Diane Ormrand', 228)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Al Homan', 229)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Regina Sauer Properties', 230)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Stately Homes', 231)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Asher Properties', 232)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Lorraine Fleming', 233)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Hardisty', 234)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Elaine Swaine', 235)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Homes Indeed', 236)
GO

-- 
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Penny Prideaux', 237)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Rhonda Raad', 238)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Rose Eedes', 239)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Kim Hunter', 240)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Private Property Group', 241)
GO

--
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Chaska Homes', 242)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Blue Route Estates', 243)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Cavinden International', 244)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Platinum Latsky Properties', 245)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Brookes Property Group', 246)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'PWF Properties', 247)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Sea Point Estate', 248)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Lee Gautschi Properties', 249)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Elizabeth Tromp Properties', 250)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Pears', 251)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Urban Village', 252)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Brenda Dickinson & Associates', 253)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Hurwitz Homes', 254)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Ron Durbach Estates', 255)
GO

--
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Averil Brink Properties', 256)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Blue Raven Properties', 257)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Jenni Dennis Estates', 258)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'NDS Estates', 259)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Middleton Estates', 260)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Investa Homes', 261)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Gordon Knight Properties', 262)
GO

-- 
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Pennywise', 263)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Sandi Dickson', 264)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Dogon', 265)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Ikapa Properties', 266)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Dream Homes', 267)
GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Trafalgar Property', 268)
GO

--
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Placemakers', 269)
GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES (N'Marion Wannenberg Properties', 270)
GO

--
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'3%.COM',271) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'4 PROPERTY GROUP',272) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'AACHEN HOME SERVICES',273) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ACUTTS',274) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'AGENTS IN PROPERTY',275) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'AGNT',276) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ALERON REAL ESTATE',277) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'AMG REAL ESTATE',278) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ANDRADE PROPERTIES',279) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ANNE DE VILLIERS ESTATES',280) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ANVIL PROPERTY',281) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'APOLLO PROPERTIES',282) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'APPLE',283) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'APRIX DEVELOPMENTS',284) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ARMER PROPERTIES',285) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ASAP',286) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ASH BROOK',287) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ASSET PROPERTY GROUP',288) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'AT PROP HUB',289) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'AT REDS PROPERTIES',290) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'AULA',291) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'AZI REAL ESTATE',292) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'BE PROP',293) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'BEACON REAL ESTATE',294) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'BECKY HOMES',295) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'BENSHAW PROPERTIES',296) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'BESMART PROPERTIES',297) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'BIANCA PROPERTIES',298) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'BIDDERS CHOICE',299) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'BIGEYE PROPERTIES',300) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'BLUE SQUARE',301) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'BONDVEST REALTY GROUP',302) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'BRAAM LE ROUX',303) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'BRIGHTLINK REAL ESTATE',304) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'BROOKLYN PROPERTIES',305) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'BUY AND SELL',306) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'BUY PROPERTY DIRECT',307) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'CASA NOSTRA',308) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'CENTIRENE',309) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'CENTRIDGE',310) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'CENTU HOMES',311) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'CENTURION REAL ESTATE',312) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'CENTURY 21',313) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'CHOPROP',314) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'CONNOISSEUR PROPERTIES',315) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'CRAFFORD',316) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'DAINTREE ESTATES',317) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'DALENE VERMAAS PROPERTIES',318) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'DELCOR REAL ESTATE',319) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'DELFERRO PROPERTIES',320) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'DEZYNA PROPERTIES',321) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'DIC HOMES',322) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'DIEU DONE PROPERTIES',323) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'DIPLOMATIC PROPERTIES',324) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'DITHOTO',325) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'DONAU PROPERTIES',326) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'DORMEHL PROPERTY GROUP',327) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'DOUGLAS KEYS PROPERTIES',328) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'DUNAMIS PROPERTIES',329) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'DUNNE PROPERTIES',330) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'DYNAMIC AUCTIONERS',331) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'E PROPERTIES',332) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ELBE STRUWIG PROPERTIES',333) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ELIZE DE BRUIN',334) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ELMARIE AND WILLIE',335) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ELMIEN AND TEAM',336) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'EMJAY',337) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'EN INFITY',338) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ERA',339) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ESQUIRE ESTATES',340) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'FALCON ESTATES',341) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'FAR PROPERTIES',342) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'FEEL AT HOME PROPERTIES',343) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'FITZANNE SALES',344) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'FRANCESKA BEATTIE PROPERTIES',345) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'FREQUENT PROPERTIES',346) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'GALETTI KNIGHT',347) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'GEM',348) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'GEORGE WEBSTER PROPERTIES',349) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'GEYER ESTATES',350) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'GRADION PROPERTY',351) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'GREENDOOR PROPERTIES',352) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'GREENFIELD PROPERTIES',353) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'GREY BUCK ESTATES',354) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'HOMESDOT',355) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'HOUSE CALL PROPERTIES',356) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'HOUSES2HOMES',357) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'HUURKOR',358) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ILA PROPERTIES',359) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'IMPACT ESTATES',360) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'INFINITYVEST',361) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'INFOPROP',362) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'INPA',363) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'INTRO REAL ESTATE',364) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'INVESTOR REAL ESTATE',365) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'JANA REINECKE PROPERTIES',366) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'JAZZ PROPERTIES',367) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'JBE PROPERTY',368) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'JOAN HOMES PROPERTY GROUP',369) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'JOHAN SCHOEMAN PROPERTIES',370) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'JUST DO IT',371) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'JUSTINVEST',372) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'KAREN JONKER',373) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'KAYALETO REAL ESTATE',374) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'KEAGEOZ',375) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'KELLER WILLIAMS',376) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'KHAYA 2 KHAYA',377) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'KIBO PROPERTIES',378) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'KOLVER PROPERTIES',379) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'LA CONSULTING',380) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'LANDLORD ESTATES',381) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'LANGENHOVEN',382) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'LDP PEACE AT HOME PROPERTIES',383) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'LEANNE GRAAF',384) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'LEASURE AND WILDLIFE PROPERTIES',385) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'LEGAL PRO REALTY',386) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'LETTIE STEYN PROPERTIES',387) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'LETTING EXPERTS',388) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'LINDA WENTWORTH PROPERTIES',389) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'LINDSEAL REAL ESTATE',390) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'LIZ MARAIS PROPERTIES',391) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'MAGDA LOURENS PROPERTIES',392) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'MAP PROPERTIES',393) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'MARCINO REAL ESTATE',394) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'MARIHET OOSTHUIZEN',395) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'MATTHYSEN PROPERTY GROUP',396) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'MIDTEAM',397) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'MIKE VAN WYK PROPERTIES',398) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ML PROPERTY',399) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'MT DEVELOPMENTS',400) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'MUMBI PROPERTIES',401) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'NATIONAL LETTING',402) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'NELPROP REAL ESTATE',403) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'NYDES',404) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'OASE PROPERTIES',405) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'OCEAN PROPERTIES',406) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'OCTOBER SKYE PROPERTIES',407) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'OFFICE BOOK',408) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'OLEA',409) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ONLY REALTY',410) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'OPEN HOUSE REAL ESTATE',411) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'OUT THE BOX REALTY',412) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'PAKO AGANANG',413) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'PALMGROVE',414) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'PENUEL PROPERTIES',415) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'PERFORMER PROPERTIES',416) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'PERSEMIE MACK PROPERTIES',417) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'PHILMARINE PROPERTIES',418) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'PLATINUM KEY PROPERTIES',419) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'PLATINUM RESIDENTIAL',420) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'POLKADOTS PROPERTY',421) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'PRETOR',422) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'PROFITX LTD',423) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'PROPERTIES@MIDSTREAM',424) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'PROPERTY 100',425) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'PROPERTY BY KITTY',426) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'PROPERTY DEN',427) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'PROPERTY ENTREPRENEUR',428) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'PROPERTY LEGENDS',429) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'PROPERTY PRINCIPAL',430) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'PROPERTY SHOP',431) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'PROPERTY SQM',432) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'PROPERTY UNITED',433) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'PROPERTY.CO.ZA',434) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'PROSPERITY REAL ESTATE',435) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'PS HOME',436) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'PUBLIC PROPERTY',437) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'REAL ONE',438) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'REALTORS INTERNATIONAL',439) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'REALTY CHECK',440) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'REALTY MERCHANT INVESTMENTS',441) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'RED STAR AUCTIONERS',442) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'RIETTE ;UWS PROPERTIES',443) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'RIVENDELL PROPERTIES',444) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'RIZE',445) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'RM REALTERS',446) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ROOIHUIS',447) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ROSANNAS',448) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ROSE SQUARE',449) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'RVE',450) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'SA REALTY',451) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'SELLMORE HOMES',452) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'SHERYL SCOTT PROPERTIES',453) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'SHOWDAY PROPERTIES',454) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'SIGMA PROP',455) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'SILKON PROPERTIES',456) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'SILVER FALLS',457) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'SIRIUS PROPERTIES',458) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'SMILE PROPERTY',459) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'SNOOKS ESTATES',460) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'SOUTHDOWNS',461) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'SOUTHERN PEOPLE PROPERTIES',462) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'SOUTHERN STAYING PRETORIA',463) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'STATURE REALTY',464) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'STEEPLE',465) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'SUGAR REALTY',466) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'SULA',467) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'SUPERIOR REALTY',468) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'SUPREMO PROPERTIES',469) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'SUZEL ESTATES',470) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'SWEETHOME PROPERTIES',471) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'TASK US PROPERTIES',472) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'TD PROPERTIES',473) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'TEAM1 WATERFRONT',474) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'TERRAVITA PROPERTIES',475) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'THE STAND MAN PROPERTIES',476) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'THINUS STRYDOM',477) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'TIGER REALTY',478) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'TIP TOP PROPERTIES',479) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'TOPNET',480) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'TRINITY ESTATES',481) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'UNITED AUCTIONERS',482) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'URBAN LIMITS',483) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'UYSIE DE KLERK',484) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'VARTRUST',485) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'VARTRUST REAL ESTATE',486) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'VISAGIE PROPERTIES',487) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'VRJ LIFESTYLE PROPERTIES',488) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'WATSON PROPERTIES',489) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'WAYPOINT REAL ESTATE',490) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'Y PROP',491) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ZANI',492) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ZENITH',493) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ZOBEL',494) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ZOTOS',495) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'ZWARTKOP ESTATES',496) ;

--
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'Sinclair Estates',497) GO
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'McLaren Properties',498) GO

INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'Olive Properties',499) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'Chapple Real Estate',500) ;
INSERT [dbo].[agency] ([agency_name], [agency_id]) VALUES(N'Alliance Group',501) ;

SET IDENTITY_INSERT [dbo].[agency] OFF
GO