#!/usr/bin/env python3
import subprocess

DOCKER_IMAGE_NAME = "stocks-web-image"

def execute_command(command, failure_message):
    exit_code = subprocess.call(command)
    if exit_code != 0:
        print(f"{failure_message} (exit code: {exit_code})")
        exit(exit_code)

os.chdir("..") # change to repo root

# TODO: inject the production connection string into appsettings.Production.json
# TODO: we need a MySQL container, too

execute_command("dotnet publish -c Release", "Failed to build release-version of code")
execute_command(f"docker build -t {DOCKER_IMAGE_NAME} -f Stocks.Web/Dockerfile .", "Failed to create docker image")

# For now: run it in a temporal container that we nuke at the end. To not nuke it, remove --rm
execute_command(f"docker run -it --rm {DOCKER_IMAGE_NAME}", "Failed to run docker image")
