FROM cnhub.feinian.net/build/aspnet-quic:8.0.1
ARG source

WORKDIR /app

EXPOSE 8080
EXPOSE 8443
EXPOSE 9080
EXPOSE 9443


COPY ${source:-.} .

ENTRYPOINT ["dotnet", "Yaginx.dll"]