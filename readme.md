# Proyecto: Sistema de Gestión de Bibliotecas Virtuales (SGBV)

Este es un proyecto universitario que implementa un "Sistema de Gestión de Bibliotecas Virtuales" (SGBV). El objetivo es
crear una API RESTful para gestionar usuarios, recursos (libros, audiolibros) y préstamos virtuales.

---

## 🏛️ Arquitectura y Stack Tecnológico

La solución está construida sobre una arquitectura de **API monolítica** limpia, utilizando **C#** y el framework **.NET
8** (Web API).

El stack principal utilizado es el siguiente:

* **Lenguaje/Framework:** C# con .NET 8 (Web API)
* **Contenerización:** Docker y Docker Compose
* **Base de Datos:** PostgreSQL
* **Gestión de BD:** PgAdmin

---

## 🚀 Cómo Ejecutar el Proyecto (con Docker)

Todo el entorno de desarrollo (la API, la base de datos y PgAdmin) está orquestado con Docker Compose, facilitando su
ejecución con un solo comando.

### Prerrequisitos

* Tener [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado y en ejecución.

### Pasos para Ejecutar

1. Clona este repositorio:
   ```bash
   git clone [URL-DEL-REPOSITORIO]
   cd [NOMBRE-DEL-PROYECTO]
   ```

2. Ejecuta Docker Compose desde la raíz del proyecto:
   (Asegúrate de estar en la carpeta donde se encuentra el archivo `docker-compose.yml`)

   ```bash
   docker-compose up -d --build
   ```

3. ¡Listo! Docker construirá las imágenes y levantará los tres contenedores (API, DB y PgAdmin).

---

## 🌐 Servicios Disponibles

Una vez levantados los contenedores, los servicios estarán disponibles en las siguientes URLs:

### 1. API (SGBV)

* **URL Base:** `http://localhost:8080`
* **Swagger (Documentación):** `http://localhost:8080/swagger`

*(Nota: El puerto `8080` es un ejemplo. Puedes verificar el puerto mapeado en tu archivo `docker-compose.yml`)*

### 2. PgAdmin (Gestor de Base de Datos)

* **URL:** `http://localhost:5050`
* **Usuario:** `admin@admin.com` (O el que definas en tu `docker-compose.yml`)
* **Contraseña:** `admin` (O la que definas en tu `docker-compose.yml`)

#### Conexión a la BD desde PgAdmin

Al abrir PgAdmin, deberás registrar un nuevo servidor para conectarte a la base de datos de PostgreSQL. Usa las
siguientes credenciales (basadas en los nombres de servicio de Docker):

* **Host:** `postgres_db` (Este es el *nombre del servicio* de Docker, no `localhost`)
* **Port:** `5432`
* **Nombre de la BD:** `sgbv_db` (O el que definas en tu `docker-compose.yml`)
* **Usuario:** `postgres`
* **Contraseña:** `password_secret` (O la que definas en tu `docker-compose.yml`)

---

## 📖 Endpoints de la API

La documentación completa de la API, con todos los endpoints disponibles y sus modelos de datos, se genera
automáticamente con Swagger.

Visita la documentación interactiva en:
**[http://localhost:5173](http://localhost:5173/swagger)**