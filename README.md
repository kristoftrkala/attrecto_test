# Attrecto Todo

## Build & run
1.  open **cmd** and create local sql server with the following command: `sqllocaldb create "Local"`
2.  run script "**init_db.sql**" found in the **Tools** folder
	this script initalizes the database with 2 roles,and 2 users
		- normal user: email: **test@test.com**, password: **1**
		- admin user: email: **admin@admin.com**, password: **admin**
3.  there is a postman collection in the Tools folder as well to manually test the endpoints
4.  api documentation via **swagger**: https://localhost:7226/swagger/index.html
5.  logs are channeled into seq
> seq can be downloaded from https://datalust.co/download, and is listining on http://localhost:5341/ by default.