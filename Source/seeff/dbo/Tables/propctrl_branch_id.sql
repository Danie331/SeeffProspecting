CREATE TABLE [dbo].[propctrl_branch_id] (
    [propBranchId] INT      IDENTITY (1, 1) NOT NULL,
    [fkBranchId]   INT      NOT NULL,
    [fkPropCtrlId] INT      NULL,
    [lastUpdated]  DATETIME DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([propBranchId] ASC)
);

