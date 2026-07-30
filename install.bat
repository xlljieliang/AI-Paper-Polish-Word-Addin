@echo off
chcp 65001 >nul
title AI论文润色助手 — 一键安装
setlocal enabledelayedexpansion

echo ============================================
echo   AI论文润色助手 - Word COM加载项一键安装
echo ============================================
echo.

:: 获取当前脚本所在目录（作为安装目录）
set "INSTALL_DIR=%~dp0"
:: 去掉末尾的反斜杠
if "%INSTALL_DIR:~-1%"=="\" set "INSTALL_DIR=%INSTALL_DIR:~0,-1%"

echo [1/3] 检查文件...
if not exist "%INSTALL_DIR%\AIPolishCOMAddin.dll" (
    echo [错误] 未找到 AIPolishCOMAddin.dll！
    echo        请确认本脚本与dll文件在同一目录下。
    echo.
    echo        当前目录：%INSTALL_DIR%
    echo.
    pause
    exit /b 1
)
echo          找到 AIPolishCOMAddin.dll ✓
echo.

echo [2/3] 配置注册表...
:: 生成临时reg文件，替换路径占位符
set "REG_FILE=%TEMP%\AIPolish_install.reg"

(
echo Windows Registry Editor Version 5.00
echo.
echo [HKEY_CURRENT_USER\Software\Microsoft\Office\Word\Addins\AIPolishCOMAddin]
echo "FriendlyName"="AI论文润色助手"
echo "Description"="AI顶会论文润色插件 - 支持NeurIPS/ICLR/ICML/CVPR/ICCV/ECCV/AAAI"
echo "LoadBehavior"=dword:00000003
echo "Manifest"="file:///%INSTALL_DIR:\=\\%\\AIPolishCOMAddin.dll"
echo "CommandLineSafe"=dword:00000000
) > "%REG_FILE%"

:: 导入注册表
regedit /s "%REG_FILE%"
if %ERRORLEVEL% NEQ 0 (
    echo [错误] 注册表导入失败！请以管理员身份运行本脚本。
    pause
    exit /b 1
)
del "%REG_FILE%"
echo          注册表配置完成 ✓
echo.

echo [3/3] 验证安装...
reg query "HKCU\Software\Microsoft\Office\Word\Addins\AIPolishCOMAddin" >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo          安装确认成功 ✓
) else (
    echo [警告] 注册表查询未确认，请手动检查。
)
echo.

echo ============================================
echo   ✅ 安装完成！
echo.
echo   请按以下步骤启用插件：
echo   1. 打开 Microsoft Word
echo   2. 点击 文件 → 选项 → 加载项
echo   3. 管理下拉框选择 "COM加载项" → 转到
echo   4. 勾选 "AI论文润色助手" → 确定
echo.
echo   首次使用需配置API参数（插件会自动引导）
echo.
echo   卸载方法：双击 Unregister_Addin.reg
echo ============================================
echo.

pause
