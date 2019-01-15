@ECHO OFF  
echo 准备卸载服务  
pause  
REM The following directory is for .NET 4.0  
set DOTNETFX2=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
set PATH=%PATH%;%DOTNETFX2%
echo 卸载服务...  
echo ---------------------------------------------------  
InstallUtil /u    E:\ZT\yitongwuxian\InternetOf ThingsCenter\IntelligentAgriculture-Video\DBRoute\yeetong.WindowsServer\bin\Debug\yeetong_DBRoute.exe
echo ---------------------------------------------------  
echo 安装卸载成功！  
pause  
==========================================  