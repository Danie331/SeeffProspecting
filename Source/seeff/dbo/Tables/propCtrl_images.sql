CREATE TABLE [dbo].[propCtrl_images] (
    [propCtrl_Id]           NUMERIC (18)   IDENTITY (1, 1) NOT NULL,
    [propCtrl_link]         NVARCHAR (100) NULL,
    [propCtrl_featured]     BIT            CONSTRAINT [DF__propCtrl__propC__38F95D68] DEFAULT (0) NULL,
    [propCtrl_default]      BIT            CONSTRAINT [DF__propCtrl__propC__39ED81A1] DEFAULT (0) NULL,
    [propCtrl_imageName]    VARCHAR (255)  NULL,
    [fkPropertyReference]   NUMERIC (18)   NULL,
    [fkPropertyId]          NUMERIC (18)   NULL,
    [propCtrl_imageCreated] BIT            NULL,
    [propCtrl_sold]         BIT            NULL
);

