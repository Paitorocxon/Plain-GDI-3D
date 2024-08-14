@echo off
setlocal enabledelayedexpansion

REM Ziel-Datei definieren
set "outputFile=bigfile.txt"

REM Sicherstellen, dass die Ziel-Datei leer ist oder neu erstellt wird
> "%outputFile%" echo.

REM Durch alle .cs-Dateien im aktuellen Verzeichnis iterieren
for %%F in (*.cs) do (
    echo Dateiname: %%F >> "%outputFile%"
    type "%%F" >> "%outputFile%"
    echo. >> "%outputFile%"
)

echo Fertig.
