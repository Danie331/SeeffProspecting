CREATE TABLE [dbo].[user_signature] (
    [user_signature_id] INT           IDENTITY (1, 1) NOT NULL,
    [registration_id]   INT           NOT NULL,
    [user_designation]  VARCHAR (500) NULL,
    [google_plus]       VARCHAR (500) NULL,
    [linkedin]          VARCHAR (500) NULL,
    [facebook]          VARCHAR (500) NULL,
    [twitter]           VARCHAR (500) NULL,
    [pintrest]          VARCHAR (500) NULL,
    [blog]              VARCHAR (500) NULL,
    [flickr]            VARCHAR (500) NULL,
    [my_page]           VARCHAR (500) NULL,
    [youtube]           VARCHAR (500) NULL,
    [seeff]             VARCHAR (500) NULL
);

