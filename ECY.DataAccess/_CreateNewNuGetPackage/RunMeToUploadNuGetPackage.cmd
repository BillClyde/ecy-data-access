@ECHO OFF
REM === DO NOT EDIT THIS FILE ===
REM Run this script to upload a NuGet package to the NuGet gallery.
REM When you run this script it will prompt you for a NuGet package file (.nupkg) and then upload it to the NuGet gallery.
REM The project's .nupkg file should be in the same directory as the project's .dll/.exe file (typically bin\Debug or bin\Release).
REM You may edit the Config.ps1 file to adjust the settings used to upload the package to the NuGet gallery.
REM To run this script from within Visual Studio, right-click on this file from the Solution Explorer and choose Run.
SET THIS_SCRIPTS_DIRECTORY=%~dp0
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& '%THIS_SCRIPTS_DIRECTORY%DoNotModify\UploadNuGetPackage.ps1'"