# Sustento Teórico y Técnico de Cambios de Arquitectura (CAMBIOS_ARQUITECTURA.md)

Este documento contiene el sustento formal de ingeniería de software para el jurado de la universidad. Detalla las 6 mejoras de arquitectura de sistemas distribuidos implementadas sobre el sistema monolítico original para aportar escalabilidad, desacoplamiento, tolerancia a fallos y alta disponibilidad.

---

## 🛠️ 1. API Gateway con YARP (Reverse Proxy)

### Detalles de la Implementación
*   **Proyecto creado**: `backend/ApiGateway` (.NET 9.0 Web API con el framework Microsoft YARP).
*   **Configuración**: [appsettings.json](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/backend/ApiGateway/appsettings.json) expuesto en el puerto principal `5200`. Enruta dinámicamente `/api/Libros`, `/api/Ejemplares`, `/api/Autores` y `/api/Categorias` hacia el Microservicio de Catálogo (`http://localhost:5201`), y el resto de operaciones (`/api/Prestamos`, `/api/Usuarios`, etc.) hacia el Monolito (`http://localhost:5180`).
*   **Frontend**: Se actualizó [vite.config.ts](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/frontend/frontend/vite.config.ts) para apuntar todas sus peticiones al puerto del Gateway (`5200`) de forma unificada.

### Justificación en Sistemas Distribuidos
*   **Abstracción de Red (Single Entry Point)**: El cliente (frontend) no necesita saber la ubicación física ni los puertos de los diferentes microservicios backend. El API Gateway unifica la superficie de red, lo que permite reubicar, balancear o apagar microservicios sin modificar la interfaz gráfica.
*   **Seguridad y Centralización**: Facilita la implementación de políticas transversales como el control de CORS, limitación de tasa de peticiones (Rate Limiting) y autenticación perimetral antes de delegar el procesamiento a los microservicios de negocio.

---

## 📬 2. Bus de Eventos Asíncronos (RabbitMQ + MassTransit)

### Detalles de la Implementación
*   **Broker de Mensajería**: Contenedor Docker de RabbitMQ (`rabbitmq:3-management`) en los puertos `5672` (mensajería) y `15672` (consola web).
*   **Contratos Compartidos**: Biblioteca `Shared.Contracts` que contiene el evento serializado `PrestamoCreadoEvent`.
*   **Publisher (Monolito)**: Registrado MassTransit v8.2.2. Inyecta `IPublishEndpoint` en [PrestamosController.cs](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/backend/NeoLibro.WebAPI/Controllers/PrestamosController.cs) y [ReservasController.cs](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/backend/NeoLibro.WebAPI/Controllers/ReservasController.cs) para publicar eventos en colas de RabbitMQ.
*   **Consumer (Notificaciones.Service)**: Worker Service independiente (`backend/Notificaciones.Service`) que consume el evento asíncronamente a través de `PrestamoCreadoConsumer` e imprime la simulación del envío de correos.

### Justificación en Sistemas Distribuidos
*   **Desacoplamiento Temporal**: El monolito no depende de que el servicio de notificaciones esté activo para completar la creación de un préstamo. Publica el mensaje en RabbitMQ y responde de inmediato al usuario, logrando una latencia mínima de interfaz.
*   **Tolerancia a Fallos y Consistencia Eventual**: Si el servicio de notificaciones experimenta una caída, los eventos de préstamos se acumulan de manera segura en RabbitMQ. Al reanudarse el microservicio, procesa los mensajes acumulados de forma automática, garantizando la consistencia eventual sin pérdida de datos.

---

## 🌐 3. Servidor de Contenido Estático (CDN Simulada con Nginx)

### Detalles de la Implementación
*   **Contenedor**: Servidor estático e independiente Nginx (`nginx:alpine`) levantado en Docker y expuesto en el puerto **`8080`**.
*   **Volumen de Recursos**: Carpeta `backend/cdn-resources` conteniendo las subcarpetas `portadas/` y `pdf/`.
*   **Frontend**: Modificado [Catalog.tsx](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/frontend/frontend/src/features/catalog/pages/Catalog/Catalog.tsx) para renderizar portadas de libros y proveer descargas de PDFs utilizando la dirección de red directa de la CDN (`http://localhost:8080/recursos/...`).

### Justificación en Sistemas Distribuidos
*   **Resource Offloading**: Descarga al servidor de aplicación principal de procesar y transferir archivos pesados (imágenes, PDFs de lectura), delegando esta tarea a un software optimizado para transferencia estática.
*   **Simulación de Caché de Borde (Edge Caching)**: Representa el comportamiento de las redes de entrega de contenido (CDN) en la nube, donde el contenido estático se distribuye geográficamente cerca del cliente final para optimizar los tiempos de carga global.

---

## ⚡ 4. Caché Distribuido (Redis)

### Detalles de la Implementación
*   **Servicio de Caché**: Contenedor Docker de `redis:alpine` expuesto en el puerto **`6379`**.
*   **Integración**: Inyección de `IDistributedCache` mediante el paquete `Microsoft.Extensions.Caching.StackExchangeRedis` en el nuevo microservicio de Catálogo.
*   **Lógica**: En `LibrosController.cs` (dentro de `Catalogo.Service`), el endpoint `Listar` verifica la clave `Catalog_Libros_List` en Redis. Si hay **Hit**, retorna directamente; en caso de **Miss**, consulta a SQL Server, guarda el resultado serializado en Redis por 5 minutos y responde.

### Justificación en Sistemas Distribuidos
*   **Reducción de Carga en Base de Datos**: Evita consultas repetidas e intensivas al motor relacional SQL Server para datos de catálogo que cambian con muy baja frecuencia, protegiendo la base de datos de cuellos de botella.
*   **Caché Fuera de Proceso (Out-of-Process Caching)**: A diferencia de la memoria en caché interna (In-Memory), almacenar la caché en Redis permite que múltiples instancias levantadas horizontalmente de `Catalogo.Service` compartan el mismo estado de caché rápido, facilitando la escalabilidad elástica.

---

## 🔀 5. Separación de Lectura y Escritura (Patrón de Réplica de BD a Nivel de Software)

### Detalles de la Implementación
*   **Cadenas de Conexión**: Configuración de `WriteConnection` (BD Principal) y `ReadOnlyConnection` (Réplica de Lectura) en el [appsettings.json](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/backend/NeoLibro.WebAPI/appsettings.json) de ambos proyectos backend.
*   **Fábrica Dinámica**: Implementación en `Program.cs` usando `IHttpContextAccessor` para inspeccionar el verbo HTTP de la petición actual:
    *   Si es `GET`, se conecta a la réplica e imprime: `📥 [DB-READ] Conectando a replica de lectura...`.
    *   Si es `POST`, `PUT`, `DELETE`, se conecta a la principal e imprime: `📤 [DB-WRITE] Ejecutando transaccion en BD principal...`.
*   **Inyección**: Todos los repositorios ADO.NET y `ApplicationDbContext` (Entity Framework Core) resuelven la cadena de conexión dinámicamente en tiempo de ejecución.

### Justificación en Sistemas Distribuidos
*   **Patrón CQRS (Read/Write Split)**: Aísla el tráfico masivo de consultas (que suele representar el 90% del uso en sistemas de biblioteca) del tráfico transaccional de escritura. Esto permite escalar las bases de datos agregando múltiples réplicas físicas de lectura.
*   **Consistencia y Mitigación de Lag**: Las escrituras se dirigen siempre al nodo maestro primario para garantizar consistencia transaccional inmediata, mientras que las lecturas aceptan consistencia eventual a cambio de máxima velocidad.

---

## 📚 6. Extracción del Microservicio de Catálogo

### Detalles de la Implementación
*   **Nuevo Microservicio**: Proyecto independiente `Catalogo.Service` ejecutándose en el puerto **`5201`**. Alberga todo el dominio de libros, ejemplares, autores y categorías.
*   **Desacoplamiento de Datos (Comunicación Síncrona HTTP)**: En el Monolito (`NeoLibro.WebAPI`), se implementaron **`HttpLibroRepository`** y **`HttpEjemplarRepository`** bajo las interfaces originales. Cuando el Monolito procesa préstamos o reservas y necesita validar un libro, realiza peticiones síncronas usando `HttpClient` hacia `Catalogo.Service`.

### Justificación en Sistemas Distribuidos
*   **Desacoplamiento Funcional y Organizacional**: Aísla el catálogo del ciclo de vida del monolito de operaciones. Si el catálogo se cae temporalmente, el módulo de usuarios y multas sigue operable de forma aislada.
*   **Escalabilidad Independiente**: El catálogo recibe la gran mayoría de visitas en una biblioteca digital. Al extraerlo a un microservicio separado, este puede escalarse con más recursos en hardware (CPU, memoria o réplicas de pods en Kubernetes) de forma aislada del monolito de transacciones.
