

Module Module1
    Public Property area As Double
    Property mycgi As clsCGI4VBNET
    Property enc As Text.Encoding
    Property vid As String
    Property rid As String
    Property serial As String()
    Property fs As String
    Property gemcode As String
    Property FsPositionInShapeFile As String = "1"
    Property username As String
    Property postgis As String
    Public host, datenbank, schema, tabelle, dbuser, dbpw, dbport As String
    Dim erfolg As Boolean = False
    Private Property isDebugmode As Boolean = False
    'http://w2gis02.kreis-of.local/cgi-bin/apps/paradigmaex/serialserver/pg/serialserver.cgi?user=feij&vid=27063&rid=36592&gemcode=729&FS=FS0607290050049100000&postgis=1
    'http://w2gis02.kreis-of.local/cgi-bin/apps/paradigmaex/serialserver/pg/serialserver.cgi?user=feinen_j&vid=9609&rid=57326&gemcode=729&FS=FS0607290050049000000&postgis=1
    Sub Main()
        mycgi = New clsCGI4VBNET("dr.j.feinen@kreis-offenbach.de")
#If DEBUG Then
        isDebugmode = True
#Else
        isDebugmode = False
#End If

        getCgiParams(isDebugmode)
        protokoll(isDebugmode)
        showCgiParams()
        enc = System.Text.Encoding.GetEncoding(("iso-8859-2"))
        gemcode = "729"
        If isDebugmode Then
            vid = "9609"
            rid = "57326"
            postgis = "1"
            fs = "FS0607290050049000000"
            '  serial = "opopo"
        End If
        If Not eingabeist_ok() Then
            mycgi.SendHeader("Eingaben unvollständig")
            mycgi.Send("Eingaben unvollständig")
            End
        End If
        l("pg erkannt")

        host = "w2gis02" : datenbank = "postgis20" : schema = "flurkarte" : tabelle = "basis_f" : dbuser = "postgres" : dbpw = "lkof4" : dbport = "5432"
        erfolg = getSerialFromPostgis(host, datenbank, schema, tabelle, dbuser, dbpw, dbport,
                                                 fs, serial)
        area = getAreaFromPostgis(host, datenbank, schema, tabelle, dbuser, dbpw, dbport,
                                         fs)
        l("area=" & area)
        l("serial.Count=" & serial.Count)
        If erfolg Then
            For i = 0 To serial.Count - 1
                l("ausgabe " & i & " " & serial(i))
                insert_raumbezug2polygon(serial(i))
            Next

            l("erfolgreiches ende")
        End If
        l("  ende")
        mycgi.SendHeader("ok")
        mycgi.Send(serial(0))
    End Sub

    Private Function eingabeist_ok() As Boolean
        l("eingabeist_ok-------------------")
        Try
            If CInt(vid) < 1 Or CInt(rid) < 1 Then
                l("Fehler :vid) < 1 Or CInt(rid) < 1")
                Return False
            End If
            If String.IsNullOrEmpty(fs) Then
                l("Fehler :fs " & fs)
                Return False
            End If
            If String.IsNullOrEmpty(gemcode) Then
                l("Fehler :gemcode " & gemcode)
                '    Return False
            End If
            Return True
        Catch ex As Exception
            l("Fehler ineingabeist_ok : " & ex.ToString)
            Return False
        End Try
    End Function

    Private Sub protokoll(isDebugmode As Boolean)
        With My.Application.Log.DefaultFileLogWriter
            '.CustomLocation = "d:\websys\protokoll\mapshare_prequel.log"
            If Not isDebugmode Then
                .CustomLocation = "d:\websys\" & "protokoll"
            Else
                .CustomLocation = "c:\" & "protokoll"
            End If
            .BaseFileName = "serialserver_" & username & "_" & vid & "_" ' & rid
            '  .Location = Logging.LogFileLocation.ExecutableDirectory
            .AutoFlush = True
            .Append = False
        End With
        l("protokoll now: " & Now)
    End Sub

    Private Sub showCgiParams()
        l("-----------------showCgiParams ---------------------- ")
        l("username: " & username)
        l("rid: " & rid)
        l("vid: " & vid)
        l("gemcode: " & gemcode)
        l("fs: " & fs)
        l("postgis: " & postgis)
        l("---------------- showCgiParams ende ")
    End Sub

    Private Sub getCgiParams(istdebugmode As Boolean)
        l("getCgiParams -------------------------" & istdebugmode)
        Try
            If istdebugmode Then
                'username = "feinen_j"
                'vid = "22535"
                'rid = "26929"
                'fs = "FS0607280020000100700" 'der dateiname kann nicht über cgi geleitet werden. funzt nicht
                'gemcode = "728"
                '        rbtyp   fst = 2
            Else
                username = mycgi.GetCgiValue("user")
                vid = (mycgi.GetCgiValue("vid"))
                rid = (mycgi.GetCgiValue("rid"))
                gemcode = (mycgi.GetCgiValue("gemcode"))
                fs = (mycgi.GetCgiValue("fs"))
                postgis = (mycgi.GetCgiValue("postgis"))
            End If
        Catch ex As Exception
            l("fehler in getCgiParams: " & ex.ToString)
        End Try
    End Sub
    Sub l(t As String)
        My.Application.Log.WriteEntry(t)
    End Sub

    Sub nachricht(t As String)
        My.Application.Log.WriteEntry(t)
    End Sub



End Module
