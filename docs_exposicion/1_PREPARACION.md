# Guía de Preparación del Entorno (PREPARACION.md)

Este documento detalla los prerrequisitos, configuración de base de datos e instrucciones paso a paso para levantar y ejecutar de manera limpia el sistema distribuido de biblioteca universitaria desde cero.

---

## 1. Requisitos Previos de Software

Antes de iniciar la ejecución, asegúrate de tener instaladas las siguientes herramientas en tu entorno local:

*   **SDK de .NET 9.0**: Requerido para compilar y ejecutar los proyectos de C# (API Gateway, Monolito, Catálogo y Notificaciones).
    *   *Descarga*: [dotnet.microsoft.com/download/dotnet/9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
*   **Node.js (v18.0 o superior)** y **npm**: Requeridos para inicializar, compilar y ejecutar la interfaz web interactiva en React + Vite.
    *   *Descarga*: [nodejs.org](https://nodejs.org/)
*   **Docker Desktop**: Necesario para aprovisionar las colas de mensajería (RabbitMQ), el caché en memoria distribuido (Redis) y el servidor de archivos estáticos que simula la CDN (Nginx).
    *   *Descarga*: [docker.com/products/docker-desktop](https://www.docker.com/products/docker-desktop/)
*   **Microsoft SQL Server**: Motor de base de datos relacional local (instancia por defecto `MSSQLSERVER` o `SQLEXPRESS` escuchando en `localhost`).

---

## 2. Configuración de la Base de Datos Inicial

El esquema y los datos deben cargarse de la siguiente manera:

1.  **Levantar el servicio de SQL Server** en tu equipo local.
2.  **Ejecutar el script SQL de creación de estructura**:
    *   Abre el archivo [BibliotecaFISI_Simplificado.sql](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/database/BibliotecaFISI_Simplificado.sql) (ubicado en `database/`) usando **SQL Server Management Studio (SSMS)** o tu cliente de preferencia y ejecútalo completamente. Esto creará la base de datos `BibliotecaFISI` con sus tablas y procedimientos almacenados clave.
    *   Alternativamente, puedes ejecutarlo desde la consola usando `sqlcmd`:
        ```bash
        sqlcmd -S localhost -E -i database/BibliotecaFISI_Simplificado.sql
        ```
3.  **Cargar el Catálogo de Libros y Ejemplares**:
    *   Navega a la carpeta `database/` y ejecuta el script de importación de datos en Python para sembrar los 1,326 libros y 3,373 ejemplares de prueba extraídos del CSV real de la facultad:
        ```bash
        cd database
        python cargar_datos_completos.py
        ```
4.  **Crear el Administrador de Pruebas**:
    *   Ejecuta el script para sembrar la cuenta administrativa por defecto en la base de datos:
        ```bash
        python crear_administrador.py
        ```

---

## 3. Despliegue de Infraestructura Compartida (Docker)

El orquestador de contenedores configurará RabbitMQ, Redis y la CDN de Nginx en tu puerto local.

1.  Abre una terminal en la carpeta `backend/` (donde reside el archivo [docker-compose.yml](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/backend/docker-compose.yml)).
2.  Ejecuta el comando de aprovisionamiento en segundo plano (`detached`):
    ```bash
    docker compose up -d
    ```
3.  Comprueba que los siguientes contenedores están activos en tu Docker Desktop:
    *   `rabbitmq_biblioteca`: Expone mensajería en `5672` y la consola web de administración en `15672`.
    *   `redis_biblioteca`: Expone el servicio de caché distribuido en el puerto `6379`.
    *   `cdn_biblioteca`: Servidor Nginx que simula una red de distribución de contenidos en el puerto `8080` apuntando al volumen local `backend/cdn-resources/`.

---

## 4. Ejecución del Backend (.NET)

Para simular la arquitectura distribuida en local, debes abrir **cuatro terminales independientes** en tu sistema operativo y ejecutar cada componente en su puerto asignado:

*   **Terminal 1: API Gateway (YARP)** (Puerto de entrada principal: `5200`):
    ```bash
    cd backend/ApiGateway
    dotnet run
    ```
*   **Terminal 2: Microservicio de Catálogo (Catalogo.Service)** (Puerto interno: `5201`):
    ```bash
    cd backend/Catalogo.Service
    dotnet run
    ```
*   **Terminal 3: Monolito de Operaciones (NeoLibro.WebAPI)** (Puerto interno: `5180`):
    ```bash
    cd backend/NeoLibro.WebAPI
    dotnet run
    ```
*   **Terminal 4: Servicio de Notificaciones (Notificaciones.Service)** (Servicio en segundo plano sin puerto HTTP público):
    ```bash
    cd backend/Notificaciones.Service
    dotnet run
    ```

---

## 5. Ejecución del Frontend (React + Vite)

Para levantar el portal del estudiante y del administrador:

1.  Abre una terminal en la carpeta [frontend/frontend](file:///c:/Users/USUARIO/OneDrive/Desktop/taller%20distribuidos/biblioteca-facultad/frontend/frontend/).
2.  Si es la primera vez que levantas el proyecto, restaura los paquetes de Node:
    ```bash
    npm install
    ```
3.  Inicia el servidor de desarrollo de Vite:
    ```bash
    npm run dev
    ```
4.  Abre el navegador web e ingresa a `http://localhost:5173` (o la dirección provista por la consola). El portal realizará todas sus consultas de red de manera invisible al puerto `5200` del API Gateway.
