CREATE TABLE [dbo].[permission] (
    [permission_id]            INT          IDENTITY (1, 1) NOT NULL,
    [fk_registration_id]       BIGINT       NOT NULL,
    [fk_permission_section_id] INT          NOT NULL,
    [fk_permission_type_id]    INT          NOT NULL,
    [permission_create_date]   DATETIME     DEFAULT (getdate()) NOT NULL,
    [permission_create_user]   VARCHAR (50) NOT NULL,
    [permission_update_date]   DATETIME     NULL,
    [permission_update_user]   VARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([permission_id] ASC),
    CONSTRAINT [fk_permission_section] FOREIGN KEY ([fk_permission_section_id]) REFERENCES [dbo].[permission_section] ([permission_section_id]),
    CONSTRAINT [fk_permission_type] FOREIGN KEY ([fk_permission_type_id]) REFERENCES [dbo].[permission_type] ([permission_type_id]),
    CONSTRAINT [fk_user_registration] FOREIGN KEY ([fk_registration_id]) REFERENCES [dbo].[user_registration] ([registration_id])
);

