# Template Project for Creating a Web API with .NET

## Features

- JWT Authentication
- User control with roles
- Authentication with Keycloak (currently under development, local users only now)

## How to Develop

Using VSCode and DevContainer, simply open the project in VSCode and reopen in DevContainer.

### Start Server

After running the command below, access swagger at localhost:5000

```
cd src && dotnet run
```

### known issues

You need to reload the window after launch to activate the CSharpier Formatter.  
For more details, visit https://github.com/microsoft/vscode/issues/189839  
I hope this issue will be fixed in a future update.

## How to Deploy

Use Docker Compose:

```bash
docker compose up -d
```

## References

### DevContainers

https://containers.dev/implementors/json_reference/

### EntityFramework

https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations
https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many

### JWT

https://qiita.com/zaburo/items/7595d886116848521e6c

### LDAP Authentication

https://qiita.com/v1tam1n/items/0c377c477d463db77a34
https://medium.com/@ahmed.gaduo_93938/how-to-implement-keycloak-authentication-in-a-net-core-application-ce8603698f24
