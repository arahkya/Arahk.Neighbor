# Arahk.Neighbor — Auth (Login / Register / Email OTP)

Clean Architecture + Blazor Server + MudBlazor. Thai UI copy from approved UX package.

## Prerequisites
- .NET 8 SDK

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

## Notes
- Persistence: in-memory (demo). Restart clears users.
- Email: `InMemoryEmailSender` — OTP code shown on verify page in development.
- Forgot password: placeholder toast only.
