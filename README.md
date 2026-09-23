# Arahk.Neighbor — Auth (Login / Register / Email OTP)

Clean Architecture + Blazor Server + MudBlazor. Thai UI copy from approved UX package.

## Prerequisites
- .NET 10 SDK (`global.json` pins `10.0.401`)

## Run
```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/Arahk.Neighbor.Web
```

Then open the printed URL (typically `http://localhost:5xxx`).

- Login: `/login`
- Register: `/register`
- Email OTP: `/verify-email` (after register)
- Home (after verify/login): `/`
- My Profile: `/profile` (signed-in)

## Persistence (phase A)
- **EF Core + SQLite** behind Application repository interfaces.
- Connection string: `ConnectionStrings:Neighbor` in `appsettings.json` / `appsettings.Development.json`.
- Default: `Data Source=neighbor.db` — file is created under the Web project **content root** (same folder as `appsettings.json` when running `dotnet run --project src/Arahk.Neighbor.Web`).
- Schema: `EnsureCreated` on startup (phase A). Migrations can replace this later without changing repository contracts.
- Dev seed (Development only): verified demo user + permission masters/role defaults when rows are missing (never overwrites existing data).
- Auth session (`AuthSessionState`) stays in-memory scoped — restart clears the signed-in cookie/session UI state, not DB rows.

## Notes
- Email: `InMemoryEmailSender` — OTP code shown on verify page in development.
- Forgot password: placeholder toast only.
- Local DB files (`*.db`, `*.db-shm`, `*.db-wal`) are gitignored.

## UI chrome lock (AppBar vs nav)
- **AppBar** pad-x **24** (`px-6`) — height Mud default **64** (not Dense/56); prior **16** rejected as edge-glued
- **Nav item** pad-x **32** (`px-8`) — intentionally wider than AppBar (lock: AppBar 24 < nav 32)
- Drawer content / section label pad-x **24**; nav item gap **8**
- Desktop wordmark: AppBar only (no drawer duplicate)
- Main content clears Fixed AppBar via Mud (`--mud-appbar-height`); main side pad mobile **24** / desktop **48**
- Spacing/heights: MudBlazor 9 `N×4` utilities + `Size` enum — do not revive `--nb-space-*` / `--nb-size-*-height` as SoT
