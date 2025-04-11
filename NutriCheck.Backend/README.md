# 🥗 NutriCheck - Backend

Este es el backend del sistema NutriCheck, desarrollado en C# con ASP.NET Core Web API.


## ✅ Funcionalidades implementadas

### Pacientes

- [x] Modelo `Paciente` con: nombre, edad, género, altura, peso, objetivo y nutricionistaId
- [x] `POST /api/pacientes` → Registro de nuevos pacientes
- [x] `GET /api/pacientes` → Listado general de pacientes registrados
- [x] `PUT /api/pacientes/{id}` → Edición de paciente existente
- [x] `DELETE /api/pacientes/{id}` → Eliminación de paciente
- [x] Swagger configurado para probar la API fácilmente

### Nutricionistas

- [x] Modelo `Nutricionista` (Id, Nombre, Email, Password)
- [x] Precarga automática de nutricionista de prueba en memoria
- [x] `POST /api/auth/login` → Inicio de sesión básico con email y password
- [x] Validación simple sin tokens por ahora

---

## 🛠️ Tecnologías utilizadas

- C#
- .NET 7
- ASP.NET Core Web API
- Entity Framework Core (InMemory)
- Swagger
- Git / GitHub

---

## 🚀 Cómo correr el proyecto

1. Clonar el repositorio:

```bash
git clone https://github.com/pablobarcala/nutricheck-back.git
cd nutricheck-back/NutriCheck.Backend
