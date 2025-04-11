## ✅ Funcionalidades implementadas hasta ahora

### Pacientes

- [x] Modelo `Paciente` con: nombre, edad, género, altura, peso, objetivo y nutricionistaId
- [x] `POST /api/pacientes` → Registro de nuevos pacientes
- [x] `GET /api/pacientes` → Listado general de pacientes registrados
- [x] `PUT /api/pacientes/{id}` → Edición de paciente existente
- [x] `DELETE /api/pacientes/{id}` → Eliminación de paciente por ID
- [x] Swagger configurado para probar la API fácilmente

---

## 🛠️ Tecnologías utilizadas

- C#
- .NET 7
- ASP.NET Core Web API
- Entity Framework Core (InMemory)
- Swagger (auto-documentación)
- Git / GitHub

---

## 📌 Próximas tareas por implementar

### Backend

- [ ] Endpoint `PUT /api/pacientes/{id}` para editar pacientes
- [ ] Endpoint `DELETE /api/pacientes/{id}` para eliminar pacientes
- [ ] Validaciones de entrada (campos obligatorios, rangos válidos, etc.)
- [ ] Agregar autenticación básica o JWT (login de nutricionista)
- [ ] Migrar de base InMemory a base real (SQLite o SQL Server)

### Organización

- [ ] Crear rama `develop` y empezar a trabajar en `feature/*`
- [ ] Subir cambios del mockup de Figma y conectarlo con el backend

---

## 🚀 Cómo correr el proyecto

1. Clonar el repositorio:

```bash
git clone https://github.com/pablobarcala/nutricheck-back.git
cd nutricheck-back/NutriCheck.Backend