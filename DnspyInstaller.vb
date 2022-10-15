Imports System.ComponentModel
Imports System.IO
Imports System.Net
Imports System.Threading
Imports Ionic.Zip

Public Class DnspyInstaller
    Public ReadOnly Url As String =
        "https://github.com/dnSpy/dnSpy/releases/download/v6.1.8/dnSpy-netframework.zip"
    Public ReadOnly PathToDownload As String =
        BewTemp & "\dnspy.zip"
    Public Structure Progress
        Public SizeInByte As Int64
        Public TotalDownloadedInByte As Int64
        Public PercentageDownload As Single
    End Structure
    Dim workerThread As Thread
    Public Function IsBusy()
        Return workerThread IsNot Nothing
    End Function
    Public Sub Download(callbackComplete As Action,
                        callbackProgress As Action(Of Progress),
                        Optional callbackError As Action(Of Exception) = Nothing)
        If workerThread IsNot Nothing Then Exit Sub
        If Not Directory.Exists(BewTemp) Then Directory.CreateDirectory(BewTemp)
        If callbackError Is Nothing Then callbackError =
            Sub(a As Exception)
            End Sub

        ServicePointManager.Expect100Continue = True
        ServicePointManager.SecurityProtocol =
            SecurityProtocolType.Tls Or
            SecurityProtocolType.Tls11 Or
            SecurityProtocolType.Tls12 Or
            SecurityProtocolType.Ssl3

        workerThread = New Thread(
            Sub()
                Dim downloader As New WebClient()
                AddHandler downloader.DownloadProgressChanged,
                Sub(o As Object, progressArg As DownloadProgressChangedEventArgs)
                    Dim p As New Progress
                    With p
                        .TotalDownloadedInByte = progressArg.BytesReceived
                        .SizeInByte = progressArg.TotalBytesToReceive
                        .PercentageDownload = progressArg.ProgressPercentage
                    End With
                    callbackProgress(p)
                End Sub
                AddHandler downloader.DownloadFileCompleted,
                Sub(o As Object, evt As AsyncCompletedEventArgs)
                    If evt.Error Is Nothing Then
                        If Not Directory.Exists(DnspyDir) Then Directory.CreateDirectory(DnspyDir)
                        Dim filezip = ZipFile.Read(PathToDownload)
                        For Each f In filezip.Entries
                            f.Extract(DnspyDir, ExtractExistingFileAction.OverwriteSilently)
                        Next
                        callbackComplete()
                    Else
                        callbackError(evt.Error)
                    End If
                    workerThread = Nothing
                End Sub
                downloader.DownloadFileAsync(New Uri(Url), PathToDownload)
            End Sub)
        workerThread.Start()
    End Sub
End Class
