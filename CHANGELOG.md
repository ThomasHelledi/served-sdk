# Changelog

All notable changes to Served.SDK will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [CalVer](https://calver.org/) (YYYY.M.PATCH).

## [1.4.0] - 2026-01-29

### Added

- **UnifiedHQ Ecosystem Integration**
  - All default endpoints now point to `apis.unifiedhq.ai`
  - Full Forge observability platform integration
  - Analytics dashboard support for viewing traces

- **Fork & Pipeline Support**
  - GitHub Actions workflow for forked repositories
  - CI/CD templates for custom pipelines
  - Build and test automation

- **Documentation Overhaul**
  - Updated all links to UnifiedHQ platform
  - Added Forge analytics documentation
  - Fork and extend guide

### Changed

- Default API URL is now `https://apis.unifiedhq.ai`
- Repository moved to `github.com/unifiedhq/served-sdk`
- Package tags updated to include UnifiedHQ, Forge, Analytics

---

## [2026.1.2] - 2026-01-17

### Added

- **Tracing & Observability**: Full tracing support with OpenTelemetry integration
  - `IServedTracer` interface for custom tracing operations
  - `ServedTracer` implementation with async buffering and export
  - `TracingHttpHandler` for automatic HTTP request instrumentation
  - `ServedClientBuilder` fluent pattern for advanced configuration
  - Auto-detection of configuration from environment variables
  - Forge platform integration for Served-native observability
  - OTLP export support for industry-standard collectors

- **Error Categorization**: Automatic categorization of API errors
  - `ErrorCategory` enum with Authentication, Authorization, Validation, etc.
  - `ErrorCategoryHelper` for HTTP status code and exception categorization
  - Severity-based logging (Debug, Info, Warning, Error, Critical)

- **Metrics Collection**: Built-in metrics for monitoring
  - Request duration histogram
  - Request count counter
  - Error rate tracking
  - Slow request detection

- **Builder Pattern**: New `ServedClientBuilder` for fluent configuration
  - `WithTracing()` for enabling observability
  - `WithTimeout()` for request timeouts
  - `WithDefaultHeader()` for custom headers

### Changed

- `ServedClient` now implements `IDisposable` for proper resource cleanup
- Updated package description and tags to include tracing/observability
- Switched to CalVer versioning (YYYY.M.PATCH)

### Dependencies

- Added `OpenTelemetry` 1.8.0
- Added `OpenTelemetry.Exporter.OpenTelemetryProtocol` 1.8.0
- Added `System.Diagnostics.DiagnosticSource` 10.0.0

---

## [1.1.0] - 2025-01-13

### Added

#### New Module-Based API Architecture
- **Core Infrastructure**
  - `IHttpClient` interface for HTTP operations
  - `IApiClient<TEntity, TDetail, TCreate, TUpdate, TQuery>` generic interface for CRUD operations
  - `ApiClientBase` abstract base class with pagination and query building
  - `BulkApiClientBase` extended base class with bulk operation support
  - `ApiModuleBase` base class for organizing related resources

- **API Modules**
  - `ProjectManagement` - Projects and Tasks resources
  - `DevOpsModule` - Repositories, PullRequests, and Pipelines
  - `FinanceModule` - Invoices with cache pattern support
  - `SalesModule` - Sales Pipelines and Deals
  - `Registration` - Time Registrations
  - `Companies` - Customers
  - `Identity` - Employees and API Keys
  - `Calendar` - Agreements/Appointments
  - `BoardModule` - Boards and Sheets
  - `Reporting` - Dashboards and Datasources
  - `TenantModule` - Tenants and Workspaces
  - `BootstrapModule` - Application initialization

- **Bulk Operations**
  - `BulkCreateProjectsRequest`, `BulkUpdateProjectsRequest`, `BulkDeleteProjectsRequest`
  - `BulkCreateTasksRequest`, `BulkUpdateTasksRequest`, `BulkDeleteTasksRequest`
  - `BulkCreateTimeRegistrationsRequest`, `BulkUpdateTimeRegistrationsRequest`, `BulkDeleteTimeRegistrationsRequest`
  - `BulkCreateAgreementsRequest`, `BulkUpdateAgreementsRequest`, `BulkDeleteAgreementsRequest`
  - `BulkCreateCustomersRequest`, `BulkUpdateCustomersRequest`, `BulkDeleteCustomersRequest`
  - `BulkUpdateTaskStatusRequest` for batch status updates

- **Model Enhancements**
  - Added `TaskStatusId`, `TaskTypeId`, `IsOpen`, `AssignedToId`, `AssignedToName` to task models
  - Added `ProjectName`, `TaskName`, `EmployeeName`, `Description` to time registration models
  - Added `CustomerName`, `ProjectId`, `ProjectName` to agreement models
  - Added `EmployeeSummary`, `EmployeeDetail`, `EmployeeQueryParams` for employee management
  - Added `WorkspaceSummary`, `WorkspaceDetail` for workspace management
  - Added entity and datasource models for reporting

### Changed

- All `QueryParams` classes now extend `Common.QueryParams` for consistent pagination
- `QueryParams.Skip` is now nullable (`int?`) for better null handling
- Improved DateTime handling in query parameters (supports both string and DateTime?)

### Backwards Compatibility

- All legacy client access patterns (`client.Projects`, `client.Tasks`, etc.) remain fully supported
- Existing code will continue to work without modifications
- Module-based access is recommended for new development

## [1.0.0] - 2024-12-01

### Added

- Initial release of Served.SDK
- Project management client
- Task management client
- Time registration client
- Customer management client
- Agreement/appointment client
- Finance/invoices client
- Employee management client
- DevOps integration client
- Board/sheet client
- Sales CRM client
- Dashboard client
- Datasource client
- Tenant management client
- Bootstrap client
- Error handling with `ServedApiException`
- Async/await support throughout
