# Guía de Exposición y Sustentación (GUIA_EXPOSICION.md)

Este documento provee el flujo narrativo y los pasos prácticos recomendados para demostrar con éxito la arquitectura distribuida del sistema de biblioteca universitaria ante el jurado calificador.

---

## 🖥️ Paso 1: Preparación del Escenario (Antes de la llamada del jurado)

1.  **Docker Desktop abierto**: Muestra la lista de contenedores saludables en ejecución:
    *   `rabbitmq_biblioteca` (puertos `5672` / `15672`)
    *   `redis_biblioteca` (puerto `6379`)
    *   `cdn_biblioteca` (puerto `8080`)
2.  **Organización de Terminales (Recomendado tenerlas visibles en paralelo)**:
    *   **Consola 1 (API Gateway)**: Corriendo en el puerto `5200`.
    *   **Consola 2 (Monolito NeoLibro.WebAPI)**: Corriendo en el puerto `5180`.
    *   **Consola 3 (Catálogo Catalogo.Service)**: Corriendo en el puerto `5201`.
    *   **Consola 4 (Servicio de Notificaciones)**: Procesador de colas en segundo plano.
3.  **Navegador Web listo**:
    *   Pestaña A: Frontend interactivo en `http://localhost:5173`.
    *   Pestaña B: Panel de administración de RabbitMQ en `http://localhost:15672` (usuario/clave: `guest`/`guest`).

---

## 🔑 Paso 2: Autenticación y API Gateway (YARP)

1.  En la web, inicia sesión con la cuenta de administrador:
    *   **Usuario**: `admin@unmsm.edu.pe`
    *   **Contraseña**: `Admin123!`
2.  *Guion sugerido*:
    > *"Estimados miembros del jurado, estamos ingresando a la aplicación a través de nuestro portal web. Toda la comunicación de red del frontend no se dirige a un servidor monolítico clásico, sino que viaja de manera invisible al puerto 5200 de nuestro API Gateway desarrollado con YARP. El Gateway se encarga de recibir de forma única todas las peticiones, verificar las cookies de sesión y redirigir dinámicamente el tráfico hacia el microservicio adecuado basándose en la ruta URL."*

---

## 🌐 Paso 3: Demostración de la CDN Simulada (Nginx)

1.  Navega a la sección **Catálogo** en la web.
2.  Busca un libro y haz clic en el botón de **Ver Detalles** (ícono del ojo azul).
3.  Muestra la interfaz:
    *   La **portada del libro** se renderiza perfectamente en el lado izquierdo.
    *   El botón azul **"Leer PDF (CDN)"** descarga el archivo adjunto.
4.  *Guion sugerido*:
    > *"Al entrar a ver el libro, podemos visualizar la portada y descargar el PDF completo. Estos recursos pesados no consumen ancho de banda del backend ni se guardan en la base de datos transaccional; son despachados directamente por una CDN local simulada con Nginx en el puerto 8080. Esto optimiza drásticamente la latencia y descarga al servidor de su procesamiento binario."*

---

## ⚡ Paso 4: Demostración de Caché y Lectura (Redis + BD Réplica)

1.  Mantén a la vista la consola de **Catalogo.Service** (puerto `5201`).
2.  Navega nuevamente a la sección **Catálogo** (o presiona F5).
3.  **Primera Carga (Cache Miss)**:
    *   En la consola del Catálogo se imprimirá:
        ```text
        📥 [DB-READ - CATALOGO] Conectando a replica...
        🔍 [SQL] Cache Miss - Consultando base de datos
        ```
    *   *Guion sugerido*:
        > *"Dado que es la primera vez que ingresamos, los libros no están pre-cargados. El resolvedor dinámico de base de datos desvía la petición a la cadena de conexión de réplica 'ReadOnlyConnection', y dado que no se encuentra en caché (Cache Miss), consulta a SQL Server y lo almacena inmediatamente en Redis."*
4.  **Segunda Carga (Cache Hit)**:
    *   Recarga el catálogo. De inmediato verás en la consola del Catálogo el log:
        ```text
        🚀 [REDIS] Cache Hit - Libros obtenidos de memoria
        ```
    *   *Guion sugerido*:
        > *"Al realizar la consulta por segunda vez, los datos ya no viajan a SQL Server; se devuelven en milisegundos directamente desde nuestro caché distribuido Redis en Docker. Esto reduce la carga del motor SQL casi a cero para lecturas repetidas."*

---

## 📤 Paso 5: Demostración de Escritura y Separación (Monolito)

1.  Ten a la vista la consola de **NeoLibro.WebAPI** (puerto `5180`).
2.  En el frontend o Swagger, realiza una acción de modificación de estado (por ejemplo, registrar un nuevo Préstamo o Aprobar una Reserva).
3.  Observa inmediatamente la consola del Monolito:
    *   Se imprimirá de forma explícita el log en recuadro:
        ```text
        📤 [DB-WRITE] Ejecutando transaccion en BD principal...
        ```
4.  *Guion sugerido*:
    > *"Cuando el usuario realiza una acción que muta el estado (como crear un préstamo), nuestro interceptor de solicitudes detecta que el método HTTP es de tipo escritura y enruta dinámicamente la transacción a la base de datos principal 'WriteConnection'. Esto demuestra la separación de responsabilidades físicas de Lectura y Escritura (CQRS/BD Split) a nivel de código de forma transparente."*

---

## 📬 Paso 6: Bus de Eventos Asíncronos (RabbitMQ + MassTransit)

1.  Alinea en la pantalla la consola del Monolito y la consola del microservicio de **Notificaciones.Service**.
2.  Al confirmar el préstamo en el paso anterior, muestra cómo la consola de Notificaciones reacciona en menos de un segundo imprimiendo:
    ```text
    📬 NUEVO EVENTO RECIBIDO: PrestamoCreadoEvent
       ID Prestamo: 1 | ID Usuario: 2 | Libro: FISICA III
       Simulacion: Enviando email a angel.obregon@unmsm.edu.pe...
    ```
3.  *Guion sugerido*:
    > *"Por último, observamos la integración dirigida por eventos. Al crearse el préstamo en el monolito, este no consume tiempo enviando correos de manera síncrona. Simplemente publica un evento 'PrestamoCreadoEvent' en RabbitMQ y libera al usuario final. El microservicio de Notificaciones consume de forma independiente y asíncrona este mensaje para procesarlo. Si el servicio de correos se cayera, el mensaje aguardará seguro en la cola de RabbitMQ garantizando la consistencia eventual y robustez del sistema distribuido."*
