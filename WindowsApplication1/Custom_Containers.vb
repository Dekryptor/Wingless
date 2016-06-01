Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports System.ComponentModel.Design



#Region "unfifnished TabControl "
Class FlatTabControl : Inherits TabControl

    Dim x! = 0
    Dim x1! = 0
    Dim t! = 0
    Dim tx! = 0
    Dim a As New Interpolation
    Dim p1, p2 As Bitmap
    Dim tempt% = 0
#Region " Properties"

    Protected Overrides Sub CreateHandle()
        MyBase.CreateHandle()
        Alignment = TabAlignment.Top
    End Sub

#Region " Colors"

    <Category("Colors")> _
    Public Property BaseColor As Color
        Get
            Return _BaseColor
        End Get
        Set(value As Color)
            _BaseColor = value
        End Set
    End Property

    <Category("Colors")> _
    Public Property ActiveColor As Color
        Get
            Return _ActiveColor
        End Get
        Set(value As Color)
            _ActiveColor = value
        End Set
    End Property

#End Region

#End Region

#Region " Colors"

    Private BGColor As Color = Color.FromArgb(50, 50, 50)
    Private _BaseColor As Color = Color.FromArgb(245, 245, 245)
    Private _ActiveColor As Color = Color.FromArgb(0, 170, 220)

#End Region

    Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or _
                 ControlStyles.ResizeRedraw Or ControlStyles.OptimizedDoubleBuffer, True)
        DoubleBuffered = True
        'BackColor = Color.FromArgb(60, 70, 73)

        Font = New Font("Segoe UI", 10)
        SizeMode = TabSizeMode.Fixed
        ItemSize = New Size(120, 40)

    End Sub
    Protected Overrides Sub OnCreateControl()
        MyBase.OnCreateControl()

        Dim c() As Color
        If rescol(Parent.BackColor) = Color.White Then
            c = {Parent.BackColor, col(45)}
        Else
            c = {Parent.BackColor, col(225)}
        End If
        _BaseColor = ThemeControl154.blend(c)
        p1 = New Bitmap(SelectedTab.Height, SelectedTab.Width)
        p2 = New Bitmap(SelectedTab.Height, SelectedTab.Width)
        tabtobitmap(TabPages(0), p1)
    End Sub


    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim tc As Boolean = False
        If t < 150 Then
            t += 1
            x = a.GetValue(tx, GetTabRect(SelectedIndex).X, t, 150, Interpolation.Type.SmoothStep, 2)
            x1 = a.GetValue(GetTabRect(SelectedIndex).X, tx, t, 150, Interpolation.Type.Smootherstep, 1)
        Else
            Me.AccessibleDescription = "adsw"
            For Each c As Control In SelectedTab.Controls
                c.Show()
            Next
        End If

        Dim B = New Bitmap(Width, Height) : Dim G = Graphics.FromImage(B)
        With G
            .SmoothingMode = 2
            .TextRenderingHint = 5
            .Clear(Parent.BackColor)

            .DrawRectangle(New Pen(col(40, rescol(Parent.BackColor))), 3, 3, Width - 7, Height - 7)

            Try : SelectedTab.BackColor = _BaseColor : Catch : End Try

            For i = 0 To TabCount - 1
				Dim Base As New Rectangle(New Point(GetTabRect(i).Location.X + 2, GetTabRect(i).Location.Y), New Size(GetTabRect(i).Width, GetTabRect(i).Height))
				Dim BaseSize As New Rectangle(Base.Location.X + 3, Base.Location.Y + 3, Base.Width, Base.Height)


				If i = SelectedIndex Then

					.FillRectangle(New SolidBrush(Invert(ActiveColor)), New Rectangle(x + 4, BaseSize.Height - 3 + BaseSize.Y, BaseSize.Width, 3))

                    '-- Text
                    .DrawString(TabPages(i).Text, Font, New SolidBrush(ActiveColor), BaseSize, CenterSF)

                Else

                    '-- Text
                    .DrawString(TabPages(i).Text, Font, New SolidBrush(rescol(Parent.BackColor)), BaseSize, CenterSF)
                End If
            Next
        End With
        tabtobitmap(SelectedTab, p1)

        If t > 0 And t < 150 Then
            tabtobitmap(SelectedTab, p2)
            SelectedTab.CreateGraphics.DrawImageUnscaled(p1, x1, 0)
            SelectedTab.CreateGraphics.DrawImageUnscaled(p2, SelectedTab.Width - x1 + 1, 0)
            For Each c As Control In SelectedTab.Controls
                c.Hide()
            Next
        End If

        MyBase.OnPaint(e)
        G.Dispose()
        e.Graphics.InterpolationMode = 7
        e.Graphics.DrawImageUnscaled(B, 0, 0)
        B.Dispose()

    End Sub


    Protected Overrides Sub OnSelecting(e As System.Windows.Forms.TabControlCancelEventArgs)
        MyBase.OnSelecting(e)
        tx = x
        t = 0
        If Not DesignMode Then Me.AccessibleDescription = "Animated Control"



    End Sub

    Sub tabtobitmap(c As Control, b As Bitmap)
        c.DrawToBitmap(b, c.DisplayRectangle)
    End Sub

End Class
#End Region



