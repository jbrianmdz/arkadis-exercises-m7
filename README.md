**Módulo 07 - Desarrollo de Aplicaciones para Revit**
**Ejercicios para la Especialización internacional  BIM en Programación Visual y Automatización con IA**

**Software: Revit 2026**

Para usar el código para cada caso es bueno entender que el código MiCodigo.cs se reparte de la siguiente manera:

- **Caso 01** es MiCodigo_Caso01.cs
- **Caso 02** es MiCodigo_Caso02.cs
- **Caso 03** es MiCodigo_Caso03.cs
- **Caso 04** es MiCodigo_Caso04.cs
- **Caso 05** es MiCodigo_Caso05.cs

<img width="694" height="534" alt="image" src="https://github.com/user-attachments/assets/68079aef-b0df-42f8-8a28-a0cb04c7cd4e" />

A continuación el código del archivo **InstructivoParaRevit.addin**


```xml
<?xml version="1.0" encoding="utf-8"?>
<RevitAddIns>
	<AddIn Type="Command">
		<Name>Mi Primer Aplicacion</Name>
		<Assembly>AppPrimerEjercicio\PrimerEjercicio.dll</Assembly>
		<FullClassName>PrimerEjercicio.MiCodigo</FullClassName>
		<AddInId>AF47E58F-B9D7-4212-9B1F-09660645C75C</AddInId>
		<VendorId>BMM</VendorId>
		<Text>Mi Primer Aplicacion</Text>
	</AddIn>
</RevitAddIns>
```
  
A continuación el **código del proyecto**:
  
```xml
<Project Sdk="Microsoft.NET.Sdk">

	<PropertyGroup>
		<TargetFramework>net8.0-windows</TargetFramework>
		<ImplicitUsings>enable</ImplicitUsings>
		<Nullable>disable</Nullable>
		<Platforms>x64</Platforms>
		<PlatformTarget>x64</PlatformTarget>
		<MSBuildWarningsAsMessages>MSB3276;MSB3277</MSBuildWarningsAsMessages>

		<!-- 👇 ESTA LÍNEA: fuerza a copiar EPPlus y todas sus dependencias al bin -->
		<CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
	</PropertyGroup>

	<ItemGroup>
		<PackageReference Include="EPPlus" Version="8.6.1" />
	</ItemGroup>

	<ItemGroup>
		<Reference Include="RevitAPI">
			<HintPath>..\..\..\..\..\..\Program Files\Autodesk\Revit 2026\RevitAPI.dll</HintPath>
			<Private>False</Private>
		</Reference>
		<Reference Include="RevitAPIUI">
			<HintPath>..\..\..\..\..\..\Program Files\Autodesk\Revit 2026\RevitAPIUI.dll</HintPath>
			<Private>False</Private>
		</Reference>
	</ItemGroup>

	<!-- ================================================================= -->
	<!-- DESPLIEGUE AUTOMÁTICO A LA CARPETA DE ADD-INS DE REVIT 2026        -->
	<!-- Crea la carpeta si no existe y copia TODO lo necesario.           -->
	<!-- ================================================================= -->
	<Target Name="DeployToRevit" AfterTargets="Build">

		<PropertyGroup>
			<RevitAddins>$(ProgramData)\Autodesk\Revit\Addins\2026</RevitAddins>
			<DeployFolder>$(RevitAddins)\AppPrimerEjercicio</DeployFolder>
		</PropertyGroup>

		<ItemGroup>
			<!-- Todos los DLL de salida MENOS los de Revit (Revit ya los tiene) -->
			<DeployFiles Include="$(TargetDir)*.dll"
						 Exclude="$(TargetDir)RevitAPI.dll;$(TargetDir)RevitAPIUI.dll" />

			<!-- deps.json: necesario para que .NET resuelva EPPlus y dependencias -->
			<DeployFiles Include="$(TargetDir)$(AssemblyName).deps.json" />

			<!-- pdb: opcional, útil para depurar (líneas en el stack trace) -->
			<DeployFiles Include="$(TargetDir)$(AssemblyName).pdb" />
		</ItemGroup>

		<!-- 1) Crear las carpetas si no existen -->
		<MakeDir Directories="$(RevitAddins)" Condition="!Exists('$(RevitAddins)')" />
		<MakeDir Directories="$(DeployFolder)" Condition="!Exists('$(DeployFolder)')" />

		<!-- 2) Copiar el .addin a la raíz de Addins\2026 -->
		<Copy SourceFiles="$(ProjectDir)InstructivoParaRevit.addin"
			  DestinationFolder="$(RevitAddins)"
			  SkipUnchangedFiles="true" />

		<!-- 3) Copiar tu DLL + EPPlus + dependencias a la subcarpeta -->
		<Copy SourceFiles="@(DeployFiles)"
			  DestinationFolder="$(DeployFolder)"
			  SkipUnchangedFiles="true" />

		<!-- 4) Reporte -->
		<Message Importance="High" Text=" " />
		<Message Importance="High" Text="=======================================================================" />
		<Message Importance="High" Text="📦 DESPLIEGUE A REVIT COMPLETADO" />
		<Message Importance="High" Text="   📁 addin:   $(RevitAddins)\InstructivoParaRevit.addin" />
		<Message Importance="High" Text="   📁 carpeta: $(DeployFolder)" />
		<Message Importance="High" Text="   📄 copiados: @(DeployFiles->'%(Filename)%(Extension)', ', ')" />
		<Message Importance="High" Text="=======================================================================" />
		<Message Importance="High" Text=" " />

	</Target>

</Project>
```

