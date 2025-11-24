# HotelManagementSystem

- Make sure you have SQL Server installed and running. You can use SQL Server Express or any other edition. 
- Geting the latest code from the GIT repository https://github.com/aghaqasim83/HotelManagementSystem.
- Make sure all the project dependencies are restored. You can do this by running the following command in the terminal at the solution root:
  ```
  dotnet restore
  ```
- set the connection string in the `appsettings.json` file located in the "HotelManagementSystem" project. Update the `DefaultConnection` string to point to your SQL Server instance and database. Example:
  ```json
  "ConnectionStrings": {
	"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=HotelManagementSystem;Trusted_Connection=True;TrustServerCertificate=True;"
  }
  ```
- Set Web project as the startup project. And build the solution:
  ```
  dotnet build
  ```
- Now Run the web project "HotelManagementSystem". This will start the application and host it locally.
- Start the app and open: `https://localhost:{port}/swagger/index.html` (or `/swagger`).
- The database "HotelManagementSystem" will be created with 3 tables in it 
   	- [dbo].[Booking]
	- [dbo].[Hotel]
	- [dbo].[Room]
- Now call this endpoint to seed demo data into the database:
  ```
  POST https://localhost:{port}/api/DatabaseManager/Seed?seedDemo=true
  ```
- Now you can call the API endpoints by passing the correct Id's in the request.
- Now sample postman requests are added into the colleciton "Hotel Management API.postman_collection.json"