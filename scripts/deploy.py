import subprocess

subprocess.call("docker run -it -p 80:8080 -e DOTNET_URLS=http://+:8080 webapp-web-image")
