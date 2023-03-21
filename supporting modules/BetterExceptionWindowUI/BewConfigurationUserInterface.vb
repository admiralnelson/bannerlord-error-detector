Imports MCM.Abstractions.FluentBuilder
Imports MCM.Common
Imports BetterExceptionWindow
Public Module BewConfigurationUserInterface
    Dim registered = False
    Dim dontshowMessageBox = False
    ReadOnly BewConfigUIVersion = GetAssemblyByDll("BetterExceptionWindowConfigUI.dll").GetName().Version.ToString()
    Private Sub ShowRestartMessageBox()
        If dontshowMessageBox Then Exit Sub
        MsgBoxBannerlord("Restart", "Restart Bannerlord to take effect")
    End Sub

    Public Sub BuildUI()
        Dim isDownloading = False
        dontshowMessageBox = True

        If registered Then Exit Sub
        Dim basebuilder = BaseSettingsBuilder _
        .Create(
            "org.calradia.admiralnelson.betterexceptionwindow.ui.config",
            "Better Exception Window")
        If basebuilder Is Nothing Then Exit Sub

        basebuilder _
        .SetFormat("json2") _
        .CreateGroup(
            "Behaviour",
            Function(builder)
                Return builder _
                .AddBool(
                    "AllowInDebugger",
                    "{=AllowInDebugger}Allow In Debugger",
                    New ProxyRef(Of Boolean)(
                    Function() As Boolean
                        Return AllowInDebugger
                    End Function,
                    Sub(o As Boolean)
                        AllowInDebugger = o
                        ShowRestartMessageBox()
                        SaveSettings()
                    End Sub),
                    Function(boolBuilder)
                        Return boolBuilder _
                               .SetHintText("{=AllowInDebuggerHint}Better Exception Window will trigger when debugger is attached")
                    End Function) _
                .AddBool(
                    "EnableStdoutConsole",
                    "{=EnableStdoutConsole}Enable Stdout/Stdin console",
                    New ProxyRef(Of Boolean)(
                    Function() As Boolean
                        Return EnableStdoutConsole
                    End Function,
                    Sub(o As Boolean)
                        EnableStdoutConsole = o
                        If EnableStdoutConsole Then SpawnConsole()
                        SaveSettings()
                    End Sub),
                    Function(boolBuilder)
                        Return boolBuilder _
                               .SetHintText("{=EnableStdoutConsoleHint}Checking this will spawn console window.")
                    End Function) _
                    .AddBool(
                    "DisableBewButterlibException",
                    "{=AllowInDebugger}Disable BewButterlib Exception",
                    New ProxyRef(Of Boolean)(
                    Function() As Boolean
                        Return DisableBewButterlibException
                    End Function,
                    Sub(o As Boolean)
                        DisableBewButterlibException = o
                        ShowRestartMessageBox()
                        SaveSettings()
                    End Sub),
                    Function(boolBuilder)
                        Return boolBuilder _
                               .SetHintText("{=AllowInDebuggerHint}Uncheck this options if you want to restore the old behaviour, Butterlib exception then followed by BEW window. Restart the game to take effect.")
                    End Function) _
                    .AddBool(
                    "CatchNative2Managed",
                    "{=EnableNative2Managed}Enable Native code to Managed code exception handler",
                    New ProxyRef(Of Boolean)(
                    Function() As Boolean
                        Return CatchNative2Managed
                    End Function,
                    Sub(o As Boolean)
                        CatchNative2Managed = o
                        ShowRestartMessageBox()
                        SaveSettings()
                    End Sub),
                    Function(boolBuilder)
                        Return boolBuilder _
                               .SetHintText("{=CatchNative2ManagedHint}This will put exception handlers in Native code to Managed code procedures. Uncheck this if you're experiencing slowdowns in mission or battles.")
                    End Function)
            End Function) _
           .CreateGroup("Dnspy Debugger",
                Function(builder)
                    Return builder _
                   .AddButton(
                    "InstallDnspy",
                    "{=InstallDnspyLabel}Install Dnspy now",
                    New ProxyRef(Of Action)(
                        Function() As Action
                            Return Sub()
                                If isDownloading Then Exit Sub
                                If IsDnspyAvailable() Then
                                           Print("Dnspy already installed", False)
                                           Exit Sub
                                End If
                                       MsgBoxBannerlord("Install Dnspy", "Do you want to install Dnspy now?",
                                       Sub()
                                           Print("Installing Dnspy...")
                                           Dim dnspyInstall As New DnspyInstaller
                                           isDownloading = True
                                           dnspyInstall.Download(
                                               Sub()
                                                   MsgBoxBannerlord("Dnspy install", "...is completed!")
                                                   isDownloading = False
                                               End Sub,
                                               Sub(prog As DnspyInstaller.Progress)
                                                   Print($"Downloading...({Math.Round(prog.PercentageDownload)}%) {CInt(prog.TotalDownloadedInByte / 1000)}K/{CInt(prog.SizeInByte / 1000)}K")
                                               End Sub,
                                               Sub(ex As Exception)
                                                   MsgBoxBannerlord("An error occured", $"Installation progress was interrupted. Reason: {ex.Message}")
                                                   Print($"Installation progress was interrupted. Reason: {ex.Message}")
                                                   isDownloading = False
                                               End Sub
                                           )
                                       End Sub,
                                       Sub()

                                       End Sub)
                                   End Sub
                        End Function,
                    Sub(o As Action)

                    End Sub),
                    "{=InstallDnspyButton}Install now",
                    Function(boolBuilder)
                        Return boolBuilder _
                            .SetHintText("{=InstallDnspyHint}Install Dnspy debugger")
                    End Function) _
                    .AddButton(
                    "AttachDnspy",
                    "{=AttachNowLabel}Attach dnSpy now",
                    New ProxyRef(Of Action)(
                        Function() As Action
                            Return Sub()
                                       If Debugger.IsAttached Then
                                           Print("The game is running under debugger already")
                                           Exit Sub
                                       End If
                                       If IsDnspyAvailable() Then
                                           Print("Please wait... Don't spam this button", False)
                                           AttachDnspy()
                                           Unpatch()
                                           Exit Sub
                                       End If
                                       Print("Dnspy is not installed in BetterExceptionWindow", False)
                                   End Sub
                        End Function,
                    Sub(o As Action)

                    End Sub),
                    "{=AttachNowButton}Attach now",
                    Function(boolBuilder)
                        Return boolBuilder _
                            .SetHintText("{=AttachHint}Attach Dnspy debugger now")
                    End Function) _
                    .AddButton(
                    "Restart",
                    "{=RestartDnspyLabel}Restart with dnSpy attached",
                    New ProxyRef(Of Action)(
                        Function() As Action
                            Return Sub()
                                       If Debugger.IsAttached Then
                                           Print("The game is running under debugger already")
                                           Exit Sub
                                       End If
                                       If IsDnspyAvailable() Then
                                           MsgBoxBannerlord("Warning!", "This will restart your game! Are you sure? Save your game first if neccessary",
                                         Sub()
                                             RestartAndAttachDnspy()
                                         End Sub,
                                         Sub()

                                         End Sub,
                                         "Do as I say!"
                                       )
                                           Exit Sub
                                       End If
                                       Print("Dnspy is not installed in BetterExceptionWindow")
                                   End Sub
                        End Function,
                    Sub(o As Action)

                    End Sub),
                    "{=RestartButton}Restart",
                    Function(boolBuilder)
                        Return boolBuilder _
                            .SetHintText("{=RestartDebuggerHint}Restart the game with debugger attached")
                    End Function)
                End Function) _
        .CreateGroup("Other",
         Function(builder)
             Return builder _
                .AddButton(
                "Crashtest",
                "{=CrashTest}Crash Test: Throw an exception",
                New ProxyRef(Of Action)(
                    Function() As Action
                        Return Sub()
                                   MsgBoxBannerlord("Warning!", "This will crash your game! Are you sure?",
                                Sub()
                                    Throw New Exception("Exception test on UI event")
                                End Sub,
                                Sub()

                                End Sub,
                                "Do as I say!"
                            )
                               End Sub
                    End Function,
                Sub(o As Action)

                End Sub),
                "{=TestItNow}Test it now",
                Function(boolBuilder)
                    Return boolBuilder _
                        .SetHintText("{=AllowInDebuggerHint}This will crash your game!")
                End Function) _
                 .AddButton(
                "CrashtestNativeCode",
                "{=CrashTest}Crash Test: Throw native code exception",
                New ProxyRef(Of Action)(
                    Function() As Action
                        Return Sub()
                                   MsgBoxBannerlord("Warning!", "This will crash your game! Are you sure?",
                                Sub()
                                    BewUnsafeExceptionTester.Unsafe.TriggerAccessViolation()
                                End Sub,
                                Sub()

                                End Sub,
                                "Do as I say!"
                            )
                               End Sub
                    End Function,
                Sub(o As Action)

                End Sub),
                "{=TestItNow}Test it now",
                Function(boolBuilder)
                    Return boolBuilder _
                        .SetHintText("{=AllowInDebuggerHint}This will crash your game!")
                End Function) _
                .AddButton(
                    "About",
                    "{=AboutBewLabel}About BetterExceptionWindow",
                    New ProxyRef(Of Action)(
                        Function() As Action
                            Return Sub()
                                       MsgBoxBannerlord("About this program",
                                                     Version & vbNewLine &
                                                    "BetterExceptionWindowConfigUI component version: " & BewConfigUIVersion & vbNewLine & vbNewLine &
                                                    "Git commit: " & Commit & vbNewLine &
                                                    "Source code: https://github.com/admiralnelson/bannerlord-error-detector")
                                   End Sub
                        End Function,
                    Sub(o As Action)

                    End Sub),
                  "{=AboutButton}About...",
                  Function(boolBuilder)
                      Return boolBuilder _
                            .SetHintText("{=AboutHint}About this program.")
                  End Function) _
                  .AddButton(
                    "Feedback",
                    "{=AboutBewLabel}Send anonymous feedback",
                    New ProxyRef(Of Action)(
                        Function() As Action
                            Return Sub()
                                       Process.Start("https://forms.office.com/r/MPSahZmpRt")
                                       Print("Thank you!", False)
                                   End Sub
                        End Function,
                    Sub(o As Action)

                    End Sub),
                  "{=AboutButton}Send feedback...",
                  Function(boolBuilder)
                      Return boolBuilder _
                            .SetHintText("{=AboutHint}Submit your thoughts and feedback about the current state of Better Exception Window via online survey forms. It takes less than a minute to complete.")
                  End Function)

         End Function) _
        .BuildAsGlobal() _
        .Register()
        registered = True
        dontshowMessageBox = False
    End Sub
End Module