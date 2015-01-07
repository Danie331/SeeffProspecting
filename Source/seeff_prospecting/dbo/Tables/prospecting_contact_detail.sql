CREATE TABLE [dbo].[prospecting_contact_detail] (
    [prospecting_contact_detail_id] INT           IDENTITY (1, 1) NOT NULL,
    [contact_detail_type]           INT           NOT NULL,
    [contact_person_id]             INT           NOT NULL,
    [contact_detail]                VARCHAR (255) NOT NULL,
    [intl_dialing_code_id]          INT           NULL,
    [eleventh_digit]                INT           NULL,
    [is_primary_contact]            BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([prospecting_contact_detail_id] ASC),
    FOREIGN KEY ([contact_detail_type]) REFERENCES [dbo].[prospecting_contact_detail_type] ([contact_detail_type_id]),
    FOREIGN KEY ([contact_person_id]) REFERENCES [dbo].[prospecting_contact_person] ([contact_person_id]),
    FOREIGN KEY ([intl_dialing_code_id]) REFERENCES [dbo].[prospecting_area_dialing_code] ([prospecting_area_dialing_code_id])
);

