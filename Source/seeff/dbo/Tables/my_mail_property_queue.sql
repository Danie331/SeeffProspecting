CREATE TABLE [dbo].[my_mail_property_queue] (
    [queue_id]              INT      IDENTITY (1, 1) NOT NULL,
    [fk_property_reference] INT      NOT NULL,
    [processed_status]      INT      DEFAULT ((1)) NOT NULL,
    [banner_type]           INT      NOT NULL,
    [inserted_date]         DATETIME NOT NULL,
    [processed_date]        DATETIME NULL,
    PRIMARY KEY CLUSTERED ([queue_id] ASC)
);

