# Setting up secrets for local development

If using MongoDb Cloud, and running the project from soure 

```
dotnet user-secrets set "StoreSettings__ConnectionString" "mongodb+srv://<username:<password>@<server>/spark?retryWrites=true&w=majority"
```