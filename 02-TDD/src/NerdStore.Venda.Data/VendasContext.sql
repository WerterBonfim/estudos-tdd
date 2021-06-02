IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE SEQUENCE [MinhaSequencia] AS int START WITH 1000 INCREMENT BY 1 NO MINVALUE NO MAXVALUE NO CYCLE;
GO

CREATE TABLE [Vouchers] (
    [Id] uniqueidentifier NOT NULL,
    [TipoDesconto] int NOT NULL,
    CONSTRAINT [PK_Vouchers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Pedidos] (
    [Id] uniqueidentifier NOT NULL,
    [Codigo] int NOT NULL DEFAULT (NEXT VALUE FOR MinhaSequencia),
    [VoucherId] uniqueidentifier NULL,
    [Status] int NOT NULL,
    [VoucherUtilizado] bit NOT NULL,
    [ValorDeDesconto] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Pedidos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Pedidos_Vouchers_VoucherId] FOREIGN KEY ([VoucherId]) REFERENCES [Vouchers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [PedidoItems] (
    [Id] uniqueidentifier NOT NULL,
    [PedidoId] uniqueidentifier NOT NULL,
    [Quantidade] int NOT NULL,
    [Titulo] varchar(250) NOT NULL,
    CONSTRAINT [PK_PedidoItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PedidoItems_Pedidos_PedidoId] FOREIGN KEY ([PedidoId]) REFERENCES [Pedidos] ([Id]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_PedidoItems_PedidoId] ON [PedidoItems] ([PedidoId]);
GO

CREATE INDEX [IX_Pedidos_VoucherId] ON [Pedidos] ([VoucherId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20210602205358_Inicial', N'5.0.6');
GO

COMMIT;
GO

