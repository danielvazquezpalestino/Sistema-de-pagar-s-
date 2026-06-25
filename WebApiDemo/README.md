# Sistema de Gestión de Pagarés - ASP.NET Core 10 Web API

Backend completo para un sistema de gestión de pagarés con autenticación personalizada, auditoría automática y control de acceso basado en roles.

## Requisitos Previos

- .NET 10 SDK
- MySQL 8.0+
- Base de datos existente `pagares` con el esquema especificado

## Configuración de la Cadena de Conexión

Edita `appsettings.json` y actualiza la cadena de conexión:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=pagares;User=root;Password=tucontraseña;AllowPublicKeyRetrieval=true;"
}
```

**IMPORTANTE:** Cambia `tucontraseña` por tu contraseña real de MySQL.

## Ejecutar el Proyecto

El proyecto usa un enfoque **Database-First** - NO ejecuta migraciones que modifiquen el esquema existente.

```bash
cd WebApiDemo
dotnet run
```

La API estará disponible en:
- HTTP: `http://localhost:5161`
- HTTPS: `https://localhost:5001` (si está configurado)

**Swagger UI:** En modo desarrollo, la interfaz de Swagger estará disponible en:
- `http://localhost:5161/swagger`

## Primera Configuración - Crear Usuario Administrador

Como no hay ASP.NET Identity, necesitas insertar el primer usuario administrador directamente en MySQL:

```sql
INSERT INTO usuarios (nombre, correo, contrasena, rol, fecha_registro)
VALUES ('Administrador', 'admin@pagares.com', '$2a$11$YourHashedPasswordHere', 'administrador', NOW());
```

Para generar el hash de BCrypt, puedes usar:

```csharp
var hash = BCrypt.Net.BCrypt.HashPassword("tuPassword");
Console.WriteLine(hash);
```

## Endpoints de la API

### Autenticación

#### POST /api/auth/login
Inicia sesión y establece cookie de autenticación.

```bash
curl -X POST http://localhost:5161/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"correo":"admin@pagares.com","contrasena":"tuPassword"}' \
  -c cookies.txt
```

#### POST /api/auth/logout
Cierra sesión y elimina cookie.

```bash
curl -X POST http://localhost:5161/api/auth/logout \
  -b cookies.txt
```

#### GET /api/auth/me
Obtiene información del usuario autenticado.

```bash
curl -X GET http://localhost:5161/api/auth/me \
  -b cookies.txt
```

### Pagarés

#### GET /api/pagares
Lista todos los pagarés. Los abogados solo ven los suyos, los administradores ven todos.

```bash
curl -X GET http://localhost:5161/api/pagares \
  -b cookies.txt
```

#### GET /api/pagares/{id}
Obtiene un pagaré específico. Registra acción CONSULTA en auditoría.

```bash
curl -X GET http://localhost:5161/api/pagares/1 \
  -b cookies.txt
```

#### POST /api/pagares
Crea un nuevo pagaré. Registra acción CREACIÓN en auditoría.

```bash
curl -X POST http://localhost:5161/api/pagares \
  -H "Content-Type: application/json" \
  -b cookies.txt \
  -d '{
    "numeroExpediente": "EXP-2024-001",
    "monto": 15000.00,
    "promesaPago": "Prometo pagar la cantidad de...",
    "beneficiario": "Juan Pérez",
    "fechaVencimiento": "2024-12-31",
    "lugarPago": "Ciudad de México",
    "fechaElaboracion": "2024-01-01",
    "lugarSuscripcion": "Ciudad de México",
    "firma": "firma_digital_base64"
  }'
```

#### PUT /api/pagares/{id}
Actualiza un pagaré existente. Registra acción MODIFICACIÓN en auditoría.

```bash
curl -X PUT http://localhost:5161/api/pagares/1 \
  -H "Content-Type: application/json" \
  -b cookies.txt \
  -d '{
    "numeroExpediente": "EXP-2024-001",
    "monto": 20000.00,
    "promesaPago": "Prometo pagar la cantidad de...",
    "beneficiario": "Juan Pérez",
    "fechaVencimiento": "2024-12-31",
    "lugarPago": "Ciudad de México",
    "fechaElaboracion": "2024-01-01",
    "lugarSuscripcion": "Ciudad de México",
    "firma": "firma_digital_base64"
  }'
```

#### DELETE /api/pagares/{id}
Elimina un pagaré. Solo administradores.

```bash
curl -X DELETE http://localhost:5161/api/pagares/1 \
  -b cookies.txt
```

#### POST /api/pagares/{id}/imprimir
Registra acción IMPRESIÓN en auditoría y devuelve datos del pagaré.

```bash
curl -X POST http://localhost:5161/api/pagares/1/imprimir \
  -b cookies.txt
```

### Usuarios (Solo Administradores)

#### GET /api/usuarios
Lista todos los usuarios. Solo administradores.

```bash
curl -X GET http://localhost:5161/api/usuarios \
  -b cookies.txt
```

#### GET /api/usuarios/{id}
Obtiene un usuario específico. Solo administradores.

```bash
curl -X GET http://localhost:5161/api/usuarios/1 \
  -b cookies.txt
```

#### POST /api/usuarios
Crea un nuevo usuario. La contraseña se hashea automáticamente con BCrypt. Solo administradores.

```bash
curl -X POST http://localhost:5161/api/usuarios \
  -H "Content-Type: application/json" \
  -b cookies.txt \
  -d '{
    "nombre": "María López",
    "correo": "maria@pagares.com",
    "contrasena": "password123",
    "rol": "abogado"
  }'
```

#### PUT /api/usuarios/{id}
Actualiza un usuario. Si se proporciona contraseña, se hashea con BCrypt. Solo administradores.

```bash
curl -X PUT http://localhost:5161/api/usuarios/2 \
  -H "Content-Type: application/json" \
  -b cookies.txt \
  -d '{
    "nombre": "María López García",
    "correo": "maria@pagares.com",
    "contrasena": "nuevoPassword",
    "rol": "abogado"
  }'
```

#### DELETE /api/usuarios/{id}
Elimina un usuario. Solo administradores.

```bash
curl -X DELETE http://localhost:5161/api/usuarios/2 \
  -b cookies.txt
```

### Auditoría (Solo Administradores)

#### GET /api/auditoria
Lista todos los registros de auditoría con filtros opcionales.

```bash
curl -X GET "http://localhost:5161/api/auditoria?idPagare=1&idUsuario=2&accion=CREACION" \
  -b cookies.txt
```

#### GET /api/auditoria/{idPagare}
Obtiene el rastro de auditoría para un pagaré específico.

```bash
curl -X GET http://localhost:5161/api/auditoria/1 \
  -b cookies.txt
```

### Respaldos (Solo Administradores)

#### GET /api/respaldos
Lista todos los registros de respaldo.

```bash
curl -X GET http://localhost:5161/api/respaldos \
  -b cookies.txt
```

#### POST /api/respaldos
Crea un registro de respaldo.

```bash
curl -X POST http://localhost:5161/api/respaldos \
  -H "Content-Type: application/json" \
  -b cookies.txt \
  -d '{"descripcion": "Respaldo diario"}'
```

## Patrón Singleton

El proyecto implementa el patrón Singleton de dos formas:

### 1. Singleton Manual (SingletonLogger)
- Ubicación: `Patterns/SingletonLogger.cs`
- Implementación clásica con `Lazy<T>` para thread-safety
- Almacena hasta 100 operaciones recientes de la API en memoria
- Métodos: `Log(message)`, `GetRecentLogs()`, `ClearLogs()`
- Constructor privado, propiedad estática `Instance`

### 2. Singleton DI (AppConfigurationService)
- Ubicación: `Services/AppConfigurationService.cs`
- Registrado como Singleton en el contenedor de DI
- Contiene configuración de la aplicación (nombre, versión, tamaño de página, roles permitidos)
- Inyectado en controladores para configuración de paginación

## Flujo de Auditoría

El sistema registra automáticamente las siguientes acciones en la tabla `auditorias`:

- **CREACION**: Cuando se crea un nuevo pagaré
- **MODIFICACION**: Cuando se actualiza o elimina un pagaré
- **CONSULTA**: Cuando se consulta un pagaré específico
- **IMPRESION**: Cuando se solicita imprimir un pagaré

Cada registro de auditoría incluye:
- `id_auditoria`: ID único
- `id_pagare`: ID del pagaré relacionado
- `id_usuario`: ID del usuario que realizó la acción
- `accion`: Tipo de acción (ENUM)
- `fecha`: Timestamp de la acción

## Roles y Permisos

### Administrador
- Acceso total a todos los endpoints
- Puede crear, editar y eliminar usuarios
- Puede ver todos los pagarés (no solo los propios)
- Puede ver y filtrar auditoría completa
- Puede gestionar respaldos

### Abogado
- Solo puede ver y editar sus propios pagarés
- No puede gestionar usuarios
- No puede acceder a auditoría ni respaldos
- Puede crear, editar y consultar sus pagarés
- Puede imprimir pagarés

## Seguridad

- **Autenticación**: Cookie-based (NO JWT, NO ASP.NET Identity)
- **Hashing de contraseñas**: BCrypt.Net-Next
- **CORS**: Configurado para Angular dev server (http://localhost:4200)
- **HTTPS**: Redirección habilitada
- **Validación**: ModelState validation en todos los endpoints
- **Manejo de errores**: Middleware global con respuestas JSON

## Estructura del Proyecto

```
WebApiDemo/
├── Controllers/          # Controladores API
├── Data/                # DbContext
├── Models/              # Entidades EF Core
├── DTOs/                # Data Transfer Objects
│   ├── Auth/
│   ├── Pagares/
│   └── Usuarios/
├── Services/            # Lógica de negocio
├── Repositories/        # Acceso a datos
├── Patterns/            # Patrones de diseño (Singleton)
├── Middleware/          # Middleware personalizado
├── appsettings.json     # Configuración
└── Program.cs           # Punto de entrada
```

## Tecnologías

- .NET 10.0
- ASP.NET Core Web API
- Entity Framework Core (Pomelo MySQL)
- BCrypt.Net-Next
- Autenticación con Cookies personalizada
- CORS para Angular
- OpenAPI/Swagger
