# Sparkur
Running Spark Fhir server

[![Build status](https://dev.azure.com/losolio/Sparkur/_apis/build/status/helselosenfhir%20-%20CI)](https://dev.azure.com/losolio/Sparkur/_build/latest?definitionId=13)


## Quickstart

**Prerequisites:** `docker-compose`, 4GB RAM allocated to docker.

```bash
# Clone the repository
git clone https://github.com/losol/Sparkur.git
cd Sparkur

# Build and run the application
docker-compose build
docker-compose up
```

The application will now be live at `localhost:5555`.   
Use the following credentials to login:

```text
Username: admin@email.com
Password: Str0ngPa$$word
```

Visit `localhost:5555/admin/maintenance` to load sample data.