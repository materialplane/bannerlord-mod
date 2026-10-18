@echo off
echo Stopping MediaWiki server on port 4328...
for /f "tokens=5" %%a in ('netstat -aon ^| findstr :4328 ^| findstr LISTENING') do taskkill /f /pid %%a
echo MediaWiki server stopped.
pause
