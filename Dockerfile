FROM mcr.microsoft.com/dotnet/sdk:7.0

WORKDIR /app
COPY Unifico .
RUN dotnet restore
ENTRYPOINT [ "dotnet", "test" ]