Imports Devart.Data.MySql

' Ruben Rivera | Michael Aparicio | Henry Maldonado

Public Class DatabaseHelper
    Private connectionString As String = "Server=localhost;Database=libreria;User Id=root;Password=1234;CharSet=utf8mb4;"
    'Modificar dependiendo de configuaracion , usar MySql

    ' Método para obtener la conexión
    Private Function GetConnection() As MySqlConnection
        Return New MySqlConnection(connectionString)
    End Function

    ' Método para obtener todos los libros
    Public Function GetAllBooks() As DataTable
        Dim dt As New DataTable()
        Using conn As MySqlConnection = GetConnection()
            conn.Open()
            Dim cmd As New MySqlCommand("SELECT * FROM libros", conn)
            Dim adapter As New MySqlDataAdapter(cmd)
            adapter.Fill(dt)
        End Using
        Return dt
    End Function

    ' Método para agregar un libro
    Public Sub AddBook(titulo As String, autor As String, editorial As String, fecha_publicacion As Date, genero As String, precio As Decimal, cantidad_disponible As Integer, isbn As String, descripcion As String)
        Using conn As MySqlConnection = GetConnection()
            conn.Open()
            Dim cmd As New MySqlCommand("INSERT INTO libros (titulo, autor, editorial, fecha_publicacion, genero, precio, cantidad_disponible, isbn, descripcion) VALUES (@titulo, @autor, @editorial, @fecha_publicacion, @genero, @precio, @cantidad_disponible, @isbn, @descripcion)", conn)
            cmd.Parameters.AddWithValue("@titulo", titulo)
            cmd.Parameters.AddWithValue("@autor", autor)
            cmd.Parameters.AddWithValue("@editorial", editorial)
            cmd.Parameters.AddWithValue("@fecha_publicacion", fecha_publicacion)
            cmd.Parameters.AddWithValue("@genero", genero)
            cmd.Parameters.AddWithValue("@precio", precio)
            cmd.Parameters.AddWithValue("@cantidad_disponible", cantidad_disponible)
            cmd.Parameters.AddWithValue("@isbn", isbn)
            cmd.Parameters.AddWithValue("@descripcion", descripcion)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ' Método para actualizar un libro
    Public Sub UpdateBook(id_libro As Integer, titulo As String, autor As String, editorial As String, fecha_publicacion As Date, genero As String, precio As Decimal, cantidad_disponible As Integer, isbn As String, descripcion As String)
        Using conn As MySqlConnection = GetConnection()
            conn.Open()
            Dim cmd As New MySqlCommand("UPDATE libros SET titulo=@titulo, autor=@autor, editorial=@editorial, fecha_publicacion=@fecha_publicacion, genero=@genero, precio=@precio, cantidad_disponible=@cantidad_disponible, isbn=@isbn, descripcion=@descripcion WHERE id_libro=@id_libro", conn)
            cmd.Parameters.AddWithValue("@id_libro", id_libro)
            cmd.Parameters.AddWithValue("@titulo", titulo)
            cmd.Parameters.AddWithValue("@autor", autor)
            cmd.Parameters.AddWithValue("@editorial", editorial)
            cmd.Parameters.AddWithValue("@fecha_publicacion", fecha_publicacion)
            cmd.Parameters.AddWithValue("@genero", genero)
            cmd.Parameters.AddWithValue("@precio", precio)
            cmd.Parameters.AddWithValue("@cantidad_disponible", cantidad_disponible)
            cmd.Parameters.AddWithValue("@isbn", isbn)
            cmd.Parameters.AddWithValue("@descripcion", descripcion)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    ' Método para eliminar un libro
    Public Sub DeleteBook(id_libro As Integer)
        Using conn As MySqlConnection = GetConnection()
            conn.Open()
            Dim cmd As New MySqlCommand("DELETE FROM libros WHERE id_libro=@id_libro", conn)
            cmd.Parameters.AddWithValue("@id_libro", id_libro)
            cmd.ExecuteNonQuery()
        End Using
    End Sub
End Class
