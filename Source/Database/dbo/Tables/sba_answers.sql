CREATE TABLE [dbo].[sba_answers] (
    [registration_id] INT NOT NULL,
    [sba_question_id] INT NOT NULL,
    [applicable]      BIT NULL,
    [importance]      INT NULL,
    [difficulty]      INT NULL
);

