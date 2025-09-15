# Welcome to CytoNET's Documentation

This document is intended to give future developers a solid understanding of our tech stack choices and the rationale behind them. Our goal was to modernize and extend an existing C#-based system while maintaining compatibility with older components where necessary.

Refer to the sidebar at the left or the top nav bar to find what you need. Or use the search bar. Up to you.

## Tech Stack

- Frontend
  - Component Library: Tailwindcss
- Backend:
  - ASP.NET Core
- Database:
  - SQLite
- ORM:
  - Entity Framework Core
- Frontend Library:
  - D3.js (for dynamic map generation)
- CI/CD:
  - Azure Pipelines(via GitHub Actions)

## Commands

- `mkdocs new [dir-name]` - Create a new project.
- `mkdocs serve` - Start the live-reloading docs server.
- `mkdocs build` - Build the documentation site.
- `mkdocs -h` - Print help message and exit.

## Project layout

    mkdocs.yml    # The configuration file.
    docs/
        index.md  # The documentation homepage.
        database/
            schema.md
        csv-import/
            setup.md
            usage.md
            troubleshooting.md
        help.md    # For general issues
