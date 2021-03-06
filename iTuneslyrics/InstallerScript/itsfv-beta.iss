; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

[Code]

function InitializeSetup(): Boolean;
var
    ErrorCode: Integer;
    NetFrameWorkInstalled : Boolean;
    Result1 : Boolean;
begin

           NetFrameWorkInstalled := RegKeyExists(HKLM,'SOFTWARE\Microsoft\.NETFramework\policy\v3.0');
           if NetFrameWorkInstalled =true then
           begin
              Result := true;
           end;
           if NetFrameWorkInstalled = false then
           begin
               NetFrameWorkInstalled := RegKeyExists(HKLM,'SOFTWARE\Microsoft\.NETFramework\policy\v2.0');
               if NetFrameWorkInstalled =true then
               begin
                  Result := true;
               end;
               if NetFrameWorkInstalled =false then
               begin
                         Result1 := MsgBox('This setup requires the .NET Framework. Please download and install the .NET Framework and run this setup again.',
                              mbInformation, MB_OKCANCEL) = idOK;
                         if Result1 =false then
                         begin
                            Result:=false;
                         end
                         else
                         begin
                              Result:=false;
                                ShellExec('open', 'http://go.microsoft.com/fwlink/?LinkId=70848', '', '', SW_SHOW, ewNoWait, ErrorCode);
                        end;
               end;
          end;
end;

[Setup]

AppName=iLyrics
AppVerName=iLyrics 1.1.1.2 BETA
;VersionInfoVersion=5.5.2.5
;VersionInfoTextVersion=5.5.2.5 BETA
VersionInfoCompany=Senthil Kumar
VersionInfoDescription=iTunes Lyrics Importer
AppPublisher=McoreD
AppPublisherURL=http://code.google.com/p/ilyrics/
AppSupportURL=http://code.google.com/p/ilyrics/
AppUpdatesURL=http://code.google.com/p/ilyrics/
DefaultDirName={userdocs}\Applications\iLyrics
DefaultGroupName=iTunes\Plugins
AllowNoIcons=yes
InfoBeforeFile=..\VersionHistory.txt
;InfoAfterFile=iLyrics\VersionHistory.txt
Compression=lzma
SolidCompression=yes
PrivilegesRequired=none
OutputDir=..\Output\

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\bin\Release\iTuneslyrics.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion
;Source: "..\bin\Release\*.mp3"; DestDir: "{app}"; Flags: ignoreversion

;Source: "..\bin\Release\Interop.iTunesLib.dll"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\iLyrics"; Filename: "{app}\iTuneslyrics.exe"
Name: "{userdesktop}\iLyrics"; Filename: "{app}\iTuneslyrics.exe"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\iLyrics"; Filename: "{app}\iTuneslyrics.exe"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\iTuneslyrics.exe"; Description: "{cm:LaunchProgram,iLyrics}"; Flags: nowait postinstall skipifsilent
;Filename: "{app}\manual-iLyrics.pdf"; Description: "{cm:LaunchProgram,iLyrics Manual}"; Flags: nowait unchecked postinstall shellexec skipifsilent
