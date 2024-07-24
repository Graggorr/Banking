# Banking

To run this service you have to own docker container. You can download it here: https://desktop.docker.com/win/main/amd64/Docker%20Desktop%20Installer.exe?utm_source=docker&utm_medium=webreferral&utm_campaign=dd-smartbutton&utm_location=module

To run it into the docker you had to open powershell and move into the solution directory.
For example: cd C:\Users\gragg\source\repos\Banking
Then, you have to write down and execute following command: docker-compose up -d
After the execution of command and downloading of all images, you can see in your docker container that service and database are running currently.

To run integration tests you should have service and database activated and run. Then you should move to your IDE (visual studio for example) and run all tests.
