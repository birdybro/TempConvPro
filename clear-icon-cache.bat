@echo off
echo Clearing Windows Icon Cache...
echo.

taskkill /F /IM explorer.exe

timeout /t 2 /nobreak >nul

cd /d %userprofile%\AppData\Local\Microsoft\Windows\Explorer

attrib -h IconCache.db
del IconCache.db

attrib -h iconcache_*.db
del iconcache_*.db

echo Icon cache cleared!
echo.
echo Restarting Windows Explorer...
start explorer.exe

echo.
echo Done! Please check your exe icon now.
pause
