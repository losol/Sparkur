# Multicontainer on azure

```
version: "3.4"  
services:  
  sparkur:
    container_name: sparkur
    image: losolio/sparkur
    restart: always
    environment:
      - SparkSettings__Endpoint=https://sparkdock.azurewebsites.net/fhir
      - StoreSettings__ConnectionString=mongodb://root:CosmicTopSecret@mongodb:27017/spark?authSource=admin
      - ExamplesSettings__FilePath=/app/example_data/fhir_examples/r4_examples.zip
    ports:
      - "80:80"
      - "443:443"
    links:
      - mongodb
    depends_on:
      - mongodb
  mongodb:
    container_name: mongodb
    image: mongo
    environment:
      MONGO_INITDB_ROOT_USERNAME: root
      MONGO_INITDB_ROOT_PASSWORD: CosmicTopSecret
    # volumes:
      # - ${WEBAPP_STORAGE_HOME}/site:/data/db
    ports:
      - "27017:27017"
```