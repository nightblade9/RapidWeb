import subprocess

subprocess.call("docker run -it -p 8080:8080 -e DOTNET_URLS=http://+:8080 stocks-web-image")
