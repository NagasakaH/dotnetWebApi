FROM mcr.microsoft.com/dotnet/sdk:8.0
RUN useradd -m -u ${UID:-1000} developer
USER developer
RUN dotnet tool install --global csharpier \
  && dotnet tool install --global dotnet-ef --version 8.0.7
ENV PATH $PATH:/home/developer/.dotnet/tools
