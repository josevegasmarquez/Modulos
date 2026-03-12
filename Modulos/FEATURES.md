# Guía de Características Modernas

Este documento detalla los aspectos técnicos de la modernización de la UI y seguridad implementados en el proyecto **Modulos**.

## 🎨 Sistema de Diseño

Se ha implementado un sistema de diseño personalizado sobre Bootstrap 5, utilizando variables CSS para una fácil personalización de la paleta de colores.

### Variables de Color (`site.css`)
```css
:root {
    --primary-color: #4f46e5;
    --primary-hover: #4338ca;
    --glass-bg: rgba(255, 255, 255, 0.7);
    --border-radius: 12px;
}
```

### Componentes UI
- **Glassmorphism**: Uso de `backdrop-filter: blur(12px)` para crear efectos de cristal en tarjetas y sidebar.
- **Sidebar Navegación**: Componente persistente con estados dinámicos controlados por CSS y JS.
- **Botones Modernos**: Clase `.btn-modern` con transiciones de escala al presionar.

## 🔒 Mejoras de Seguridad y UX

### Flujo de Contraseña
- **Primer Ingreso**: Al detectar la bandera `MustChangePassword`, el sistema redirige automáticamente a la vista de cambio de contraseña.
- **Prevención de Errores**: En la vista de cambio de contraseña, se ha bloqueado la función de pegado (`onpaste="return false;"`) para asegurar que el usuario conozca la nueva contraseña que está ingresando.

### Validación de Campos
- **Retroalimentación Específica**: Los errores de validación de la API (como DNI duplicado o requisitos de complejidad de contraseña) se mapean directamente al campo correspondiente en el formulario usando `ModelState`.
- **Visibilidad**: Botones de ojo integrados en los campos de contraseña para alternar la visibilidad de manera segura.

## ⚙️ Tecnologías Frontend
- **Inter (Google Fonts)**: Tipografía moderna para mejor legibilidad.
- **Bootstrap Icons**: Set de iconos consistente para toda la aplicación.
- **jQuery Validation**: Validaciones en el cliente integradas con los DataAnnotations del modelo.
