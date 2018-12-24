FROM microsoft/dotnet:aspnetcore-runtime
COPY ./publish /publish
WORKDIR /publish
ENTRYPOINT ["dotnet", "Gavinhow.SpotifyStatistics.Backend.dll"]
