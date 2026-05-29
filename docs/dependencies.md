# Dependencies

Ki Ascension does not use external NuGet packages.

The mod depends on the standard tModLoader mod development environment:

- Terraria, supplied through tModLoader
- tModLoader assemblies and `tModLoader.targets`
- .NET SDK/runtime used by the installed tModLoader version

The project file imports:

```xml
<Import Project="..\tModLoader.targets" Condition="Exists('..\tModLoader.targets')" />
```

That means the repo should be cloned or extracted under:

```text
Documents/My Games/Terraria/tModLoader/ModSources/KiAscension
```

From that location, the parent `ModSources` folder should contain `tModLoader.targets`, and tModLoader's in-game `Workshop > Develop Mods` build flow supplies the required framework references.

Running `dotnet build` from an arbitrary download folder will not work unless `..\tModLoader.targets` exists, because the standalone .NET SDK does not know where Terraria or tModLoader assemblies live.

## Package Audit

There are no `<PackageReference>` entries in `KiAscension.csproj`.

Source files use only:

- `System` and `System.Collections.Generic`
- `Microsoft.Xna.Framework`
- `Terraria`, `Terraria.ID`, `Terraria.ModLoader`, and related tModLoader namespaces
- local `KiAscension.*` namespaces

Do not commit Terraria or tModLoader DLLs into this repository. Users should install tModLoader normally, then build this mod from `ModSources`.
