FROM mcr.microsoft.com/dotnet/sdk:7.0
RUN useradd -m -u ${UID:-1000} developer
USER developer
RUN dotnet tool install --global csharpier \
  && dotnet tool install --global dotnet-ef --version 7.0.14
ENV PATH $PATH:/home/developer/.dotnet/tools
RUN apt-get update && apt-get install -y --no-install-recommends \
ssh-client \
&& apt-get clean \
&& rm -rf /var/lib/apt/lists/*
