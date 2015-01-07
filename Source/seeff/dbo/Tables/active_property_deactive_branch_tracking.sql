CREATE TABLE [dbo].[active_property_deactive_branch_tracking] (
    [pk_active_property_id] INT IDENTITY (1, 1) NOT NULL,
    [fk_branch_id]          INT NOT NULL,
    [fk_property_id]        INT NOT NULL,
    CONSTRAINT [PK_active_property_deactive_branch_tracking] PRIMARY KEY CLUSTERED ([pk_active_property_id] ASC)
);

