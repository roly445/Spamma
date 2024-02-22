dotnet build -c Release ../Spamma.Api.slnf
dotnet test -c Release /p:CoverletOutputFormat=lcov ../Spamma.Api.slnf