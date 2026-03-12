# Modulos - Sistema de Gestión de Usuarios

Este proyecto es una aplicación web escalable construida con **.NET 8.0**, enfocada en la administración eficiente de usuarios con una experiencia de usuario moderna y segura.

## Características Destacadas

### 🎨 Interfaz Moderna y UX
- **Diseño Premium**: Interfaz basada en **Glassmorphism** con efectos de desenfoque de fondo y tarjetas semitransparentes.
- **Navegación Intuitiva**: Sidebar lateral fija con micro-animaciones y estados activos claros.
- **Micro-interacciones**: Animaciones de entrada (fade-in) y retroalimentación táctil en botones.
- **Diseño Responsivo**: Adaptabilidad completa para dispositivos móviles y tablets.

### 🔒 Seguridad y Gestión
- **Cambio de Contraseña Obligatorio**: Los usuarios registrados por un administrador deben cambiar su contraseña temporal en el primer acceso.
- **Roles y Permisos**: Sistema basado en roles (`SuperAdmin`, `Admin`, `User`) con acceso restringido a funciones administrativas.
- **Validación Robusta**: Restricción de pegado en campos de confirmación y mensajes de error específicos por campo.
- **Privacidad**: Toggles de visibilidad para campos de contraseña.

## Estructura del Proyecto

- **Modulos.API**: Backend RESTful que utiliza ASP.NET Core Identity y JWT para la seguridad.
- **Modulos.Client**: Frontend MVC con una capa visual modernizada que consume la API.

Para más detalles técnicos sobre la implementación de la UI y seguridad, consulte [FEATURES.md](FEATURES.md).

## Requisitos Implementados

1.  **Registro de Usuarios**: Solo `Admin` y `SuperAdmin` pueden registrar nuevos usuarios.
2.  **Contraseña Temporal**: Flujo de seguridad forzado para nuevos usuarios.
3.  **Seguridad JWT**: Comunicación cifrada entre Cliente y API.
4.  **DNI Único**: Longitud exacta de 8 caracteres numéricos.
5.  **Perfiles Editables**: Gestión completa de datos personales del usuario.

## Configuración

### Backend (Modulos.API)
1.  Configurar la cadena de conexión en `appsettings.json`.
2.  Ejecutar `dotnet ef database update`.

### Frontend (Modulos.Client)
1.  Configurar `ApiSettings:BaseUrl` en `appsettings.json`.
2.  Ejecutar con `dotnet run`.

## Tecnologías
- ASP.NET Core 8.0
- Entity Framework Core (SQL Server)
- Bootstrap 5 + Custom Modern CSS
- JWT (JSON Web Tokens)
