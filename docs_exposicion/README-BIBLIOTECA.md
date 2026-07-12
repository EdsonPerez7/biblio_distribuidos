# 📚 Sistema de Gestión de Biblioteca Universitaria

## 🎯 Descripción
Sistema completo de gestión de biblioteca universitaria desarrollado con **React + TypeScript** (frontend) y **.NET 9 Web API** (backend) con **SQL Server** (base de datos).

## 🚀 Características Principales

### 📖 Gestión de Libros
- Catálogo completo con información bibliográfica
- Sistema de clasificación LCC (Library of Congress Classification)
- Búsqueda avanzada por título y autor
- Gestión de ejemplares físicos
- Control de disponibilidad en tiempo real

### 👥 Gestión de Usuarios
- Sistema de roles (Estudiante, Profesor, Bibliotecaria, Administrador)
- Autenticación con email institucional
- Códigos universitarios únicos

### 📋 Sistema de Préstamos
- Préstamos y devoluciones
- Renovación de préstamos
- Control de atrasos y multas
- Historial de préstamos por usuario

### 📊 Dashboard y Reportes
- Estadísticas en tiempo real
- Vista de préstamos activos
- Control de ejemplares disponibles

## 🛠️ Tecnologías Utilizadas

### Frontend
- **React 18** con TypeScript
- **Vite** como bundler
- **Tailwind CSS** para estilos
- **Lucide React** para iconos
- **Axios** para llamadas HTTP

### Backend
- **.NET 9 Web API**
- **C#** con Entity Framework
- **SQL Server** como base de datos
- **Autenticación basada en cookies**
- **Swagger** para documentación de API

### Base de Datos
- **SQL Server** con esquema normalizado
- **Relaciones N:M** para autores y categorías
- **Triggers** para automatización
- **Vistas** para consultas optimizadas

## 📁 Estructura del Proyecto

```
biblioteca-facultad/
├── backend/
│   └── NeoLibro.WebAPI/          # API .NET
│       ├── Controllers/          # Controladores REST
│       ├── Models/               # Modelos de datos
│       ├── LogicaNegocio/        # Lógica de negocio
│       ├── AccesoDatos/          # Acceso a datos
│       └── Helpers/              # Utilidades
├── frontend/
│   └── frontend/                 # Aplicación React
│       ├── src/
│       │   ├── api/              # Servicios de API
│       │   ├── pages/            # Componentes de páginas
│       │   └── components/       # Componentes reutilizables
└── database/
    └── BibliotecaFISI.sql        # Script de base de datos
```

## 🚀 Instalación y Configuración

### 1. Configurar la Base de Datos
```sql
-- Ejecutar el script en SQL Server
-- Archivo: database/BibliotecaFISI.sql
```

### 2. Configurar el Backend
```bash
cd backend/NeoLibro.WebAPI

# Restaurar paquetes NuGet
dotnet restore

# Configurar cadena de conexión en appsettings.json
"ConnectionStrings": {
    "cnnNeoLibroDB": "Server=localhost;Database=BibliotecaFISI;Trusted_Connection=true;TrustServerCertificate=true;"
}

# Ejecutar la aplicación
dotnet run
```

### 3. Configurar el Frontend
```bash
cd frontend/frontend

# Instalar dependencias
npm install

# Ejecutar en modo desarrollo
npm run dev
```

## 🌐 URLs de Acceso

- **Frontend**: http://localhost:5173
- **Backend API**: http://localhost:5180
- **Swagger**: http://localhost:5180/swagger

## 📋 Endpoints Principales

### Autenticación
- `POST /api/Usuarios/login` - Iniciar sesión
- `POST /api/Usuarios/registrar` - Registrar usuario
- `GET /api/Usuarios/verificar-sesion` - Verificar sesión

### Libros
- `GET /api/Libros` - Listar libros
- `GET /api/Libros/{id}` - Obtener libro por ID
- `GET /api/Libros/buscar` - Buscar libros

### Préstamos
- `GET /api/Prestamos/mis-prestamos` - Mis préstamos
- `POST /api/Prestamos` - Crear préstamo
- `PUT /api/Prestamos/{id}/devolucion` - Procesar devolución

### Ejemplares
- `GET /api/Ejemplares` - Listar ejemplares
- `GET /api/Ejemplares/libro/{libroId}` - Ejemplares por libro

## 👤 Roles de Usuario

### Estudiante
- Ver catálogo de libros
- Solicitar préstamos
- Ver sus préstamos activos

### Profesor
- Todas las funciones de Estudiante
- Acceso a recursos digitales
- Recomendaciones de libros

### Bibliotecaria
- Todas las funciones anteriores
- Gestionar préstamos y devoluciones
- Administrar ejemplares
- Ver reportes

### Administrador
- Todas las funciones anteriores
- Gestionar usuarios
- Administrar libros y autores
- Configuración del sistema

## 🔧 Configuración de Desarrollo

### Variables de Entorno
```bash
# Backend (.NET)
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://localhost:5180

# Frontend (Vite)
VITE_API_URL=http://localhost:5180
```

### Proxy de Desarrollo
El frontend está configurado para redirigir automáticamente las llamadas `/api/*` al backend en `http://localhost:5180`.

## 📊 Base de Datos

### Tablas Principales
- **Usuarios** - Información de usuarios
- **Libros** - Catálogo bibliográfico
- **Ejemplares** - Copias físicas de libros
- **Prestamos** - Registro de préstamos
- **Autores** - Información de autores
- **Categorias** - Clasificación de libros

### Características
- **Normalización** completa de datos
- **Relaciones N:M** para autores y categorías
- **Triggers** para automatización
- **Vistas** para consultas optimizadas
- **Índices** para rendimiento

## 🎨 Características de UI/UX

### Diseño Moderno
- **Gradientes** y efectos visuales
- **Animaciones** suaves con CSS
- **Responsive** design
- **Dark theme** por defecto

### Componentes
- **Carruseles** para navegación
- **Modales** para información detallada
- **Búsqueda** en tiempo real
- **Estados** de carga y error

## 🚀 Próximas Funcionalidades

- [ ] Sistema de reservas
- [ ] Notificaciones por email
- [ ] Reportes avanzados
- [ ] Integración con APIs externas
- [ ] App móvil
- [ ] Sistema de multas automático

## 📝 Notas de Desarrollo

### Backend
- Arquitectura en capas (Controllers → LN → DA)
- Autenticación basada en cookies
- Validaciones de negocio
- Manejo de errores centralizado

### Frontend
- Componentes funcionales con hooks
- TypeScript para type safety
- Estado local y global
- Optimización de rendimiento

## 🤝 Contribución

1. Fork el proyecto
2. Crear una rama para tu feature
3. Commit tus cambios
4. Push a la rama
5. Abrir un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo `LICENSE` para más detalles.

---

**Desarrollado con ❤️ para la Facultad de Ingeniería de Sistemas e Informática**
