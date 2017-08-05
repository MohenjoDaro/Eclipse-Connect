' Imports
Imports System
Imports System.Net
Imports System.Text.RegularExpressions

Imports System.Threading
Imports System.Threading.Tasks

Imports SharpCompress

Imports Google
Imports Google.Apis.Auth.OAuth2
Imports Google.Apis.Drive.v3
Imports Google.Apis.Drive.v3.Data
Imports Google.Apis.Services

Imports Google.Apis.Auth
Imports Google.Apis.Download
Imports Newtonsoft

Imports MediaFireSDK

Imports Dropbox.Api


Public Class frmMain
    ' Globals
    Private urlPath As String
    Private urlPrefix As String
    Private currentVersion As String
    Private versionsArray() As String
    Private versionLoc() As Integer
    Private arraySize As Integer
    Private fileExt As String
    Private ExtLoc As String

    Private Running As Boolean
    Private errorOccurred As Boolean

    Private maxCount As Integer
    Private curCount As Integer

    Private myWebClient As New WebClient()

    'Constants
    Private path As String = AppDomain.CurrentDomain.BaseDirectory()
    Private tempPath As String = AppDomain.CurrentDomain.BaseDirectory() & "temp"

    ' Dropbox
    Private dbx As DropboxClient = New DropboxClient("zlzjrip7rat64kt")

    ' Google
    Private Service As DriveService = New DriveService

    ' Label
    Private Delegate Sub setLabelTxtInvoker(ByVal text As String, ByVal lbl As Label)

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Set Background Image
        If IO.File.Exists(path & "lib/background.png") Then Me.BackgroundImage = Image.FromFile(path & "lib/background.png")

        ' Check the Config File
        checkConfig()

        ' Show any errors
        If urlPath = vbNullString Then
            lblError.Text = "No URL Selected"
            Exit Sub
        End If
        If Not urlExists() Then
            lblError.Text = "URL not Responding"
            Exit Sub
        End If

        If needUpdate() Then
            ' Things to get out of the way
            prgbarUpdate.Visible = True

            'Control.CheckForIllegalCrossThreadCalls = False
            btnUpdate.Enabled = True
            bgwUpdater.RunWorkerAsync()
        Else

        End If
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        ' Pause and Unpause
        Running = Not Running
        If Running Then btnUpdate.Text = "Pause" Else btnUpdate.Text = "Resume"
    End Sub

    Private Sub checkConfig()
        If IO.File.Exists(path & "Config.txt") Then
            ' Read Config Values
            Using ConfigReader As IO.StreamReader = My.Computer.FileSystem.OpenTextFileReader(path & "Config.txt")
                urlPath = ConfigReader.ReadLine()
                urlPrefix = ConfigReader.ReadLine()
                fileExt = ConfigReader.ReadLine()
                currentVersion = ConfigReader.ReadLine()
                ExtLoc = ConfigReader.ReadLine()
            End Using

            ' Correct the Values
            urlPath = urlPath.Substring(4)
            urlPrefix = urlPrefix.Substring(10)
            fileExt = fileExt.Substring(4)
            currentVersion = currentVersion.Substring(8)
            ExtLoc = ExtLoc.Substring(7)
        Else
            ' Create Config File
            Using ConfigWriter As IO.StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(path & "Config.txt", False)
                ConfigWriter.WriteLine("URL=https://www.eclipseorigins.com/topic/86170/er-version-history-and-changelog")
                ConfigWriter.WriteLine("URLPrefix=https://drive.google.com/file/")
                ConfigWriter.WriteLine("Ext=.rar")
                ConfigWriter.WriteLine("Version=0.0.0")
                ConfigWriter.WriteLine("ExtLoc=")
            End Using

            ' Set Default Values
            urlPath = "https://www.eclipseorigins.com/Thread-Eclipse-Renewal"
            urlPrefix = "http://www.mediafire.com/file/"
            fileExt = ".rar"
            currentVersion = "0.0.0"
            ExtLoc = vbNullString
        End If

        lblLocalVersion.Text = "Local Version: " & currentVersion

        ' Set Extraction Location
        If ExtLoc = vbNullString Then
            ExtLoc = path & "Exraction Location" ' Default Location
            If Not IO.Directory.Exists(ExtLoc) Then My.Computer.FileSystem.CreateDirectory(ExtLoc)
        ElseIf Not ExtLoc.Contains("\") Then
            ExtLoc = path & ExtLoc ' Custom Folder
            If Not IO.Directory.Exists(ExtLoc) Then My.Computer.FileSystem.CreateDirectory(ExtLoc)
        End If
    End Sub

    Private Sub saveConfig(ByVal line As Byte, ByVal text As String)
        Dim lines() As String = IO.File.ReadAllLines(path & "Config.txt") ' Get Config Text

        ' Change Line
        lines(line - 1) = text

        ' Save Selected Config Line
        IO.File.WriteAllLines(path & "Config.txt", lines)
    End Sub

    Private Function urlExists()
        Dim req As WebRequest
        Dim res As WebResponse

        req = WebRequest.Create(urlPath)

        Try
            res = req.GetResponse()
        Catch e As WebException
            urlExists = False ' URLPath isn't Valid
            Exit Function
        End Try

        urlExists = True ' URLPath is valid
    End Function

    Private Function needUpdate()
        Dim wb As New WebClient
        Dim html As String = wb.DownloadString(urlPath) ' Get URL Code

        Dim versionFormat As String = ">Version: " ' The > is for finding the right version location
        Dim numberFormat As String = "##.##.##"

        Dim BooleanAnswer As Boolean = html.Contains(versionFormat)
        Dim HowMany As Integer
        Dim FoundList As List(Of Integer)
        Dim tempVersion As String

        If BooleanAnswer Then
            ' Find how many versions
            HowMany = FindIndexes(versionFormat, html, numberFormat.Length).Count

            ' Find Version Locations
            FoundList = FindIndexes(versionFormat, html, numberFormat.Length)
            ReDim versionsArray(FoundList.Count)
            ReDim versionLoc(FoundList.Count)
            arraySize = 0

            For i As Integer = 0 To FoundList.Count - 1
                tempVersion = Regex.Replace((Trim(Mid(html, FoundList(i), Len(versionFormat & numberFormat)))), "[^0-9.]", "")
                If Not tempVersion = vbNullString Then
                    If Not versionsArray.Contains(tempVersion) Then
                        arraySize += 1
                        versionsArray(arraySize) = Regex.Replace((Trim(Mid(html, FoundList(i), Len(versionFormat & numberFormat)))), "[^0-9.]", "")
                        versionLoc(arraySize) = FoundList(i)
                    End If
                End If
            Next i

            ' Add Versions to List
            ReDim Preserve versionsArray(arraySize)
            ReDim Preserve versionLoc(arraySize)
            Array.Sort(versionsArray, versionLoc)

            ' Show Update Version
            lblVersion.Text = versionFormat.Substring(1) & versionsArray(arraySize)

            If currentVersion = versionsArray(arraySize) Then
                needUpdate = False ' Up to Date
                Exit Function
            End If
        Else
            lblError.Text = "Version not Found" ' Couldn't Find Versions
            needUpdate = False
            Exit Function
        End If

        needUpdate = True ' Need Update
    End Function

    Private Function FindIndexes(ByVal searchWord As String, ByVal src As String, ByVal addLength As Integer) As List(Of Integer)
        ' Find URL Locations
        Dim searchSRC As String = src
        Dim toFind As String = searchWord
        Dim lastIndex As Integer = 0
        Dim listOfIndexes As New List(Of Integer)
        Do Until lastIndex < 0
            lastIndex = searchSRC.IndexOf(toFind, lastIndex + toFind.Length)
            If lastIndex >= 0 Then
                listOfIndexes.Add(lastIndex)
            End If
        Loop
        Return listOfIndexes
    End Function

    Private Function findUpdate(ByVal updateNum As Integer, ByVal count As Integer, ByVal maxDownloads As Integer)
        Dim tempUrl As String

        tempUrl = getDownloadUrl(updateNum)

        setLabelTxt("Preparing to Download Update " & count & " of " & maxDownloads & "...", lblProgress)
        If tempUrl.Contains("dropbox") Then
            ' Needs to be Redone using the API

            ' Check URL end
            If tempUrl = tempUrl.EndsWith("?dl=1") Then
                ' Good to Go
            ElseIf tempUrl = tempUrl.EndsWith("?dl=0") Then
                tempUrl = tempUrl.Substring(0, Len(tempUrl) - 1) & "1" ' Add 1 at the End
            Else
                tempUrl = tempUrl & "?dl=1" ' Add dl at the End
            End If

            ' Progress
            AddHandler myWebClient.DownloadProgressChanged, AddressOf myWebClient_ProgressChanged
            findUpdate = downloadUpdate(tempUrl, versionsArray(updateNum).ToString) ' Downloads Direct

        ElseIf tempUrl.Contains("drive.google") Then
            ' Get the file ID for download
            If tempUrl.Contains("/d/") Then
                tempUrl = tempUrl.Substring(tempUrl.IndexOf("/d/") + 3)
            ElseIf tempUrl.Contains("id=") Then
                tempUrl = tempUrl.Substring(tempUrl.IndexOf("id=") + 3)
            End If
            ' Trim Excess
            If tempUrl.Contains("/") Then tempUrl = tempUrl.Substring(0, tempUrl.IndexOf("/"))

            ' Download
            findUpdate = downloadGoogle(tempUrl, versionsArray(updateNum).ToString)

        ElseIf tempUrl.Contains("mediafire") Then
            ' To Do
            findUpdate = False
        Else
            ' Direct Download
            ' Progress
            AddHandler myWebClient.DownloadProgressChanged, AddressOf myWebClient_ProgressChanged

            ' Download
            findUpdate = downloadUpdate(tempUrl, versionsArray(updateNum).ToString)

        End If

    End Function

    Private Function getDownloadUrl(ByVal updateNum As Integer)
        Dim wb As New WebClient
        Dim html As String = wb.DownloadString(urlPath)
        Dim tempUrl As String
        Dim allowedChars As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~:/?#[]@!$&'()*+,;="

        tempUrl = html.Substring(versionLoc(updateNum)) ' URL Area
        tempUrl = tempUrl.Substring(tempUrl.IndexOf(urlPrefix)) ' URL Start

        For i As Integer = 1 To tempUrl.Length ' Get End of URL
            If Not allowedChars.Contains(tempUrl.Chars(i)) Then
                tempUrl = tempUrl.Substring(0, i)
                Exit For
            End If
        Next

        getDownloadUrl = tempUrl
    End Function

    Private Function downloadUpdate(ByVal downloadUrl As String, ByVal name As String)
        myWebClient.DownloadFile(downloadUrl, tempPath & "\" & name & fileExt)
        downloadUpdate = True
    End Function

    Private Sub bgwUpdater_DoWork(sender As Object, e As ComponentModel.DoWorkEventArgs) Handles bgwUpdater.DoWork
        Dim count As Integer, percentDone As Integer
        Dim tempPercent As Integer = 0
        Dim previousVersion As Boolean, reachedVersion As Boolean, startVersion As Integer

        If Not IO.Directory.Exists(tempPath) Then My.Computer.FileSystem.CreateDirectory(tempPath) ' Create Temp Dir
        If versionsArray.Contains(currentVersion) Then previousVersion = True ' User has a Previous Version

        ' Download Updates
        For i As Integer = 1 To arraySize
            ' Pause
            Do While Not Running
                Thread.Sleep(500)
            Loop
            Do While myWebClient.IsBusy ' Wait for download to finish
                Thread.Sleep(500)
            Loop

            If Not previousVersion Then ' No Previous Verion
                count += 1
                curCount = count
                maxCount = arraySize

                If Not IO.File.Exists(tempPath & "\" & versionsArray(i) & fileExt) Then
                    errorOccurred = Not findUpdate(i, count, arraySize) ' Check for File
                    Exit For ' Error
                End If
            Else
                If reachedVersion Then ' Passed Previous Version
                    count += 1
                    curCount = count
                    maxCount = arraySize - startVersion

                    If Not IO.File.Exists(tempPath & "\" & versionsArray(i) & fileExt) Then
                        errorOccurred = Not findUpdate(i, count, arraySize - startVersion) ' Check for File
                        Exit For ' Error
                    End If
                Else
                    If versionsArray(i) = currentVersion Then reachedVersion = True ' Reached Current Version
                    startVersion = i
                End If
            End If

            percentDone = (count / (arraySize - tempPercent)) * 100
            bgwUpdater.ReportProgress(percentDone)
        Next

        ' Download Error
        If errorOccurred Then
            Call deleteTempFiles()
            bgwUpdater.CancelAsync()
            Exit Sub
        End If

        Dim fileCount As Integer = IO.Directory.GetFiles(tempPath).Count
        maxCount = fileCount
        count = 0
        bgwUpdater.ReportProgress(0)

        ' Extract and Apply Updates
        For Each tempfile In IO.Directory.GetFiles(tempPath)
            ' Pause
            Do While Not Running
                Thread.Sleep(500)
            Loop

            errorOccurred = Not extractFile(tempfile, count + 1, fileCount)

            count += 1
            curCount = count
            percentDone = (count / fileCount) * 100
            bgwUpdater.ReportProgress(percentDone)
        Next

        ' Extraction Error
        If errorOccurred Then
            Call deleteTempFiles()
            bgwUpdater.ReportProgress(percentDone)
            bgwUpdater.CancelAsync()
            Exit Sub
        End If

        setLabelTxt("Finished", lblProgress)
        Call deleteTempFiles()
        bgwUpdater.CancelAsync()
    End Sub

    Private Sub bgwUpdater_ProgressChanged(sender As Object, e As ComponentModel.ProgressChangedEventArgs) Handles bgwUpdater.ProgressChanged
        If errorOccurred Or e.ProgressPercentage = 100 Then btnUpdate.Enabled = 0
        prgbarUpdate.Value = e.ProgressPercentage
    End Sub

    Private Sub myWebClient_ProgressChanged(ByVal sender As Object, ByVal e As DownloadProgressChangedEventArgs)
        Dim downloadedSize As Double = Double.Parse(e.BytesReceived)
        Dim totalSize As Double = Double.Parse(e.TotalBytesToReceive)
        Dim percent As Integer = (downloadedSize / totalSize) * 100

        ' Pause
        Do While Not Running
            Thread.Sleep(500)
        Loop

        ' Show Downloaded Size
        If downloadedSize < 102400 Then
            Dim downloaded As Decimal = Math.Round(downloadedSize / 1024, 1, MidpointRounding.AwayFromZero)
            setLabelTxt("Downloaded " & downloaded & " KB " & curCount & " of " & maxCount, lblProgress)
        Else
            Dim downloaded As Decimal = Math.Round(downloadedSize / 1024000, 1, MidpointRounding.AwayFromZero)
            setLabelTxt("Downloaded " & downloaded & " MB " & curCount & " of " & maxCount, lblProgress)
        End If
        bgwUpdater.ReportProgress(percent)
    End Sub

    Private Sub Download_ProgressChanged(ByVal progress As IDownloadProgress)
        ' Pause
        Do While Not Running
            Thread.Sleep(500)
        Loop

        ' Show Downloaded Size
        If progress.BytesDownloaded < 102400 Then
            Dim downloaded As Decimal = Math.Round(progress.BytesDownloaded / 1024, 1, MidpointRounding.AwayFromZero)
            setLabelTxt("Downloaded " & downloaded & " KB " & curCount & " of " & maxCount, lblProgress)
        Else
            Dim downloaded As Decimal = Math.Round(progress.BytesDownloaded / 1024000, 1, MidpointRounding.AwayFromZero)
            setLabelTxt("Downloaded " & downloaded & " MB " & curCount & " of " & maxCount, lblProgress)
        End If
    End Sub

    Private Sub setLabelTxt(ByVal text As String, ByVal lbl As Label)
        ' Update Label From Bgw
        If lbl.InvokeRequired Then
            lbl.Invoke(New setLabelTxtInvoker(AddressOf setLabelTxt), text, lbl)
        Else
            lbl.Text = text
        End If
    End Sub

    Private Sub authGoogle()
        Dim ClientId = "463653528866-ch102to3gralat01iju1tr8bb5kmst25.apps.googleusercontent.com"
        Dim ClientSecret = "p5M9RLRbaUhlZglmV17Um3cu"
        Dim MyUserCredential As UserCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(New ClientSecrets() With {.ClientId = ClientId, .ClientSecret = ClientSecret}, {DriveService.Scope.Drive}, "user", CancellationToken.None).Result
        Service = New DriveService(New BaseClientService.Initializer() With {.HttpClientInitializer = MyUserCredential, .ApplicationName = "Google Drive VB Dot Net"})
    End Sub

    Private Function downloadGoogle(ByVal downloadID As String, ByVal name As String)
        Dim downloadError As Boolean

        If Not Service.ApplicationName = "Google Drive VB Dot Net" Then authGoogle()

        Dim request = Service.Files.Get(downloadID)

        ' Download Drive File
        Using FileStream = New IO.FileStream(tempPath & "\" & name & fileExt, IO.FileMode.Create, IO.FileAccess.Write)
            ' Pause
            Do While Not Running
                Thread.Sleep(500)
            Loop

            ' Progress Change Handler
            AddHandler request.MediaDownloader.ProgressChanged, AddressOf Download_ProgressChanged

            ' Set Chunk Size and Download
            request.MediaDownloader.ChunkSize = 10240 ' 10 kilobytes
            request.DownloadWithStatus(FileStream)

            ' Check if Download Failed
            If request.DownloadWithStatus(FileStream).Status = DownloadStatus.Failed Then downloadError = True

        End Using

        If downloadError Then
            setLabelTxt("An Error Occurred While Downloading", lblError) ' Update Error Label
            Call deleteTempFiles() ' Delete Files
            downloadGoogle = False
        Else
            downloadGoogle = True
        End If
    End Function

    Private Sub authDropBox()
        ' Might do later, have workaround for now

    End Sub

    Private Sub downloadDropBox(ByVal downloadUrl As String, ByVal name As String)
        ' Might do later, have workaround for now

    End Sub

    Public Function extractFile(ByVal tempFile As String, ByVal count As Integer, ByVal maxFiles As Integer)
        Dim totalSize As Double

        ' Get File Size
        setLabelTxt("Determining File Size " & count & " of " & maxFiles & " " & "...", lblProgress)
        Using extractStream As IO.Stream = IO.File.OpenRead(tempFile)
            Try ' Extract File
                Dim readerSize = Readers.ReaderFactory.Open(extractStream)

                Do While readerSize.MoveToNextEntry()
                    Do While Not Running
                        Thread.Sleep(500)
                    Loop

                    totalSize += readerSize.Entry.Size
                Loop
            Catch ex As Exception
                setLabelTxt("An Error Occurred While Getting File Size", lblError)
                extractFile = False
                Exit Function
            End Try
        End Using

        ' Extract files
        setLabelTxt("Extracting Updates " & count & " of " & maxFiles & " " & "...", lblProgress)
        Using extractStream As IO.Stream = IO.File.OpenRead(tempFile)
            Dim tempSize As Double = 0
            Dim reader = Readers.ReaderFactory.Open(extractStream)
            Dim extractOptions As New Readers.ExtractionOptions
            extractOptions.ExtractFullPath = True
            extractOptions.Overwrite = True

            Do While reader.MoveToNextEntry()
                ' Pause
                Do While Not Running
                    Thread.Sleep(500)
                Loop

                ' Write Extracted File
                Readers.IReaderExtensions.WriteEntryToDirectory(reader, ExtLoc, extractOptions)

                ' Update Progress Bar
                Dim extFile As String = reader.Entry.Key.ToString
                Dim percent As Integer = (tempSize / totalSize) * 100

                setLabelTxt("Extracting Update " & count & " of " & maxFiles & " " & percent & "%..." & Chr(13) & extFile, lblProgress)
                tempSize += reader.Entry.Size
                If tempSize > totalSize Then tempSize = totalSize
                bgwUpdater.ReportProgress(percent)
            Loop

            ' Update Version
            currentVersion = versionsArray((versionsArray.Count - 1) - maxFiles + count)
            setLabelTxt("Local Version: " & currentVersion, lblLocalVersion)
            saveConfig(4, "Version=" & currentVersion)
        End Using

        extractFile = True

    End Function

    Private Sub frmMain_Closing(sender As Object, e As EventArgs) Handles MyBase.Closing
        If bgwUpdater.IsBusy Then bgwUpdater.CancelAsync()
        Call deleteTempFiles()
    End Sub

    Private Sub deleteTempFiles()
        If IO.Directory.Exists(tempPath) Then My.Computer.FileSystem.DeleteDirectory(tempPath, FileIO.DeleteDirectoryOption.DeleteAllContents)
    End Sub
End Class
