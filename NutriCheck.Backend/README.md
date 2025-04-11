# 🥗 NutriCheck - Backend

Este es el backend de NutriCheck, una aplicación de gestión nutricional para profesionales. Desarrollado con **C# y ASP.NET Core Web API**.

---

## ✅ Funcionalidades principales

### 👤 Pacientes
- [x] Modelo `Paciente` con nombre, edad, género, altura, peso, objetivo y nutricionistaId
- [x] `POST /api/pacientes` → Crear paciente
- [x] `GET /api/pacientes` → Listar pacientes
- [x] `PUT /api/pacientes/{id}` → Editar paciente
- [x] `DELETE /api/pacientes/{id}` → Eliminar paciente

### 🧑‍⚕️ Nutricionistas
- [x] Modelo `Nutricionista` (id, nombre, email, password)
- [x] Precarga de nutricionista de prueba al iniciar el proyecto

### 🍽️ Dietas
- [x] Modelo `Dieta` que asocia un plato a un paciente por fecha y tipo (desayuno, almuerzo, etc.)
- [x] `POST /api/dietas` → Asignar un plato a un paciente
- [x] `GET /api/dietas?pacienteId=1&fecha=yyyy-MM-dd` → Ver dieta de un paciente en una fecha

### 🍲 Platos de comida
- [x] Modelo `PlatoComida` vinculado a un nutricionista
- [x] `POST /api/nutricionista/comidas/crear` → Crear plato con nombre, ingredientes, receta opcional, calorías y proteínas
- [x] `GET /api/nutricionista/comidas/listar?nutricionistaId=1` → Listar platos creados por un nutricionista

### 🧾 Comidas registradas por pacientes
- [x] Modelo `Comida` con pacienteId, tipo, nombre, calorías y fecha
- [x] `POST /api/comidas` → Registrar comida por paciente
- [x] `GET /api/comidas?fecha=yyyy-MM-dd` → Ver comidas registradas por todos los pacientes ese día

---

## 📚 Documentación con Swagger

La API está completamente documentada y puede ser explorada mediante Swagger.

🔗 Accedé desde tu navegador cuando ejecutás el proyecto:  
`http://localhost:5070/swagger`

### Incluye:
- Descripciones de cada endpoint (`summary`)
- Tipos de respuestas esperadas (`201`, `400`, `404`, etc.)
- Parámetros bien definidos (`[FromBody]`, `[FromQuery]`)
- Visualización clara de qué datos enviar y recibir

---

## 🛡️ Validaciones aplicadas

Todos los endpoints incluyen validaciones para asegurar la integridad de los datos:

- Nombres y campos obligatorios
- Calorías > 0
- Proteínas ≥ 0
- Edad del paciente > 0
- Fechas válidas

---

## ⚙️ Tecnologías utilizadas

- C# + .NET 7
- ASP.NET Core Web API
- Entity Framework Core (InMemory)
- Swagger (OpenAPI)
- Visual Studio Code / CLI
- Git + GitHub

---

## 🚀 Cómo correr el proyecto

1. Cloná el repositorio:

```bash
git clone https://github.com/pablobarcala/nutricheck-back.git
cd nutricheck-back/NutriCheck.Backend
