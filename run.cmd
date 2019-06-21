@echo off
set /p token=<%userprofile%\OneDrive\Projects\SlackToken.txt
dotnet src\bin\Release\netcoreapp2.0\SlackPublicaties.dll %token%