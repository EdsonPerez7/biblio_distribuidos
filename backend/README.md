# 📚 Biblioteca FISI - Backend API

[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-green.svg)](https://docs.microsoft.com/en-us/aspnet/core/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-red.svg)](https://www.microsoft.com/en-us/sql-server)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## 🎯 Descripción

**Biblioteca FISI** es un sistema de gestión de biblioteca desarrollado para la Facultad de Ingeniería de Sistemas e Informática. Este backend proporciona una API REST completa para la gestión de libros, usuarios, préstamos, multas y más.

### ✨ Características Principales

- 🏗️ **Arquitectura Modular** con separación clara de responsabilidades
- 🔐 **Autenticación y Autorización** por roles (Estudiante, Profesor, Bibliotecaria, Administrador)
- 📚 **Gestión Completa de Biblioteca** (libros, ejemplares, préstamos, multas)
- 🎨 **API REST** con documentación automática (Swagger)
- 🗄️ **Base de Datos SQL Server** con ADO.NET
- 🧪 **Testing** con pruebas unitarias e integración
- 📖 **Documentación Completa** para desarrolladores

---

## 🚀 Inicio Rápido

### 📋 Prerrequisitos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server 2022](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) o superior
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/)
- [SQL Server Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)

### ⚡ Instalación

1. **Clonar el repositorio**:
   ```bash
   git clone https://github.com/tu-usuario/biblioteca-fisi.git
   cd biblioteca-fisi/backend
   ```

2. **Configurar la base de datos**:
   ```bash
   # Ejecutar el script de creación de base de datos
   sqlcmd -S localhost -i database/BibliotecaFISI.sql
   
   # Insertar datos de prueba (opcional)
   sqlcmd -S localhost -i database/DatosPrueba.sql
   ```

3. **Configurar la cadena de conexión**:
   ```json
   // appsettings.json
   {
     "ConnectionStrings": {
       "cnnNeoLibroDB": "Server=localhost;Database=BibliotecaFISI;Trusted_Connection=true;TrustServerCertificate=true;"
     }
   }
   ```

4. **Instalar dependencias**:
   ```bash
   dotnet restore
   ```

5. **Ejecutar la aplicación**:
   ```bash
   dotnet run
   ```

6. **Probar la API**:
   - Abrir navegador en: `http://localhost:5180`
   - Documentación Swagger: `http://localhost:5180/swagger`

---

## 🏗️ Arquitectura

### **Estructura del Proyecto**

```
📁 backend/NeoLibro.WebAPI/
│
├── 📁 Business/                    # 🧠 Lógica de Negocio
│   ├── UsuarioBusiness.cs         # Lógica de usuarios
│   ├── LibroBusiness.cs           # Lógica de libros
│   ├── EjemplarBusiness.cs        # Lógica de ejemplares
│   ├── PrestamoBusiness.cs        # Lógica de préstamos
│   ├── MultaBusiness.cs           # Lógica de multas
│   ├── AutorBusiness.cs           # Lógica de autores
│   └── CategoriaBusiness.cs       # Lógica de categorías
│
├── 📁 Data/                       # 💾 Repositories
│   ├── UsuarioRepository.cs       # Acceso a datos de usuarios
│   ├── LibroRepository.cs         # Acceso a datos de libros
│   ├── EjemplarRepository.cs      # Acceso a datos de ejemplares
│   ├── PrestamoRepository.cs      # Acceso a datos de préstamos
│   ├── MultaRepository.cs         # Acceso a datos de multas
│   ├── AutorRepository.cs         # Acceso a datos de autores
│   └── CategoriaRepository.cs     # Acceso a datos de categorías
│
├── 📁 Interfaces/                 # 📋 Contratos
│   ├── IUsuarioRepository.cs      # Contrato repository usuarios
│   ├── IUsuarioBusiness.cs        # Contrato business usuarios
│   ├── ILibroRepository.cs        # Contrato repository libros
│   ├── ILibroBusiness.cs          # Contrato business libros
│   └── ... (interfaces para todos los módulos)
│
├── 📁 Models/                     # 📦 Modelos de Datos
│   ├── 📁 Entities/               # Entidades de base de datos
│   │   ├── Usuario.cs             # Entidad Usuario
│   │   ├── Libro.cs               # Entidad Libro
│   │   ├── Ejemplar.cs            # Entidad Ejemplar
│   │   └── ... (todas las entidades)
│   │
│   ├── 📁 DTOs/                   # Objetos de transferencia
│   │   ├── LibroDTO.cs            # DTO para libros
│   │   ├── PrestamoDTO.cs         # DTO para préstamos
│   │   └── ... (todos los DTOs)
│   │
│   └── 📁 Requests/               # Modelos de petición
│       ├── LoginRequest.cs        # Request de login
│       ├── UsuarioRegistroRequest.cs # Request de registro
│       └── ... (todos los requests)
│
├── 📁 Controllers/                # 🌐 Controladores API
│   ├── UsuariosController.cs      # API de usuarios
│   ├── LibrosController.cs        # API de libros
│   ├── EjemplaresController.cs    # API de ejemplares
│   ├── PrestamosController.cs     # API de préstamos
│   ├── MultasController.cs        # API de multas
│   ├── AutoresController.cs       # API de autores
│   └── CategoriasController.cs    # API de categorías
│
├── 📁 Helpers/                    # 🛠️ Utilidades
│   └── SeguridadHelper.cs         # Helper de seguridad
│
├── Program.cs                     # 🚀 Configuración de la aplicación
├── appsettings.json              # ⚙️ Configuración general
└── appsettings.Development.json  # ⚙️ Configuración desarrollo
```

### **Patrones de Diseño Implementados**

| **Patrón** | **Descripción** | **Ubicación** |
|------------|-----------------|---------------|
| **Repository Pattern** | Abstrae el acceso a datos | `Data/` + `Interfaces/` |
| **Business Logic Layer** | Encapsula reglas de negocio | `Business/` + `Interfaces/` |
| **Dependency Injection** | Inyección de dependencias | `Program.cs` |
| **DTO Pattern** | Transferencia de datos | `Models/DTOs/` |
| **Request/Response Pattern** | Modelos de petición | `Models/Requests/` |

---

## 📚 Módulos del Sistema

### 👥 **Módulo Usuarios**
- **Funcionalidades**: Login, registro, perfil, cambio de contraseña
- **Roles**: Estudiante, Profesor, Bibliotecaria, Administrador
- **Endpoints**: `/api/Usuarios/*`

### 📖 **Módulo Libros**
- **Funcionalidades**: CRUD de libros, búsqueda, filtros
- **Relaciones**: Autores, Categorías, Ejemplares
- **Endpoints**: `/api/Libros/*`

### 📚 **Módulo Ejemplares**
- **Funcionalidades**: Gestión de copias físicas de libros
- **Estados**: Disponible, Prestado, En reparación, Perdido
- **Endpoints**: `/api/Ejemplares/*`

### 🔄 **Módulo Préstamos**
- **Funcionalidades**: Crear, renovar, devolver préstamos
- **Validaciones**: Disponibilidad, límites por rol, fechas
- **Endpoints**: `/api/Prestamos/*`

### 💰 **Módulo Multas**
- **Funcionalidades**: Cálculo automático, pago, historial
- **Tipos**: Retraso, pérdida, daño
- **Endpoints**: `/api/Multas/*`

### ✍️ **Módulo Autores**
- **Funcionalidades**: CRUD de autores, biografías
- **Relaciones**: Libros (muchos a muchos)
- **Endpoints**: `/api/Autores/*`

### 🏷️ **Módulo Categorías**
- **Funcionalidades**: CRUD de categorías, clasificación
- **Relaciones**: Libros (muchos a muchos)
- **Endpoints**: `/api/Categorias/*`

---

## 🔐 Autenticación y Autorización

### **Sistema de Roles**

| **Rol** | **Permisos** | **Descripción** |
|---------|-------------|-----------------|
| **Estudiante** | Ver catálogo, solicitar préstamos, ver historial | Usuario básico |
| **Profesor** | Ver catálogo, préstamos extendidos, ver historial | Usuario con privilegios |
| **Bibliotecaria** | Gestión de préstamos, multas, ejemplares | Personal de biblioteca |
| **Administrador** | Acceso completo al sistema | Administrador del sistema |

### **Endpoints por Rol**

```csharp
// Ejemplo de autorización
[Authorize(Roles = "Bibliotecaria,Administrador")]
public IActionResult CrearLibro([FromBody] Libro libro) { ... }

[Authorize(Roles = "Estudiante,Profesor")]
public IActionResult MisPrestamos() { ... }
```

---

## 🧪 Testing

### **Ejecutar Pruebas**

```bash
# Ejecutar todas las pruebas
dotnet test

# Ejecutar pruebas con cobertura
dotnet test --collect:"XPlat Code Coverage"

# Ejecutar pruebas específicas
dotnet test --filter "Category=Unit"
```

### **Tipos de Pruebas**

- **Unit Tests**: Pruebas de lógica de negocio
- **Integration Tests**: Pruebas de endpoints
- **Repository Tests**: Pruebas de acceso a datos

---

## 📖 Documentación

### **Documentación Disponible**

| **Archivo** | **Descripción** |
|-------------|-----------------|
| **[ARQUITECTURA.md](./ARQUITECTURA.md)** | Arquitectura detallada del sistema |
| **[GUIA_DESARROLLO.md](./GUIA_DESARROLLO.md)** | Guía para implementar nuevas funcionalidades |
| **[MIGRACION.md](./MIGRACION.md)** | Historial de migración de arquitectura |
| **[PRUEBAS_INTEGRACION.md](./PRUEBAS_INTEGRACION.md)** | Guía de pruebas |
| **[OPTIMIZACIONES.md](./OPTIMIZACIONES.md)** | Mejoras futuras |

### **API Documentation**

- **Swagger UI**: `http://localhost:5180/swagger`
- **OpenAPI JSON**: `http://localhost:5180/swagger/v1/swagger.json`

---

## 🛠️ Desarrollo

### **Agregar Nueva Funcionalidad**

1. **Leer la guía**: [GUIA_DESARROLLO.md](./GUIA_DESARROLLO.md)
2. **Seguir el patrón**: Repository → Business → Controller
3. **Escribir tests**: Unit tests + Integration tests
4. **Documentar**: Actualizar Swagger y documentación

### **Estructura de Commits**

```bash
# Formato de commits
feat: agregar módulo de reservas
fix: corregir validación de fechas en préstamos
docs: actualizar documentación de API
test: agregar pruebas para módulo de multas
```

### **Branches**

- `main`: Código de producción
- `develop`: Código de desarrollo
- `feature/nombre-funcionalidad`: Nuevas funcionalidades
- `hotfix/nombre-fix`: Correcciones urgentes

---

## 🚀 Despliegue

### **Desarrollo Local**

```bash
# Ejecutar en modo desarrollo
dotnet run --environment Development

# Ejecutar con hot reload
dotnet watch run
```

### **Producción**

```bash
# Compilar para producción
dotnet publish -c Release -o ./publish

# Ejecutar en producción
dotnet ./publish/NeoLibro.WebAPI.dll
```

### **Docker (Opcional)**

```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["NeoLibro.WebAPI.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NeoLibro.WebAPI.dll"]
```

---

## 🤝 Contribución

### **Cómo Contribuir**

1. **Fork** el repositorio
2. **Crear** una rama para tu funcionalidad (`git checkout -b feature/nueva-funcionalidad`)
3. **Commit** tus cambios (`git commit -m 'feat: agregar nueva funcionalidad'`)
4. **Push** a la rama (`git push origin feature/nueva-funcionalidad`)
5. **Abrir** un Pull Request

### **Estándares de Código**

- **C#**: Seguir convenciones de Microsoft
- **Testing**: Mínimo 80% de cobertura
- **Documentación**: Comentarios en métodos públicos
- **Commits**: Usar formato convencional

---

## 📞 Soporte

### **Contacto**

- **Desarrollador**: Sistema Biblioteca FISI
- **Email**: soporte@biblioteca-fisi.edu
- **Issues**: [GitHub Issues](https://github.com/tu-usuario/biblioteca-fisi/issues)

### **Recursos**

- **Documentación**: [Wiki del proyecto](https://github.com/tu-usuario/biblioteca-fisi/wiki)
- **Changelog**: [CHANGELOG.md](./CHANGELOG.md)
- **Licencia**: [LICENSE](./LICENSE)

---

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo [LICENSE](./LICENSE) para más detalles.

---

## 🏆 Reconocimientos

- **Facultad de Ingeniería de Sistemas e Informática** - Por el apoyo y recursos
- **Comunidad .NET** - Por las mejores prácticas y patrones
- **Contribuidores** - Por las mejoras y sugerencias

---

**📅 Última actualización:** 14/10/2025  
**👨‍💻 Desarrollador:** Sistema Biblioteca FISI  
**📋 Versión:** 1.0 - Backend API Completo  
**🔗 Frontend:** [Ver Frontend](../frontend/README.md)
