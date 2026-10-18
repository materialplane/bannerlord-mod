# Phase 01: Project Scaffolding & SubModule Wiring

## 1. Objective
Establish the clean architecture solution structure for *Realms of Dust*, configure TaleWorlds assembly references, author the official `SubModule.xml`, and establish the base `MBSubModuleBase` entry point.

---

## 2. Directory Structure to Create
```
src/
├── RealmsOfDust.Core/       # Domain interfaces, constants, DTOs (Pure C# / .NET 6)
├── RealmsOfDust.Domain/     # Business logic, pairing algorithms, reset services
├── RealmsOfDust.Engine/     # TaleWorlds campaign behaviors and event listeners
└── RealmsOfDust.Main/       # SubModule.xml, entry point, dependency wiring
```

---

## 3. Key Components
1. **`RealmsOfDust.sln`**: Root Visual Studio / .NET 6 solution file.
2. **`RealmsOfDust.Main/SubModule.xml`**:
   * Module Id: `RealmsOfDust`
   * Name: `Realms of Dust`
   * Version: `v1.0.0`
   * Target Game Version: `v1.2.*` / `v1.3.*`
   * Dependencies: `Native`, `SandBoxCore`, `Sandbox`, `StoryMode`
3. **`RealmsOfDustSubModule.cs`**: Subclass of `MBSubModuleBase`, implementing `OnSubModuleLoad` and `InitializeGameStarter`.

---

## 4. Verification Criteria
* `dotnet build` succeeds across all four projects with 0 errors and 0 warnings.
* All files remain strictly under 200 lines of code.
