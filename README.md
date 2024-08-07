# ASP.NET Core Template Project

Tiny web application with registration and authentication. 

- Blazor and C# on the front-end
- SQLite or MySQL on the back-end
- Secure password hashing via BCrypt

# Developer Environment Setup

Download the following:

- .NET 8 SDK
- Visual Studio Code
- MySQL 8.0.35 - install:
    - MySQL Server 8.0.35 (set up the root password to be "password")
    - MySQL Workbench 8.0.34
    - Connector/NET 8.0.33.1
    - Launch the workbench, connect, and create a new schema called `Stocks`

Open the source project and press F5 in VS Code; it should run and show the dashboard.

Browse to `/admin/HealthCheck` and verify that the database connection check shows `Successful`.

Note that we're currently using SQLite to simplify deployment.


# Functionality Included

- Registration and login
- Website health check

# Architecture

History repeats, and so do web architectures. We're back to HTML pages with code-behind. For example, `Register.cshtml` contains the registration form, while `Register.cshtml.cs` contains the code-behind.

# Running the App via Docker

To run the application via Docker:

- From a shell, `cd` into `scripts` and run `python .\build_docker_image.py`
- When the process drops you in the docker container, type `exit`
- Run `docker compose up` to start the app and map port 8080 from the container to port 80 on the host (your PC)
- Open a new browser window and browse to `localhost:80`

Note that we're using SQLite for data:
- The database isn't high-performance, but it works, even with migrations
- The database exists as a file on disk (`prod.db` in the docker container under `./Stocks.Web`)
- The DB file seems to persist even after building a new version of the same image

To import the locally-built image into prod:

- From `scripts` run `python export_docker_image.py`
- On the prod machine, run `docker load -i latest_image.tar`
- Run the image in a container with the script `deploy.py` from `scripts`

Open a browser and browse to `http://localhost:80`. Tada!

Note that we currently use SQLite, to simplify deployment. The DB file persists even when you update the image, as long as you run it in the same container.

