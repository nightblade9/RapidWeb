#!/usr/bin/env python3
import datetime
import subprocess

DOCKER_IMAGE_NAME = "webapp-web-image"
date = datetime.datetime.now().strftime('%Y%m%d-%H%M') 
subprocess.call(f"docker save -o ../latest_image-{date}.tar {DOCKER_IMAGE_NAME}")