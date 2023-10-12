Public Class clsCGItemplate
  ' lädt die Zeilen aus der angegebenen Datei in das globale
  ' Array und dimensioniert dieses entsprechend
  Property  templateString As string = ""
  Private cgiOBJ As clsCGI4VBNET
  Public File$ = ""
  Public Path$ = ""
  Public enc As System.Text.Encoding
  Public Sub New(ByRef cgiObj1 As clsCGI4VBNET)
    Try
      cgiOBJ = cgiObj1
    Catch e As Exception
    End Try
  End Sub
  Private Sub korrekturTemplatepath()
    Dim datei$ = Path & "\" & File
        '  My.Application.Log.WriteEntry("templatefilekorrektur, vorher: " & datei$)
    If Not IO.File.Exists(datei$) Then
      Path = Path & "\data\"
    End If
  End Sub
  Public Function loadTemplateFile() As String
    Try
      korrekturTemplatepath()
      If System.IO.File.Exists(IO.Path.Combine(Path$, File$)) Then
        Using sr As New IO.StreamReader(IO.Path.Combine(Path$, File$), cgiOBJ.enc)
          templateString$ = sr.ReadToEnd()
        End Using
      Else
        cgiOBJ.Send("Fehler: Templatefile fehlt!")
        cgiOBJ.Send(Path & File$)
        Return "ERROR: File not found: path:" & Path$ & ", File: " & File$
      End If
      Return "ok"
    Catch e As Exception
      Return GetFehlerHinweis(e)
    End Try
  End Function

  Private Shared Function GetFehlerHinweis$(ByVal e As Exception)
    Dim FehlerHinweis$
    FehlerHinweis$ = "loadTemplateFile, Fehler: " & vbCrLf + _
     e.Message + " " & vbCrLf + _
     e.StackTrace + " " & vbCrLf + _
     e.Source + " "
    Return FehlerHinweis
  End Function
  Public Function replace(ByVal varName As String, ByVal xValue As String) As String
    Try
      If IsNothing(xValue) Then xValue = " "

      templateString$ = templateString$.Replace(varName, xValue)
      Return "ok"
    Catch e As Exception
      Return GetFehlerHinweis(e)
    End Try
  End Function
  Public Function SendTemplateFileneu() As String
    Try
      cgiOBJ.Send("Content-type: text/html; charset=ISO-8859-1" & vbCrLf)
      Console.WriteLine(templateString$)
      Return "ok"
    Catch e As Exception
      Return GetFehlerHinweis(e)
    End Try
  End Function
  Public Function SendTemplateFile() As String
    Try
      Console.WriteLine(templateString$)
      Return "ok"
    Catch e As Exception
      Return GetFehlerHinweis(e)
    End Try
  End Function
  Public Function writeTemplateFile(ByRef filename$) As String
    Try
      Console.WriteLine(templateString$)
      My.Computer.FileSystem.WriteAllText(filename$, templateString$, False, cgiOBJ.enc)
      Return "ok"
    Catch e As Exception
      Return GetFehlerHinweis(e)
    End Try
  End Function
End Class
