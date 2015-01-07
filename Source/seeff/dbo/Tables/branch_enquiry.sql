CREATE TABLE [dbo].[branch_enquiry] (
    [pk_branch_enquiry_id]   INT           IDENTITY (1, 1) NOT NULL,
    [branch_enquiry_name]    VARCHAR (255) NULL,
    [branch_enquiry_surname] VARCHAR (255) NULL,
    [branch_enquiry_contact] VARCHAR (255) NULL,
    [branch_enquiry_email]   VARCHAR (255) NULL,
    [branch_enquiry_comment] TEXT          NULL,
    [branch_enquiry_no]      VARCHAR (255) NULL,
    [fkBranchId]             INT           NULL,
    [branch_enquiry_added]   DATETIME      NULL,
    [branch_subscribe]       TINYINT       DEFAULT (0) NULL,
    PRIMARY KEY CLUSTERED ([pk_branch_enquiry_id] ASC) WITH (FILLFACTOR = 90)
);


GO
CREATE NONCLUSTERED INDEX [foreignKeys]
    ON [dbo].[branch_enquiry]([fkBranchId] ASC) WITH (FILLFACTOR = 90);

