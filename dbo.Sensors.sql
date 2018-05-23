CREATE TABLE [dbo].[Sensors] (
    [Id]             INT      IDENTITY (1, 1) NOT NULL,
    [SensorTypeId]   INT      NOT NULL,
    [ProductionDate] DATETIME NULL,
    [UploadInterval]  INT      NOT NULL,
    [BatchSize]      INT      NOT NULL,
    [UserId]         INT      NOT NULL,
    CONSTRAINT [PK_dbo.Sensors] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Sensors_dbo.SensorTypes_SensorTypeId] FOREIGN KEY ([SensorTypeId]) REFERENCES [dbo].[SensorTypes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.Sensors_dbo.Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_SensorTypeId]
    ON [dbo].[Sensors]([SensorTypeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [dbo].[Sensors]([UserId] ASC);

