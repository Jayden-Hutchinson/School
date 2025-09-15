# Getting Started

## Prerequisites

> Clone the repo from `link`

From the root directery:

> run `dotnet restore` to install all required dependencies

> run `dotnet run` to start the server locally.

```
dotnet restore
dotnet run
```

## 🧩 Why this tech stack?

We built this project using the following stack to balance legacy compatibility with modern development standards:

### ASP.NET Core MVC + Entity Framework Core

- We were expected to work with an existing codebase written in C#.
- In the end, we chose to create our own website extension for Kinector rather than build directly on top of the legacy code.
- Still, we kept the same tech ecosystem for compatibility and ease of future integration.

### SQLite

- Lightweight and perfect for development/testing environments.
- Easy to maintain and fully supported by EF Core.

### Tailwind CSS

- Utility-first CSS framework for rapidly building clean, responsive UIs.
- Integrated well with our Razor views and partials.

### D3.js

- Used for map generation and interactive visualizations.
- This was actually carried over from the previous team’s implementation, so we kept it to avoid rebuilding existing functionality from scratch.

### Azure + GitHub Actions (CI/CD)

- We used GitHub Actions to automate testing and deployment.
- Deployed to Azure App Service for seamless .NET hosting.

```

```
