# ClinicFlow SaaS - Microservices Architecture (Startup-Scale)

## 1) System Topology

- **API Gateway (YARP)**: single entry point, JWT validation, routing, correlation propagation.
- **Auth Service**: registration/login, JWT + refresh token lifecycle, role claims.
- **Patient Service**: patient CRUD, search and pagination, tenant-aware uniqueness.
- **Appointment Service**: booking with conflict detection + transaction safety.
- **Prescription Service**: issue prescriptions and generate downloadable PDF (mock).
- **Notification Service**: daily reminder background job (mock WhatsApp).
- **Frontend**: Angular SPA with route guards, role-based visibility, interceptors.

All services:
- Clean Architecture style (`Domain`, `Application`, `Infrastructure`, `Api`).
- Database-per-service (SQL Server preferred).
- `TenantId`, audit fields, soft delete in every aggregate.
- Async end-to-end.
- Structured logging (Serilog) and exception middleware.

## 2) Request Flow

1. Client logs in via `/auth/login` through gateway.
2. Auth service returns short-lived JWT + refresh token.
3. Gateway validates JWT and forwards to downstream services.
4. Services enforce tenant/role checks and persist to their own DB.
5. Notification service runs daily and logs next-day reminders.

## 3) Inter-Service Communication

- Typed `HttpClient` registrations in each service for cross-service calls.
- Polly resilience policies: retry + circuit breaker + timeout.
- Failure handling: graceful fallback and domain-safe outcomes.

## 4) ER Diagram (Text)

```text
AuthDb
  Users (Id PK, TenantId, Email, PasswordHash, IsActive, CreatedAt, UpdatedAt, IsDeleted)
  Roles (Id PK, TenantId, Name, IsDeleted)
  UserRoles (UserId FK -> Users, RoleId FK -> Roles, TenantId)
  RefreshTokens (Id PK, UserId FK -> Users, Token, ExpiresAt, RevokedAt, CreatedAt)

PatientDb
  Patients (Id PK, TenantId, FullName, Phone, Age, Gender, Address, MedicalHistoryNotes, ...audit, IsDeleted)

AppointmentDb
  Appointments (Id PK, TenantId, PatientId, DoctorUserId, StartAtUtc, EndAtUtc, Status, Notes, ...audit, IsDeleted)

PrescriptionDb
  Prescriptions (Id PK, TenantId, PatientId, AppointmentId, MedicinesJson, Dosage, Notes, CreatedDateUtc, ...audit, IsDeleted)

NotificationDb
  ReminderLogs (Id PK, TenantId, AppointmentId, Channel, Status, SentAtUtc, Message, ...audit)
```

## 5) Indexing Strategy

- Common for all major tables: `(TenantId, IsDeleted)` filtered index.
- Patients: unique index `(TenantId, Phone)` and nonclustered `(TenantId, FullName, Phone)`.
- Appointments: unique filtered index `(TenantId, DoctorUserId, StartAtUtc)` where `Status <> Cancelled` and `IsDeleted = 0`.
- Refresh tokens: `(UserId, ExpiresAt)`.
- Reminder logs: `(TenantId, SentAtUtc DESC)`.

## 6) Multi-Tenant Strategy

- Tenant from JWT claim (`tenant_id`) and enforced in all repositories.
- Global query filters for `TenantId` + `IsDeleted` where practical.
- All commands write `CreatedBy`, `UpdatedAt`, etc.

## 7) API Gateway Routes (YARP)

See `src/gateway/ApiGateway/appsettings.json`.

## 8) Angular Architecture

- `core`: auth service, jwt interceptor, route guard, app initializer.
- `shared`: reusable UI components/models.
- `features`: auth, dashboard, patients, appointments, prescriptions.

## 9) Scalability Notes

- Stateless APIs + horizontal scaling.
- Idempotency keys for booking endpoint (future).
- Event bus integration (future): outbox pattern ready.
- Move background jobs to dedicated worker instances as traffic grows.
