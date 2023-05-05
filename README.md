# Simples adaptação do código em C++ do FiraSim Client para C#

## Para compilar usando o Visual Studio: 
### Adicionar pacotes:  
    dotnet add package Google.Protobuf
    dotnet add package Grpc
    dotnet add package Grpc.Net.Client
    dotnet add package Grpc.Tools
### Adicionar ao csproj:
```
  <ItemGroup>
    <Protobuf Include="juiz\protobuf\vssref_common.proto" />
 </ItemGroup>  
