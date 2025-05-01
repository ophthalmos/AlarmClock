#define MyAppName "AlarmClock"
#define MyAppVersion "1.0.0.0"

[Setup]
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
SetupMutex={#MyAppName}SetupMutex,Global\{#MyAppName}SetupMutex
VersionInfoVersion={#MyAppVersion}
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
AppPublisher=Wilhelm Happe
VersionInfoCopyright=(C) 2025, W. Happe
AppPublisherURL=https://www.ophthalmostar.de/
AppSupportURL=https://www.ophthalmostar.de/
AppUpdatesURL=https://www.ophthalmostar.de/
DefaultDirName={autopf}\{#MyAppName}
DisableWelcomePage=yes
DisableDirPage=no
DisableReadyPage=yes
CloseApplications=yes
WizardStyle=modern
WizardSizePercent=100
SetupIconFile=AlarmClock.ico
UninstallDisplayIcon={app}\AlarmClock.exe
DefaultGroupName=AlarmClock
AppId=AlarmClock
TimeStampsInUTC=yes
OutputDir=.
OutputBaseFilename={#MyAppName}Setup
Compression=lzma2/max
SolidCompression=yes
DirExistsWarning=no
MinVersion=0,10.0

[Languages]
Name: "German"; MessagesFile: "compiler:Languages\German.isl"

[Files]
Source: "bin\x64\Release\net8.0-windows\AlarmClock.exe"; DestDir: "{app}"; Permissions: users-modify; Flags: ignoreversion
Source: "bin\x64\Release\net8.0-windows\{#MyAppName}.dll"; DestDir: "{app}"; Permissions: users-modify; Flags: ignoreversion
Source: "bin\x64\Release\net8.0-windows\warning.wav"; DestDir: "{app}"; Permissions: users-modify; Flags: ignoreversion
Source: "bin\x64\Release\net8.0-windows\reminder.wav"; DestDir: "{app}"; Permissions: users-modify; Flags: ignoreversion
Source: "bin\x64\Release\net8.0-windows\{#MyAppName}.runtimeconfig.json"; DestDir: "{app}"; Permissions: users-modify; Flags: ignoreversion
Source: "Lizenzvereinbarung.txt"; DestDir: "{app}"; Permissions: users-modify;

[Icons]
Name: "{userdesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppName}.exe"
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppName}.exe"

[Registry]
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "AlarmClock"; ValueData: "{app}\{#MyAppName}.exe"; Flags: createvalueifdoesntexist uninsdeletevalue

[Run]
Filename: "{app}\{#MyAppName}.exe"; Description: "{#MyAppName} starten"; Flags: postinstall nowait skipifsilent shellexec

[Messages]
BeveledLabel=
WinVersionTooLowError=Das Programm erfordert eine höhere Windowsversion.
ConfirmUninstall=Möchten Sie %1 und alle Komponenten entfernen? Eine Deinstallation ist vor einem Update nicht erforderlich.

[CustomMessages]
RemoveSettings=Möchten Sie die Einstellungsdatei ebenfalls entfernen?

[Code]
procedure CurUninstallStepChanged (CurUninstallStep: TUninstallStep);
var
  mres : integer;
begin
  case CurUninstallStep of                   
    usPostUninstall:
      begin
        mres := MsgBox(CustomMessage('RemoveSettings'), mbConfirmation, MB_YESNO or MB_DEFBUTTON2)
        if mres = IDYES then
          begin
          DelTree(ExpandConstant('{userappdata}\{#MyAppName}'), True, True, True);
          end;
      end;
  end;
end; 

procedure DeinitializeSetup();
var
  FilePath: string;
  BatchPath: string;
  S: TArrayOfString;
  ResultCode: Integer;
begin
  if ExpandConstant('{param:deleteSetup|false}') = 'true' then
  begin
    FilePath := ExpandConstant('{srcexe}');
    begin
      BatchPath := ExpandConstant('{%TEMP}\') + 'delete_' + ExtractFileName(ExpandConstant('{tmp}')) + '.bat';
      SetArrayLength(S, 7);
      S[0] := ':loop';
      S[1] := 'del "' + FilePath + '"';
      S[2] := 'if not exist "' + FilePath + '" goto end';
      S[3] := 'goto loop';
      S[4] := ':end';
      S[5] := 'rd "' + ExpandConstant('{tmp}') + '"';
      S[6] := 'del "' + BatchPath + '"';
      if SaveStringsToFile(BatchPath, S, True) then
      begin
        Exec(BatchPath, '', '', SW_HIDE, ewNoWait, ResultCode)
      end;
    end;
  end;
end;

procedure InitializeWizard;
var
  StaticText: TNewStaticText;
begin
  StaticText := TNewStaticText.Create(WizardForm);
  StaticText.Parent := WizardForm.FinishedPage;
  StaticText.Left := WizardForm.FinishedLabel.Left;
  StaticText.Top := WizardForm.FinishedLabel.Top + 120;
  StaticText.Font.Style := [fsBold];
  StaticText.Caption := 'Als besonderes Feature ist die angezeigte Uhr'#13'hindurchklickbar. Um die Uhr auf die gewünschte'#13'Position verschieben zu können, müssen sie'#13'"Hindurchklickbar" vorübergehend abstellen.'#13'Das gelingt duurch einen Klick mit der rechten'#13'Maustaste auf das TrayIcon im Infobereich.'#13''#13'Nota bene: Strg+Win+J und Shift+Strg+Win+J';
end;
