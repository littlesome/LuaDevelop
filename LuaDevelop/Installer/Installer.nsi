;--------------------------------

!include "MUI.nsh"
!include "Sections.nsh"
!include "FileAssoc.nsh"
!include "LogicLib.nsh"
!include "WordFunc.nsh"

;--------------------------------

; Define version info
!define VERSION "1.0.0"

; Installer details
VIAddVersionKey "CompanyName" "LuaDevelop.org"
VIAddVersionKey "ProductName" "LuaDevelop Installer"
VIAddVersionKey "LegalCopyright" "LuaDevelop.org 2005-2013"
VIAddVersionKey "FileDescription" "LuaDevelop Installer"
VIAddVersionKey "ProductVersion" "${VERSION}.0"
VIAddVersionKey "FileVersion" "${VERSION}.0"
VIProductVersion "${VERSION}.0"

; The name of the installer
Name "LuaDevelop"

; The captions of the installer
Caption "LuaDevelop ${VERSION} Setup"
UninstallCaption "LuaDevelop ${VERSION} Uninstall"

; The file to write
OutFile "Binary\LuaDevelop.exe"

; Default installation folder
InstallDir "$PROGRAMFILES\LuaDevelop\"

; Define executable files
!define EXECUTABLE "$INSTDIR\LuaDevelop.exe"
!define WIN32RES "$INSTDIR\Settings\Icon.ico"

; Get installation folder from registry if available
InstallDirRegKey HKLM "Software\LuaDevelop" ""

; Vista redirects $SMPROGRAMS to all users without this
RequestExecutionLevel admin

; Use replace and version compare
!insertmacro WordReplace
!insertmacro VersionCompare

; Required props
SetFont /LANG=${LANG_ENGLISH} "Tahoma" 8
SetCompressor /SOLID lzma
CRCCheck on
XPStyle on

;--------------------------------

; Interface Configuration

!define MUI_HEADERIMAGE
!define MUI_ABORTWARNING

;--------------------------------

; Pages

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_COMPONENTS
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH
!insertmacro MUI_LANGUAGE "English"

;--------------------------------

; InstallTypes

InstType "Default"
InstType "Standalone/Portable"
InstType "un.Default"
InstType "un.Full"

;--------------------------------

; Functions

Function GetDotNETVersion
	
	Push $0
	Push $1
	System::Call "mscoree::GetCORVersion(w .r0, i ${NSIS_MAX_STRLEN}, *i) i .r1"
	StrCmp $1 "error" 0 +2
	StrCpy $0 "not_found"
	Pop $1
	Exch $0
	
FunctionEnd

Function NotifyInstall
	
	SetOverwrite on
	IfFileExists "$INSTDIR\.local" Local 0
	IfFileExists "$LOCALAPPDATA\LuaDevelop\*.*" User Done
	Local:
	SetOutPath "$INSTDIR"
	File "/oname=.update" "..\Bin\Debug\.local"
	User:
	SetOutPath "$LOCALAPPDATA\LuaDevelop"
	File "/oname=.update" "..\Bin\Debug\.local"
	Done:
	
FunctionEnd

;--------------------------------

; Install Sections

Section "LuaDevelop" Main
	
	SectionIn 1 2 RO
	SetOverwrite on
	
	SetOutPath "$INSTDIR"
	
	; Clean library
	RMDir /r "$INSTDIR\Library"

	; Clean old Flex PMD
	IfFileExists "$INSTDIR\Tools\flexpmd\flex-pmd-command-line-1.1.jar" 0 +2
	RMDir /r "$INSTDIR\Tools\flexpmd"
	
	; Copy all files
	File /r /x .svn /x .empty /x *.db /x *.lib /x *.idb /x *.exp /x Exceptions.log /x .local /x .multi /x *.pdb /x *.vshost.exe /x *.vshost.exe.config /x *.vshost.exe.manifest /x "..\Bin\Debug\Data\" /x "..\Bin\Debug\Settings\" /x "..\Bin\Debug\Snippets\" /x "..\Bin\Debug\Templates\" "..\Bin\Debug\*.*"
	
	SetOverwrite off
	
	IfFileExists "$INSTDIR\.local" +6 0
	RMDir /r "$INSTDIR\Data"
	RMDir /r "$INSTDIR\Settings"
	RMDir /r "$INSTDIR\Snippets"
	RMDir /r "$INSTDIR\Templates"
	RMDir /r "$INSTDIR\Projects"
	
	SetOutPath "$INSTDIR\Settings"
	File /r /x .svn /x .empty /x *.db /x LayoutData.fdl /x SessionData.fdb /x SettingData.fdb "..\Bin\Debug\Settings\*.*"
	
	SetOutPath "$INSTDIR\Snippets"
	File /r /x .svn /x .empty /x *.db "..\Bin\Debug\Snippets\*.*"
	
	SetOutPath "$INSTDIR\Templates"
	File /r /x .svn /x .empty /x *.db "..\Bin\Debug\Templates\*.*"

	SetOutPath "$INSTDIR\Projects"
	File /r /x .svn /x .empty /x *.db "..\Bin\Debug\Projects\*.*"

	; Remove PluginCore from plugins...
	Delete "$INSTDIR\Plugins\PluginCore.dll"
	
	; Write update flag file...
	Call NotifyInstall
	
SectionEnd

Section "Desktop Shortcut" DesktopShortcut
	
	SetOverwrite on
	SetShellVarContext all
	
	CreateShortCut "$DESKTOP\LuaDevelop.lnk" "${EXECUTABLE}" "" "${EXECUTABLE}" 0
	
SectionEnd

Section "Quick Launch Item" QuickShortcut
	
	SetOverwrite on
	SetShellVarContext all
	
	CreateShortCut "$QUICKLAUNCH\LuaDevelop.lnk" "${EXECUTABLE}" "" "${EXECUTABLE}" 0
	
SectionEnd

SectionGroup "Language" LanguageGroup

Section "No changes" NoChangesLocale
	
	; Don't change the locale
	
SectionEnd

Section "English" EnglishLocale
	
	SetOverwrite on
	IfFileExists "$INSTDIR\.local" Local 0
	IfFileExists "$LOCALAPPDATA\LuaDevelop\*.*" User Done
	Local:
	ClearErrors
	FileOpen $1 "$INSTDIR\.locale" w
	IfErrors Done
	FileWrite $1 "en_US"
	FileClose $1
	User:
	ClearErrors
	FileOpen $1 "$LOCALAPPDATA\LuaDevelop\.locale" w
	IfErrors Done
	FileWrite $1 "en_US"
	FileClose $1
	Done:
	
SectionEnd

Section "Chinese" ChineseLocale
	
	SetOverwrite on
	IfFileExists "$INSTDIR\.local" Local 0
	IfFileExists "$LOCALAPPDATA\LuaDevelop\*.*" User Done
	Local:
	ClearErrors
	FileOpen $1 "$INSTDIR\.locale" w
	IfErrors Done
	FileWrite $1 "zh_CN"
	FileClose $1
	User:
	ClearErrors
	FileOpen $1 "$LOCALAPPDATA\LuaDevelop\.locale" w
	IfErrors Done
	FileWrite $1 "zh_CN"
	FileClose $1
	Done:
	
SectionEnd

SectionGroupEnd

SectionGroup "Advanced"

Section "Start Menu Group" StartMenuGroup
	
	SectionIn 1	
	SetOverwrite on
	SetShellVarContext all
	
	CreateDirectory "$SMPROGRAMS\LuaDevelop"
	CreateShortCut "$SMPROGRAMS\LuaDevelop\LuaDevelop.lnk" "${EXECUTABLE}" "" "${EXECUTABLE}" 0
	CreateShortCut "$SMPROGRAMS\LuaDevelop\Uninstall.lnk" "$INSTDIR\Uninstall.exe" "" "$INSTDIR\Uninstall.exe" 0
	
SectionEnd

Section "Registry Modifications" RegistryMods
	
	SectionIn 1
	SetOverwrite on
	SetShellVarContext all
	
	Delete "$INSTDIR\.multi"
	Delete "$INSTDIR\.local"
	
	DeleteRegKey /ifempty HKCR "Applications\LuaDevelop.exe"	
	DeleteRegKey /ifempty HKLM "Software\Classes\Applications\LuaDevelop.exe"
	DeleteRegKey /ifempty HKCU "Software\Classes\Applications\LuaDevelop.exe"
	
	!insertmacro APP_ASSOCIATE "luaproj" "LuaDevelop.AS2Project" "LuaDevelop Lua Project" "${WIN32RES}" "" "${EXECUTABLE}"

	!insertmacro APP_ASSOCIATE "ldi" "LuaDevelop.Theme" "LuaDevelop Theme File" "${WIN32RES}" "" "${EXECUTABLE}"
	!insertmacro APP_ASSOCIATE "ldm" "LuaDevelop.Macros" "LuaDevelop Macros File" "${WIN32RES}" "" "${EXECUTABLE}"
	!insertmacro APP_ASSOCIATE "ldt" "LuaDevelop.Template" "LuaDevelop Template File" "${WIN32RES}" "" "${EXECUTABLE}"
	!insertmacro APP_ASSOCIATE "lda" "LuaDevelop.Arguments" "LuaDevelop Arguments File" "${WIN32RES}" "" "${EXECUTABLE}"
	!insertmacro APP_ASSOCIATE "lds" "LuaDevelop.Snippet" "LuaDevelop Snippet File" "${WIN32RES}" "" "${EXECUTABLE}"
	!insertmacro APP_ASSOCIATE "ldb" "LuaDevelop.Binary" "LuaDevelop Binary File" "${WIN32RES}" "" "${EXECUTABLE}"
	!insertmacro APP_ASSOCIATE "ldl" "LuaDevelop.Layout" "LuaDevelop Layout File" "${WIN32RES}" "" "${EXECUTABLE}"
	!insertmacro APP_ASSOCIATE "ldz" "LuaDevelop.Zip" "LuaDevelop Zip File" "${WIN32RES}" "" "${EXECUTABLE}"
	
	!insertmacro APP_ASSOCIATE_REMOVEVERB "LuaDevelop.LuaProject" "ShellNew"

	!insertmacro APP_ASSOCIATE_REMOVEVERB "LuaDevelop.Theme" "ShellNew"	
	!insertmacro APP_ASSOCIATE_REMOVEVERB "LuaDevelop.Macros" "ShellNew"
	!insertmacro APP_ASSOCIATE_REMOVEVERB "LuaDevelop.Template" "ShellNew"
	!insertmacro APP_ASSOCIATE_REMOVEVERB "LuaDevelop.Arguments" "ShellNew"
	!insertmacro APP_ASSOCIATE_REMOVEVERB "LuaDevelop.Snippet" "ShellNew"
	!insertmacro APP_ASSOCIATE_REMOVEVERB "LuaDevelop.Binary" "ShellNew"
	!insertmacro APP_ASSOCIATE_REMOVEVERB "LuaDevelop.Layout" "ShellNew"
	!insertmacro APP_ASSOCIATE_REMOVEVERB "LuaDevelop.Zip" "ShellNew"
	
	; Write uninstall section keys
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LuaDevelop" "InstallLocation" "$INSTDIR"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LuaDevelop" "Publisher" "LuaDevelop.org"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LuaDevelop" "DisplayVersion" "${VERSION}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LuaDevelop" "DisplayName" "LuaDevelop"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LuaDevelop" "Comments" "Thank you for using LuaDevelop."
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LuaDevelop" "UninstallString" "$INSTDIR\Uninstall.exe"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LuaDevelop" "DisplayIcon" "${EXECUTABLE}"
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LuaDevelop" "NoModify" 1
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LuaDevelop" "NoRepair" 1
	WriteRegStr HKLM "Software\LuaDevelop" "CurrentVersion" ${VERSION}
	WriteRegStr HKLM "Software\LuaDevelop" "" $INSTDIR
	WriteUninstaller "$INSTDIR\Uninstall.exe"
	
	!insertmacro UPDATEFILEASSOC
	
SectionEnd

Section "Standalone/Portable" StandaloneMode
	
	SectionIn 2
	SetOverwrite on
	
	SetOutPath "$INSTDIR"
	File ..\Bin\Debug\.local
	
SectionEnd

Section "Multi Instance Mode" MultiInstanceMode
	
	SetOverwrite on
	
	SetOutPath "$INSTDIR"
	File ..\Bin\Debug\.multi
	
SectionEnd

SectionGroupEnd

;--------------------------------

; Install section strings

!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
!insertmacro MUI_DESCRIPTION_TEXT ${Main} "Installs the main program and other required files."
!insertmacro MUI_DESCRIPTION_TEXT ${RegistryMods} "Associates integral file types and adds the required uninstall configuration."
!insertmacro MUI_DESCRIPTION_TEXT ${StandaloneMode} "Runs as standalone using only local setting files. NOTE: Not for standard users and manual upgrade only."
!insertmacro MUI_DESCRIPTION_TEXT ${MultiInstanceMode} "Allows multiple instances of LuaDevelop to be executed. NOTE: There are some open issues with this."
!insertmacro MUI_DESCRIPTION_TEXT ${NoChangesLocale} "Keeps the current language on update and defaults to English on clean install."
!insertmacro MUI_DESCRIPTION_TEXT ${EnglishLocale} "Changes LuaDevelop's display language to English on next restart."
!insertmacro MUI_DESCRIPTION_TEXT ${ChineseLocale} "Changes LuaDevelop's display language to Chinese on next restart."
!insertmacro MUI_DESCRIPTION_TEXT ${StartMenuGroup} "Creates a start menu group and adds default LuaDevelop links to the group."
!insertmacro MUI_DESCRIPTION_TEXT ${QuickShortcut} "Installs a LuaDevelop shortcut to the Quick Launch bar."
!insertmacro MUI_DESCRIPTION_TEXT ${DesktopShortcut} "Installs a LuaDevelop shortcut to the desktop."
!insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------

; Uninstall Sections

Section "un.LuaDevelop" UninstMain
	
	SectionIn 1 2 RO
	SetShellVarContext all
	
	Delete "$DESKTOP\LuaDevelop.lnk"
	Delete "$QUICKLAUNCH\LuaDevelop.lnk"
	Delete "$SMPROGRAMS\LuaDevelop\LuaDevelop.lnk"
	Delete "$SMPROGRAMS\LuaDevelop\Uninstall.lnk"
	RMDir "$SMPROGRAMS\LuaDevelop"
	
	RMDir /r "$INSTDIR\Docs"
	RMDir /r "$INSTDIR\Library"
	RMDir /r "$INSTDIR\Plugins"
	RMDir /r "$INSTDIR\Projects"
	RMDir /r "$INSTDIR\Tools"
	
	IfFileExists "$INSTDIR\.local" +5 0
	RMDir /r "$INSTDIR\Data"
	RMDir /r "$INSTDIR\Settings"
	RMDir /r "$INSTDIR\Snippets"
	RMDir /r "$INSTDIR\Templates"
	
	Delete "$INSTDIR\README.txt"
	Delete "$INSTDIR\FirstRun.fdb"
	Delete "$INSTDIR\Exceptions.log"
	Delete "$INSTDIR\LuaDevelop.exe"
	Delete "$INSTDIR\LuaDevelop.exe.config"
	Delete "$INSTDIR\PluginCore.dll"
	Delete "$INSTDIR\SciLexer.dll"
	Delete "$INSTDIR\Scripting.dll"
	Delete "$INSTDIR\Antlr3.dll"
	Delete "$INSTDIR\Aga.dll"
	Delete "$INSTDIR\LuaInject.dll"
	Delete "$INSTDIR\Decoda.dll"
	Delete "$INSTDIR\dbghelp.dll"
	
	Delete "$INSTDIR\Uninstall.exe"
	RMDir "$INSTDIR"
	
	!insertmacro APP_UNASSOCIATE "luaproj" "LuaDevelop.LuaProject"
	
	!insertmacro APP_UNASSOCIATE "ldi" "LuaDevelop.Theme"
	!insertmacro APP_UNASSOCIATE "ldm" "LuaDevelop.Macros"
	!insertmacro APP_UNASSOCIATE "ldt" "LuaDevelop.Template"
	!insertmacro APP_UNASSOCIATE "lda" "LuaDevelop.Arguments"
	!insertmacro APP_UNASSOCIATE "lds" "LuaDevelop.Snippet"
	!insertmacro APP_UNASSOCIATE "ldb" "LuaDevelop.Binary"
	!insertmacro APP_UNASSOCIATE "ldl" "LuaDevelop.Layout"
	!insertmacro APP_UNASSOCIATE "ldz" "LuaDevelop.Zip"
	
	DeleteRegKey /ifempty HKLM "Software\LuaDevelop"
	DeleteRegKey /ifempty HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LuaDevelop"
	
	DeleteRegKey /ifempty HKCR "Applications\LuaDevelop.exe"	
	DeleteRegKey /ifempty HKLM "Software\Classes\Applications\LuaDevelop.exe"
	DeleteRegKey /ifempty HKCU "Software\Classes\Applications\LuaDevelop.exe"
	
	!insertmacro UPDATEFILEASSOC
	
SectionEnd

Section /o "un.Settings" UninstSettings
	
	SectionIn 2
	
	Delete "$INSTDIR\.multi"
	Delete "$INSTDIR\.local"
	Delete "$INSTDIR\.locale"
	
	RMDir /r "$INSTDIR\Data"
	RMDir /r "$INSTDIR\Settings"
	RMDir /r "$INSTDIR\Snippets"
	RMDir /r "$INSTDIR\Templates"
	RMDir /r "$LOCALAPPDATA\LuaDevelop"
	RMDir "$INSTDIR"
	
SectionEnd

;--------------------------------

; Uninstall section strings

!insertmacro MUI_UNFUNCTION_DESCRIPTION_BEGIN
!insertmacro MUI_DESCRIPTION_TEXT ${UninstMain} "Uninstalls the main program, other required files and registry modifications."
!insertmacro MUI_DESCRIPTION_TEXT ${UninstSettings} "Uninstalls all settings and snippets from the install directory and user's application data directory."
!insertmacro MUI_UNFUNCTION_DESCRIPTION_END

;--------------------------------

; Event functions

Function .onInit
	
	; Check if the installer is already running
	System::Call 'kernel32::CreateMutexA(i 0, i 0, t "LuaDevelop ${VERSION}") i .r1 ?e'
	Pop $0
	StrCmp $0 0 +3
	MessageBox MB_OK|MB_ICONSTOP "The LuaDevelop ${VERSION} installer is already running."
	Abort
	
	Call GetDotNETVersion
	Pop $0
	${If} $0 == "not_found"
	MessageBox MB_OK|MB_ICONSTOP "You need to install Microsoft.NET 2.0 runtime before installing LuaDevelop."
	Abort
	${EndIf}
	StrCpy $0 $0 "" 1 # skip "v"
	${VersionCompare} $0 "2.0.50727" $1
	${If} $1 == 2
	MessageBox MB_OK|MB_ICONSTOP "You need to install Microsoft.NET 2.0 runtime before installing LuaDevelop. You have $0."
	${EndIf}
	
	; Default to English
	StrCpy $1 ${NoChangesLocale}
	call .onSelChange
	
FunctionEnd

Function .onSelChange

	${If} ${SectionIsSelected} ${LanguageGroup}
	!insertmacro UnSelectSection ${LanguageGroup}
	!insertmacro SelectSection $1
	${Else}
	!insertmacro StartRadioButtons $1
	!insertmacro RadioButton ${NoChangesLocale}
	!insertmacro RadioButton ${EnglishLocale}
	!insertmacro RadioButton ${ChineseLocale}
	!insertmacro EndRadioButtons
	${EndIf}
	
FunctionEnd

;--------------------------------
