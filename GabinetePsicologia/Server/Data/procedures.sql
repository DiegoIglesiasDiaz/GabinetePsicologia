SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetPacienteByUserName]
@username varchar(40)
AS
BEGIN

	SET NOCOUNT ON;

	SELECT * from Pacientes ps inner join  AspNetUsers us on us.Id = ps.ApplicationUserId where us.UserName = @username
END
GO

CREATE PROCEDURE [dbo].[GetPsicologoByUserName]
@username varchar(40)
AS
BEGIN

	SET NOCOUNT ON;

	SELECT * from Psicologos ps inner join  AspNetUsers us on us.Id = ps.ApplicationUserId where us.UserName = @username
END
GO

CREATE PROCEDURE [dbo].[GetPsicologoById]
@id uniqueidentifier
AS
BEGIN

	SET NOCOUNT ON;

	SELECT * from Psicologos ps inner join  AspNetUsers us on us.Id = ps.ApplicationUserId where us.Id = @id
END

Go

CREATE TRIGGER eliminarMensaje_despues_nueve_meses
ON Chat
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Calcula la fecha límite
    DECLARE @fecha_limite AS DATE;
    SET @fecha_limite = DATEADD(MONTH, -9, GETDATE());

    -- Elimina las filas que cumplen la condición
    DELETE FROM Chat
    WHERE Date < @fecha_limite;
END;
GO