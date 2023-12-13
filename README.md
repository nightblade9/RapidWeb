# Stocks Spectator

Tiny web application that manages and tracks your stocks over time, across multiple accounts.

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

Browse to `/HealthCheck` and verify that the database connection check shows `Successful`.

# Running the App via Docker

To run the application via Docker:

- From a shell, `cd` into `scripts` and run `python .\build_docker_image.py`
- When the process drops you in the docker container, type `exit`
- Run `docker compose up` to start the app and map port 8080 from the container to the host
- Open a new browser window and browse to `localhost:8080`

Note that we're using SQLite for data:
- The database isn't high-performance, but it works, even with migrations
- The database exists as a file on disk (`prod.db` in the docker container under `./Stocks.Web`)
- The DB file seems to persist even after building a new version of the same image
