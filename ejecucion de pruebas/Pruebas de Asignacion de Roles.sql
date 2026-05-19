
EXEC [dbo].[proc_Usuarios_Agregar] @Rut = 10, @Estado = 1,  @nombre = 'USUARIO 001', @apellido = 'CENABAST', @organismo = 'CENABAST'   
EXEC [dbo].[proc_Usuarios_Agregar] @Rut = 11, @Estado = 1,  @nombre = 'USUARIO 002', @apellido = 'CENABAST', @organismo = 'CENABAST'   
EXEC [dbo].[proc_Usuarios_Agregar] @Rut = 12, @Estado = 1,  @nombre = 'USUARIO 003', @apellido = 'MINSAL', @organismo = 'MINSAL'   
EXEC [dbo].[proc_Usuarios_Agregar] @Rut = 13, @Estado = 1,  @nombre = 'USUARIO 004', @apellido = 'MINSAL', @organismo = 'MINSAL'   


-- 1) Usuario con CON (Gestor CENABAST)
EXEC dbo.proc_UsuarioRol_Agregar @Rut='10', @CRol='CON';

-- 2) Intentar agregar ADP (Administra CENABAST) → debe FALLAR (50105/50106)
EXEC dbo.proc_UsuarioRol_Agregar @Rut='10', @CRol='ADP';

-- 3) Usuario con ADM, intentar COP → debe FALLAR (incompatible)
EXEC dbo.proc_UsuarioRol_Agregar @Rut='11', @CRol='ADM';
EXEC dbo.proc_UsuarioRol_Agregar @Rut='11', @CRol='COP'; -- error

-- 4) Usuario con rol MINSAL, intentar CENABAST → debe FALLAR por R1 (organismos)
EXEC dbo.proc_UsuarioRol_Agregar @Rut='12', @CRol='MIN';
EXEC dbo.proc_UsuarioRol_Agregar @Rut='12', @CRol='CON'; -- error

-- 5) Usuario con rol CENABAST, intentar MINSAL → debe FALLAR por R1 (organismos)
EXEC dbo.proc_UsuarioRol_Agregar @Rut='13', @CRol='CON';
EXEC dbo.proc_UsuarioRol_Agregar @Rut='13', @CRol='MIN'; -- error
