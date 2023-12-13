#!/usr/bin/env python3
import subprocess

DOCKER_IMAGE_NAME = "stocks-web-image"

subprocess.call(f"docker save -o latest_image.tar {DOCKER_IMAGE_NAME}")