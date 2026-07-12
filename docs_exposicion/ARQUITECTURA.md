# 🏗️ Arquitectura del Sistema - Biblioteca FISI

## 📋 Índice
1. [Visión General](#visión-general)
2. [Arquitectura Actual](#arquitectura-actual)
3. [Estructura de Carpetas](#estructura-de-carpetas)
4. [Patrones Implementados](#patrones-implementados)
5. [Flujo de Datos](#flujo-de-datos)
6. [Módulos del Sistema](#módulos-del-sistema)
7. [Inyección de Dependencias](#inyección-de-dependencias)
8. [Diagramas](#diagramas)
9. [Mejores Prácticas](#mejores-prácticas)
10. [Guías de Desarrollo](#guías-de-desarrollo)

---

## 🎯 Visión General

El sistema Biblioteca FISI ha sido migrado de una arquitectura monolítica N-Layer a una **arquitectura modular con separación clara de responsabilidades**, implementando patrones de diseño modernos y mejores prácticas de desarrollo.

### 🎯 Objetivos Logrados
- ✅ **Separación de responsabilidades** clara y bien definida
- ✅ **Mantenibilidad** mejorada significativamente
- ✅ **Escalabilidad** preparada para crecimiento futuro
- ✅ **Testabilidad** con interfaces y inyección de dependencias
- ✅ **Reutilización** de código a través de interfaces
- ✅ **Flexibilidad** para cambios y extensiones
- ✅ **Documentación completa** para desarrollo futuro
- ✅ **Código limpio** sin duplicaciones ni archivos obsoletos

---

## 🏗️ Arquitectura Actual

### **Patrón Principal: N-Layer Mejorado con Repository Pattern**

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                       │
│  ┌─────────────────┐  ┌─────────────────┐  ┌──────────────┐ │
│  │   Controllers   │  │   Middleware    │  │   Filters    │ │
│  └─────────────────┘  └─────────────────┘  └──────────────┘ │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    BUSINESS LAYER                           │
│  ┌─────────────────┐  ┌─────────────────┐  ┌──────────────┐ │
│  │  Business Logic │  │   Validation    │  │   Services   │ │
│  │   (Business/)   │  │   (Business/)   │  │  (Business/) │ │
│  └─────────────────┘  └─────────────────┘  └──────────────┘ │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                     DATA LAYER                              │
│  ┌─────────────────┐  ┌─────────────────┐  ┌──────────────┐ │
│  │   Repository    │  │   Data Access   │  │   Entities   │ │
│  │   (Data/)       │  │   (ADO.NET)     │  │ (Entities/)  │ │
│  └─────────────────┘  └─────────────────┘  └──────────────┘ │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    DATABASE LAYER                           │
│  ┌─────────────────┐  ┌─────────────────┐  ┌──────────────┐ │
│  │   SQL Server    │  │   Stored Procs  │  │   Views      │ │
│  │   Database      │  │   (Opcional)    │  │ (Opcional)   │ │
│  └─────────────────┘  └─────────────────┘  └──────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

---

## 📁 Estructura de Carpetas

```
backend/NeoLibro.WebAPI/
├── 📁 Business/                    # Lógica de negocio
│   ├── UsuarioBusiness.cs
│   ├── LibroBusiness.cs
│   ├── EjemplarBusiness.cs
│   ├── PrestamoBusiness.cs
│   ├── MultaBusiness.cs
│   ├── AutorBusiness.cs
│   └── CategoriaBusiness.cs
│
├── 📁 Data/                       # Acceso a datos (Repository)
│   ├── UsuarioRepository.cs
│   ├── LibroRepository.cs
│   ├── EjemplarRepository.cs
│   ├── PrestamoRepository.cs
│   ├── MultaRepository.cs
│   ├── AutorRepository.cs
│   └── CategoriaRepository.cs
│
├── 📁 Interfaces/                 # Contratos (Interfaces)
│   ├── IUsuarioRepository.cs
│   ├── IUsuarioBusiness.cs
│   ├── ILibroRepository.cs
│   ├── ILibroBusiness.cs
│   ├── IEjemplarRepository.cs
│   ├── IEjemplarBusiness.cs
│   ├── IPrestamoRepository.cs
│   ├── IPrestamoBusiness.cs
│   ├── IMultaRepository.cs
│   ├── IMultaBusiness.cs
│   ├── IAutorRepository.cs
│   ├── IAutorBusiness.cs
│   ├── ICategoriaRepository.cs
│   └── ICategoriaBusiness.cs
│
├── 📁 Models/                     # Modelos de datos
│   ├── 📁 Entities/               # Entidades de base de datos
│   │   ├── Usuario.cs
│   │   ├── Libro.cs
│   │   ├── Ejemplar.cs
│   │   ├── Prestamo.cs
│   │   ├── Multa.cs
│   │   ├── Autor.cs
│   │   └── Categoria.cs
│   │
│   ├── 📁 DTOs/                   # Data Transfer Objects
│   │   ├── LibroDTO.cs
│   │   ├── PrestamoDTO.cs
│   │   ├── ResumenMultasDTO.cs
│   │   ├── EstadisticasUsuarioDTO.cs
│   │   └── PerfilUsuarioDTO.cs
│   │
│   └── 📁 Requests/               # Modelos de petición
│       ├── LoginRequest.cs
│       ├── UsuarioRegistroRequest.cs
│       ├── ActualizarPerfilRequest.cs
│       ├── CambiarContrasenaRequest.cs
│       ├── SolicitarRecuperacionRequest.cs
│       ├── ResetearContrasenaRequest.cs
│       ├── CrearPrestamoRequest.cs
│       ├── DevolucionRequest.cs
│       ├── RenovacionRequest.cs
│       ├── PagarMultaRequest.cs
│       └── CrearMultaRequest.cs
│
├── 📁 Controllers/                # Controladores API
│   ├── UsuariosController.cs
│   ├── LibrosController.cs
│   ├── EjemplaresController.cs
│   ├── PrestamosController.cs
│   ├── MultasController.cs
│   ├── AutoresController.cs
│   └── CategoriasController.cs
│
├── 📁 Helpers/                    # Utilidades
│   └── SeguridadHelper.cs
│
├── Program.cs                     # Configuración de la aplicación
└── appsettings.json              # Configuración
```

---

## 🎨 Patrones Implementados

### 1. **Repository Pattern**
- **Propósito**: Abstrae el acceso a datos
- **Ubicación**: `Data/` + `Interfaces/`
- **Beneficios**: 
  - Separación clara entre lógica de negocio y acceso a datos
  - Fácil testing con mocks
  - Flexibilidad para cambiar la fuente de datos

### 2. **Business Logic Layer**
- **Propósito**: Contiene la lógica de negocio
- **Ubicación**: `Business/` + `Interfaces/`
- **Beneficios**:
  - Validaciones centralizadas
  - Reglas de negocio encapsuladas
  - Reutilización de lógica

### 3. **Dependency Injection**
- **Propósito**: Inversión de control
- **Ubicación**: `Program.cs`
- **Beneficios**:
  - Bajo acoplamiento
  - Fácil testing
  - Configuración centralizada

### 4. **Interface Segregation**
- **Propósito**: Contratos bien definidos
- **Ubicación**: `Interfaces/`
- **Beneficios**:
  - Principio de responsabilidad única
  - Fácil implementación de mocks
  - Contratos claros

### 5. **DTO Pattern**
- **Propósito**: Transferencia de datos optimizada
- **Ubicación**: `Models/DTOs/`
- **Beneficios**:
  - Separación entre entidades internas y datos expuestos
  - Optimización de transferencia
  - Seguridad de datos

---

## 🔄 Flujo de Datos

### **Flujo Típico de una Petición API:**

```
1. Cliente → Controller
   ↓
2. Controller → Business Layer
   ↓
3. Business Layer → Repository
   ↓
4. Repository → Database
   ↓
5. Database → Repository
   ↓
6. Repository → Business Layer
   ↓
7. Business Layer → Controller
   ↓
8. Controller → Cliente
```

### **Ejemplo Práctico - Crear Usuario:**

```csharp
// 1. Controller recibe petición
[HttpPost]
public IActionResult Crear([FromBody] UsuarioRegistroRequest request)
{
    // 2. Llama al Business Layer
    var resultado = _usuarioBusiness.Crear(request);
    return resultado ? Ok() : BadRequest();
}

// 3. Business Layer valida y procesa
public bool Crear(UsuarioRegistroRequest request)
{
    // Validaciones de negocio
    if (string.IsNullOrEmpty(request.Email)) return false;
    
    // 4. Llama al Repository
    return _usuarioRepository.Crear(usuario);
}

// 5. Repository accede a datos
public bool Crear(Usuario usuario)
{
    // Lógica de ADO.NET
    using (var connection = new SqlConnection(_connectionString))
    {
        // INSERT INTO Usuarios...
    }
}
```

---

## 🧩 Módulos del Sistema

### **1. Módulo Usuarios**
- **Entidad**: `Usuario`
- **Repository**: `UsuarioRepository`
- **Business**: `UsuarioBusiness`
- **Controller**: `UsuariosController`
- **Funcionalidades**: Login, registro, perfil, cambio de contraseña

### **2. Módulo Libros**
- **Entidad**: `Libro`
- **Repository**: `LibroRepository`
- **Business**: `LibroBusiness`
- **Controller**: `LibrosController`
- **Funcionalidades**: CRUD libros, búsqueda, catálogo

### **3. Módulo Ejemplares**
- **Entidad**: `Ejemplar`
- **Repository**: `EjemplarRepository`
- **Business**: `EjemplarBusiness`
- **Controller**: `EjemplaresController`
- **Funcionalidades**: Gestión de ejemplares, disponibilidad

### **4. Módulo Préstamos**
- **Entidad**: `Prestamo`
- **Repository**: `PrestamoRepository`
- **Business**: `PrestamoBusiness`
- **Controller**: `PrestamosController`
- **Funcionalidades**: Préstamos, devoluciones, renovaciones

### **5. Módulo Multas**
- **Entidad**: `Multa`
- **Repository**: `MultaRepository`
- **Business**: `MultaBusiness`
- **Controller**: `MultasController`
- **Funcionalidades**: Gestión de multas, pagos

### **6. Módulo Autores**
- **Entidad**: `Autor`
- **Repository**: `AutorRepository`
- **Business**: `AutorBusiness`
- **Controller**: `AutoresController`
- **Funcionalidades**: CRUD autores, búsqueda

### **7. Módulo Categorías**
- **Entidad**: `Categoria`
- **Repository**: `CategoriaRepository`
- **Business**: `CategoriaBusiness`
- **Controller**: `CategoriasController`
- **Funcionalidades**: CRUD categorías, organización

---

## 💉 Inyección de Dependencias

### **Configuración en Program.cs:**

```csharp
// Repositories
builder.Services.AddScoped<IUsuarioRepository>(provider => 
    new UsuarioRepository(connectionString));
builder.Services.AddScoped<ILibroRepository>(provider => 
    new LibroRepository(connectionString));
// ... otros repositories

// Business Services
builder.Services.AddScoped<IUsuarioBusiness, UsuarioBusiness>();
builder.Services.AddScoped<ILibroBusiness, LibroBusiness>();
// ... otros business services
```

### **Uso en Controllers:**

```csharp
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioBusiness _usuarioBusiness;

    public UsuariosController(IUsuarioBusiness usuarioBusiness)
    {
        _usuarioBusiness = usuarioBusiness;
    }
}
```

---

## 📊 Diagramas

### **Diagrama de Dependencias:**

```
Controllers
    ↓
Business Layer (Interfaces)
    ↓
Repository Layer (Interfaces)
    ↓
Data Access (ADO.NET)
    ↓
Database (SQL Server)
```

### **Diagrama de Módulos:**

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   Usuarios  │    │   Libros    │    │ Ejemplares  │
│             │    │             │    │             │
│ Controller  │    │ Controller  │    │ Controller  │
│ Business    │    │ Business    │    │ Business    │
│ Repository  │    │ Repository  │    │ Repository  │
└─────────────┘    └─────────────┘    └─────────────┘
       │                   │                   │
       └───────────────────┼───────────────────┘
                           │
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│  Préstamos  │    │   Multas    │    │   Autores   │
│             │    │             │    │             │
│ Controller  │    │ Controller  │    │ Controller  │
│ Business    │    │ Business    │    │ Business    │
│ Repository  │    │ Repository  │    │ Repository  │
└─────────────┘    └─────────────┘    └─────────────┘
                           │
                   ┌─────────────┐
                   │ Categorías  │
                   │             │
                   │ Controller  │
                   │ Business    │
                   │ Repository  │
                   └─────────────┘
```

---

## ✅ Mejores Prácticas

### **1. Naming Conventions**
- **Interfaces**: `I[Nombre]Repository`, `I[Nombre]Business`
- **Implementaciones**: `[Nombre]Repository`, `[Nombre]Business`
- **Entidades**: `[Nombre]` (singular)
- **DTOs**: `[Nombre]DTO`
- **Requests**: `[Nombre]Request`

### **2. Responsabilidades por Capa**
- **Controllers**: Solo manejo de HTTP, validación básica
- **Business**: Lógica de negocio, validaciones complejas
- **Repository**: Solo acceso a datos, sin lógica de negocio
- **Entities**: Solo propiedades, sin lógica

### **3. Manejo de Errores**
- **Controllers**: Manejo de códigos HTTP apropiados
- **Business**: Validaciones y reglas de negocio
- **Repository**: Manejo de excepciones de base de datos

### **4. Testing**
- **Unit Tests**: Para Business Layer y Repository
- **Integration Tests**: Para Controllers
- **Mocking**: Usar interfaces para testing

---

## 🚀 Guías de Desarrollo

### **Agregar un Nuevo Módulo:**

1. **Crear Entidad** en `Models/Entities/`
2. **Crear Interface Repository** en `Interfaces/`
3. **Crear Repository** en `Data/`
4. **Crear Interface Business** en `Interfaces/`
5. **Crear Business** en `Business/`
6. **Crear Controller** en `Controllers/`
7. **Registrar en Program.cs**
8. **Crear DTOs/Requests** si es necesario

### **Modificar un Módulo Existente:**

1. **Actualizar Entidad** si es necesario
2. **Modificar Repository** para nuevos métodos
3. **Actualizar Business** con nueva lógica
4. **Modificar Controller** para nuevos endpoints
5. **Actualizar DTOs/Requests** si es necesario

### **Testing de un Módulo:**

```csharp
// Ejemplo de Unit Test
[Test]
public void UsuarioBusiness_Crear_ConEmailValido_RetornaTrue()
{
    // Arrange
    var mockRepository = new Mock<IUsuarioRepository>();
    var business = new UsuarioBusiness(mockRepository.Object);
    
    // Act
    var resultado = business.Crear(usuarioValido);
    
    // Assert
    Assert.IsTrue(resultado);
}
```

---

## 📈 Beneficios de la Nueva Arquitectura

### **Mantenibilidad**
- ✅ Código más organizado y modular
- ✅ Separación clara de responsabilidades
- ✅ Fácil localización de funcionalidades
- ✅ Cambios aislados por módulo

### **Escalabilidad**
- ✅ Fácil agregar nuevos módulos
- ✅ Arquitectura preparada para crecimiento
- ✅ Reutilización de componentes
- ✅ Flexibilidad para cambios

### **Testabilidad**
- ✅ Interfaces permiten mocking fácil
- ✅ Lógica de negocio aislada
- ✅ Unit tests independientes
- ✅ Integration tests claros

### **Calidad**
- ✅ Principios SOLID aplicados
- ✅ Patrones de diseño implementados
- ✅ Código más legible y documentado
- ✅ Mejores prácticas seguidas

---

## 🔮 Futuras Mejoras

### **Corto Plazo**
- [ ] Agregar logging estructurado
- [ ] Implementar caching
- [ ] Mejorar manejo de errores
- [ ] Agregar validaciones automáticas

### **Mediano Plazo**
- [ ] Implementar CQRS
- [ ] Agregar Event Sourcing
- [ ] Implementar microservicios
- [ ] Agregar API Gateway

### **Largo Plazo**
- [ ] Migrar a .NET 10+
- [ ] Implementar GraphQL
- [ ] Agregar machine learning
- [ ] Implementar real-time features

## 🛠️ Guías de Desarrollo

### 📚 Documentación Disponible
- **[MIGRACION.md](./MIGRACION.md)** - Historial completo de la migración
- **[GUIA_DESARROLLO.md](./GUIA_DESARROLLO.md)** - Guía completa para implementar nuevas funcionalidades
- **[PRUEBAS_INTEGRACION.md](./PRUEBAS_INTEGRACION.md)** - Guía de pruebas
- **[OPTIMIZACIONES.md](./OPTIMIZACIONES.md)** - Mejoras futuras
- **[README.md](./README.md)** - Documentación principal del proyecto

### 📖 Cómo Usar Esta Documentación
1. **Para nuevos desarrolladores**: Comenzar con [README.md](./README.md)
2. **Para implementar funcionalidades**: Usar [GUIA_DESARROLLO.md](./GUIA_DESARROLLO.md)
3. **Para entender la migración**: Revisar [MIGRACION.md](./MIGRACION.md)
4. **Para hacer pruebas**: Seguir [PRUEBAS_INTEGRACION.md](./PRUEBAS_INTEGRACION.md)

---

**📅 Última actualización:** 14/10/2025  
**👨‍💻 Arquitecto:** Sistema Biblioteca FISI  
**📋 Versión:** 2.0 - Arquitectura Modular Completa
