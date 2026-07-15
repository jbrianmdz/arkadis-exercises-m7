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
		<!-- Desactivamos Nullable para eliminar las advertencias de advertencia CS8600 y CS8602 de golpe -->
		<Nullable>disable</Nullable>

		<!-- Fuerza al proyecto a compilar en x64 -->
		<Platforms>x64</Platforms>
		<PlatformTarget>x64</PlatformTarget>

		<!-- Oculta las advertencias de conflictos de versiones en .NET 8 -->
		<MSBuildWarningsAsMessages>MSB3276;MSB3277</MSBuildWarningsAsMessages>
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

	<!-- REPORTES POST-COMPILACIÓN -->
	<Target Name="ShowBuildSummary" AfterTargets="Build">
		<ItemGroup>
			<RevitAPIRef Include="@(ReferencePath)" Condition="'%(FileName)' == 'RevitAPI'" />
			<RevitAPIUIRef Include="@(ReferencePath)" Condition="'%(FileName)' == 'RevitAPIUI'" />
			<EPPlusRef Include="@(ReferencePath)" Condition="$([System.String]::new('%(FileName)').StartsWith('EPPlus'))" />
		</ItemGroup>

		<Message Importance="High" Text=" " />
		<Message Importance="High" Text="=======================================================================" />
		<Message Importance="High" Text=" 🚀¡COMPILACIÓN COMPLETADA CON ÉXITO!" />
		<Message Importance="High" Text=" 📂 ARCHIVO DLL GENERADO:" />
		<Message Importance="High" Text=" 👉 $(TargetPath)" />
		<Message Importance="High" Text="-----------------------------------------------------------------------" />
		<Message Importance="High" Text=" 📚 VERIFICACIÓN DE REFERENCIAS Y PAQUETES:" />

		<Message Importance="High" Text="   ✔️ RevitAPI cargada correctamente desde:" Condition="'@(RevitAPIRef)' != ''" />
		<Message Importance="High" Text="      ↳ %(RevitAPIRef.FullPath)" Condition="'@(RevitAPIRef)' != ''" />

		<Message Importance="High" Text="   ✔️ RevitAPIUI cargada correctamente desde:" Condition="'@(RevitAPIUIRef)' != ''" />
		<Message Importance="High" Text="      ↳ %(RevitAPIUIRef.FullPath)" Condition="'@(RevitAPIUIRef)' != ''" />

		<Message Importance="High" Text="   ✔️ EPPlus (NuGet) enlazado correctamente desde:" Condition="'@(EPPlusRef)' != ''" />
		<Message Importance="High" Text="      ↳ %(EPPlusRef.FullPath)" Condition="'@(EPPlusRef)' != ''" />

		<Message Importance="High" Text="=======================================================================" />
		<Message Importance="High" Text=" " />
	</Target>

</Project>
```

