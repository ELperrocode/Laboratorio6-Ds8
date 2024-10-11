Imports Devart.Data.MySql
' Ruben Rivera | Michael Aparicio | Henry Maldonado

'/*
'-- Crear la base de datos si no existe
'CREATE DATABASE IF NOT EXISTS libreria;
'
'-- Seleccionar la base de datos
'USE libreria;
'
'-- Crear la tabla de libros
'CREATE TABLE libros (
'    id_libro INT AUTO_INCREMENT PRIMARY KEY, 
'    titulo VARCHAR(255) NOT NULL,              
'    autor VARCHAR(255) NOT NULL,               
'    editorial VARCHAR(255),                   
'    fecha_publicacion DATE,                     
'    genero VARCHAR(100),                        
'    precio DECIMAL(10, 2),                    
'    cantidad_disponible INT DEFAULT 0,         
'    isbn VARCHAR(20),                         
'    descripcion TEXT,                          
'    fecha_ingreso TIMESTAMP DEFAULT CURRENT_TIMESTAMP  
');*/

Public Class Form1
    ' Instancia de la clase ConexionMySQL
    Private dbHelper As New DatabaseHelper()

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadBooks()
    End Sub
    Private Sub LoadBooks()
        Dim books As DataTable = dbHelper.GetAllBooks()
        DataGridViewBooks.DataSource = books

        ' Deshabilitar la opción de agregar filas directamente en el DataGridView
        DataGridViewBooks.AllowUserToAddRows = False
        ' Asumiendo que la columna "id" es la primera columna en el DataGridView
        DataGridViewBooks.Columns("id_libro").ReadOnly = True
        DataGridViewBooks.Columns("fecha_ingreso").ReadOnly = True


        ' Agregar evento para editar
        AddHandler DataGridViewBooks.CellValueChanged, AddressOf dataGridViewBooks_CellValueChanged
    End Sub




    Private Sub dataGridViewBooks_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
            Dim updatedRow As DataGridViewRow = DataGridViewBooks.Rows(e.RowIndex)
            Dim id_libro As Integer = Convert.ToInt32(updatedRow.Cells("id_libro").Value)

            ' Solo actualiza si hay un valor en el título
            If Not String.IsNullOrEmpty(updatedRow.Cells("titulo").Value?.ToString()) Then
                dbHelper.UpdateBook(id_libro,
                                updatedRow.Cells("titulo").Value.ToString(),
                                updatedRow.Cells("autor").Value.ToString(),
                                updatedRow.Cells("editorial").Value.ToString(),
                                DateTime.Parse(updatedRow.Cells("fecha_publicacion").Value.ToString()),
                                updatedRow.Cells("genero").Value.ToString(),
                                Decimal.Parse(updatedRow.Cells("precio").Value.ToString()),
                                Integer.Parse(updatedRow.Cells("cantidad_disponible").Value.ToString()),
                                updatedRow.Cells("isbn").Value.ToString(),
                                updatedRow.Cells("descripcion").Value.ToString())
            End If
        End If
    End Sub

    Private Sub btnDeleteBook_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If DataGridViewBooks.SelectedRows.Count > 0 Then
            Dim selectedRow As DataGridViewRow = DataGridViewBooks.SelectedRows(0)
            Dim id_libro As Integer = Convert.ToInt32(selectedRow.Cells("id_libro").Value)

            ' Mostrar mensaje de confirmación
            Dim result As DialogResult = MessageBox.Show("¿Estás seguro de que deseas eliminar este libro?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

            If result = DialogResult.Yes Then
                ' Elimina el libro de la base de datos
                dbHelper.DeleteBook(id_libro)

                ' Recarga los libros
                LoadBooks()
            End If
        Else
            MessageBox.Show("Por favor, selecciona toda la fila del libro para eliminar.")
        End If
    End Sub




    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        ' Validar que todos los campos estén llenos antes de proceder
        If String.IsNullOrEmpty(txtTitle.Text) OrElse String.IsNullOrEmpty(txtAuthor.Text) OrElse String.IsNullOrEmpty(txtPublisher.Text) OrElse
        String.IsNullOrEmpty(txtGenre.Text) OrElse String.IsNullOrEmpty(txtPrice.Text) OrElse String.IsNullOrEmpty(txtQuantity.Text) OrElse
        String.IsNullOrEmpty(txtISBN.Text) OrElse String.IsNullOrEmpty(txtDescription.Text) Then
            MessageBox.Show("Por favor, completa todos los campos.")
            Return
        End If

        Try
            ' Convertir y validar datos
            Dim price As Decimal = Decimal.Parse(txtPrice.Text)
            Dim quantity As Integer = Integer.Parse(txtQuantity.Text)
            Dim publicationDate As DateTime = dtpPublicationDate.Value

            ' Agregar el libro a la base de datos
            dbHelper.AddBook(txtTitle.Text, txtAuthor.Text, txtPublisher.Text, publicationDate, txtGenre.Text, price, quantity, txtISBN.Text, txtDescription.Text)

            ' Recargar los libros en el DataGridView
            LoadBooks()

            ' Limpiar los campos después de agregar el libro
            txtTitle.Clear()
            txtAuthor.Clear()
            txtPublisher.Clear()
            txtGenre.Clear()
            txtPrice.Clear()
            txtQuantity.Clear()
            txtISBN.Clear()
            txtDescription.Clear()
            dtpPublicationDate.Value = DateTime.Now

            ' Mostrar un mensaje de confirmación
            MessageBox.Show("Libro agregado exitosamente.")

        Catch ex As Exception
            ' Manejar cualquier error durante el proceso de inserción   
            MessageBox.Show("Ocurrió un error al agregar el libro: " & ex.Message)
        End Try
    End Sub


    'Validaciones 

    Private Sub DataGridView1_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs) Handles DataGridViewBooks.EditingControlShowing
        Dim columnIndex As Integer = DataGridViewBooks.CurrentCell.ColumnIndex

        ' Verificar si la columna es "cantidad"
        If DataGridViewBooks.Columns(columnIndex).Name = "cantidad_disponible" Then
            ' Eliminar cualquier controlador previo para evitar duplicados
            RemoveHandler e.Control.KeyPress, AddressOf Integer_KeyPress

            ' Asignar el nuevo controlador KeyPress para verificar solo enteros
            AddHandler e.Control.KeyPress, AddressOf Integer_KeyPress
        ElseIf DataGridViewBooks.Columns(columnIndex).Name = "precio" Then
            ' Para la columna precio, usar el validador de números decimales
            RemoveHandler e.Control.KeyPress, AddressOf Numeric_KeyPress
            AddHandler e.Control.KeyPress, AddressOf Numeric_KeyPress
        Else
            ' Si no es una columna de números, eliminar el controlador si existía
            RemoveHandler e.Control.KeyPress, AddressOf Integer_KeyPress
            RemoveHandler e.Control.KeyPress, AddressOf Numeric_KeyPress
        End If
    End Sub


    Private Sub Numeric_KeyPress(sender As Object, e As KeyPressEventArgs)
        ' Permitir solo números, el punto (.) para decimales, y controlar la tecla Backspace
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) AndAlso (e.KeyChar <> ".") Then
            e.Handled = True
        End If

        ' Permitir solo un punto decimal
        Dim textBox As TextBox = CType(sender, TextBox)
        If (e.KeyChar = ".") AndAlso textBox.Text.IndexOf(".") > -1 Then
            e.Handled = True
        End If
    End Sub

    Private Sub Integer_KeyPress(sender As Object, e As KeyPressEventArgs)
        ' Permitir solo números y la tecla Backspace
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub DataGridView1_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles DataGridViewBooks.CellValidating
        ' Verificar si se está editando la columna de fecha de publicación
        If DataGridViewBooks.Columns(e.ColumnIndex).Name = "fecha_publicacion" Then
            Dim newDate As String = e.FormattedValue.ToString()
            Dim parsedDate As DateTime

            ' Intentar parsear la fecha
            If Not DateTime.TryParse(newDate, parsedDate) Then
                MessageBox.Show("Por favor, ingresa una fecha válida.")
                e.Cancel = True ' Cancelar la edición si la fecha no es válida
            End If
        End If
    End Sub

    Private Sub txtQuantity_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtQuantity.KeyPress
        ' Permitir solo números y la tecla Backspace
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub txtPrice_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtPrice.KeyPress
        ' Permitir solo números, el punto (.) para decimales, y controlar la tecla Backspace
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) AndAlso (e.KeyChar <> ".") Then
            e.Handled = True
        End If

        ' Permitir solo un punto decimal
        Dim textBox As TextBox = CType(sender, TextBox)
        If (e.KeyChar = ".") AndAlso textBox.Text.IndexOf(".") > -1 Then
            e.Handled = True
        End If
    End Sub

End Class
