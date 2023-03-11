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

