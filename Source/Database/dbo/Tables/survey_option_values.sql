CREATE TABLE [dbo].[survey_option_values] (
    [survey_option_values_id] INT           IDENTITY (1, 1) NOT NULL,
    [survey_id]               INT           NOT NULL,
    [value_desc]              VARCHAR (100) NOT NULL,
    [value_type]              VARCHAR (20)  NOT NULL,
    [mandatory]               BIT           CONSTRAINT [DF_survey_option_values_mandatory] DEFAULT ((1)) NOT NULL
);

