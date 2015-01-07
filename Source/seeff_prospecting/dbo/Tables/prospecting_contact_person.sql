CREATE TABLE [dbo].[prospecting_contact_person] (
    [contact_person_id]  INT              IDENTITY (1, 1) NOT NULL,
    [person_title]       INT              NULL,
    [person_gender]      VARCHAR (1)      NOT NULL,
    [id_number]          VARCHAR (14)     NOT NULL,
    [firstname]          VARCHAR (255)    NOT NULL,
    [surname]            VARCHAR (255)    NOT NULL,
    [job_title]          VARCHAR (255)    NOT NULL,
    [propcntrl_buyer_id] INT              NULL,
    [referral_network]   BIT              NULL,
    [investor]           BIT              NULL,
    [is_popi_restricted] BIT              DEFAULT ((0)) NOT NULL,
    [created_date]       DATETIME         NULL,
    [updated_date]       DATETIME         NULL,
    [created_by]         UNIQUEIDENTIFIER NULL,
    [comments_notes]     VARCHAR (MAX)    NULL,
    PRIMARY KEY CLUSTERED ([contact_person_id] ASC),
    FOREIGN KEY ([person_title]) REFERENCES [dbo].[prospecting_person_title] ([prospecting_person_title_id])
);

