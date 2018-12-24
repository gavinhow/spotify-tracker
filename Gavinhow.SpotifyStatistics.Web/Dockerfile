FROM microsoft/dotnet:aspnetcore-runtime
COPY ./publish /publish
WORKDIR /publish
EXPOSE 80/tcp
ENTRYPOINT ["dotnet", "Gavinhow.SpotifyStatistics.Web.dll"]
