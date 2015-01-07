CREATE TABLE [dbo].[feed_out_service_traffic] (
    [pk_traffic_id]           INT           IDENTITY (1, 1) NOT NULL,
    [traffic_monitor_name]    VARCHAR (255) NULL,
    [fkBranchId]              VARCHAR (255) NULL,
    [proxy_server_IP_address] VARCHAR (255) NULL,
    [IP_Address]              VARCHAR (255) NULL,
    [webReference]            VARCHAR (50)  NULL,
    [date_time]               DATETIME      DEFAULT (getdate()) NULL,
    PRIMARY KEY CLUSTERED ([pk_traffic_id] ASC)
);

