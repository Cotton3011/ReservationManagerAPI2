
# .NET 8 SDKを使ってアプリをビルドするためのステージ
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

#コンテナ内の作業ディレクトリを指定する
WORKDIR /src

#csprojだけを先にコピーして、Nuget復元を行う
COPY ["src/ReservationManagerAPI2/ReservationManagerAPI2.csproj", "src/ReservationManagerAPI2/"]
RUN dotnet restore "src/ReservationManagerAPI2/ReservationManagerAPI2.csproj"

#残りのソースコードをコピーする
COPY . .

#アプリをRelease校正で発行する 
# -c:configuration どの構成でビルドするか　今回はRelease　-o:output発行したファイルをどこに出力するか
RUN dotnet publish "src/ReservationManagerAPI2/ReservationManagerAPI2.csproj" -c Release -o /app/publish

#.NET 8 ASP.NET Runtimeを使ってアプリを実行するためのステージ
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

#実行時の作業ディレクトリを指定する
WORKDIR /app

#APIがコンテナ内で待ち受けるポートを示す
EXPOSE 8080

#buildステージで発行したファイルだけを実行用Imageへコピーする
COPY --from=build /app/publish .

#コンテナ起動時にWeb APIを起動する
ENTRYPOINT ["dotnet", "ReservationManagerAPI2.dll"]
