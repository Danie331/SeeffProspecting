CREATE TABLE [dbo].[survey_option] (
    [survey_option_id] INT           IDENTITY (1, 1) NOT NULL,
    [survey_id]        INT           NOT NULL,
    [option_desc]      VARCHAR (250) NOT NULL
);

