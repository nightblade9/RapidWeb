# RapidWeb

[![.NET](https://github.com/nightblade9/RapidWeb/actions/workflows/dotnet.yml/badge.svg)](https://github.com/nightblade9/RapidWeb/actions/workflows/dotnet.yml)

RapidWeb is a quick-start web application, intended as a base template website (just add business logic)!

Features:
- Registration 
- Authentication (login)
- Deploys to a Docker container

Note that it is NOT production ready; at a minimum, you need to connect to a production-grade database (not a SQLite one).

The tech stack:

- Blazor and C# on the front-end
- SQLite on the back-end, with DB migrations
- Secure password hashing via BCrypt

# Developer Environment Setup

Download the following:

- .NET 8 SDK
- Visual Studio Code
- MySQL 8.0.35 - install:
    - MySQL Server 8.0.35 (set up the root password to be "password")
    - MySQL Workbench 8.0.34
    - Connector/NET 8.0.33.1
    - Launch the workbench, connect, and create a new schema called `WebApp`

Open the source project and press F5 in VS Code; it should run and show the dashboard.

Browse to `/admin/HealthCheck` and verify that the database and API connection checks shows `Successful`.

Note that, even though they're separate projects, the API and UI respond to the same port.

Note that we're currently using SQLite to simplify deployment.


# Functionality Included

- Registration and login
- Website health check
- Separate web-based API

# Architecture

## DB Access in the UI?!

History repeats, and so do web architectures. We're back to HTML pages with code-behind. This is not as bad as it sounds: for simple websites that you need to spin up fast, that need absolutely blazing fast speed, it's faster, easier, and cleaner to simply use this architecture. Deployment is also a snap: there's only the main `WebApp.Web` project that you need to deploy. It doesn't scale well, though, as you are scaling either the entire app stack, or the database.

For an example of this, check the registration and login pages in `WebApp.Web`.  For example, `Register.cshtml` contains the registration form, while `Register.cshtml.cs` contains the code-behind. Both have references to repository instances, which have access to the DB directly.

## A Separate REST API?

If you think you may need to scale up your front-ends and/or back-ends individually, or if you simply prefer a cleaner separation of concerns, you can put your business logic inside the `WebApp.Api` project. This project deploys as a separate (back-end) container, while `WenApp.web` becomes the front-end container.

For an example of this, check the `HealthCheck.cshtml.cs` page. It uses `HttpClient` to make a call to the API to fetch the API's status.

# Running the App via Docker

To run the application via Docker (TODO: update this with two-container instructions):

- From a shell, `cd` into `scripts` and run `python .\build_docker_image.py`
- When the process drops you in the docker container, type `exit`
- Run `docker compose up` to start the app and map port 8080 from the container to port 80 on the host (your PC)
- Open a new browser window and browse to `localhost:80`

Note that we're using SQLite for data:
- The database isn't high-performance, but it works, even with migrations
- The database exists as a file on disk (`prod.db` in the docker container under `./WebApp.Web`)
- The DB file seems to persist even after building a new version of the same image

To import the locally-built image into prod:

- From `scripts` run `python export_docker_image.py`
- On the prod machine, run `docker load -i latest_image.tar`
- Run the image in a container with the script `deploy.py` from `scripts`

Open a browser and browse to `http://localhost:80`. Tada!

Note that we currently use SQLite, to simplify deployment. The DB file persists even when you update the image, as long as you run it in the same container.

# License & Legal Disclaimer

This software is provided *"as is"* without any warranties. By using this software, you agree to the terms outlined in [LICENSE](./LICENSE).
