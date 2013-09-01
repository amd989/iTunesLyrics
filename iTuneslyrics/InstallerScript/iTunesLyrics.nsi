;NSIS Modern User Interface
;Basic Example Script
;Written by Joost Verburg

;--------------------------------
;Include Modern UI

  !include "MUI.nsh"

;--------------------------------
;General

  ;Name and file
  Name "iTunes Lyrics Importer"
  OutFile "iLyrics 1.1.exe"

  ;Default installation folder
  InstallDir "$PROGRAMFILES\iLyrics"
  
  ;Get installation folder from registry if available
  InstallDirRegKey HKCU "Software\iLyrics" ""
  
  ;--------------------------------
  ;Variables
  
    Var MUI_TEMP
  Var STARTMENU_FOLDER

;--------------------------------
;Interface Settings

  !define MUI_ABORTWARNING

;--------------------------------
;Pages

  !insertmacro MUI_PAGE_COMPONENTS
  !insertmacro MUI_PAGE_DIRECTORY
  
    ;Start Menu Folder Page Configuration
    !define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKCU" 
    !define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\iLyrics" 
    !define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "Start Menu Folder"
    
  !insertmacro MUI_PAGE_STARTMENU Application $STARTMENU_FOLDER
  
  !insertmacro MUI_PAGE_INSTFILES
  
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES
  
;--------------------------------
;Languages
 
  !insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Installer Sections

Section "iTunes Lyrics Importer" SecDummy

  SetOutPath "$INSTDIR"
  
  ;Put file there
  File "..\bin\Release\Interop.iTunesLib.dll"
  File "..\bin\Release\iTuneslyrics.exe"
  
  ;Store installation folder
  WriteRegStr HKCU "Software\iLyrics" "" $INSTDIR
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\iLyrics" \
                 "DisplayName" "iTunes Lyrics Importer"
WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\iLyrics" \
                 "UninstallString" "$INSTDIR\Uninstall.exe"
  
  ;Create uninstaller
  WriteUninstaller "$INSTDIR\Uninstall.exe"
  
    !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
      
      ;Create shortcuts
      CreateDirectory "$SMPROGRAMS\$STARTMENU_FOLDER"
      CreateShortCut "$SMPROGRAMS\$STARTMENU_FOLDER\iLyrics.lnk" "$INSTDIR\iTuneslyrics.exe"
      CreateShortCut "$SMPROGRAMS\$STARTMENU_FOLDER\Uninstall.lnk" "$INSTDIR\Uninstall.exe"
    
  !insertmacro MUI_STARTMENU_WRITE_END

SectionEnd

;--------------------------------
;Descriptions

  ;Language strings
  LangString DESC_SecDummy ${LANG_ENGLISH} "Install iLyrics"

  ;Assign language strings to sections
  !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${SecDummy} $(DESC_SecDummy)
  !insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------
;Uninstaller Section

Section "Uninstall"

  ;ADD YOUR OWN FILES HERE...

  Delete "$INSTDIR\iTuneslyrics.exe"
  Delete "$INSTDIR\Interop.iTunesLib.dll"
  Delete "$INSTDIR\Uninstall.exe"

  RMDir "$INSTDIR"
  
    !insertmacro MUI_STARTMENU_GETFOLDER Application $MUI_TEMP
      
    Delete "$SMPROGRAMS\$MUI_TEMP\Uninstall.lnk"
    Delete "$SMPROGRAMS\$MUI_TEMP\iLyrics.lnk"
    
    ;Delete empty start menu parent diretories
    StrCpy $MUI_TEMP "$SMPROGRAMS\$MUI_TEMP"
   
    startMenuDeleteLoop:
  	ClearErrors
      RMDir $MUI_TEMP
      GetFullPathName $MUI_TEMP "$MUI_TEMP\.."
      
      IfErrors startMenuDeleteLoopDone
    
      StrCmp $MUI_TEMP $SMPROGRAMS startMenuDeleteLoopDone startMenuDeleteLoop
  startMenuDeleteLoopDone:

  DeleteRegKey /ifempty HKCU "Software\iLyrics"
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\iLyrics"

SectionEnd