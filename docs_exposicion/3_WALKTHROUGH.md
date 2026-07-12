# Walkthrough: Bus de Eventos Asíncronos (RabbitMQ) y Microservicio de Notificaciones

Se ha implementado con éxito la **Mejora 3: Bus de eventos asíncronos y Microservicio de Notificaciones** en tu arquitectura distribuida.

## Componentes y Archivos Creados

1. **Infraestructura con Docker**:
   - Creado [docker-compose.yml](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/backend/docker-compose.yml) para levantar una instancia de RabbitMQ (`rabbitmq:3-management`) con interfaz web en el puerto `15672` y bus de mensajería en el puerto `5672`.

2. **Proyecto Compartido: Shared.Contracts**:
   - Creado el proyecto en `backend/Shared.Contracts` y registrado en la solución [backend.sln](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/backend/backend.sln).
   - Contiene la definición del evento [PrestamoCreadoEvent.cs](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/backend/Shared.Contracts/PrestamoCreadoEvent.cs) (`record` de C# con `PrestamoId`, `UsuarioId`, `EmailUsuario`, `TituloLibro` y `FechaDevolucionPrevista`).

3. **Publisher en el Monolito (NeoLibro.WebAPI)**:
   - Configurada la inicialización de **MassTransit con RabbitMQ** en [Program.cs](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/backend/NeoLibro.WebAPI/Program.cs).
   - Inyectado `IPublishEndpoint` en [PrestamosController.cs](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/backend/NeoLibro.WebAPI/Controllers/PrestamosController.cs) y [ReservasController.cs](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/backend/NeoLibro.WebAPI/Controllers/ReservasController.cs).
   - Al crear un préstamo de manera directa (`CrearPrestamo`) o al aprobar una reserva (`Aprobar`), el monolito recopila la información del libro/usuario de forma asíncrona y publica el evento `PrestamoCreadoEvent` en RabbitMQ en lugar de manejar la notificación internamente.

4. **Microservicio: Notificaciones.Service**:
   - Creado un Worker Service en `backend/Notificaciones.Service` y registrado en la solución [backend.sln](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/backend/backend.sln).
   - Creado el consumidor [PrestamoCreadoConsumer.cs](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/backend/Notificaciones.Service/Consumers/PrestamoCreadoConsumer.cs) que recibe los eventos y registra la acción simulando el envío del correo electrónico.
   - Configurada la recepción automatizada de MassTransit con RabbitMQ en [Program.cs](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/backend/Notificaciones.Service/Program.cs).

---

## Guía de Ejecución y Pruebas

Para levantar todo el ecosistema distribuido y validar que los mensajes fluyan de un servicio a otro, sigue estos pasos:

### 1. Levantar RabbitMQ (Docker)
1. Abre tu terminal de Docker en la carpeta `backend` y ejecuta:
   ```bash
   docker compose up -d
   ```
2. Puedes comprobar que esté listo accediendo a la consola de administración en el navegador:
   - **URL**: `http://localhost:15672`
   - **Credenciales**: `guest` / `guest`

### 2. Ejecutar los Servicios del Backend (Monolito, Gateway y Notificaciones)
Puedes usar **Visual Studio 2022** configurando *Proyectos de inicio múltiples* para arrancar los tres proyectos a la vez, o bien abrir terminales de comando independientes:

*   **Terminal 1 (Monolito)**:
    ```bash
    cd backend/NeoLibro.WebAPI
    dotnet run
    ```
*   **Terminal 2 (API Gateway)**:
    ```bash
    cd backend/ApiGateway
    dotnet run
    ```
*   **Terminal 3 (Servicio de Notificaciones)**:
    ```bash
    cd backend/Notificaciones.Service
    dotnet run
    ```

### 3. Levantar el Frontend React
*   **Terminal 4 (Frontend)**:
    ```bash
    cd frontend/frontend
    npm run dev
    ```

### 4. Validar el Flujo de Notificaciones
1. Entra a `http://localhost:5173/` e inicia sesión.
2. Ve al **Catálogo** y solicita el préstamo de cualquier libro.
3. Observa la terminal del **Servicio de Notificaciones** (`Notificaciones.Service`). Deberías ver aparecer un log estructurado similar a este en tiempo real:
   ```text
   info: Notificaciones.Service.Consumers.PrestamoCreadoConsumer[0]
         ==================================================
         📬 NUEVO EVENTO RECIBIDO: PrestamoCreadoEvent
            ID Prestamo: 124
            ID Usuario: 1
            Email: admin@unmsm.edu.pe
            Libro: LOGICA MATEMATICA un enfoque axiomatico
            Devolucion Prevista: 15/07/2026
            Simulacion: Enviando email a admin@unmsm.edu.pe para el libro 'LOGICA MATEMATICA un enfoque axiomatico'...
         ==================================================
   ```
   Esto comprueba que el evento fue publicado por el monolito en RabbitMQ, ruteado al microservicio e interpretado correctamente por el consumidor asíncrono.

---

## 🛠️ Resolución de Problemas (Troubleshooting) Durante el Proceso

Durante la integración y puesta en marcha del ecosistema distribuido, se identificaron y solucionaron los siguientes puntos técnicos:

### 1. Error de Enlace de Configuración de YARP (API Gateway)
*   **Problema**: El API Gateway fallaba al arrancar con el error `Unable to load or apply the proxy configuration.` debido a que la sintaxis de `Routes` y `Clusters` en el `appsettings.json` no cumplía exactamente con la firma de enrutamiento de YARP.
*   **Solución**: Se eliminaron los campos internos `"RouteId"` y `"ClusterId"` y se reorganizó la configuración para estructurarlos como un objeto/diccionario JSON cuya llave actúa directamente como el identificador del recurso.

### 2. Conflicto de Licencia en MassTransit v9 (Backend)
*   **Problema**: Al arrancar `NeoLibro.WebAPI` y `Notificaciones.Service` se lanzaba una excepción crítica: `[Failure] License must be specified with SetLicense/SetLicenseLocation...`. Esto ocurre porque MassTransit v9 requiere una clave o configuración de licencia de uso comercial.
*   **Solución**: Se removió `MassTransit.AspNetCore` (obsoleto/integrado a partir de v8) y se degradaron las referencias de los paquetes `MassTransit` y `MassTransit.RabbitMQ` a la versión **`8.2.2`** (completamente abierta y sin requisitos de licencia), resolviendo la restricción y logrando que los servicios inicien limpiamente.

### 3. Prueba de Eventos con Swagger
*   **Detalle**: Dado que el botón "Reservar" en la interfaz de usuario está oculto por seguridad para los usuarios con rol *Administrador*, se recomendó y ejecutó la validación del flujo mediante el Swagger del Gateway en `http://localhost:5200/swagger/index.html`.
*   **Resultado**: Al hacer `POST /api/Prestamos` con el payload de prueba y la sesión activa del navegador, se validó exitosamente que YARP redirige la petición, el monolito genera el préstamo en SQL Server, publica el evento en RabbitMQ, y el microservicio lo consume de forma asíncrona imprimiendo la simulación del email.

---

## 🌐 Simulación de CDN para Recursos Digitales (Fase 1)

Se ha implementado con éxito la **Fase 1: Simulación de CDN** para descargar y renderizar recursos digitales pesados (portadas de libros y archivos PDF) de manera independiente al backend.

### 1. Servidor Estático Nginx (Local CDN)
*   **Contenedor**: Se agregó el servicio `cdn` al archivo [docker-compose.yml](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/backend/docker-compose.yml) usando `nginx:alpine` expuesto en el puerto **`8080`**.
*   **Carpetas de recursos**: Creado el directorio físico [cdn-resources](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/backend/cdn-resources) con subdirectorios `portadas/` y `pdf/`.
*   **Recursos simulados**:
    *   `portada-ejemplo.png`: Una portada moderna diseñada profesionalmente usando inteligencia artificial generativa.
    *   `libro-ejemplo.pdf`: Un documento digital de prueba listo para lectura.

### 2. Cambios en la Interfaz de Usuario (Frontend)
*   Se modificó [Catalog.tsx](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/frontend/frontend/src/features/catalog/pages/Catalog/Catalog.tsx) dentro del Modal de Detalles del libro.
*   Ahora, al hacer clic en **Ver Detalles** (ícono de ojo) en cualquier libro:
    *   Se carga a la izquierda de forma asíncrona la **portada del libro** directamente desde la CDN: `http://localhost:8080/recursos/portadas/portada-ejemplo.png`.
    *   Se despliega un botón azul de **"Leer PDF (CDN)"** que enlaza directamente al archivo digital: `http://localhost:8080/recursos/pdf/libro-ejemplo.pdf`.

### 3. Validación y Pruebas Visuales
1. Ingresa al catálogo: `http://localhost:5173`.
2. Haz clic en el ícono de **Ver detalles** (el ojo azul en la columna derecha de cualquier libro).
3. Comprueba cómo se carga la portada y el enlace al PDF en tiempo de ejecución. Ambos provienen del servidor estático aislado de Nginx, simulando el funcionamiento de una CDN de producción.

---

## ⚡ Caché en Memoria con Redis (Fase 2)

Se ha implementado con éxito la **Fase 2: Redis Cache** para optimizar la velocidad de obtención de libros en el catálogo de la biblioteca.

### 1. Servidor de Caché (Redis)
*   **Contenedor**: Se agregó el servicio `redis` al archivo [docker-compose.yml](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/backend/docker-compose.yml) usando `redis:alpine` en el puerto por defecto **`6379`**.

### 2. Dependencias y Configuración
*   Se instaló el paquete NuGet `Microsoft.Extensions.Caching.StackExchangeRedis` en el proyecto monolito.
*   En [Program.cs](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/backend/NeoLibro.WebAPI/Program.cs) se registró la caché distribuida de Redis apuntando a `localhost:6379` e indicando el prefijo `"Biblioteca_"`.

### 3. Modificación de LibrosController
*   En [LibrosController.cs](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/backend/NeoLibro.WebAPI/Controllers/LibrosController.cs) se inyectó la interfaz `IDistributedCache` de ASP.NET Core y el servicio `ILogger`.
*   El método `Listar` (GET `/api/Libros`) ahora tiene la siguiente lógica distribuida:
    1.  **Búsqueda en Caché**: Intenta obtener la clave `Catalog_Libros_List` desde Redis.
    2.  **Cache Hit**: Si existe, se deserializa la lista JSON y se retorna de inmediato, registrando en consola:
        `🚀 [REDIS] Cache Hit - Libros obtenidos de memoria`
    3.  **Cache Miss**: Si no existe, se realiza la consulta a SQL Server, se serializa el resultado a JSON, se guarda en Redis con una expiración absoluta de **5 minutos**, y se registra en consola:
        `🔍 [SQL] Cache Miss - Consultando base de datos`

### 4. Instrucciones para la Demostración Técnica
1.  **Reiniciar el Monolito**: Dado que el monolito ya estaba en ejecución, detén el proceso (`Ctrl + C` en la consola de `NeoLibro.WebAPI`) y vuelve a ejecutarlo:
    ```bash
    dotnet run
    ```
2.  **Primera Carga (Cache Miss)**: Recarga el catálogo desde la interfaz web o Swagger. En la terminal de la API se imprimirá el recuadro de `[SQL] Cache Miss`.
3.  **Siguientes Cargas (Cache Hit)**: Recarga la página. Comprobarás que las consultas no viajan a SQL Server y responden de manera instantánea imprimiendo en el log: `[REDIS] Cache Hit`.

---

## 🔀 Separación de Lectura y Escritura - Réplica de BD (Fase 3)

Se ha implementado con éxito la **Fase 3: Separación de Lectura y Escritura** a nivel de software mediante la interceptación en tiempo de ejecución del tipo de petición HTTP.

### 1. Configuración de Conexiones
*   Se añadieron las cadenas `"WriteConnection"` (base de datos principal para mutaciones) y `"ReadOnlyConnection"` (réplica para lecturas) al [appsettings.json](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/backend/NeoLibro.WebAPI/appsettings.json) del Monolito.

### 2. Resolución Dinámica de Conexión en C#
*   En [Program.cs](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/backend/NeoLibro.WebAPI/Program.cs) se registró `AddHttpContextAccessor()` y se definió una fábrica `Scoped` para la resolución del `string` de conexión de la base de datos basado en el verbo HTTP:
    *   **Peticiones GET (Lecturas)**: Obtienen la `ReadOnlyConnection` e imprimen en consola:
        `📥 [DB-READ] Conectando a replica de lectura...`
    *   **Peticiones POST, PUT, DELETE (Escrituras/Mutaciones)**: Obtienen la `WriteConnection` e imprimen en consola:
        `📤 [DB-WRITE] Ejecutando transaccion en BD principal...`
*   Todos los repositorios ADO.NET clásicos y la configuración de `ApplicationDbContext` (Entity Framework) se modificaron para inyectar esta cadena de conexión dinámica en lugar del string estático inicial.

### 3. Instrucciones de Validación
1.  Detén el backend (`Ctrl + C` en la terminal de `NeoLibro.WebAPI`) y arráncalo nuevamente con `dotnet run`.
2.  **Validar Lectura (GET)**: Navega por la interfaz (ej. Catálogo de Libros o Mis Préstamos). Dado que son peticiones de consulta, observarás en la terminal el log del bloque `[DB-READ]`.
3.  **Prueba de Escritura (POST/MUTACIÓN)**: Realiza un préstamo o aprueba una reserva. Puesto que es una operación de cambio de estado, observarás en la terminal el log del bloque `[DB-WRITE]`.

---

## 📚 Extracción del Microservicio de Catálogo (Fase 4)

Se ha completado con éxito la **Fase 4: Extracción del Microservicio de Catálogo** para independizar el dominio de libros, ejemplares, autores y categorías del Monolito.

### 1. Creación e Integración de `Catalogo.Service`
*   Se creó el nuevo proyecto Web API C# llamado `Catalogo.Service` en el puerto **`5201`**.
*   Se migraron físicamente los controladores (`LibrosController`, `EjemplaresController`, `AutoresController` y `CategoriasController`), repositorios SQL, clases de negocio y entidades asociadas desde `NeoLibro.WebAPI`.
*   Se reubicaron las configuraciones de Redis Cache y cadenas de conexión de base de datos a `Catalogo.Service` para que el catálogo maneje sus consultas directamente.

### 2. Comunicación Síncrona entre Microservicios (HttpClient)
*   En el Monolito (`NeoLibro.WebAPI`), se implementaron **`HttpLibroRepository`** y **`HttpEjemplarRepository`** bajo `ILibroRepository` y `IEjemplarRepository` respectivamente.
*   Cuando el Monolito requiere validar un ejemplar o consultar el título de un libro para sus transacciones internas (préstamos y notificaciones), realiza una llamada HTTP síncrona `HttpClient` al endpoint de `Catalogo.Service` en `http://localhost:5201/`.

### 3. Configuración del API Gateway (YARP)
*   Se actualizó el [appsettings.json](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/backend/ApiGateway/appsettings.json) del API Gateway (`5200`) redirigiendo los tráficos `/api/Libros`, `/api/Ejemplares`, `/api/Autores` y `/api/Categorias` al clúster de Catálogo en el puerto `5201`, manteniendo el resto al Monolito original en el puerto `5180`.

### 4. Instrucciones de Validación
1.  Inicia el nuevo microservicio en una nueva consola:
    ```bash
    cd backend/Catalogo.Service
    dotnet run
    ```
2.  Reinicia el Monolito y el API Gateway.
3.  Navega por la aplicación web. El frontend se comunica transparentemente a través del puerto `5200` y YARP distribuye las solicitudes de catálogo a `5201` y el resto a `5180`.
