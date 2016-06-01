Imports System.Drawing.Drawing2D
Imports System.ComponentModel


'<DefaultEvent("TextChanged")> Class customTextBox
'#Region "declare"
'    Inherits ThemeControl154
'    Public voff As Double = 0
'    Public animate As Boolean = False
'    Public Base As TextBox
'    Public C1 As Color
'    Public P1 As Pen
'    Public grb As LinearGradientBrush
'#Region "prop"
'    Public _TextAlign As HorizontalAlignment = HorizontalAlignment.Left
'    Property TextAlign() As HorizontalAlignment
'        Get
'            Return _TextAlign
'        End Get
'        Set(ByVal value As HorizontalAlignment)
'            _TextAlign = value
'            If Base IsNot Nothing Then
'                Base.TextAlign = value
'            End If
'        End Set
'    End Property
'    Public _MaxLength As Integer = 32767
'    Property MaxLength() As Integer
'        Get
'            Return _MaxLength
'        End Get
'        Set(ByVal value As Integer)
'            _MaxLength = value
'            If Base IsNot Nothing Then
'                Base.MaxLength = value
'            End If
'        End Set
'    End Property
'    Public _ReadOnly As Boolean
'    Property [ReadOnly]() As Boolean
'        Get
'            Return _ReadOnly
'        End Get
'        Set(ByVal value As Boolean)
'            _ReadOnly = value
'            If Base IsNot Nothing Then
'                Base.ReadOnly = value
'            End If
'        End Set
'    End Property
'    Public _UseSystemPasswordChar As Boolean
'    Property UseSystemPasswordChar() As Boolean
'        Get
'            Return _UseSystemPasswordChar
'        End Get
'        Set(ByVal value As Boolean)
'            _UseSystemPasswordChar = value
'            If Base IsNot Nothing Then
'                Base.UseSystemPasswordChar = value
'            End If
'        End Set
'    End Property
'    Public _Multiline As Boolean
'    Property Multiline() As Boolean
'        Get
'            Return _Multiline
'        End Get
'        Set(ByVal value As Boolean)
'            _Multiline = value
'            If Base IsNot Nothing Then
'                Base.Multiline = value

'                If value Then
'                    LockHeight = 0
'                    Base.Height = Height - 11
'                Else
'                    LockHeight = Base.Height + 11
'                End If
'            End If
'        End Set
'    End Property
'    Overrides Property Text As String
'        Get
'            Return MyBase.Text
'        End Get
'        Set(ByVal value As String)
'            MyBase.Text = value
'            If Base IsNot Nothing Then
'                Base.Text = value
'            End If
'        End Set
'    End Property
'    Overrides Property Font As Font
'        Get
'            Return MyBase.Font
'        End Get
'        Set(ByVal value As Font)
'            MyBase.Font = value
'            If Base IsNot Nothing Then
'                Base.Font = value
'                Base.Location = New Point(3, 5)
'                Base.Width = Width - 6

'                If Not _Multiline Then
'                    LockHeight = Base.Height + 11
'                End If
'            End If
'        End Set
'    End Property

'#End Region

'    Protected Overrides Sub OnCreation()
'        If Not Controls.Contains(Base) Then
'            Controls.Add(Base)
'            Base.Left = 5 : Base.Top = 5
'            Base.Width = Width - 15
'        End If
'    End Sub
'#End Region
'#Region "funct"
'    Public Sub OnBaseTextChanged(ByVal s As Object, ByVal e As EventArgs)
'        Text = Base.Text
'    End Sub
'    Public Sub OnBaseKeyDown(ByVal s As Object, ByVal e As KeyEventArgs)
'        If e.Control AndAlso e.KeyCode = Keys.A Then
'            Base.SelectAll()
'            e.SuppressKeyPress = True
'        End If
'    End Sub
'    Protected Overrides Sub OnResize(ByVal e As EventArgs)
'        Base.Location = New Point(5, 5)
'        Base.Width = Width - 15


'        If _Multiline Then
'            Base.Height = Height - 11
'        End If


'        MyBase.OnResize(e)
'    End Sub



'    Protected Overloads Sub OnMouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseMove
'        animate = True
'    End Sub
'    Protected Overloads Sub OnMouseLeave(sender As Object, e As EventArgs) Handles MyBase.MouseLeave
'        animate = False
'    End Sub
'    Protected Overloads Sub OnGotFocus(sender As Object, e As EventArgs) Handles MyBase.GotFocus
'        animate = True
'    End Sub
'    Protected Overloads Sub OnlostFocus(sender As Object, e As EventArgs) Handles MyBase.LostFocus
'        animate = False
'    End Sub

'    Protected Overrides Sub OnForeColorChanged(e As EventArgs)
'        MyBase.OnForeColorChanged(e)
'        Base.ForeColor = ForeColor
'    End Sub

'#End Region

'    Public Sub ori()
'        G.InterpolationMode = InterpolationMode.HighQualityBicubic
'        G.SmoothingMode = SmoothingMode.AntiAlias

'        G.Clear(BackColor)


'        G.FillRectangle(New SolidBrush(P1.Color), Width - 10, 0, 10, Height)


'        Dim p As Pen = New Pen(C1, 5)
'        G.SetClip(New Rectangle(Width - 10, 0, 10, Height))
'        For i = -20 To Height + 20 Step 10
'            p = New Pen(BackColor, 5)
'            G.DrawLine(p, Width, CInt(i + voff), Width - 20, CInt(i + 15 + voff))
'        Next
'        p = New Pen(Color.FromArgb(75, 0, 0, 0))
'        G.DrawLine(p, Width - 10, 0, Width - 10, Height)

'        G.ResetClip()


'        DrawBorders(P1)

'        p.Dispose()
'    End Sub

'    Public Sub soft()

'    End Sub


'    Protected Overrides Sub PaintHook()
'        If animate Then voff += 0.5
'        If voff > 9 Then voff = 0
'        ori()
'    End Sub

'    Sub New()

'        SetStyle(ControlStyles.ResizeRedraw Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint Or ControlStyles.Selectable, True)
'        DoubleBuffered = True

'        Base = New TextBox

'        Base.Font = Font
'        Base.Text = Text
'        Base.MaxLength = _MaxLength
'        Base.Multiline = _Multiline
'        Base.ReadOnly = _ReadOnly
'        Base.UseSystemPasswordChar = _UseSystemPasswordChar
'        Base.BorderStyle = BorderStyle.None
'        Base.ForeColor = ForeColor

'        If _Multiline Then
'            Base.Height = Height - 11
'        Else
'            LockHeight = Base.Height + 11
'        End If

'        AddHandler Base.TextChanged, AddressOf OnBaseTextChanged
'        AddHandler Base.KeyDown, AddressOf OnBaseKeyDown
'        AddHandler Base.MouseMove, AddressOf OnMouseMove
'        AddHandler Base.MouseLeave, AddressOf OnMouseLeave
'        AddHandler Base.GotFocus, AddressOf OnGotFocus
'        AddHandler Base.LostFocus, AddressOf OnlostFocus


'        SetColor("Border1", 0, 120, 204)
'        IsAnimated = True

'        Me.AccessibleDescription = "Animated Control"
'    End Sub

'End Class 'DISPOSE LEFT




<DefaultEvent("TextChanged")> Class custom_Animated_Text
#Region "declare"
    Inherits ThemeControl154
    WithEvents Base As TextBox
    Public isf As Integer = 0
    Dim bc As Color
    Dim tp As Pen
    Dim tb As SolidBrush
    Dim w1! = 0.0
    Dim w2! = 0.0
    Dim x As Single = 0.0
    Dim sc As Boolean = True

#Region "prop"


    Public _TextAlign As HorizontalAlignment = HorizontalAlignment.Left : <Category("Behavior")> Property TextAlign() As HorizontalAlignment
        Get
            Return _TextAlign
        End Get
        Set(ByVal value As HorizontalAlignment)
            _TextAlign = value
            If Base IsNot Nothing Then
                Base.TextAlign = value
            End If
        End Set
    End Property
    Public _MaxLength As Integer = 32767 : <Category("Behavior")> Property MaxLength() As Integer
        Get
            Return _MaxLength
        End Get
        Set(ByVal value As Integer)
            _MaxLength = value
            If Base IsNot Nothing Then
                Base.MaxLength = value
            End If
        End Set
    End Property
    Public _ReadOnly As Boolean : <Category("Behavior")> Property [ReadOnly]() As Boolean
        Get
            Return _ReadOnly
        End Get
        Set(ByVal value As Boolean)
            _ReadOnly = value
            If Base IsNot Nothing Then
                Base.ReadOnly = value
            End If
        End Set
    End Property
    Public _UseSystemPasswordChar As Boolean : <Category("Behavior")>
    Property UseSystemPasswordChar() As Boolean
        Get
            Return _UseSystemPasswordChar
        End Get
        Set(ByVal value As Boolean)
            _UseSystemPasswordChar = value
            If Base IsNot Nothing Then
                Base.UseSystemPasswordChar = value
            End If
        End Set
    End Property
    Public _Multiline As Boolean : <System.ComponentModel.Category("Behavior")>
    Property Multiline() As Boolean
        Get
            Return _Multiline
        End Get
        Set(ByVal value As Boolean)
            _Multiline = value
            If Base IsNot Nothing Then
                Base.Multiline = value

                If value Then
                    LockHeight = 0
                    Base.Height = Height - 2
                Else
                    LockHeight = Base.Height + 2
                End If
            End If
        End Set
    End Property

    Overrides Property Text As String
        Get
            Return MyBase.Text
        End Get
        Set(ByVal value As String)
            MyBase.Text = value
            If Base IsNot Nothing Then
                Base.Text = value
            End If
        End Set
    End Property
    Overrides Property Font As Font
        Get
            Return MyBase.Font
        End Get
        Set(ByVal value As Font)
            MyBase.Font = value
            If Base IsNot Nothing Then
                Base.Font = value
                Base.Location = New Point(0, 0)
                Base.Width = Width

                If Not _Multiline Then
                    LockHeight = Base.Height + 2
                End If
            End If
        End Set
    End Property

#End Region

    Protected Overrides Sub OnCreation()
        If Not Controls.Contains(Base) Then
            Controls.Add(Base)
            Base.Left = 0 : Base.Top = 0
            Base.Width = Width
            Base.Height = Height - 2
        End If
    End Sub
#End Region
#Region "funct"
    Public Sub OnBaseTextChanged(ByVal s As Object, ByVal e As EventArgs)
        Text = Base.Text
    End Sub
    Public Sub OnBaseKeyDown(ByVal s As Object, ByVal e As KeyEventArgs)
        If e.Control AndAlso e.KeyCode = Keys.A Then
            Base.SelectAll()
            e.SuppressKeyPress = True
        End If
    End Sub
    Protected Overrides Sub OnResize(ByVal e As EventArgs)
        Base.Location = New Point(0, 0)
        Base.Width = Width
        Base.Height = Height - 2
        MyBase.OnResize(e)
    End Sub
    Protected Overrides Sub OnForeColorChanged(e As EventArgs)
        MyBase.OnForeColorChanged(e)
        Base.ForeColor = ForeColor
    End Sub
    Protected Overrides Sub OnbackColorChanged(e As EventArgs)
        MyBase.OnBackColorChanged(e)
        Base.BackColor = BackColor
    End Sub


    Public Sub gf()
        isf = 1
    End Sub
    Public Sub lf()
        isf = 0
        w1 = 0.0
        w2 = 0.0
    End Sub

#End Region

    Protected Overrides Sub PaintHook()

        G.InterpolationMode = InterpolationMode.HighQualityBicubic
        G.SmoothingMode = SmoothingMode.AntiAlias
        G.Clear(Parent.BackColor)
        Base.BackColor = Parent.BackColor 'init

        'v = ((v * (N - 1)) + w) / N; 
        'where v is the current value, w is the value towards which we want to move, and N is the slowdown factor. 
        'The higher N, the slower v approaches w.      <----- WEIGHTED AVERAGE

        If isf = 1 Then
            w1 = ((w1 * (100 - 1)) + (Width - x)) / 100 'wid-x                   <-------= SAVE THIS
            w2 = ((w2 * (100 - 1)) + x) / 100   'x
        Else
            Me.AccessibleDescription = ""
        End If ' anim

        mb(bc, tb)
        G.FillRectangle(tb, x, Height - 2, w1, 2)
        G.FillRectangle(tb, x - w2, Height - 2, w2, 2) ' draw1

        mp(bw(col(15, ForeColor)), tp)
        G.DrawLine(tp, 0, Height - 2, Width, Height - 2)
        mp(bw(col(55, ForeColor)), tp)
        G.DrawLine(tp, 0, Height - 1, Width, Height - 1) 'draw 2

        tb.Dispose()
        tp.Dispose() 'dispose


    End Sub



    Sub New()

        SetStyle(ControlStyles.ResizeRedraw Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint Or ControlStyles.Selectable, True)
        DoubleBuffered = True

        Base = New TextBox

        Base.Font = Font
        Base.Text = Text
        Base.MaxLength = _MaxLength
        Base.Multiline = _Multiline
        Base.ReadOnly = _ReadOnly
        Base.UseSystemPasswordChar = _UseSystemPasswordChar
        Base.BorderStyle = BorderStyle.None
        Base.ForeColor = ForeColor
        Base.BackColor = BackColor

        Base.Height = Height - 4
        LockHeight = Base.Height + 4

        AddHandler Base.TextChanged, AddressOf OnBaseTextChanged
        AddHandler Base.KeyDown, AddressOf OnBaseKeyDown
        AddHandler Base.GotFocus, AddressOf gf
        AddHandler Base.LostFocus, AddressOf lf
        AddHandler Base.MouseDown, AddressOf md
        AddHandler Me.MouseDown, AddressOf md


        bc = col(10, 160, 255)

    End Sub
    Public Sub md(sender As Object, e As MouseEventArgs)
        Me.AccessibleDescription = "Animated Control"
        x = e.X
        isf = 1
    End Sub

End Class 'DISPOSE LEFT



