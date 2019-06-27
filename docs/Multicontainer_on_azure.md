# Multicontainer on azure

```
version: "3.1"  
services:  
  sparkur:
    container_name: sparkur
    image: losolio/sparkur
    restart: always
    environment:
      - MongStoreSettings__Url=mongodb://root:CosmicTopSecret@mongodb:27017/spark?authSource=admin
    ports:
      - "80:80"
      - "443:443"
    depends_on:
      - mongodb
  mongodb:
    container_name: mongodb
    image: mongo
    environment:
      MONGO_INITDB_ROOT_USERNAME: root
      MONGO_INITDB_ROOT_PASSWORD: CosmicTopSecret
    ports:
      - "27017:27017"
```