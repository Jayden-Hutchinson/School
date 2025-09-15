# I Need Help pls!

## Database Migrations

If you're encountering errors related to migrations or schema mismatches, you're likely dealing with pending or outdated Entity Framework Core migrations.

This usually happens when: 
- You’ve made changes to the model but haven’t applied migrations.
- The SQLite database file still has old tables from a previous run.


### 🔄 How to Reset the Database

> [!WARNING]  
>  This process will delete all data in the database.

1. Delete the SQLite databases file manually
   The files look like this:

   > _database_name_.db

2. Delete all pending migrations (optional)

   If the migrations folder is messy or broken:
   
   `dotnet ef migrations remove`
   (Run multiple times until no migrations remain.)

4. Recreate and apply migrations
   
   `dotnet ef migrations add InitialCreate`
   `dotnet ef database update`

5. Verify it's working

   Run the app and confirm that tables are generated correctly in the new .db file.
   

### 📚 Useful EF Core Commands

   Add a new migration
   
   `dotnet ef migrations add MigrationName`
   
   Apply migrations to database
   
   `dotnet ef database update`
   
   Remove last migration
   
   `dotnet ef migrations remove`
   
   List all migrations
   
   `dotnet ef migrations list`

> [!TIP]
> 💡 Use these commands based on official EF Core Documentation for more help.
