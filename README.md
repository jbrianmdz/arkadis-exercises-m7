# arkadis-exercises-m7
Ejercicios para la Especialización internacional  BIM en Programación Visual y Automatización con IA

**Software: Revit 2026**

Para usar el código para cada caso es bueno entender que el código Command.cs se reparte de la siguiente manera:

**Caso 01** es Command_Caso01.cs
**Caso 02** es Command_Caso02.cs
**Caso 03** es Command_Caso02.cs
**Caso 04** es Command_Caso02.cs
**Caso 05** es Command_Caso02.cs

<img width="717" height="534" alt="image" src="https://github.com/user-attachments/assets/f8229bd4-f7ac-4584-9422-e8e14c3949be" />

A continuación el código del archivo **HolaRevit.addin**

```
<?xml version="1.0" encoding="utf-8"?>
<RevitAddIns>
	<AddIn Type="Command">
		<Name>Hola Revit</Name>
		<Assembly>C:\Users\jbria\source\repos\HolaRevit\HolaRevit\bin\x64\Debug\net8.0-windows\HolaRevit.dll</Assembly>
		<FullClassName>HolaRevit.Command</FullClassName>
		<AddInId>BBAFB5FD-9727-47C8-A2DC-E01136F48413</AddInId>
		<VendorId>BMM</VendorId>
		<Text>Hola Revit</Text>
	</AddIn>
</RevitAddIns>
```html
 
A continuación el **código del proyecto**:

```
<Project Sdk="Microsoft.NET.Sdk">

	<PropertyGroup>
		<TargetFramework>net8.0-windows</TargetFramework>
		<ImplicitUsings>enable</ImplicitUsings>
		<Nullable>enable</Nullable>
		<Platforms>AnyCPU;x64</Platforms>
		<UseWPF>true</UseWPF>
		<UseWindowsForms>true</UseWindowsForms>
		<CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
	</PropertyGroup>

	<PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|AnyCPU'">
		<NoWarn>1701;1702; MSB3277</NoWarn>
	</PropertyGroup>

	<PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|x64'">
		<NoWarn>1701;1702; MSB3277</NoWarn>
	</PropertyGroup>

	<PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Release|AnyCPU'">
		<NoWarn>1701;1702; MSB3277</NoWarn>
	</PropertyGroup>

	<PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Release|x64'">
		<NoWarn>1701;1702; MSB3277</NoWarn>
	</PropertyGroup>

	<ItemGroup>
		<PackageReference Include="EPPlus" Version="8.5.4">
			<Private>true</Private>
			<CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
		</PackageReference>
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

</Project>
```

