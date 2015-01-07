CREATE TABLE [dbo].[schools_registration] (
    [pk_school_registration_id]   INT           IDENTITY (1, 1) NOT NULL,
    [school_registration_name]    VARCHAR (255) NULL,
    [school_registration_surname] VARCHAR (255) NULL,
    [school_registration_tel]     VARCHAR (255) NULL,
    [school_registration_email]   VARCHAR (255) NOT NULL,
    [school_registration_suburb]  VARCHAR (255) NULL,
    [school_registration_school]  VARCHAR (255) NULL,
    [school_registration_added]   DATETIME      NULL,
    [school_registration_no]      VARCHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED ([pk_school_registration_id] ASC) WITH (FILLFACTOR = 90)
);

