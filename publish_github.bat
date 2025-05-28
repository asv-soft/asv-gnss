@echo off
rem ====== projects ======

set projects=Asv.Gnss

rem ====== projects ======

rem copy version to text file, then in variable
git describe --tags >./version.txt
SET /p VERSION=<version.txt
DEL version.txt

(for %%p in (%projects%) do (
	cd src\%%p\bin\Release\
	dotnet nuget push %%p.%VERSION:~1%.nupkg --source https://nuget.pkg.github.com/asv-soft/index.json
	cd ../../../../
)) 





