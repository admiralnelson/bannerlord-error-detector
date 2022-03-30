Imports MCM.Abstractions.FluentBuilder
Imports MCM.Abstractions.Ref
Imports BetterExceptionWindow
Public Module BewConfigurationUserInterface
    Dim registered = False
    Public Sub BuildUI()
        If registered Then Exit Sub
        BaseSettingsBuilder _
        .Create(
            "org.calradia.admiralnelson.betterexceptionwindow.ui.config",
            "Better Exception Window") _
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
                        SaveSettings()
                    End Sub),
                    Function(boolBuilder)
                        Return boolBuilder _
                               .SetHintText("{=AllowInDebuggerHint}Better Exception Window will trigger when debugger is attached")
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
                        SaveSettings()
                    End Sub),
                    Function(boolBuilder)
                        Return boolBuilder _
                               .SetHintText("{=AllowInDebuggerHint}Uncheck this options if you want to restore the old behaviour, Butterlib exception then followed by BEW window")
                    End Function) _
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
                 "Restart",
                 "{=CrashTest}Restart with dnSpy attached",
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
                  "{=TestItNow}Restart",
                  Function(boolBuilder)
                      Return boolBuilder _
                            .SetHintText("{=AllowInDebuggerHint}Restart the game with debugger attached")
                  End Function) _
                 .AddButton(
                 "About",
                 "{=CrashTest}About BetterExceptionWindow",
                   New ProxyRef(Of Action)(
                    Function() As Action
                        Return Sub()
                                   MsgBoxBannerlord("About this program",
                                                     Version & vbNewLine &
                                                    "Git commit: " & Commit & vbNewLine &
                                                    "Source code: https://github.com/admiralnelson/bannerlord-error-detector")
                               End Sub
                    End Function,
                    Sub(o As Action)

                    End Sub),
                  "{=TestItNow}About...",
                  Function(boolBuilder)
                      Return boolBuilder _
                            .SetHintText("{=AllowInDebuggerHint}About this program.")
                  End Function)
            End Function) _
            .BuildAsGlobal() _
            .Register()
        registered = True
    End Sub
End Module