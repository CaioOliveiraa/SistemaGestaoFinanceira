# Stage 1: Angular build
FROM node:20-alpine AS ui
WORKDIR /ui
# ajuste o caminho do package.json do seu frontend
COPY apps/frontend/package*.json ./
RUN npm ci
COPY apps/frontend/ .
RUN npm run build -- --configuration=production
# Saída esperada: /ui/dist/frontend/browser

# Stage 2: .NET build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY apps/backend/FinanceSystem.API/*.csproj apps/backend/FinanceSystem.API/
RUN dotnet restore apps/backend/FinanceSystem.API/FinanceSystem.API.csproj
COPY apps/backend/FinanceSystem.API/ apps/backend/FinanceSystem.API/
RUN dotnet publish apps/backend/FinanceSystem.API/FinanceSystem.API.csproj -c Release -o /out

# Stage 3: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out ./
# copia o Angular para wwwroot
COPY --from=ui /ui/dist/frontend/browser ./wwwroot

# Porta única do app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Segurança: não copie .env para a imagem
ENTRYPOINT ["dotnet", "FinanceSystem.API.dll"]
