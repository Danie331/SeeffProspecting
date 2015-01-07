CREATE TABLE [dbo].[certificate_track] (
    [cert_it]         INT           IDENTITY (1, 1) NOT NULL,
    [course_itemid]   INT           NOT NULL,
    [registration_id] INT           NOT NULL,
    [cert_name]       VARCHAR (512) NOT NULL,
    [cert_print]      DATETIME      NOT NULL,
    [cert_email]      VARCHAR (512) NOT NULL,
    [cert_file_name]  VARCHAR (512) NOT NULL
);

