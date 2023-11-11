FROM mcr.microsoft.com/dotnet/aspnet:7.0 as Base

WORKDIR /app

COPY . .

EXPOSE 3080
EXPOSE 3090

ENTRYPOINT ['dotnet', 'LeaveDayAPI.HttpApi.Host.dll']