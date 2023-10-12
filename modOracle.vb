Imports System.Data.SqlClient
Imports System.Data
Module modOracle
    Function insert_raumbezug2polygon(wkt As String) As Integer
        l("in insert_raumbezug2polygon -------------------------------------")
        Dim myParadigmaconn As New SqlConnection
        Dim com As SqlCommand
        Dim SQLupdate$
        Dim newid&
 
        l("in insert_raumbezug2polygon ---- 1")
        Dim myParadigmabuild As New SqlConnectionStringBuilder
        With myParadigmabuild
            .DataSource = "kh-w-sql02"
            .InitialCatalog = "Paradigma"
            .UserID = "sgis"
            .Password = "WinterErschranzt.74"
        End With


        myParadigmaconn.ConnectionString = myParadigmabuild.ConnectionString
        ''.Host = "kh-w-sql02"
        '.Schema = "Paradigma"
        '.Tabelle = "paradigma.kreis-of.local"

        '.UserID = "sgis"
        '.Password = "WinterErschranzt.74"
        '.dbtyp = "sqls"
        'myoracle = New SqlConnection("Data Source=(DESCRIPTION=" &
        '                    "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" & host & ")(PORT=1521)))" &
        '                    "(LOAD_BALANCE=yes)(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" & ServiceName & ")));" &
        '                    "User Id=" & dbuser & ";Password=" & dbpw & ";")


        l("in insert_raumbezug2polygon ---- vor open aaa")
        l("in insert_raumbezug2polygon ---- myoracle " & myParadigmaconn.ConnectionString)
        Try
            myParadigmaconn.Open()
            l("in doDatenbank ---- nach open")
            SQLupdate = String.Format("INSERT INTO {0} (RAUMBEZUGSID,VORGANGSID,TYP,AREAQM,SERIALSHAPE) " +
                  " VALUES (@RAUMBEZUGSID,@VORGANGSID,@TYP,@AREAQM,@SERIALSHAPE)",
                   "RAUMBEZUG2GEOPOLYGON")

            SQLupdate$ = SQLupdate$ & ";SELECT CAST(scope_identity() AS int);"
            com = New SqlCommand(SQLupdate$, myParadigmaconn)

            com.Parameters.AddWithValue("@RAUMBEZUGSID", rid)
            com.Parameters.AddWithValue("@VORGANGSID", vid)
            com.Parameters.AddWithValue("@SERIALSHAPE", wkt)
            com.Parameters.AddWithValue("@TYP", 2)
            com.Parameters.AddWithValue("@AREAQM", area)


            If String.IsNullOrEmpty(SQLupdate) Then
                '  nachricht("Fehler in GetNewid&: SQLstring ist leer!!!")
                Return -3
            End If

            com.CommandText = SQLupdate
            com.CommandType = CommandType.Text
            'Dim p_theid As New SqlParameter

            'p_theid.DbType = DbType.Decimal
            'p_theid.Direction = ParameterDirection.ReturnValue
            'p_theid.ParameterName = ":R1"
            'com.Parameters.Add(p_theid)
            Dim kobjssss = com.ExecuteScalar
            If kobjssss Is Nothing Then
                newid = 0
            Else
                newid = CLng(kobjssss.ToString)
            End If

            myParadigmaconn.Close()
            myParadigmaconn.Dispose()
            l("newid " & newid)
            Return CInt(newid)
        Catch oex As SqlException
            l("Fehler in GetNewid&:" & oex.ToString & " / " & SQLupdate$)
            Return -1
        Catch ex As Exception
            l("Fehler in GetNewid&:" & ex.ToString & " / " & SQLupdate)
            Return -2
        End Try
        myParadigmaconn.Close()

    End Function
End Module
