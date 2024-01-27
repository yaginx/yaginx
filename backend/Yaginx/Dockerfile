FROM mcr.microsoft.com/dotnet/aspnet:8.0
ARG source

ENV TZ=Asia/Shanghai LANG=zh_CN.UTF-8
ENV PNPM_HOME="/pnpm"
ENV PATH="$PNPM_HOME:$PATH"

WORKDIR /app

EXPOSE 8080
EXPOSE 8443

COPY ${source:-.} .

ENTRYPOINT ["dotnet", "Yaginx.dll"]