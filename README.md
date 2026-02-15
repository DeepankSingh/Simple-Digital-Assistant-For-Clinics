# ClinicFlow - Startup SaaS Clinic Management (Microservices + Angular)

This repository contains an enterprise-grade blueprint and implementation skeleton for a multi-tenant clinic SaaS platform.

## Included

- Microservices-oriented backend design (`Auth`, `Patient`, `Appointment`, `Prescription`, `Notification`).
- API Gateway with YARP and JWT validation.
- Clean Architecture foldering in each service.
- DTO/repository/service/controller examples.
- Background job sample for reminders.
- Mock PDF generation sample.
- Angular app structure with auth guard + JWT interceptor + patient/appointment samples.
- SQL schema scripts with PK/FK/indexes and tenant support.

## Quick Navigation

- Architecture: `docs/SAAS_ARCHITECTURE.md`
- Folder map: `docs/FOLDER_STRUCTURE.txt`
- Gateway: `src/gateway/ApiGateway`
- Services: `src/services/*`
- Frontend: `frontend/angular-clinic`
- SQL scripts: `sql/clinicflow.sql`

## Core Decisions

- **Tenant-isolated data model** via `TenantId` in all tables.
- **Soft delete** and audit fields in aggregate tables.
- **Role model**: `Admin`, `Doctor`.
- **Resilience**: typed `HttpClient` with Polly retry and circuit breaker.
- **Scheduling logic**: transaction-safe appointment booking + conflict prevention.

## Next Implementation Steps

1. Convert each module into a standalone `.csproj` and wire EF Core migrations.
2. Add integration tests for booking concurrency and auth token lifecycle.
3. Containerize with Docker Compose/Kubernetes manifests.
4. Add OpenTelemetry tracing and centralized log sink (Seq/ELK).
