@echo off
setlocal enabledelayedexpansion

echo ================================================================================
echo                     REALMS OF DUST - GIT RELEASE GUARD
echo ================================================================================

if "%~1"=="" (
    echo [ERROR] Missing release version argument!
    echo.
    echo Usage:   ship.bat ^<VERSION^> ^<COMMIT_MESSAGE^>
    echo Example: ship.bat 1.1.0 "Add difficulty-scaled starting retinue"
    echo.
    echo [AI INSTRUCTION]
    echo 1. Check CHANGELOG.md and record the latest patch notes under [v%~1].
    echo 2. Rerun this script with the version and a descriptive commit message.
    echo ================================================================================
    exit /b 1
)

if "%~2"=="" (
    echo [ERROR] Missing commit message argument!
    echo Usage: ship.bat %~1 "Your commit message"
    exit /b 1
)

set "NEW_VERSION=%~1"
set "COMMIT_MSG=%~2"

echo [1/5] Updating Directory.Build.props to version %NEW_VERSION%...
powershell -Command "$path = 'src\Directory.Build.props'; $c = Get-Content $path; $c = $c -replace '<Version>.*?</Version>', '<Version>%NEW_VERSION%</Version>'; $c = $c -replace '<AssemblyVersion>.*?</AssemblyVersion>', '<AssemblyVersion>%NEW_VERSION%.0</AssemblyVersion>'; $c = $c -replace '<FileVersion>.*?</FileVersion>', '<FileVersion>%NEW_VERSION%.0</FileVersion>'; Set-Content $path $c"

echo [2/5] Updating SubModule.xml to version v%NEW_VERSION%...
powershell -Command "$path = 'src\RealmsOfDust.Main\SubModule.xml'; $c = Get-Content $path; $c = $c -replace '<Version value=\"v.*?\" />', '<Version value=\"v%NEW_VERSION%\" />'; Set-Content $path $c"

echo [3/5] Compiling and verifying solution build...
"C:\Users\drew0\.dotnet\dotnet.exe" build "src\RealmsOfDust.sln" -c Debug
if %ERRORLEVEL% neq 0 (
    echo [ERROR] Build failed! Aborting commit and push.
    exit /b 1
)

echo [4/5] Deploying build artifacts to Bannerlord Modules...
call deploy.bat

echo [5/5] Staging files, committing version v%NEW_VERSION%, and pushing to GitHub...
git add -A
git commit -m "release(v%NEW_VERSION%): %COMMIT_MSG%"
git push origin main

echo ================================================================================
echo [SUCCESS] Realms of Dust v%NEW_VERSION% built, deployed, and pushed to GitHub!
echo ================================================================================
endlocal
