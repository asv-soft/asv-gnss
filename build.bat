@echo off
rem ====== projects ======

set projects=Asv.Gnss Asv.Gnss.Prometheus Asv.Gnss.Shell Asv.Gnss.Test

rem ====== projects ======

rem install tool for update project version by git describe
dotnet tool install -g dotnet-setversion

rem copy version to text file, then in variable
git describe --abbrev=0 >./version.txt
SET /p VERSION=<version.txt
DEL version.txt

rem build all projects
(for %%p in (%projects%) do (
  	echo %%p
	setversion %VERSION% ./src/%%p/%%p.csproj
	dotnet restore ./src/%%p/%%p.csproj
	dotnet build ./src/%%p/%%p.csproj -c Release
	dotnet pack ./src/%%p/%%p.csproj -c Release
)) 




