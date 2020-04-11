Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports System.Text.RegularExpressions
Imports System.Windows.Forms
<PermissionSet(SecurityAction.Demand, Name:="FullTrust")>
<ComVisibleAttribute(True)>
Public Class ErrorWindow
    Public Shared errorString As String = "No errors :^)"
    Public Shared fullStackString As String = "No errors :^)"
    Public Shared faultingSource As String = "No module"
    Private Sub ErrorWindow_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim menu As New Windows.Forms.ContextMenu()
        menu.MenuItems.Add("none")
        widget.ContextMenu = menu
        Me.TopMost = True
        widget.ObjectForScripting = Me
        Dim html = "<html>
                       <head>
                           <style>
                            .button{
                                float:right;
                                margin-left:10px
                            }
                            .monospace {
                                font-family: 'Lucida Console', Courier, monospace;
                            }
                            #title{
                                float:left
                            }
                            .msg{
                                display: none
                            }
                            #dsc {
                                padding-top: 0px;
                                padding-bottom: 5px;
                            }
                            img {
                              float: left;
                            }
                            </style>
                       </head>
                       <body>
                           <div style='clear both;'>
                               <h1 id='title'> An unhandled exception occured! </h1>
                               <button class='button' onclick='window.external.CloseProgram()'>
                                    Close program
                                </button>
                               <button class='button' onclick='window.external.Save()'>
                                    Save this page
                                </button>
                           </div>
                           <br/>
                           <br/>
                           <hr/>
                           <table style='width: 100 %; '>
                            <tbody>
                               <tr>
                                   <td style='width: 8 %; '> 
                                       <img src='http://icons.iconarchive.com/icons/paomedia/small-n-flat/256/sign-error-icon.png' width='64' height='64'/>
                                   </td>
                                   <td>
                                       <p><b>Bannerlord has encountered a problem and needs to close. We are sorry for the inconvience. </b><br/>
                                    If you were in the middle of something, the progress you were working on might be lost.<br/>
                                    This error can be caused by a faulty module XMLs, manifest (submodule.xml), or DLL (bad one or permission error).</p>
                                   </td>
                               </tr>                            
                           </tbody>
                           </table>
                           <br/>
                           <h2>
                               <a href='#' onclick='showFaultingProcedure(this, ""Reasons"" ,""reason"")'>
                                + Show Reason
                            </a>
                           </h2>
                           <div class='msg' id='reason'>
                                Source: <span>{faultingSource}</span>
                               <p class='monospace' id='reasonPre'>
                                    {errorString}
                               </p>
                           </div>
                           <h2>
                               <a href='#' onclick='showFaultingProcedure(this, ""Full stacks"" ,""fullStack"")'>
                                + Show Full stacks
                                </a>
                           </h2>
                           <div class='msg' id='fullStack'>
                               <b>Protip: </b><p>
                                Use a debugger like <a href='http://www.w3schools.com'>dnSpy</a> or 
                                <a href='http://www.w3schools.com'>Visual studio</a> to trace the source of error, by stepping the program
                                line by line.
                                </p>
<pre id='fullStackPre'>{fullStackString}</pre>
                           </div>
                           <script>                                                        
                            function showFaultingProcedure(e, msg, el)
                            {
                                if(document.getElementById(el).style.display == 'block') {
                                    document.getElementById(el).style.display = 'none';
                                    e.innerHTML = '+ Show ' + msg
                                }
                                else
                                {
                                    document.getElementById(el).style.display = 'block';
                                    e.innerHTML = '- Hide ' + msg
                                }
                            }
                            </script>
                       </body>
                   </html>"
        html = html.Replace("{errorString}", errorString)
        html = html.Replace("{faultingSource}", faultingSource)
        html = html.Replace("{fullStackString}", fullStackString)
        widget.DocumentText = html
    End Sub

    Public Sub Test(s As String)
        MessageBox.Show(s, "client code")
    End Sub

    Public Sub Save()
        Dim filename = Str(DateTime.Now.ToFileTimeUtc()) + ".htm"
        File.WriteAllText(filename, widget.DocumentText)
        Dim filePath = Path.GetFullPath(filename)
        MessageBox.Show(filePath, "Saved to")
    End Sub

    Public Function CallStack() As String
        Return errorString
    End Function

    Public Function FullStack() As String
        Return errorString
    End Function

    Public Sub CloseProgram()
        Dim pid = Process.GetCurrentProcess().Id
        Dim proc As Process = Process.GetProcessById(pid)
        proc.Kill()
    End Sub


    Private Sub widget_Navigating(sender As Object, e As WebBrowserNavigatingEventArgs) Handles widget.Navigating
        Dim isUri = Uri.IsWellFormedUriString(e.Url.ToString(), UriKind.RelativeOrAbsolute)
        If (isUri AndAlso e.Url.ToString().StartsWith("http://")) Then
            e.Cancel = True
            Try
                Process.Start(e.Url.ToString())
            Catch ex As Exception
            End Try
            Exit Sub
            Try
                Process.Start("firefox.exe", e.Url.ToString())
            Catch ex As Exception
            End Try
            Exit Sub
            Try
                Process.Start("chrome.exe", e.Url.ToString())
            Catch ex As Exception
            End Try
        End If

    End Sub
End Class