# BlogSpot




# EntityFramework commands

To create the DB 

	Add-Migration "Inital migration"
	Update-Database

Add-migration created the file 20240504160114_Inital migration.cs which includes all of the database/table configuration
Update-Database will check the migration folder against the DB and check what is missing, then it will update it based on the migration files.