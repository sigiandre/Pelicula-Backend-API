# Pelicula-Backend-API
API RESTful con ASP.NET Core Web API


## Migration
## Agregamos en la carpeta migracion los datos
### add-migration MigracionInicial
### add-migration CreacionTablaPelicula
### add-migration creacionModelUsuario

## Actualizamos la migracion dentro de la base de datos en mi caso SQL Server
### update-database

## Cadena de Conexion
###   "ConnectionStrings": {
        "DefaultConnection": "Server=localhost,1433; Database=PeliculasApi; User=###; Password=#######;"
       },
       
       
### Gracias esto es todo por Api RestFul con AspNetCore Web API
