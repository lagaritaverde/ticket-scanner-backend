#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
RUN apt update && apt install --fix-missing -y  poppler-utils libtesseract-dev libleptonica-dev
RUN ln -s /usr/lib/x86_64-linux-gnu/libdl.so.2 /usr/lib/x86_64-linux-gnu/libdl.so

#poppler-utils
#libleptonica
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Home.Extractor/Home.Extractor.csproj", "Home.Extractor/"]
RUN dotnet restore "Home.Extractor/Home.Extractor.csproj"
COPY . .
WORKDIR "/src/Home.Extractor"
RUN dotnet build "Home.Extractor.csproj" -c Release -o /app/build

FROM build AS publish
#RUN apt update && apt install -y poppler-utils tesseract-ocr libtesseract-dev tesseract-ocr-eng tesseract-ocr-spa tesseract-ocr-cat libleptonica libleptonica-dev
RUN dotnet publish "Home.Extractor.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN ln -s /usr/lib/x86_64-linux-gnu/liblept.so.5 /app/x64/libleptonica-1.82.0.so
RUN ln -s /usr/lib/x86_64-linux-gnu/libtesseract.so.5 /app/x64/libtesseract50.so
#RUN ln -s /usr/lib/x86_64-linux-gnu/libdl.so.2 /usr/lib/x86_64-linux-gnu/libdl.so


ENTRYPOINT ["dotnet", "Home.Extractor.dll"]
