@echo off
setlocal

set "TARGET_DIR=C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\Modules\RealmsOfDust"
set "BIN_DIR=%TARGET_DIR%\bin\Win64_Shipping_Client"
set "SRC_BIN=src\RealmsOfDust.Main\bin\Debug\netstandard2.0"

echo [Realms of Dust] Deploying module to Bannerlord directory...

if not exist "%BIN_DIR%" mkdir "%BIN_DIR%"

copy /Y "%SRC_BIN%\SubModule.xml" "%TARGET_DIR%\SubModule.xml" > nul
copy /Y "%SRC_BIN%\RealmsOfDust*.dll" "%BIN_DIR%\" > nul
copy /Y "%SRC_BIN%\RealmsOfDust*.pdb" "%BIN_DIR%\" > nul

echo [Realms of Dust] Deploying plague text and localization data...
xcopy /E /I /Y "src\RealmsOfDust.Main\ModuleData" "%TARGET_DIR%\ModuleData" > nul

echo [Realms of Dust] Successfully deployed to Modules\RealmsOfDust
endlocal
