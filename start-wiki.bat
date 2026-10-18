@echo off
echo Starting MediaWiki server on bannerlord-mod.localhost:4328...
start "" "C:\laragon\bin\apache\httpd-2.4.66-260223-Win64-VS18\bin\httpd.exe" -f "C:/laragon/bin/apache/httpd-2.4.66-260223-Win64-VS18/conf/httpd-4328.conf"
echo MediaWiki is running at http://bannerlord-mod.localhost:4328/
pause
