set MOD_NAME=SRFastScrollButtons

set BUILT_DLL=".\bin\Release\%MOD_NAME%.dll"
set LIB_DLL_DIR=.\bin\Release\libs
set SYNTHRIDERS_MODS_DIR="C:\Program Files (x86)\Steam\steamapps\common\SynthRiders\Mods"

copy %BUILT_DLL% %SYNTHRIDERS_MODS_DIR%
REM copy %LIB_DLL_DIR%\* %SYNTHRIDERS_MODS_DIR%
pause