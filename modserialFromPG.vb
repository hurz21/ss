
Imports Npgsql

Module modserialFromPG
    Public myconn As NpgsqlConnection

    Private Property dt As DataTable

    Private Sub makeConnection(ByVal host As String, datenbank As String, ByVal dbuser As String, ByVal dbpw As String, ByVal dbport As String)
        Dim csb As New NpgsqlConnectionStringBuilder
        Try
            l("makeConnection")
            'If String.IsNullOrEmpty(mydb.ServiceName) Then
            'klassisch
            csb.Host = host
            ' csb. = mydb.Schema
            csb.UserName = dbuser
            csb.Password = dbpw
            csb.Database = datenbank

            csb.Port = CInt(dbport)
            csb.Pooling = False
            'csb.Protocol = 3'ProtocolVersion.Version3
            csb.MinPoolSize = 1
            csb.MaxPoolSize = 20
            'csb.Encoding =
            csb.Timeout = 15
            csb.SslMode = SslMode.Disable

            ' "Protocol=3;SSL=false;Pooling=true;MinPoolSize=1;MaxPoolSize=20;Encoding=UNICODE;Timeout=15;SslMode=Disable"
            myconn = New NpgsqlConnection(csb.ConnectionString)
            l("makeConnection fertig")
        Catch ex As Exception
            l("fehler in makeConnection" & ex.ToString)
        End Try
    End Sub
    Function getSerialFromPostgis(host As String, datenbank As String, schema As String, tabelle As String, dbuser As String, dbpw As String,
                                      dbport As String, fs As String, ByRef serial As String()) As Boolean
        l("getSerialFromPostgis1")
        makeConnection(host, datenbank, dbuser, dbpw, dbport)
        l("getSerialFromPostgis2")
        Try
            myconn.Open()
            'Dim SQL As String = "SELECT gid FROM " & schema & "." & tabelle & " where fs='" & fs & "'"
            Dim SQL As String = "SELECt ST_AsText(geom)  FROM  " & schema & "." & tabelle &
                " where gid in (SELECT gid FROM   " & schema & "." & tabelle & " where fs='" & fs & "' order by gisarea desc)"

            l(SQL)
            Dim com As New NpgsqlCommand(SQL, myconn)
            Dim da As New NpgsqlDataAdapter(com)
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey
            dt = New DataTable
            Dim _mycount = da.Fill(dt)
            ReDim serial(_mycount - 1)
            For i = 0 To dt.Rows.Count - 1
                serial(i) = CStr(dt.Rows(i).Item(i))
                l(i & " " & serial(i))
            Next
            'serial = CStr(dt.Rows(0).Item(0))
            myconn.Close()
            myconn.Dispose()
            com.Dispose()
            da.Dispose()
            l("getSerialFromPostgis fertig")
            Return True

        Catch ex As Exception
            l("fehler in getSerialFromPostgis: " & ex.ToString)
            Return False
        End Try
    End Function
    Public Function getAreaFromPostgis(host As String, datenbank As String, schema As String, tabelle As String, dbuser As String, dbpw As String, dbport As String, p8 As String) As Double 
 l("getAreaFromPostgis")
        Dim area As double
        makeConnection(host, datenbank, dbuser, dbpw, dbport)
        l("getAreaFromPostgis")
        Try
            myconn.Open()
            'Dim SQL As String = "SELECT gid FROM " & schema & "." & tabelle & " where fs='" & fs & "'"
            Dim SQL As String = "SELECt st_Area(geom)  FROM  " & schema & "." & tabelle &
                " where gid=(SELECT gid FROM   " & schema & "." & tabelle & " where fs='" & fs & "' order by gisarea desc limit 1) "
            l(SQL)
            Dim com As New NpgsqlCommand(SQL, myconn)
            Dim da As New NpgsqlDataAdapter(com)
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey
            dt = New DataTable
            Dim _mycount = da.Fill(dt)
            area = cdbl(dt.Rows(0).Item(0))
            myconn.Close()
            myconn.Dispose()
            com.Dispose()
            da.Dispose()
            l("getAreaFromPostgis fertig")
            Return area

        Catch ex As Exception
            l("fehler in getAreaFromPostgis" & ex.ToString)
            Return 0
        End Try
 End Function 

End Module
