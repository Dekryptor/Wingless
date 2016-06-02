#Region "Non-Controls"
#Region "Imports"
Imports System.Drawing.Drawing2D
Imports System.ComponentModel
Imports System.Runtime.InteropServices
#End Region


#Region "Themebase"
MustInherit Class ThemeControl154
    Inherits Control


#Region " Initialization "

    Protected G As Graphics, B As Bitmap

    Sub New()
        SetStyle(139270, True)
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)

        _ImageSize = Size.Empty
        Font = New Font("Verdana", 8S)

        MeasureBitmap = New Bitmap(1, 1)
        MeasureGraphics = Graphics.FromImage(MeasureBitmap)

        DrawRadialPath = New GraphicsPath
        upcust()
    End Sub

    Protected NotOverridable Overrides Sub OnHandleCreated(ByVal e As EventArgs)

        If Not _LockWidth = 0 Then Width = _LockWidth
        If Not _LockHeight = 0 Then Height = _LockHeight

        Transparent = _Transparent
        If _Transparent AndAlso _BackColor Then BackColor = Color.Transparent

        MyBase.OnHandleCreated(e)
    End Sub

    Private DoneCreation As Boolean
    Protected NotOverridable Overrides Sub OnParentChanged(ByVal e As EventArgs)
        If Parent IsNot Nothing Then
            OnCreation()
            DoneCreation = True
            upcust()
        End If
        MyBase.OnParentChanged(e)
    End Sub

#End Region


    Protected NotOverridable Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        If Width = 0 OrElse Height = 0 Then Return

        If _Transparent Then
            PaintHook()
            e.Graphics.DrawImage(B, 0, 0)
        Else
            G = e.Graphics
            PaintHook()
        End If
    End Sub


#Region " Size Handling "

    Protected NotOverridable Overrides Sub OnSizeChanged(ByVal e As EventArgs)
        If _Transparent Then
            InvalidateBitmap()
        End If

        Invalidate()
        MyBase.OnSizeChanged(e)
    End Sub

    Protected Overrides Sub SetBoundsCore(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal specified As BoundsSpecified)
        If Not _LockWidth = 0 Then width = _LockWidth
        If Not _LockHeight = 0 Then height = _LockHeight
        MyBase.SetBoundsCore(x, y, width, height, specified)
    End Sub

#End Region

#Region " State Handling "

    Private InPosition As Boolean
    Protected Overrides Sub OnMouseEnter(ByVal e As EventArgs)
        InPosition = True
        SetState(MouseState.Over)
        MyBase.OnMouseEnter(e)
    End Sub

    Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
        If InPosition Then SetState(MouseState.Over)
        MyBase.OnMouseUp(e)
    End Sub

    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
        If e.Button = Windows.Forms.MouseButtons.Left Then SetState(MouseState.Down)
        MyBase.OnMouseDown(e)
    End Sub

    Protected Overrides Sub OnMouseLeave(ByVal e As EventArgs)
        InPosition = False
        SetState(MouseState.None)
        MyBase.OnMouseLeave(e)
    End Sub

    Protected Overrides Sub OnEnabledChanged(ByVal e As EventArgs)
        If Enabled Then SetState(MouseState.None) Else SetState(MouseState.Block)
        MyBase.OnEnabledChanged(e)
    End Sub

    Protected State As MouseState
    Private Sub SetState(ByVal current As MouseState)
        State = current
        Invalidate()
    End Sub

#End Region

#Region "Props"
#Region " Base Properties "

    <Browsable(False), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Overrides Property BackgroundImage() As Image
        Get
            Return Nothing
        End Get
        Set(ByVal value As Image)
        End Set
    End Property
    <Browsable(False), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Overrides Property BackgroundImageLayout() As ImageLayout
        Get
            Return ImageLayout.None
        End Get
        Set(ByVal value As ImageLayout)
        End Set
    End Property

    Overrides Property Text() As String
        Get
            Return MyBase.Text
        End Get
        Set(ByVal value As String)
            MyBase.Text = value
            Invalidate()
            upcust()
        End Set
    End Property
    Overrides Property Font() As Font
        Get
            Return MyBase.Font
        End Get
        Set(ByVal value As Font)
            MyBase.Font = value
            Invalidate()
            upcust()

        End Set
    End Property

    Private _BackColor As Boolean
    <Category("Appearance")>
    Overrides Property BackColor() As Color
        Get
            Return MyBase.BackColor
        End Get
        Set(ByVal value As Color)
            If Not IsHandleCreated AndAlso value = Color.Transparent Then
                _BackColor = True
                Return
            End If
            MyBase.BackColor = value
            Invalidate()
            upcust()
        End Set
    End Property

#End Region

#Region " Public Properties "



    Private _Image As Image
    Property Image() As Image
        Get
            Return _Image
        End Get
        Set(ByVal value As Image)
            If value Is Nothing Then
                _ImageSize = Size.Empty
            Else
                _ImageSize = value.Size
            End If

            _Image = value
            Invalidate()
        End Set
    End Property

    Private _Transparent As Boolean
    Property Transparent() As Boolean
        Get
            Return _Transparent
        End Get
        Set(ByVal value As Boolean)
            _Transparent = value
            If Not IsHandleCreated Then Return

            If Not value AndAlso Not BackColor.A = 255 Then
                Throw New Exception("Unable to change value to false while a transparent BackColor is in use.")
            End If

            SetStyle(ControlStyles.Opaque, Not value)
            SetStyle(ControlStyles.SupportsTransparentBackColor, value)

            If value Then InvalidateBitmap() Else B = Nothing
            Invalidate()
        End Set
    End Property


    Private cstmstr$ : <Category("Design")> Public Property Customization As String
        Get
            Return cstmstr
        End Get
        Set(value As String)
            cstmstr = value
            Dim s(17) As String
            s = cstmstr.Split(",")

            Width = Val(s(0)) : Height = Val(s(1))
            BackColor = col(Val(s(2)), Val(s(3)), Val(s(4)), Val(s(5)))
            ForeColor = col(Val(s(6)), Val(s(7)), Val(s(8)), Val(s(9)))
            Text = s(10)

            Dim b As Boolean = False : Dim u As Boolean = False : Dim i As Boolean = False : Dim st As Boolean = False
            If s(13) = "True" Then b = True
            If s(14) = "True" Then i = True
            If s(15) = "True" Then st = True
            If s(16) = "True" Then u = True


            Font = New Font(s(11), Val(s(12)))

            Invalidate()
        End Set
    End Property


#End Region

#Region " Private Properties "

    Private _ImageSize As Size
    Protected ReadOnly Property ImageSize() As Size
        Get
            Return _ImageSize
        End Get
    End Property

    Private _LockWidth As Integer
    Protected Property LockWidth() As Integer
        Get
            Return _LockWidth
        End Get
        Set(ByVal value As Integer)
            _LockWidth = value
            If Not LockWidth = 0 AndAlso IsHandleCreated Then Width = LockWidth
        End Set
    End Property

    Private _LockHeight As Integer
    Protected Property LockHeight() As Integer
        Get
            Return _LockHeight
        End Get
        Set(ByVal value As Integer)
            _LockHeight = value
            If Not LockHeight = 0 AndAlso IsHandleCreated Then Height = LockHeight
        End Set
    End Property


#End Region


#Region " Property Helpers "
    Private Sub InvalidateBitmap()
        If Width = 0 OrElse Height = 0 Then Return
        B = New Bitmap(Width, Height, Imaging.PixelFormat.Format32bppPArgb)
        G = Graphics.FromImage(B)
    End Sub

#End Region


#Region " User Hooks "

    Protected MustOverride Sub PaintHook()

    Protected Overridable Sub OnCreation()
    End Sub

    Private Sub upcust()
        cstmstr = CStr(Width) + "," + CStr(Height) + "," +
                  CStr(BackColor.A) + "," + CStr(BackColor.R) + "," + CStr(BackColor.G) + "," + CStr(BackColor.B) + "," +
                  CStr(ForeColor.A) + "," + CStr(ForeColor.R) + "," + CStr(ForeColor.G) + "," + CStr(ForeColor.B) + "," +
                  CStr(Text) + "," +
                  CStr(Font.FontFamily.Name) + "," + CStr(Font.Size) + "," + CStr(Font.Bold) + "," + CStr(Font.Italic) + "," + CStr(Font.Strikeout) + "," + CStr(Font.Underline)
    End Sub


#End Region
#End Region

#Region "Fn"
#Region " Offset "

    Private OffsetReturnRectangle As Rectangle
    Protected Function Offset(ByVal r As Rectangle, ByVal amount As Integer) As Rectangle
        OffsetReturnRectangle = New Rectangle(r.X + amount, r.Y + amount, r.Width - (amount * 2), r.Height - (amount * 2))
        Return OffsetReturnRectangle
    End Function

    Private OffsetReturnSize As Size
    Protected Function Offset(ByVal s As Size, ByVal amount As Integer) As Size
        OffsetReturnSize = New Size(s.Width + amount, s.Height + amount)
        Return OffsetReturnSize
    End Function

    Private OffsetReturnPoint As Point
    Protected Function Offset(ByVal p As Point, ByVal amount As Integer) As Point
        OffsetReturnPoint = New Point(p.X + amount, p.Y + amount)
        Return OffsetReturnPoint
    End Function

#End Region

#Region " Center "

    Private CenterReturn As Point

    Protected Function Center(ByVal p As Rectangle, ByVal c As Rectangle) As Point
        CenterReturn = New Point((p.Width \ 2 - c.Width \ 2) + p.X + c.X, (p.Height \ 2 - c.Height \ 2) + p.Y + c.Y)
        Return CenterReturn
    End Function
    Protected Function Center(ByVal p As Rectangle, ByVal c As Size) As Point
        CenterReturn = New Point((p.Width \ 2 - c.Width \ 2) + p.X, (p.Height \ 2 - c.Height \ 2) + p.Y)
        Return CenterReturn
    End Function

    Protected Function Center(ByVal child As Rectangle) As Point
        Return Center(Width, Height, child.Width, child.Height)
    End Function
    Protected Function Center(ByVal child As Size) As Point
        Return Center(Width, Height, child.Width, child.Height)
    End Function
    Protected Function Center(ByVal childWidth As Integer, ByVal childHeight As Integer) As Point
        Return Center(Width, Height, childWidth, childHeight)
    End Function

    Protected Function Center(ByVal p As Size, ByVal c As Size) As Point
        Return Center(p.Width, p.Height, c.Width, c.Height)
    End Function

    Protected Function Center(ByVal pWidth As Integer, ByVal pHeight As Integer, ByVal cWidth As Integer, ByVal cHeight As Integer) As Point
        CenterReturn = New Point(pWidth \ 2 - cWidth \ 2, pHeight \ 2 - cHeight \ 2)
        Return CenterReturn
    End Function

#End Region

#Region " Measure "

    Private MeasureBitmap As Bitmap
    Private MeasureGraphics As Graphics 'TODO: Potential issues during multi-threading.

    Protected Function Measure() As Size
        Return MeasureGraphics.MeasureString(Text, Font, Width).ToSize
    End Function
    Protected Function Measure(ByVal text As String) As Size
        Return MeasureGraphics.MeasureString(text, Font, Width).ToSize
    End Function

#End Region

#Region " DrawBorders "

    Protected Sub DrawBorders(ByVal p1 As Pen, ByVal offset As Integer)
        DrawBorders(p1, 0, 0, Width, Height, offset)
    End Sub
    Protected Sub DrawBorders(ByVal p1 As Pen, ByVal r As Rectangle, ByVal offset As Integer)
        DrawBorders(p1, r.X, r.Y, r.Width, r.Height, offset)
    End Sub
    Protected Sub DrawBorders(ByVal p1 As Pen, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal offset As Integer)
        DrawBorders(p1, x + offset, y + offset, width - (offset * 2), height - (offset * 2))
    End Sub

    Protected Sub DrawBorders(ByVal p1 As Pen)
        DrawBorders(p1, 0, 0, Width, Height)
    End Sub
    Protected Sub DrawBorders(ByVal p1 As Pen, ByVal r As Rectangle)
        DrawBorders(p1, r.X, r.Y, r.Width, r.Height)
    End Sub
    Protected Sub DrawBorders(ByVal p1 As Pen, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
        G.DrawRectangle(p1, x, y, width - 1, height - 1)
    End Sub

#End Region


#Region " DrawPixel "

    Private DrawPixelBrush As SolidBrush

    Protected Sub DrawPixel(ByVal c1 As Color, ByVal x As Integer, ByVal y As Integer)
        If _Transparent Then
            B.SetPixel(x, y, c1)
        Else
            DrawPixelBrush = New SolidBrush(c1)
            G.FillRectangle(DrawPixelBrush, x, y, 1, 1)
        End If
    End Sub

#End Region


#Region " DrawText "

    Private DrawTextPoint As Point
    Private DrawTextSize As Size

    Protected Sub DrawText(ByVal b1 As Brush, ByVal a As HorizontalAlignment, ByVal x As Integer, ByVal y As Integer)
        DrawText(b1, Text, a, x, y)
    End Sub
    Protected Sub DrawText(ByVal b1 As Brush, ByVal text As String, ByVal a As HorizontalAlignment, ByVal x As Integer, ByVal y As Integer)
        If text.Length = 0 Then Return

        DrawTextSize = Measure(text)
        DrawTextPoint = Center(DrawTextSize)

        Select Case a
            Case HorizontalAlignment.Left
                G.DrawString(text, Font, b1, x, DrawTextPoint.Y + y)
            Case HorizontalAlignment.Center
                G.DrawString(text, Font, b1, DrawTextPoint.X + x, DrawTextPoint.Y + y)
            Case HorizontalAlignment.Right
                G.DrawString(text, Font, b1, Width - DrawTextSize.Width - x, DrawTextPoint.Y + y)
        End Select
    End Sub

    Protected Sub DrawText(ByVal b1 As Brush, ByVal p1 As Point)
        If Text.Length = 0 Then Return
        G.DrawString(Text, Font, b1, p1)
    End Sub
    Protected Sub DrawText(ByVal b1 As Brush, ByVal x As Integer, ByVal y As Integer)
        If Text.Length = 0 Then Return
        G.DrawString(Text, Font, b1, x, y)
    End Sub

#End Region

#Region " DrawImage "

    Private DrawImagePoint As Point

    Protected Sub DrawImage(ByVal a As HorizontalAlignment, ByVal x As Integer, ByVal y As Integer)
        DrawImage(_Image, a, x, y)
    End Sub
    Protected Sub DrawImage(ByVal image As Image, ByVal a As HorizontalAlignment, ByVal x As Integer, ByVal y As Integer)
        If image Is Nothing Then Return
        DrawImagePoint = Center(image.Size)

        Select Case a
            Case HorizontalAlignment.Left
                G.DrawImage(image, x, DrawImagePoint.Y + y, image.Width, image.Height)
            Case HorizontalAlignment.Center
                G.DrawImage(image, DrawImagePoint.X + x, DrawImagePoint.Y + y, image.Width, image.Height)
            Case HorizontalAlignment.Right
                G.DrawImage(image, Width - image.Width - x, DrawImagePoint.Y + y, image.Width, image.Height)
        End Select
    End Sub

    Protected Sub DrawImage(ByVal p1 As Point)
        DrawImage(_Image, p1.X, p1.Y)
    End Sub
    Protected Sub DrawImage(ByVal x As Integer, ByVal y As Integer)
        DrawImage(_Image, x, y)
    End Sub

    Protected Sub DrawImage(ByVal image As Image, ByVal p1 As Point)
        DrawImage(image, p1.X, p1.Y)
    End Sub
    Protected Sub DrawImage(ByVal image As Image, ByVal x As Integer, ByVal y As Integer)
        If image Is Nothing Then Return
        G.DrawImage(image, x, y, image.Width, image.Height)
    End Sub

#End Region

#Region " DrawGradient "

    Private DrawGradientBrush As LinearGradientBrush
    Private DrawGradientRectangle As Rectangle

    Protected Sub DrawGradient(ByVal blend As ColorBlend, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
        DrawGradientRectangle = New Rectangle(x, y, width, height)
        DrawGradient(blend, DrawGradientRectangle)
    End Sub
    Protected Sub DrawGradient(ByVal blend As ColorBlend, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal angle As Single)
        DrawGradientRectangle = New Rectangle(x, y, width, height)
        DrawGradient(blend, DrawGradientRectangle, angle)
    End Sub

    Protected Sub DrawGradient(ByVal blend As ColorBlend, ByVal r As Rectangle)
        DrawGradientBrush = New LinearGradientBrush(r, Color.Empty, Color.Empty, 90.0F)
        DrawGradientBrush.InterpolationColors = blend
        G.FillRectangle(DrawGradientBrush, r)
    End Sub
    Protected Sub DrawGradient(ByVal blend As ColorBlend, ByVal r As Rectangle, ByVal angle As Single)
        DrawGradientBrush = New LinearGradientBrush(r, Color.Empty, Color.Empty, angle)
        DrawGradientBrush.InterpolationColors = blend
        G.FillRectangle(DrawGradientBrush, r)
    End Sub


    Protected Sub DrawGradient(ByVal c1 As Color, ByVal c2 As Color, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
        DrawGradientRectangle = New Rectangle(x, y, width, height)
        DrawGradient(c1, c2, DrawGradientRectangle)
    End Sub
    Protected Sub DrawGradient(ByVal c1 As Color, ByVal c2 As Color, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal angle As Single)
        DrawGradientRectangle = New Rectangle(x, y, width, height)
        DrawGradient(c1, c2, DrawGradientRectangle, angle)
    End Sub

    Protected Sub DrawGradient(ByVal c1 As Color, ByVal c2 As Color, ByVal r As Rectangle)
        DrawGradientBrush = New LinearGradientBrush(r, c1, c2, 90.0F)
        G.FillRectangle(DrawGradientBrush, r)
    End Sub
    Protected Sub DrawGradient(ByVal c1 As Color, ByVal c2 As Color, ByVal r As Rectangle, ByVal angle As Single)
        DrawGradientBrush = New LinearGradientBrush(r, c1, c2, angle)
        G.FillRectangle(DrawGradientBrush, r)
    End Sub

#End Region

#Region " DrawRadial "

    Private DrawRadialPath As GraphicsPath
    Private DrawRadialBrush1 As PathGradientBrush
    Private DrawRadialBrush2 As LinearGradientBrush
    Private DrawRadialRectangle As Rectangle

    Sub DrawRadial(ByVal blend As ColorBlend, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
        DrawRadialRectangle = New Rectangle(x, y, width, height)
        DrawRadial(blend, DrawRadialRectangle, width \ 2, height \ 2)
    End Sub
    Sub DrawRadial(ByVal blend As ColorBlend, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal center As Point)
        DrawRadialRectangle = New Rectangle(x, y, width, height)
        DrawRadial(blend, DrawRadialRectangle, center.X, center.Y)
    End Sub
    Sub DrawRadial(ByVal blend As ColorBlend, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal cx As Integer, ByVal cy As Integer)
        DrawRadialRectangle = New Rectangle(x, y, width, height)
        DrawRadial(blend, DrawRadialRectangle, cx, cy)
    End Sub

    Sub DrawRadial(ByVal blend As ColorBlend, ByVal r As Rectangle)
        DrawRadial(blend, r, r.Width \ 2, r.Height \ 2)
    End Sub
    Sub DrawRadial(ByVal blend As ColorBlend, ByVal r As Rectangle, ByVal center As Point)
        DrawRadial(blend, r, center.X, center.Y)
    End Sub
    Sub DrawRadial(ByVal blend As ColorBlend, ByVal r As Rectangle, ByVal cx As Integer, ByVal cy As Integer)
        DrawRadialPath.Reset()
        DrawRadialPath.AddEllipse(r.X, r.Y, r.Width - 1, r.Height - 1)

        DrawRadialBrush1 = New PathGradientBrush(DrawRadialPath)
        DrawRadialBrush1.CenterPoint = New Point(r.X + cx, r.Y + cy)
        DrawRadialBrush1.InterpolationColors = blend

        If G.SmoothingMode = SmoothingMode.AntiAlias Then
            G.FillEllipse(DrawRadialBrush1, r.X + 1, r.Y + 1, r.Width - 3, r.Height - 3)
        Else
            G.FillEllipse(DrawRadialBrush1, r)
        End If
    End Sub


    Protected Sub DrawRadial(ByVal c1 As Color, ByVal c2 As Color, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
        DrawRadialRectangle = New Rectangle(x, y, width, height)
        DrawRadial(c1, c2, DrawRadialRectangle)
    End Sub
    Protected Sub DrawRadial(ByVal c1 As Color, ByVal c2 As Color, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal angle As Single)
        DrawRadialRectangle = New Rectangle(x, y, width, height)
        DrawRadial(c1, c2, DrawRadialRectangle, angle)
    End Sub

    Protected Sub DrawRadial(ByVal c1 As Color, ByVal c2 As Color, ByVal r As Rectangle)
        DrawRadialBrush2 = New LinearGradientBrush(r, c1, c2, 90.0F)
        G.FillEllipse(DrawRadialBrush2, r)
    End Sub
    Protected Sub DrawRadial(ByVal c1 As Color, ByVal c2 As Color, ByVal r As Rectangle, ByVal angle As Single)
        DrawRadialBrush2 = New LinearGradientBrush(r, c1, c2, angle)
        G.FillEllipse(DrawRadialBrush2, r)
    End Sub

#End Region

#Region " CreateRound "

    Private CreateRoundPath As GraphicsPath
    Private CreateRoundRectangle As Rectangle

    Function CreateRound(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal slope As Integer) As GraphicsPath
        CreateRoundRectangle = New Rectangle(x, y, width, height)
        Return CreateRound(CreateRoundRectangle, slope)
    End Function

    Function CreateRound(ByVal r As Rectangle, ByVal slope As Integer) As GraphicsPath
        CreateRoundPath = New GraphicsPath(FillMode.Winding)
        CreateRoundPath.AddArc(r.X, r.Y, slope, slope, 180.0F, 90.0F)
        CreateRoundPath.AddArc(r.Right - slope, r.Y, slope, slope, 270.0F, 90.0F)
        CreateRoundPath.AddArc(r.Right - slope, r.Bottom - slope, slope, slope, 0.0F, 90.0F)
        CreateRoundPath.AddArc(r.X, r.Bottom - slope, slope, slope, 90.0F, 90.0F)
        CreateRoundPath.CloseFigure()
        Return CreateRoundPath
    End Function

#End Region

#End Region

#Region "Enum"
    Enum MouseState As Byte
        None = 0
        Over = 1
        Down = 2
        Block = 3
    End Enum

    ''' <summary>
    ''' The different types of Interpolation.
    ''' </summary>
    Public Enum Type
        ''' <summary>
        ''' Start at full speed, and then retard.
        ''' </summary>
        EaseIn
        ''' <summary>
        ''' Start at zero speed, and then accelerate.
        ''' </summary>
        EaseOut
        ''' <summary>
        ''' Retard during the first half of the motion and then accelerate during the second half.
        ''' </summary>
        EaseInOut
        ''' <summary>
        ''' Complete the motion with no change in speed.
        ''' </summary>
        Linear
        ''' <summary>
        ''' Represents motion according to the CatmullRom spline.
        ''' 2 points, Pt1 and Pt2 are specifyied to represent the 2 tangents to the curve.
        ''' </summary>
        CatmullRom
        ''' <summary>
        ''' Generates a curve with slope approaching zero near the start and end points, and approaching 1 near the middle, resulting in a smooth, natural motion.
        ''' </summary>
        SmoothStep
        ''' <summary>
        ''' A smoother version of the SmoothStep Interpolation Type, with the curve flatter at the ends.
        ''' </summary>
        Smootherstep
    End Enum

    ''' <summary>
    ''' The different methods of easing for easing Interpolations.
    ''' </summary>
    Public Enum EasingMethods
        ''' <summary>
        ''' Regualar easing. The motion depends upon the power specifyied and corresponds to the curve of degree equal to the given power.
        ''' </summary>
        Regular
        ''' <summary>
        ''' The motion corresponds to the curve of 2 raised to the power 10(x-1). Exponential curve has a lot of curvature and thus results in a sudden change in value as the slope approaches infinity near the end.
        ''' </summary>
        Exponent
        ''' <summary>
        ''' Motion corresponds to a circular plot, resulting in a more 'sudden' change in values near the mid point.
        ''' </summary>
        Circular
        ''' <summary>
        ''' A gentle easing is apllied, based on the sinusoidal curve. Slope approaches 1 for most of the part of the motion.
        ''' </summary>
        Sine
        ''' <summary>
        ''' Motion resembles that of bouncing ball, reversing back a little distance near the ends.
        ''' </summary>
        Bounce
        ''' <summary>
        ''' Causes the motion to go beyond the Ending Point and then return back. Note that a similar effect can be achieved using the CatmullRom Interpolation technique with values of Pt1 and Pt2 equal to -10 and 0.
        ''' </summary>
        Jumpback
        ''' <summary>
        ''' Resembles to the motion of an elastic band, stretched and then released. Slope alternatively approaches -infinity and +infinity.
        ''' </summary>
        Elastic
        ''' <summary>
        ''' Motion represents forced stopping of a heavy object moving with high velocity. Slope approaches infinity near the start and zero near the ends.
        ''' </summary>
        Critical_Damping
        '		Flash
    End Enum
#End Region


#Region "Animation"
    Private Function GetValue(StartV#, EndV#, Time#, Duration#, IType As Type, Method As EasingMethods, Power#, Q#, R#) As Single
        Dim i As New Interpolation
        Return i.GetValue(StartV, EndV, Time, Duration, IType, Method, Power, Q, R)
    End Function
    Public Function GetValue(StartV#, EndV#, Time#, Duration#, Q#, R#) As Single
        Return GetValue(StartV, EndV, Time, Duration, Interpolation.Type.CatmullRom, Interpolation.EasingMethods.Regular, 1, Q, R)
    End Function
    Public Function GetValue(StartV#, EndV#, Time#, Duration#, IType As Interpolation.Type, Power#) As Single
        Return GetValue(StartV, EndV, Time, Duration, IType, Interpolation.EasingMethods.Regular, Power, 0, 0)
    End Function
    Public Function GetValue(StartV#, EndV#, Time#, Duration#, IType As Interpolation.Type, Method As Interpolation.EasingMethods, Power#) As Single
        Return GetValue(StartV, EndV, Time, Duration, IType, Method, Power, 0, 0)
    End Function

    Public Shared Function blend(ParamArray Colors() As Color) As Color
        Dim c As Color
        Dim r% = 0
        Dim b% = 0
        Dim g% = 0
        Dim n% = 0
        Dim a% = 0
        For Each col As Color In Colors
            n += 1
            r += col.R
            g += col.G
            b += col.B
            a += col.A
        Next
        r /= n
        g /= n
        b /= n
        a /= n
        c = Color.FromArgb(a, r, g, b)
        Return c
    End Function
#End Region
End Class
#End Region


Class Interpolation
#Region " Animation "
#Region "Declare"
	Private Property ivar As Calc = Variants.Regular
	Private Delegate Function Calc(x#, pow#) As Single

#Region "Enum"
	''' <summary>
	''' The different types of Interpolation.
	''' </summary>
	Public Enum Type
		''' <summary>
		''' Start at full speed, and then retard.
		''' </summary>
		EaseIn
		''' <summary>
		''' Start at zero speed, and then accelerate.
		''' </summary>
		EaseOut
		''' <summary>
		''' Retard during the first half of the motion and then accelerate during the second half.
		''' </summary>
		EaseInOut
		''' <summary>
		''' Complete the motion with no change in speed.
		''' </summary>
		Linear
		''' <summary>
		''' Represents motion according to the CatmullRom spline.
		''' 2 points, Pt1 and Pt2 are specifyied to represent the 2 tangents to the curve.
		''' </summary>
		CatmullRom
		''' <summary>
		''' Generates a curve with slope approaching zero near the start and end points, and approaching 1 near the middle, resulting in a smooth, natural motion.
		''' </summary>
		SmoothStep
		''' <summary>
		''' A smoother version of the SmoothStep Interpolation Type, with the curve flatter at the ends.
		''' </summary>
		Smootherstep
	End Enum

	''' <summary>
	''' The different methods of easing for easing Interpolations.
	''' </summary>
	Public Enum EasingMethods
		''' <summary>
		''' Regualar easing. The motion depends upon the power specifyied and corresponds to the curve of degree equal to the given power.
		''' </summary>
		Regular
		''' <summary>
		''' The motion corresponds to the curve of 2 raised to the power 10(x-1). Exponential curve has a lot of curvature and thus results in a sudden change in value as the slope approaches infinity near the end.
		''' </summary>
		Exponent
		''' <summary>
		''' Motion corresponds to a circular plot, resulting in a more 'sudden' change in values near the mid point.
		''' </summary>
		Circular
		''' <summary>
		''' A gentle easing is apllied, based on the sinusoidal curve. Slope approaches 1 for most of the part of the motion.
		''' </summary>
		Sine
		''' <summary>
		''' Motion resembles that of bouncing ball, reversing back a little distance near the ends.
		''' </summary>
		Bounce
		''' <summary>
		''' Causes the motion to go beyond the Ending Point and then return back. Note that a similar effect can be achieved using the CatmullRom Interpolation technique with values of Pt1 and Pt2 equal to -10 and 0.
		''' </summary>
		Jumpback
		''' <summary>
		''' Resembles to the motion of an elastic band, stretched and then released. Slope alternatively approaches -infinity and +infinity.
		''' </summary>
		Elastic
		''' <summary>
		''' Motion represents forced stopping of a heavy object moving with high velocity. Slope approaches infinity near the start and zero near the ends.
		''' </summary>
		Critical_Damping
		'		Flash
	End Enum
#End Region

#End Region


#Region "  FUNCTS"

	Private NotInheritable Class Variants

		Public Shared ReadOnly Regular = New Calc(Function(x#, pow#) Math.Pow(x, pow))
		Public Shared ReadOnly Circular = New Calc(Function(x#, pow#) 1 - Math.Sin(Math.Acos(x)))
		Public Shared ReadOnly Jumpback = New Calc(Function(x#, pow#) (x ^ pow) * (2.5 * x - 1.5))
		Public Shared ReadOnly Elastic = New Calc(Function(x#, pow#) Math.Pow(2, 10 * (x - 1)) * Math.Cos(20 * Math.PI * 1.5 / 3 * x))
		Public Shared ReadOnly Bounce = New Calc(Function(x#, pow#)
													 Dim a As Double = 0, b As Double = 1
													 Do
														 If x >= (7 - 4 * a) / 11 Then Return -Math.Pow((11 - 6 * a - 11 * x) / 4, 2) + Math.Pow(b, 2)
														 a += b : b /= 2
													 Loop
												 End Function)
		Public Shared ReadOnly Sine = New Calc(Function(x#, pow#) 1 - Math.Cos(x * (Math.PI / 2)))
		Public Shared ReadOnly Exponent = New Calc(Function(x#, pow#) 2 ^ (10 * ((x ^ pow) - 1)))
		Public Shared ReadOnly Critical_Damping = New Calc(Function(x#, pow#) (1 - Math.Exp(((-1 * x) * 5))) / 0.993262053)

	End Class

	Private Function cmr(t#, p0#, p1#, p2#, p3#)
		Return 0.5F * (
					  (2 * p1) +
					  (-p0 + p2) * t +
					  (2 * p0 - 5 * p1 + 4 * p2 - p3) * t * t +
					  (-p0 + 3 * p1 - 3 * p2 + p3) * t * t * t
					  )
	End Function
	Private Function smtstp(x#) As Double
		Return 3 * (x ^ 2) - 2 * (x ^ 3)
		'Return (x * x * (3 - 2 * x))
	End Function
	Private Function smtrstp(x#) As Double
		Return 6 * (x ^ 5) - 15 * (x ^ 4) + 10 * (x ^ 3)
	End Function

#End Region


	''' <summary>
	''' Gets the calculated value of the Interpolation.
	''' </summary>
	Public Function GetValue(StartV#, EndV#, Time#, Duration#, IType As Type, Method As EasingMethods, Power#, Q#, R#) As Single

		Dim p# = Time / Duration
		Dim rev As Boolean
		Dim c# = EndV - StartV
		If c < 0.0# Then rev = True
		c = Math.Abs(c)

		Select Case Method
			Case EasingMethods.Bounce
				ivar = Variants.Bounce
			Case EasingMethods.Circular
				ivar = Variants.Circular
			Case EasingMethods.Elastic
				ivar = Variants.Elastic
			Case EasingMethods.Exponent
				ivar = Variants.Exponent
			Case EasingMethods.Jumpback
				ivar = Variants.Jumpback
			Case EasingMethods.Regular
				ivar = Variants.Regular
			Case EasingMethods.Sine
				ivar = Variants.Sine
			Case EasingMethods.Critical_Damping
				ivar = Variants.Critical_Damping
		End Select



		Dim rt#
		Select Case IType
			Case Type.Linear
			Case Type.CatmullRom
				p = cmr(p, Q, 0, 1, R)
			Case Type.EaseIn
				p = ivar.Invoke(p, Power)
			Case Type.EaseOut
				p = 1 - ivar.Invoke(1 - p, Power)
			Case Type.EaseInOut
				p = If(p <= 0.5, ivar.Invoke(2 * p, Power) / 2, (2 - ivar.Invoke(2 * (1 - p), Power)) / 2)
			Case Type.SmoothStep
				For i = 1 To CInt(Power)
					p = smtstp(p)
				Next
			Case Type.Smootherstep
				For i = 1 To CInt(Power)
					p = smtrstp(p)
				Next
		End Select

		If rev Then p = 1 - p

		rt = p * c
		Return CSng(rt + Math.Min(StartV, EndV))
	End Function
	Public Function GetValue(StartV#, EndV#, Time#, Duration#, Q#, R#) As Single
		Return GetValue(StartV, EndV, Time, Duration, Type.CatmullRom, EasingMethods.Regular, 1, Q, R)
	End Function
	Public Function GetValue(StartV#, EndV#, Time#, Duration#, IType As Type, Power#) As Single
		Return GetValue(StartV, EndV, Time, Duration, IType, EasingMethods.Regular, Power, 0, 0)
	End Function
	Public Function GetValue(StartV#, EndV#, Time#, Duration#, IType As Type, Method As EasingMethods, Power#) As Single
		Return GetValue(StartV, EndV, Time, Duration, IType, Method, Power, 0, 0)
	End Function

#End Region
End Class


#Region "Multitmedia Timer"
#Region "License"

' Copyright (c) 2006 Leslie Sanford
' * 
' * Permission is hereby granted, free of charge, to any person obtaining a copy 
' * of this software and associated documentation files (the "Software"), to 
' * deal in the Software without restriction, including without limitation the 
' * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
' * sell copies of the Software, and to permit persons to whom the Software is 
' * furnished to do so, subject to the following conditions:
' * 
' * The above copyright notice and this permission notice shall be included in 
' * all copies or substantial portions of the Software. 
' * 
' * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
' * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
' * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
' * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
' * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
' * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
' * THE SOFTWARE.
' 

#End Region

#Region "Contact"

'
' * Leslie Sanford
' * Email: jabberdabber@hotmail.com
' 

#End Region

Namespace Multimedia


	''' <summary>
	''' Defines constants for the multimedia Timer's event types.
	''' </summary>
	Public Enum TimerMode
		''' <summary>
		''' Timer event occurs once.
		''' </summary>
		OneShot

		''' <summary>
		''' Timer event occurs periodically.
		''' </summary>
		Periodic
	End Enum

	''' <summary>
	''' Represents information about the multimedia Timer's capabilities.
	''' </summary>
	<StructLayout(LayoutKind.Sequential)>
	Public Structure TimerCaps
		''' <summary>
		''' Minimum supported period in milliseconds.
		''' </summary>
		Public periodMin As Integer

		''' <summary>
		''' Maximum supported period in milliseconds.
		''' </summary>
		Public periodMax As Integer
	End Structure

	''' <summary>
	''' Represents the Windows multimedia timer.
	''' </summary>
	Public NotInheritable Class Timer
		Implements IComponent
#Region "Timer Members"

		'Event disposedb(sender As Object, e As EventArgs)
		'	Event disposed(sender As Object, e As EventArgs)


#Region "Delegates"

		' Represents the method that is called by Windows when a timer event occurs.
		Public Delegate Sub TimeProc(ByVal id As Integer, ByVal msg As Integer, ByVal user As Integer, ByVal param1 As Integer, ByVal param2 As Integer)

		' Represents methods that raise events.
		Public Delegate Sub EventRaiser(ByVal e As EventArgs)

#End Region

#Region "Win32 Multimedia Timer Functions"

		' Gets timer capabilities.
		<DllImport("winmm.dll")>
		Public Shared Function timeGetDevCaps(ByRef caps As TimerCaps, ByVal sizeOfTimerCaps As Integer) As Integer
		End Function

		' Creates and starts the timer.
		<DllImport("winmm.dll")>
		Public Shared Function timeSetEvent(ByVal delay As Integer, ByVal resolution_Renamed As Integer, ByVal proc As TimeProc, ByVal user As Integer, ByVal mode_Renamed As Integer) As Integer
		End Function

		' Stops and destroys the timer.
		<DllImport("winmm.dll")>
		Public Shared Function timeKillEvent(ByVal id As Integer) As Integer
		End Function

		' Indicates that the operation was successful.
		Public Const TIMERR_NOERROR As Integer = 0

#End Region

#Region "Fields"

		' Timer identifier.
		Public timerID As Integer

		' Timer mode.
		'TODO: INSTANT VB TODO TASK: There is no VB.NET equivalent to 'volatile':
		'ORIGINAL LINE: public volatile TimerMode mode;
		Public mode_Renamed As TimerMode

		' Period between timer events in milliseconds.
		'TODO: INSTANT VB TODO TASK: There is no VB.NET equivalent to 'volatile':
		'ORIGINAL LINE: public volatile int period;
		Public period_Renamed As Integer

		' Timer resolution in milliseconds.
		'TODO: INSTANT VB TODO TASK: There is no VB.NET equivalent to 'volatile':
		'ORIGINAL LINE: public volatile int resolution;
		Public resolution_Renamed As Integer

		' Called by Windows when a timer periodic event occurs.
		Public timeProcPeriodic As TimeProc

		' Called by Windows when a timer one shot event occurs.
		Public timeProcOneShot As TimeProc

		' Represents the method that raises the Tick event.
		Public tickRaiser As EventRaiser

		' Indicates whether or not the timer is running.
		Public running As Boolean = False

		' Indicates whether or not the timer has been disposedb.
		'TODO: INSTANT VB TODO TASK: There is no VB.NET equivalent to 'volatile':
		'ORIGINAL LINE: public volatile bool disposedb = False;
		Public disposedb As Boolean = False

		' The ISynchronizeInvoke object to use for marshaling events.
		Public synchronizingObject_Renamed As ISynchronizeInvoke = Nothing

		' For implementing IComponent.
		Public site_Renamed As ISite = Nothing

		' Multimedia timer capabilities.
		Public Shared caps As TimerCaps

#End Region

#Region "Events"

		''' <summary>
		''' Occurs when the Timer has started;
		''' </summary>
		Public Event Started As EventHandler

		''' <summary>
		''' Occurs when the Timer has stopped;
		''' </summary>
		Public Event Stopped As EventHandler

		''' <summary>
		''' Occurs when the time period has elapsed.
		''' </summary>
		Public Event Tick As EventHandler

#End Region

#Region "Construction"

		''' <summary>
		''' Initialize class.
		''' </summary>
		Shared Sub New()
			' Get multimedia timer capabilities.
			timeGetDevCaps(caps, Marshal.SizeOf(caps))
		End Sub

		''' <summary>
		''' Initializes a new instance of the Timer class with the specified IContainer.
		''' </summary>
		''' <param name="container">
		''' The IContainer to which the Timer will add itself.
		''' </param>
		Public Sub New(ByVal container As IContainer)
            ' Required for Windows.Forms Class Composition Designer support
            container.Add(Me)

            Initialize()
		End Sub

		''' <summary>
		''' Initializes a new instance of the Timer class.
		''' </summary>
		Public Sub New()
			Initialize()
		End Sub

		Protected Overrides Sub Finalize()
			If IsRunning Then
				' Stop and destroy timer.
				timeKillEvent(timerID)
			End If
		End Sub

		' Initialize timer with default values.
		Public Sub Initialize()
			Me.mode_Renamed = TimerMode.Periodic
			Me.period_Renamed = Capabilities.periodMin
			Me.resolution_Renamed = 1

			running = False

			timeProcPeriodic = New TimeProc(AddressOf TimerPeriodicEventCallback)
			timeProcOneShot = New TimeProc(AddressOf TimerOneShotEventCallback)
			tickRaiser = New EventRaiser(AddressOf OnTick)
		End Sub

#End Region

#Region "Methods"

		''' <summary>
		''' Starts the timer.
		''' </summary>
		''' <exception cref="ObjectDisposedException">
		''' The timer has already been disposedb.
		''' </exception>
		''' <exception cref="TimerStartException">
		''' The timer failed to start.
		''' </exception>
		Public Sub Start()
			'			#Region "Require"

			If disposedb Then
				Throw New ObjectDisposedException("Timer")
			End If

			'			#End Region

			'			#Region "Guard"

			If IsRunning Then
				Return
			End If

			'			#End Region

			' If the periodic event callback should be used.
			If Mode = TimerMode.Periodic Then
				' Create and start timer.
				timerID = timeSetEvent(Period, Resolution, timeProcPeriodic, 0, CInt(Fix(Mode)))
				' Else the one shot event callback should be used.
			Else
				' Create and start timer.
				timerID = timeSetEvent(Period, Resolution, timeProcOneShot, 0, CInt(Fix(Mode)))
			End If

			' If the timer was created successfully.
			If timerID <> 0 Then
				running = True

				If SynchronizingObject IsNot Nothing AndAlso SynchronizingObject.InvokeRequired Then
					SynchronizingObject.BeginInvoke(New EventRaiser(AddressOf OnStarted), New Object() {EventArgs.Empty})
				Else
					OnStarted(EventArgs.Empty)
				End If
			Else
				Throw New TimerStartException("Unable to start multimedia Timer.")
			End If
		End Sub

		''' <summary>
		''' Stops timer.
		''' </summary>
		''' <exception cref="ObjectDisposedException">
		''' If the timer has already been disposedb.
		''' </exception>
		Public Sub [Stop]()
			'			#Region "Require"

			If disposedb Then
				Throw New ObjectDisposedException("Timer")
			End If

			'			#End Region

			'			#Region "Guard"

			If (Not running) Then
				Return
			End If

			'			#End Region

			' Stop and destroy timer.
			Dim result As Integer = timeKillEvent(timerID)

			Debug.Assert(result = TIMERR_NOERROR)

			running = False

			If SynchronizingObject IsNot Nothing AndAlso SynchronizingObject.InvokeRequired Then
				SynchronizingObject.BeginInvoke(New EventRaiser(AddressOf OnStopped), New Object() {EventArgs.Empty})
			Else
				OnStopped(EventArgs.Empty)
			End If
		End Sub

#Region "Callbacks"

		' Callback method called by the Win32 multimedia timer when a timer
		' periodic event occurs.
		Public Sub TimerPeriodicEventCallback(ByVal id As Integer, ByVal msg As Integer, ByVal user As Integer, ByVal param1 As Integer, ByVal param2 As Integer)
			If synchronizingObject_Renamed IsNot Nothing Then
				synchronizingObject_Renamed.BeginInvoke(tickRaiser, New Object() {EventArgs.Empty})
			Else
				OnTick(EventArgs.Empty)
			End If
		End Sub

		' Callback method called by the Win32 multimedia timer when a timer
		' one shot event occurs.
		Public Sub TimerOneShotEventCallback(ByVal id As Integer, ByVal msg As Integer, ByVal user As Integer, ByVal param1 As Integer, ByVal param2 As Integer)
			If synchronizingObject_Renamed IsNot Nothing Then
				synchronizingObject_Renamed.BeginInvoke(tickRaiser, New Object() {EventArgs.Empty})
				Me.Stop()
			Else
				OnTick(EventArgs.Empty)
				Me.Stop()
			End If
		End Sub

#End Region

#Region "Event Raiser Methods"

		' Raises the disposedb event.
		Public Sub OnDisposed(ByVal e As EventArgs)
			Dim handler As EventHandler = disposedEvent

			If handler IsNot Nothing Then
				handler(Me, e)
			End If
		End Sub

		' Raises the Started event.
		Public Sub OnStarted(ByVal e As EventArgs)
			Dim handler As EventHandler = StartedEvent

			If handler IsNot Nothing Then
				handler(Me, e)
			End If
		End Sub

		' Raises the Stopped event.
		Public Sub OnStopped(ByVal e As EventArgs)
			Dim handler As EventHandler = StoppedEvent

			If handler IsNot Nothing Then
				handler(Me, e)
			End If
		End Sub

		' Raises the Tick event.
		Public Sub OnTick(ByVal e As EventArgs)
			Dim handler As EventHandler = TickEvent

			If handler IsNot Nothing Then
				handler(Me, e)
			End If
		End Sub

#End Region

#End Region

#Region "Properties"

		''' <summary>
		''' Gets or sets the object used to marshal event-handler calls.
		''' </summary>
		Public Property SynchronizingObject() As ISynchronizeInvoke
			Get
				'				#Region "Require"

				If disposedb Then
					Throw New ObjectDisposedException("Timer")
				End If

				'				#End Region

				Return synchronizingObject_Renamed
			End Get
			Set(ByVal value As ISynchronizeInvoke)
				'				#Region "Require"

				If disposedb Then
					Throw New ObjectDisposedException("Timer")
				End If

				'				#End Region

				synchronizingObject_Renamed = value
			End Set
		End Property

		''' <summary>
		''' Gets or sets the time between Tick events.
		''' </summary>
		''' <exception cref="ObjectDisposedException">
		''' If the timer has already been disposedb.
		''' </exception>   
		Public Property Period() As Integer
			Get
				'				#Region "Require"

				If disposedb Then
					Throw New ObjectDisposedException("Timer")
				End If

				'				#End Region

				Return period_Renamed
			End Get
			Set(ByVal value As Integer)
				'				#Region "Require"

				If disposedb Then
					Throw New ObjectDisposedException("Timer")
				ElseIf value < Capabilities.periodMin OrElse value > Capabilities.periodMax Then
					Throw New ArgumentOutOfRangeException("Period", value, "Multimedia Timer period out of range.")
				End If

				'				#End Region

				period_Renamed = value

				If IsRunning Then
					Me.Stop()
					Start()
				End If
			End Set
		End Property

		''' <summary>
		''' Gets or sets the timer resolution.
		''' </summary>
		''' <exception cref="ObjectDisposedException">
		''' If the timer has already been disposedb.
		''' </exception>        
		''' <remarks>
		''' The resolution is in milliseconds. The resolution increases 
		''' with smaller values; a resolution of 0 indicates periodic events 
		''' should occur with the greatest possible accuracy. To reduce system 
		''' overhead, however, you should use the maximum value appropriate 
		''' for your application.
		''' </remarks>
		Public Property Resolution() As Integer
			Get
				'				#Region "Require"

				If disposedb Then
					Throw New ObjectDisposedException("Timer")
				End If

				'				#End Region

				Return resolution_Renamed
			End Get
			Set(ByVal value As Integer)
				'				#Region "Require"

				If disposedb Then
					Throw New ObjectDisposedException("Timer")
				ElseIf value < 0 Then
					Throw New ArgumentOutOfRangeException("Resolution", value, "Multimedia timer resolution out of range.")
				End If

				'				#End Region

				resolution_Renamed = value

				If IsRunning Then
					Me.Stop()
					Start()
				End If
			End Set
		End Property

		''' <summary>
		''' Gets the timer mode.
		''' </summary>
		''' <exception cref="ObjectDisposedException">
		''' If the timer has already been disposedb.
		''' </exception>
		Public Property Mode() As TimerMode
			Get
				'				#Region "Require"

				If disposedb Then
					Throw New ObjectDisposedException("Timer")
				End If

				'				#End Region

				Return mode_Renamed
			End Get
			Set(ByVal value As TimerMode)
				'				#Region "Require"

				If disposedb Then
					Throw New ObjectDisposedException("Timer")
				End If

				'				#End Region

				mode_Renamed = value

				If IsRunning Then
					Me.Stop()
					Start()
				End If
			End Set
		End Property

		''' <summary>
		''' Gets a value indicating whether the Timer is running.
		''' </summary>
		Public ReadOnly Property IsRunning() As Boolean
			Get
				Return running
			End Get
		End Property

		''' <summary>
		''' Gets the timer capabilities.
		''' </summary>
		Public Shared ReadOnly Property Capabilities() As TimerCaps
			Get
				Return caps
			End Get
		End Property

#End Region

#End Region

#Region "IComponent Members"

		Public Event disposed As System.EventHandler Implements IComponent.Disposed

		Public Property Site() As ISite Implements IComponent.Site
			Get
				Return site_Renamed
			End Get
			Set(ByVal value As ISite)
				site_Renamed = value
			End Set
		End Property

#End Region

#Region "IDisposable Members"

		''' <summary>
		''' Frees timer resources.
		''' </summary>
		Public Sub Dispose() Implements System.IDisposable.Dispose
			'			#Region "Guard"

			If disposedb Then
				Return
			End If

			'			#End Region               

			If IsRunning Then
				Me.Stop()
			End If

			disposedb = True

			OnDisposed(EventArgs.Empty)
		End Sub

#End Region
	End Class

	''' <summary>
	''' The exception that is thrown when a timer fails to start.
	''' </summary>
	Public Class TimerStartException
		Inherits ApplicationException
		''' <summary>
		''' Initializes a new instance of the TimerStartException class.
		''' </summary>
		''' <param name="message">
		''' The error message that explains the reason for the exception. 
		''' </param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub
	End Class

	''' <summary>
	''' Defines constants representing the timing format used by the Time struct.
	''' </summary>
	Public Enum TimeType
		Milliseconds = &H1
		Samples = &H2
		Bytes = &H4
		Smpte = &H8
		Midi = &H10
		Ticks = &H20
	End Enum

	''' <summary>
	''' Represents the Windows Multimedia MMTIME structure.
	''' </summary>
	<StructLayout(LayoutKind.Explicit)>
	Public Structure Time
		<FieldOffset(0)>
		Public type As Integer

		<FieldOffset(4)>
		Public milliseconds As Integer

		<FieldOffset(4)>
		Public samples As Integer

		<FieldOffset(4)>
		Public byteCount As Integer

		<FieldOffset(4)>
		Public ticks As Integer

		'
		' SMPTE
		'

		<FieldOffset(4)>
		Public hours As Byte

		<FieldOffset(5)>
		Public minutes As Byte

		<FieldOffset(6)>
		Public seconds As Byte

		<FieldOffset(7)>
		Public frames As Byte

		<FieldOffset(8)>
		Public framesPerSecond As Byte

		<FieldOffset(9)>
		Public dummy As Byte

		<FieldOffset(10)>
		Public pad1 As Byte

		<FieldOffset(11)>
		Public pad2 As Byte

		'
		' MIDI
		'

		<FieldOffset(4)>
		Public songPositionPointer As Integer
	End Structure

End Namespace

#End Region


Module Helpers

#Region " Variables"
	Friend NearSF As New StringFormat() With {.Alignment = StringAlignment.Near, .LineAlignment = StringAlignment.Near}
	Friend CenterSF As New StringFormat() With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
    ''' <summary>
    ''' The types of limiting applied.
    ''' </summary>
    Public Enum LimitingType
        ''' <summary>
        ''' The r, g, b values of the resultant color cannot be less than those of the limiting color specifyied.
        ''' </summary>
        NotLessThan
        ''' <summary>
        ''' The r, g, b values of the resultant color cannot be more than those of the limiting color specifyied.
        ''' </summary>
        NotGreaterThan
    End Enum

#End Region

#Region " Functions"


#Region "Color"
    ''' <summary>
    ''' Returns a color from the specifyied argb values.
    ''' </summary>
    ''' <param name="a">a, the Alpha value of the color.</param>
    ''' <param name="r">r, the Red value of the color.</param>
    ''' <param name="g">g, the Green value of the color.</param>
    ''' <param name="b">b, the Blue value of the color.</param>
    ''' <returns></returns>
    Public Function col(a As Byte, r As Byte, g As Byte, b As Byte) As Color
        Return Color.FromArgb(a, r, g, b)
    End Function
    ''' <summary>
    ''' Returns a color from the specifyied rgb values. The Alpha value is automatically set to 255.
    ''' </summary>
    ''' <param name="r">r, the Red value of the color.</param>
    ''' <param name="g">g, the Green value of the color.</param>
    ''' <param name="b">b, the Blue value of the color.</param>
    ''' <returns></returns>    
    Public Function col(r As Byte, g As Byte, b As Byte) As Color
        Return Color.FromArgb(r, g, b)
    End Function
    ''' <summary>
    ''' Returns a color from a specifyied solidbrush. The Alpha value is automatically set to 255.
    ''' </summary>
    ''' <param name="br">The solidbrush to use.</param>
    ''' <returns></returns>
    Public Function col(br As SolidBrush) As Color
        Return br.Color
    End Function
    ''' <summary>
    ''' Returns a color from a specifyied solidbrush with a custom Alpha value.
    ''' </summary>
    ''' <param name="a">The Alpha value.</param>
    ''' <param name="br">The solidbrush to use.</param>
    ''' <returns></returns>
    Public Function col(a As Byte, br As SolidBrush) As Color
        Return Color.FromArgb(a, br.Color)
    End Function
    ''' <summary>
    ''' Returns a color from specifyied color and percentage of colorisation.
    ''' </summary>
    ''' <param name="c">The color to use.</param>
    ''' <param name="sat!">The percentage of colorisation. 0 means Black and 1 means full color.</param>
    ''' <returns></returns>
    Public Function col(c As Color, sat!)
        Return Color.FromArgb(CByte(CInt(c.A)), CByte(Math.Min(255, Math.Max(0, CInt(c.R) * sat))), CByte(Math.Min(255, Math.Max(0, CInt(c.G) * sat))), CByte(Math.Min(255, Math.Max(0, CInt(c.B) * sat))))
    End Function
    ''' <summary>
    ''' Returns a color from specifyied color and percentage of colorisation with a limiting color appliyed to the resulting colorisation. The resulting color's rgb values can't go beyond the rgb values of the limiting color.
    ''' </summary>
    ''' <param name="c">The color to use.</param>
    ''' <param name="sat!">The percentage of colorisation. 0 means Black and 1 means full color.</param>
    ''' <param name="limit">The limiting color to use.</param>
    ''' <param name="l">The limiting type to use, either NotLessThan or NotMoreThan.</param>
    ''' <returns></returns>
    Public Function col(c As Color, sat!, limit As Color, l As LimitingType)
        Dim floor, ceil As Color
        If l = LimitingType.NotLessThan Then
            floor = limit
            ceil = col(255)
        Else
            ceil = limit
            floor = col(0)
        End If
        Return Color.FromArgb(CByte(CInt(c.A)), CByte(Math.Min(CInt(ceil.R), Math.Max(CInt(floor.R), CInt(c.R) * sat))), CByte(Math.Min(CInt(ceil.R), Math.Max(CInt(floor.G), CInt(c.G) * sat))), CByte(Math.Min(CInt(ceil.R), Math.Max(CInt(floor.B), CInt(c.B) * sat))))
    End Function
    ''' <summary>
    ''' Returns a gray color with the r,g and b values as the specified value.
    ''' </summary>
    ''' <param name="n">The gray value.</param>
    ''' <returns></returns>
    Public Function col(n As Byte) As Color
        Return Color.FromArgb(n, n, n)
    End Function
    ''' <summary>
    ''' Returns a gray color with the r,g and b values as the specified value and an additional Alpha value.
    ''' </summary>
    ''' <param name="a">The Alpha value.</param>
    ''' <param name="n">The gray value.</param>
    ''' <returns></returns>    
    Public Function col(a As Byte, n As Byte) As Color
        Return Color.FromArgb(a, n, n, n)
    End Function
    ''' <summary>
    ''' Returns a color based on a specifyied color and an Alpha value.
    ''' </summary>
    ''' <param name="a">The Alpha value.</param>
    ''' <param name="c">The color to use.</param>
    ''' <returns></returns>
    Public Function col(a As Byte, c As Color) As Color
        Return Color.FromArgb(a, c)
    End Function

    ''' <summary>
    ''' Returns a color with all the r,g,b values of the color reduced by the specifyied value.
    ''' </summary>
    ''' <param name="c">The base color.</param>
    ''' <param name="n">The value to reduce.</param>
    ''' <returns></returns>
    Public Function lc(c As Color, n As Integer) As Color
        Return Color.FromArgb(CByte(Math.Abs(CInt(c.A))), CByte(Math.Abs(CInt(c.R) - n)), CByte(Math.Abs(CInt(c.G) - n)), CByte(Math.Abs(CInt(c.B) - n)))
    End Function
    ''' <summary>
    ''' Returns a color with all the r,g,b values of the color reduced by their specifyied r,g,b values.
    ''' </summary>
    ''' <param name="c">The base color.</param>
    ''' <param name="r%">The value to reduce from the red value.</param>
    ''' <param name="g%">The value to reduce from the green value.</param>
    ''' <param name="b%">The value to reduce from the blue value.</param>
    ''' <returns></returns>
    Public Function lc(c As Color, r%, g%, b%) As Color
        Return Color.FromArgb(CByte(Math.Abs(CInt(c.A))), CByte(Math.Abs(CInt(c.R) - r)), CByte(Math.Abs(CInt(c.G) - g)), CByte(Math.Abs(CInt(c.B) - b)))
    End Function

    ''' <summary>
    ''' Returns a color with all the r,g,b values of the color increased by the specifyied value.
    ''' </summary>
    ''' <param name="c">The base color.</param>
    ''' <param name="n">The value to increase.</param>
    ''' <returns></returns>
    Public Function ic(c As Color, n As Integer) As Color
		Return Color.FromArgb(CByte(Math.Min(255, CInt(c.A))), CByte(Math.Min(255, CInt(c.R) + n)), CByte(Math.Min(255, CInt(c.G) + n)), CByte(Math.Min(255, CInt(c.B) + n)))
	End Function
    ''' <summary>
    ''' Returns a color with all the r,g,b values of the color increased by their specifyied r,g,b values.
    ''' </summary>
    ''' <param name="c">The base color.</param>
    ''' <param name="r%">The value to increase from the red value.</param>
    ''' <param name="g%">The value to increase from the green value.</param>
    ''' <param name="b%">The value to increase from the blue value.</param>
    ''' <returns></returns>   
    Public Function ic(c As Color, r%, g%, b%) As Color
        Return Color.FromArgb(CByte(Math.Min(255, CInt(c.A))), CByte(Math.Min(255, CInt(c.R) + r)), CByte(Math.Min(255, CInt(c.G) + g)), CByte(Math.Min(255, CInt(c.B) + b)))
    End Function

    ''' <summary>
    ''' Returns an inverted color from a base color.
    ''' </summary>
    ''' <param name="Color">The base color.</param>
    ''' <returns></returns>
    Public Function Invert(ByVal Color As Color) As Color
        Return Color.FromArgb(Color.A, 255 - Color.R, 255 - Color.G, 255 - Color.B)
    End Function
    ''' <summary>
    ''' Returns color.White if the base color is a dark color and color.Black if the base color is a light color.
    ''' </summary>
    ''' <param name="c"></param>
    ''' <returns></returns>
    Public Function rescol(c As Color) As Color
        'Dim t = (Val(CStr(c.R)) + Val(CStr(c.G)) + Val(CStr(c.B))) / 3 '				stupid bug
        Dim t = CInt(CByte(c.R)) + CInt(CByte(c.G)) + CInt(CByte(c.B))
        If t < 128 Then Return Color.White Else Return Color.Black
    End Function

    'lum = Y=0.3RED+0.59GREEN+0.11Blue   rgb(y,y,y)

    ''' <summary>
    ''' Returns a black-and-white version of the base color.The alpha value is automatically assumed 255.
    ''' </summary>
    ''' <param name="c">The base color</param>
    ''' <returns></returns>
    Public Function bw(c As Color) As Color
        Dim y = CByte(CInt(c.R) * 0.3) + CByte(CInt(c.G) * 0.59) + CByte(CInt(c.B) * 0.11)
        Return col(CByte(CInt(c.A)), y)
    End Function
    ''' <summary>
    ''' Returns a black-and-white version of the base color with an additional Alpha value.
    ''' </summary>
    ''' <param name="a">The Alpha Value.</param>
    ''' <param name="c">The base color.</param>
    ''' <returns></returns>
    Public Function bw(a As Byte, c As Color) As Color
        Dim y = CByte(CInt(c.R) * 0.3) + CByte(CInt(c.G) * 0.59) + CByte(CInt(c.B) * 0.11)
        Return col(a, y)
    End Function
    ''' <summary>
    ''' Returns a black-and-white version of the base color from the r,g,b values.The alpha value is automatically assumed 255.
    ''' </summary>
    ''' <param name="r">The Red value.</param>
    ''' <param name="g">The Green value.</param>
    ''' <param name="b">The Blue value.</param>
    ''' <returns></returns>
    Public Function bw(r As Byte, g As Byte, b As Byte) As Color
        Dim y = r * 0.3 + g * 0.59 + b * 0.11
        Return col(y)
    End Function
    ''' <summary>
    ''' Returns a black-and-white version of the base color from the a,r,g,b values with an additional Alpha value.
    ''' </summary>
    ''' <param name="a">The Alpha value.</param>
    ''' <param name="r">The Red value.</param>
    ''' <param name="g">The Green value.</param>
    ''' <param name="b">The Blue value.</param>
    ''' <returns></returns>
    Public Function bw(a As Byte, r As Byte, g As Byte, b As Byte) As Color
        Dim y = r * 0.3 + g * 0.59 + b * 0.11
        Return col(a, y)
    End Function
#End Region

#Region "Pen & Brush"
    ''' <summary>
    ''' Assigns a new pen made from the specifyied color to the specifyied pen variable.
    ''' </summary>
    ''' <param name="c">The base color.</param>
    ''' <param name="p">The pen variable.</param>
    Public Sub mp(c As Color, ByRef p As Pen)
        p = New Pen(c)
    End Sub
    ''' <summary>
    ''' Assigns a new pen made from the color extracted from the specifyied brush to the specifyied pen variable.
    ''' </summary>
    ''' <param name="br">The base brush.</param>
    ''' <param name="p">The pen variable.</param>
    Public Sub mp(br As Brush, ByRef p As Pen)
        p = New Pen(br)
    End Sub
    ''' <summary>
    ''' Assigns a new pen made from the specifyied color and the specifyied width to the specifyied pen variable.
    ''' </summary>
    ''' <param name="c">The base color.</param>
    ''' <param name="w">The width of the pen.</param>
    ''' <param name="p">The pen variable.</param>
    Public Sub mp(c As Color, w!, ByRef p As Pen)
        p = New Pen(c, w)
    End Sub
    ''' <summary>
    ''' Assigns a new pen made from the color extracted from the specifyied brush and the specifyied width to the specifyied pen variable.
    ''' </summary>
    ''' <param name="br">The base brush.</param>
    ''' <param name="w">The width of the pen.</param>
    ''' <param name="p">The pen variable.</param>
    Public Sub mp(br As Brush, w!, ByRef p As Pen)
        p = New Pen(br, w)
    End Sub
    ''' <summary>
    ''' Returns a new pen made from the color extracted from the specifyied brush and the specifyied width.
    ''' </summary>
    ''' <param name="br">The base brush.</param>
    ''' <param name="w">The width of the pen.</param>
    Public Function mp(br As Brush, w!) As Pen
        Return New Pen(br, w)
    End Function
    ''' <summary>
    ''' Returns a new pen made from the specifyied color.
    ''' </summary>
    ''' <param name="c">The base color.</param>
    Public Function mp(c As Color) As Pen
        Return New Pen(c)
    End Function
    ''' <summary>
    ''' Returns a new pen made from the color extracted from the specifyied brush.
    ''' </summary>
    ''' <param name="br">The base brush.</param>
    Public Function mp(br As Brush) As Pen
        Return New Pen(br)
    End Function
    ''' <summary>
    ''' Returns a new pen made from the specifyied color and the specifyied width.
    ''' </summary>
    ''' <param name="c">The base color.</param>
    ''' <param name="w">The width of the pen.</param>
    Public Function mp(c As Color, w!) As Pen
        Return New Pen(c, w)
    End Function
    ''' <summary>
    ''' Assigns a new SoliBrush made from the specifyied color to the specifyied SoliBrush variable.
    ''' </summary>
    ''' <param name="c">The base color.</param>
    ''' <param name="b">The SoliBrush variable.</param>
    Public Sub mb(c As Color, ByRef b As SolidBrush)
        b = New SolidBrush(c)
    End Sub
    ''' <summary>
    ''' Returns a new SolidBrush made from the specifyied color.
    ''' </summary>
    ''' <param name="c">The base color.</param>
    Public Function mb(c As Color) As SolidBrush
        Return New SolidBrush(c)
    End Function
#End Region

#Region "Rectangle"
    ''' <summary>
    ''' Returns a rectangle with bounds as the sender object's bounds.
    ''' </summary>
    ''' <param name="Sender">The sender object.</param>
    ''' <returns></returns>
    Function rct(Sender As Object) As Rectangle
        Return rct(0, 0, Sender.width, Sender.height)
    End Function
    ''' <summary>
    ''' Returns a rectangle with bounds as the sender object's bounds with an applied offset.
    ''' </summary>
    ''' <param name="Sender">The sender object.</param>
    ''' <param name="offset%">Offset to apply.</param>
    ''' <returns></returns>
    Function rct(Sender As Object, offset%) As Rectangle
        Return rct(offset, offset, Sender.width - 2 * offset, Sender.height - 2 * offset)
    End Function
    ''' <summary>
    ''' Returns a rectangle with the specifyied height and width.
    ''' </summary>
    ''' <param name="w%">Width of the rectangle.</param>
    ''' <param name="h%">Height of the rectangle.</param>
    ''' <returns></returns>
    Function rct(w%, h%) As Rectangle
        Return rct(0, 0, w, h)
    End Function
    ''' <summary>
    ''' Returns a rectangle with optional shifts of width, height, XLocation and YLocation.
    ''' </summary>
    ''' <param name="r">The base rectangle.</param>
    ''' <param name="xshift%">Shift in XPosition.</param>
    ''' <param name="yshift%">Shift in YPosition.</param>
    ''' <param name="WidthShift%">Shift in Width.</param>
    ''' <param name="HeightShift%">Shift in Height.</param>
    ''' <returns></returns>
    Function rct(r As Rectangle, Optional xshift% = 0, Optional yshift% = 0, Optional WidthShift% = 0, Optional HeightShift% = 0) As Rectangle
        Return rct(r.X + xshift, r.Y + yshift, r.Width + WidthShift, r.Height + HeightShift)
    End Function
    ''' <summary>
    ''' Returns the rectangle associated with the specifyied LinearGradientBrush.
    ''' </summary>
    ''' <param name="lgb">The base LinearGradientBrush.</param>
    ''' <returns></returns>
    Function rct(lgb As LinearGradientBrush) As Rectangle
        Dim r = lgb.Rectangle
        Return rct(r.X, r.Y, r.Width, r.Height)
    End Function
    ''' <summary>
    ''' Returns the rectangle associated with the specifyied LinearGradientBrush with optional shifts of width, height, XLocation and YLocation.
    ''' </summary>
    ''' <param name="lgb">The base LinearGradientBrush.</param>
    ''' <param name="xshift%">Shift in XPosition.</param>
    ''' <param name="yshift%">Shift in YPosition.</param>
    ''' <param name="WidthShift%">Shift in Width.</param>
    ''' <param name="HeightShift%">Shift in Height.</param>
    ''' <returns></returns>
    Function rct(lgb As LinearGradientBrush, Optional xshift% = 0, Optional yshift% = 0, Optional WidthShift% = 0, Optional HeightShift% = 0) As Rectangle
        Dim r = lgb.Rectangle
        Return rct(r.X + xshift, r.Y + yshift, r.Width + WidthShift, r.Height + HeightShift)
    End Function
    ''' <summary>
    ''' Returns a rectangle with the specifyied values of X, Y, Width and Height.
    ''' </summary>
    ''' <param name="x%">XPosition</param>
    ''' <param name="y%">YPosition</param>
    ''' <param name="w%">Width</param>
    ''' <param name="h%">Height</param>
    ''' <returns></returns>
    Function rct(x%, y%, w%, h%) As Rectangle
        Return New Rectangle(x, y, w, h)
    End Function
#End Region

#Region "Point"
    ''' <summary>
    ''' Returns a point with the specifyied co-ordinates.
    ''' </summary>
    ''' <param name="x!">The X co-ordinate.</param>
    ''' <param name="y!">The Y co-ordinate.</param>
    ''' <returns></returns>
    Public Function pt(x!, y!) As Point
        Return New Point(x, y)
    End Function
    ''' <summary>
    ''' Returns the location of the specifyied rectangle.
    ''' </summary>
    ''' <param name="r">The base rectangle.</param>
    ''' <returns></returns>
    Public Function pt(r As Rectangle) As Point
        Return New Point(r.Location.X, r.Location.Y)
    End Function
    ''' <summary>
    ''' Returns the location of the specifyied object.
    ''' </summary>
    ''' <param name="o">The base object.</param>
    ''' <returns></returns>
    Public Function pt(o As Object) As Point
        Return New Point(o.Location.X, o.Location.Y)
    End Function
    ''' <summary>
    ''' Returns a point with the same x and y co-ordinates.
    ''' </summary>
    ''' <param name="n!">Distance from either axis.</param>
    ''' <returns></returns>
    Public Function pt(n!) As Point
        Return New Point(n, n)
    End Function
#End Region

#Region "Rounded Rectangle"
    <DllImport("gdi32.dll")>
    Public Function CreateRoundRectRgn(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer, ByVal cx As Integer, ByVal cy As Integer) As IntPtr
    End Function
    Public Function DrawRoundRectangle1(rct As Rectangle, r As Single, MSAA As Integer, FillColor As Color, BorderColor As Color, Optional BorderWidth As Single = 1)
        If MSAA <= 0 Then MSAA = 1
        If r <= 0 Then r = 1
        Dim b As New Bitmap(rct.Width * MSAA + MSAA, rct.Height * MSAA + MSAA)
        Dim g As Graphics = Graphics.FromImage(b)
        Dim RegH As IntPtr = CreateRoundRectRgn(rct.Left, rct.Top, (rct.Width) * MSAA, (rct.Height) * MSAA, r * MSAA, r * MSAA)
        Dim RegH1 As IntPtr = CreateRoundRectRgn(rct.Left + (MSAA * (BorderWidth + 1)), rct.Top + (MSAA * (-0.5 + BorderWidth)), (rct.Width - BorderWidth - 1) * MSAA, (rct.Height - BorderWidth - 3) * MSAA, (r - 2) * MSAA, (r - 2) * MSAA)
        Dim Reg As Region = Region.FromHrgn(RegH)
        Dim Reg1 As Region = Region.FromHrgn(RegH1)

        g.InterpolationMode = InterpolationMode.HighQualityBicubic
        g.SmoothingMode = SmoothingMode.HighQuality
        g.PixelOffsetMode = PixelOffsetMode.HighQuality
        g.CompositingQuality = 2
        g.FillRegion(New SolidBrush(BorderColor), Reg)
        g.SetClip(Reg1, CombineMode.Replace)
        g.Clear(FillColor)
        Return b
    End Function
    Public Function DrawRoundRectangle(rct As Rectangle, r!, MSAA%, FillColor As Color, BorderColor As Color, Optional BorderWidth! = 1)
		If MSAA <= 0 Then MSAA = 1
		If r <= 0 Then r = 1
		Dim b As New Bitmap(rct.Width * MSAA + MSAA, rct.Height * MSAA + MSAA)
		Dim g As Graphics = Graphics.FromImage(b)
		Dim RegH As IntPtr = CreateRoundRectRgn(rct.Left, rct.Top, (rct.Width) * MSAA, (rct.Height) * MSAA, r * MSAA, r * MSAA)
		Dim RegH1 As IntPtr = CreateRoundRectRgn(rct.Left + (MSAA * (BorderWidth)), rct.Top + (MSAA * (BorderWidth)), (rct.Width - BorderWidth) * MSAA, (rct.Height - BorderWidth) * MSAA, (r - 2) * MSAA, (r - 2) * MSAA)
		Dim Reg As Region = Region.FromHrgn(RegH)
		Dim Reg1 As Region = Region.FromHrgn(RegH1)

		g.InterpolationMode = InterpolationMode.HighQualityBicubic
		g.SmoothingMode = SmoothingMode.HighQuality
		g.PixelOffsetMode = 2
		g.FillRegion(New SolidBrush(BorderColor), Reg)
		g.SetClip(Reg1, CombineMode.Replace)
		g.Clear(FillColor)
		Return b
		b.Dispose()
	End Function

#End Region



#Region "Draw Methods"
	Public Function DrawArrow(x As Integer, y As Integer, flip As Boolean) As GraphicsPath  '-- Credit: AeonHack

		Dim GP As New GraphicsPath()

		Dim W As Integer = 12
		Dim H As Integer = 6

		If flip Then
			GP.AddLine(x + 1, y, x + W + 1, y)
			GP.AddLine(x + W, y, x + H, y + H - 1)
		Else
			GP.AddLine(x, y + H, x + W, y + H)
			GP.AddLine(x + W, y + H, x + H, y)
		End If

		GP.CloseFigure()
		Return GP
	End Function
    Public Function MeasurePath(ByVal Path As GraphicsPath) As Rectangle
        Dim x, y, w, h As Integer

        For Each p As PointF In Path.PathPoints
            If x = 0 And y = 0 And w = 0 And h = 0 Then : x = p.X : y = p.Y : Continue For : End If

            If p.X < x Then x = p.X
            If p.Y < y Then y = p.Y
            If p.X > w Then w = p.X
            If p.Y > h Then h = p.Y
        Next

        Return New Rectangle(x, y, w + -x, h + -y)
    End Function
    Public Function MeasureString(ByVal Str As String, ByVal Font As Font) As Size
		Dim B As New Bitmap(32, 1)
		Dim Ret As Size
		Str &= "|"

		Using G As Graphics = Graphics.FromImage(B)
			Ret = G.MeasureString(Str, Font).ToSize

			G.Clear(Color.White)
			G.DrawString(Str, Font, Brushes.Black, B.Width - Math.Truncate(Ret.Width), -Font.Height / 2)
		End Using

		For x = B.Width - 1 To 0 Step -1
			If B.GetPixel(x, 0).R <> 255 Then Return Ret - New Size(B.Width - x, 0)
		Next
	End Function
	Public Function GetVisibleRectParts(ByVal Rect1 As Rectangle, ByVal Rect2 As Rectangle) As Rectangle()
		If Not Rect1.IntersectsWith(Rect2) Then Return {Rect1}

		Dim r1, r2, r3, r4 As New Rectangle

        'top
        r1.X = Rect1.X
		r1.Y = Rect1.Y
		r1.Width = Rect1.Width
		r1.Height = Rect2.Y - Rect1.Y

        'left
        r3.X = Rect1.X
		r3.Y = Rect1.Y + r1.Height
		r3.Width = Rect2.X - Rect1.X
		r3.Height = Rect1.Height - r1.Height

        'rigth
        r4.X = Rect2.Width + (Rect2.X - Rect1.X)
		r4.Y = Rect1.Y + r1.Height
		r4.Width = (Rect1.Width - Rect2.Width) + (Rect1.X - Rect2.X)
		r4.Height = Rect1.Height - r1.Height

        'bottom
        r2.X = Rect1.X + r3.Width
		r2.Y = Rect2.Height + (Rect2.Y - Rect1.Y)
		r2.Width = Rect1.Width - (r4.Width + r3.Width)
		r2.Height = (Rect1.Height - Rect2.Height) - (Rect2.Y - Rect1.Y)

		Dim Ret As New List(Of Rectangle) From {r1, r2, r3, r4}
		For i = 3 To 0 Step -1
			If Ret(i).Width <= 0 Or Ret(i).Height <= 0 Then Ret.RemoveAt(i)
		Next

		Return Ret.ToArray
	End Function
    Public Function PathIntersect(pth1 As GraphicsPath, pth2 As GraphicsPath) As Boolean
        If Not pth1.GetBounds.IntersectsWith(pth2.GetBounds) Then Return False

        Do
            For Each p1 As PointF In pth1.PathPoints
                Dim x, y As Boolean
                x = pth2.PathPoints(0).X >= p1.X
                y = pth2.PathPoints(0).Y >= p1.Y

                For Each p2 As PointF In pth2.PathPoints
                    If Not (p2.X >= p1.X) Or Not (p2.Y >= p1.Y) Then Continue Do
                Next

                Return True
            Next

            Return False
        Loop
    End Function
#End Region


#End Region

End Module

#End Region
