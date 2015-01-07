CREATE TABLE [dbo].[sba_skill] (
    [sba_question_id]   INT           IDENTITY (1, 1) NOT NULL,
    [skill_heading]     VARCHAR (150) NOT NULL,
    [skill]             VARCHAR (150) NOT NULL,
    [skill_description] VARCHAR (250) NOT NULL,
    [skill_order]       INT           NOT NULL
);

