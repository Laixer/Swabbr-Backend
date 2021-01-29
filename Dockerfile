# Swabbr Ecosystem

# Build the application solution
# Fixes the CA issue https://github.com/NuGet/Home/issues/10491
FROM mcr.microsoft.com/dotnet/sdk:5.0.102-ca-patch-buster-slim AS build
WORKDIR /source

# Copy and restore app
COPY . .

# Print version
RUN find . -type f -exec sed -i "s/@@VERSION@@/$(git describe --long --always)/" {} +
RUN find . -type f -exec sed -i "s/@@COMMIT@@/$(git rev-parse HEAD)/" {} +

# Publish app and libraries
WORKDIR "/source/Swabbr.Api"
RUN pwd
RUN dotnet publish -c release -o /app
RUN git describe --long --always > /app/VERSION
RUN git rev-parse HEAD > /app/COMMIT

# Build runtime image
#
# Any Swabbr application in the repository can
# be called via the CMD=<application> environment
# variable.
FROM mcr.microsoft.com/dotnet/aspnet:5.0
ENV DOTNET_PRINT_TELEMETRY_MESSAGE=false
WORKDIR /app
COPY --from=build /app .
EXPOSE 80/tcp
ENTRYPOINT "/app/Swabbr.Api"
