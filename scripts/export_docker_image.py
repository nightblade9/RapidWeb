#!/usr/bin/env python3
import datetime
import subprocess

DOCKER_IMAGE_NAME = "stocks-web-image"
date = datetime.datetime.now().strftime('%Y%m%d-%H%M') 
subprocess.call(f"docker save -o latest_image.tar ../{DOCKER_IMAGE_NAME}-{date}")