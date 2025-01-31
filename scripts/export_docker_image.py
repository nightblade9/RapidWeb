#!/usr/bin/env python3
import datetime
import subprocess

# Define image names for both the web and API services
DOCKER_WEB_IMAGE = "webapp-web-image"
DOCKER_API_IMAGE = "webapp-api-image"

# Get the current timestamp for the exported file
date = datetime.datetime.now().strftime('%Y%m%d-%H%M')

# Define output file names
WEB_IMAGE_FILE = f"../latest_web_image-{date}.tar"
API_IMAGE_FILE = f"../latest_api_image-{date}.tar"

# Build images
subprocess.call("docker compose build")

# Export both images
subprocess.call(f"docker save -o {WEB_IMAGE_FILE} {DOCKER_WEB_IMAGE}", shell=True)
subprocess.call(f"docker save -o {API_IMAGE_FILE} {DOCKER_API_IMAGE}", shell=True)

print(f"Exported {DOCKER_WEB_IMAGE} to {WEB_IMAGE_FILE}")
print(f"Exported {DOCKER_API_IMAGE} to {API_IMAGE_FILE}")
