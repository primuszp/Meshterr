# MeshTerr

MeshTerr is a legacy Windows terrain and spatial mesh viewer modernized to a current .NET WinForms/OpenTK stack.

The actively maintained application is `source/Meshterr`. The older `MeshTopology` project is kept in the repository as historical/reference code and still depends on legacy binary components.

## Current Status

- Target framework: `.NET 10` / `net10.0-windows`
- UI: Windows Forms
- Rendering: OpenTK 4 compatibility profile
- Supported terrain inputs:
  - ER Mapper raster metadata/data: `.ers` plus matching binary data
  - image-based heightmaps: `.bmp`, `.jpg`, `.jpeg`, `.png`
  - sample TIN points via the built-in `TIN > Betoltes` menu

The renderer still uses legacy OpenGL immediate mode and display lists. It is intentionally run with an OpenGL compatibility profile until the rendering layer is migrated to VBO/VAO/shaders.

## Requirements

- Windows
- .NET SDK 10.0 or newer

Check your installed SDK:

```powershell
dotnet --info
```

## Build

From the repository root:

```powershell
dotnet build source\Meshterr.sln
```

Release build:

```powershell
dotnet build source\Meshterr.sln -c Release
```

## Run

```powershell
dotnet run --project source\Meshterr\Meshterr.csproj
```

Or open `source/Meshterr.sln` in Visual Studio.

## Data Folders

- `rsgdem/` contains raster terrain examples.
- `tindem/` contains TIN/XYZ terrain examples.
- `palette/` contains palette assets.
- `texture/` contains image assets from the original project.

Generated `bin/` and `obj/` folders are intentionally ignored.

## Image Heightmap Workflow

When loading a `.png`, `.jpg`, `.jpeg`, or `.bmp` as an RSG terrain:

- `Pixelmeret` X/Y controls the real-world width and height of one pixel.
- `Min - Max` controls the real elevation range mapped from image luminance.
- The image's luminance is normalized internally from `0..1`, then scaled to the given elevation range.

## License

This is a public source-available repository, not an open-source/free-use project.

All rights are reserved by the copyright holder. You may view the code in this public
repository, but you may not use, copy, modify, redistribute, sell, sublicense, or create
derivative works without prior written permission.

See [LICENSE](LICENSE).

## Repository Notes

- `source/Meshterr` is the modernized app.
- `MeshTopology` is legacy/reference code and is not part of the main .NET 10 build.
