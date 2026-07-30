@echo off
chcp 65001 >nul
title AI论文润色助手 — 卸载
setlocal enabledelayedexpansion

echo ============================================
echo   AI论文润色助手 - 卸载
echo ============================================
echo.

echo [1/2] 移除Word COM加载项注册...
reg delete "HKCU\Software\Microsoft\Office\Word\Addins\AIPolishCOMAddin" /f >nul 2>&1
echo          已完成 ✓
echo.

echo [2/2] 是否同时删除插件设置数据（API配置等）？
echo         选"是"将清除所有已保存的配置。
echo.
choice /C YN /M "是否清除设置数据？[Y/N]"
if !ERRORLEVEL! EQU 1 (
    reg delete "HKCU\Software\AIPaperPolishAddin" /f >nul 2>&1
    echo          设置数据已清除 ✓
) else (
    echo          设置数据已保留 ✓
)
echo.

echo ============================================
echo   ✅ 卸载完成！插件已从Word中移除。
echo.
echo   如需完全清理，可手动删除插件目录：
echo   %~dp0
echo ============================================
echo.

pause
