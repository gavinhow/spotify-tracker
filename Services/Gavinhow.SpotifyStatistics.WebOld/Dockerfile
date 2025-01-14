FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
COPY ./ /publish
WORKDIR /publish
EXPOSE 80/tcp
ENTRYPOINT ["dotnet", "Gavinhow.SpotifyStatistics.Web.dll"]
