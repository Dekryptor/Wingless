Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports System.Math

Class CustomCheckbox

    Inherits ThemeControl154

    Sub New()
        SetStyle(ControlStyles.ResizeRedraw And ControlStyles.AllPaintingInWmPaint And ControlStyles.OptimizedDoubleBuffer And ControlStyles.ResizeRedraw And ControlStyles.UserPaint, True)

        LockHeight = 17
        Font = New Font("Segoe UI", 9)
        GTN = col(237, 237, 237)
        GTO = col(242, 242, 242)
        GTD = col(235, 235, 235)
        GBN = col(230, 230, 230)
        GBO = col(235, 235, 235)
        GBD = col(223, 223, 223)
        Bo = col(167, 167, 167)
        Width = 160
    End Sub

    Private X As Integer
    Dim GTN, GTO, GTD, GBN, GBO, GBD, Bo As Color

    Protected Overrides Sub OnMouseMove(e As System.Windows.Forms.MouseEventArgs)
        MyBase.OnMouseMove(e)
        X = e.Location.X
        Invalidate()
    End Sub

    Protected Overrides Sub PaintHook()
        G.Clear(BackColor)
        Dim LGB As LinearGradientBrush
        G.SmoothingMode = SmoothingMode.HighQuality
        G.CompositingQuality = CompositingQuality.HighQuality
        G.InterpolationMode = InterpolationMode.HighQualityBicubic

        Select Case State
            Case MouseState.None
                LGB = New LinearGradientBrush(New Rectangle(0, 0, 12, 12), GTN, GBN, 90.0F)
            Case MouseState.Over
                LGB = New LinearGradientBrush(New Rectangle(0, 0, 12, 12), GTO, GBO, 90.0F)
            Case Else
                LGB = New LinearGradientBrush(New Rectangle(0, 0, 12, 12), GTD, GBD, 90.0F)
        End Select


        Dim buttonpath As GraphicsPath = CreateRound(Rectangle.Round(LGB.Rectangle), 5)
        G.FillPath(LGB, CreateRound(Rectangle.Round(LGB.Rectangle), 3))
        G.SetClip(buttonpath)
        LGB = New LinearGradientBrush(New Rectangle(0, 0, 12, 5), Color.FromArgb(150, Color.White), Color.Transparent, 90.0F)
        G.FillRectangle(LGB, Rectangle.Round(LGB.Rectangle))
        G.ResetClip()
        G.DrawPath(New Pen(Bo), buttonpath)

        DrawText(New SolidBrush(ForeColor), 17, -2)


        If Checked Then

            'buttonpath = createround(Rectangle.Round(LGB.Rectangle), 5)
            'G.FillPath(LGB, createround(Rectangle.Round(LGB.Rectangle), 3))
            G.DrawPath(New Pen(Parent.BackColor), buttonpath)
            LGB = New LinearGradientBrush(New Rectangle(0, 0, 12, 12), Color.FromArgb(20, 160, 255), Color.FromArgb(0, 120, 204), 90.0F)
            G.FillRectangle(LGB, Rectangle.Round(LGB.Rectangle))

            'G.DrawImage(check, new rectangle(2, 3, check.Width, check.Height))
            G.DrawLine(New Pen(Parent.BackColor, 2), New Point(3, 8), New Point(7, 12))
            G.DrawLine(New Pen(Parent.BackColor, 2), New Point(7, 12), New Point(13, 2))


        End If
    End Sub

    Private _Checked As Boolean
    Property Checked() As Boolean
        Get
            Return _Checked
        End Get
        Set(ByVal value As Boolean)
            _Checked = value
            Invalidate()
        End Set
    End Property

    Protected Overrides Sub OnMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
        _Checked = Not _Checked
        RaiseEvent CheckedChanged(Me)
        MyBase.OnMouseDown(e)
    End Sub

    Event CheckedChanged(ByVal sender As Object)

End Class 'DISPOSE LEFT


<ComponentModel.DefaultEvent("CheckedChanged")> Class customRadio : Inherits ThemeControl154
    Dim y! = -16 : Public fy! = -16 : Public iy! = -16
    Dim x! = -16 : Public fx! = -16 : Public ix! = -16
    Dim rd As New Bitmap(16 * 4, 16 * 4)
    Dim rdb As New Bitmap(16 * 4, 16 * 4)
    Dim LGB : Dim lg2

#Region "props"
    Dim _checked As Boolean = False
    Public Event CheckedChanged(sender As Object)
    Property Checked() As Boolean
        Get
            Return _checked
        End Get
        Set(ByVal value As Boolean)
            _checked = value
            RaiseEvent CheckedChanged(Me)
            If value = True Then
                fx = 0 : fy = 0
            Else
                ix = 0 : iy = 0
            End If
            invalidatecontrols()
            AccessibleDescription = "Animated Control"
        End Set
    End Property


#End Region
    Sub New()
        Checked = False
        LockHeight = 16
    End Sub
    Protected Overrides Sub OnCreateControl()
        MyBase.OnCreateControl()
        With Graphics.FromImage(rd)
            .SmoothingMode = 2 : .InterpolationMode = 7 : .CompositingQuality = 2
            .Clear(Color.Transparent)
            LGB = New LinearGradientBrush(New Rectangle(0, 0, 64, 64), col(51, rescol(Parent.BackColor)), col(75, rescol(Parent.BackColor)), 90.0F)
            .FillEllipse(LGB, 0, 0, 60, 60)
            ' .DrawArc(New Pen(col(255, Color.Black)), 0, 8, 60, 60, -180, 180)
            .DrawArc(New Pen(col(160, Invert(rescol(Parent.BackColor)))), 0, 0, 60, 60, 0, 360)
        End With
        With Graphics.FromImage(rdb)
            .SmoothingMode = 2 : .InterpolationMode = 7
            .Clear(Color.Transparent)
            .FillEllipse(New SolidBrush(col(200, rescol(Parent.BackColor))), 12, 12, 52 - 15, 51 - 15)
        End With
    End Sub

    Protected Overrides Sub PaintHook()
        calc()

        With G
            .SmoothingMode = 2 : .InterpolationMode = 7 : .TextRenderingHint = 5
            .Clear(Parent.BackColor)
            .DrawImage(rd, 0, 0, 16, 16)
            Dim p As New GraphicsPath : p.AddEllipse(2, 2, 12, 12)
            .SetClip(p)
            .DrawImage(rdb, x, y, 16, 16)
            .ResetClip()
            .DrawString(Text, New Font("Segoe UI", 10.5), New SolidBrush(ForeColor), 17, -2)
        End With
    End Sub



    Public t! = 0
    Sub calc()
        If t < 300 Then
            t += 1
            x = GetValue(ix, fx, t, 300, Interpolation.Type.Smootherstep, 1)
            y = GetValue(iy, fy, t, 300, Interpolation.Type.Smootherstep, 1)
        Else : AccessibleDescription = " "
        End If
    End Sub

    Protected Overrides Sub OnClick(e As EventArgs)
        MyBase.OnClick(e)
        t = 0
        invalidatecontrols()
        Checked = True
        AccessibleDescription = "Animated Control"

    End Sub
    Sub invalidatecontrols()
        If Not IsHandleCreated Then Return
        For Each c As Control In Parent.Controls
            If TypeOf c Is customRadio AndAlso c IsNot Me AndAlso DirectCast(c, customRadio).Checked = True Then
                Select Case c.Left
                    Case Is < Left : ix = -16 : DirectCast(c, customRadio).fx = 16
                    Case Is > Left : ix = 16 : DirectCast(c, customRadio).fx = -16
                    Case Else : ix = 0 : DirectCast(c, customRadio).fx = 0
                End Select
                Select Case c.Top
                    Case Is < Top : iy = -16 : DirectCast(c, customRadio).fy = 16
                    Case Is > Top : iy = 16 : DirectCast(c, customRadio).fy = -16
                    Case Else : iy = 0 : DirectCast(c, customRadio).fy = 0
                End Select

                fx = 0
                fy = 0
                DirectCast(c, customRadio).ix = 0
                DirectCast(c, customRadio).iy = 0

                DirectCast(c, customRadio).t = 0
                DirectCast(c, customRadio).Checked = False
                DirectCast(c, customRadio).AccessibleDescription = "Animated Control"
            End If
        Next
    End Sub
End Class


#Region "customToggle"
<DefaultEvent("CheckedChanged"), DesignTimeVisible(False)> Class toggler : Inherits ThemeControl154
#Region "Declare"
    Dim x! : Dim t! : Dim fd% = 0
    Dim p As Pen : Dim br As SolidBrush : Dim hb, hb1, hb2 As New Bitmap(18 * 8, 30 * 8)
    Public Event CheckedChanged()
    Dim mdown As Boolean = False
    Private hr As Rectangle
    Private _Checked As Boolean : Property Checked() As Boolean
        Get
            Return _Checked
        End Get
        Set(ByVal value As Boolean)
            _Checked = value
            If _Checked Then fd = 1 Else fd = 0
            If fd = 1 Then hb = hb2 Else hb = hb1
            RaiseEvent CheckedChanged()
            If Not DesignMode Then AccessibleDescription = "Animated Control"
        End Set
    End Property
    Private _fc As Color = col(0, 120, 204) : <Browsable(True), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)> Overrides Property ForeColor() As Color
        Get
            Return _fc
        End Get
        Set(ByVal value As Color)
            _fc = value
            AccessibleDescription = ""
            paintbits(0, hb1)
            paintbits(1, hb2)
            If fd = 1 Then hb = hb2 Else hb = hb1
            Invalidate()
        End Set
    End Property
#End Region

    Sub New(HandleColor As Color, Checked As Boolean)
        LockHeight = 30 : LockWidth = 40
        ForeColor = HandleColor
        Me.Checked = Checked
    End Sub

    Protected Overrides Sub PaintHook()
        animlogic()


        G.Clear(Parent.BackColor)
        G.SmoothingMode = 2 : G.InterpolationMode = 7
        If fd = 1 Then mb(ic(_fc, 50), br) Else mb(col(200, rescol(Parent.BackColor)), br)
        G.PixelOffsetMode = 2

        G.FillRectangle(br, New Rectangle(6, 10, 28, 10))
        G.SetClip(New Rectangle(6, 10, 28, 10), CombineMode.Exclude)
        G.FillEllipse(br, New Rectangle(1, 10, 10, 10))
        G.FillEllipse(br, New Rectangle(29, 10, 10, 10))
        G.ResetClip()

        hr = rct(x, 4, 18, 30)
        G.DrawImage(hb, hr)

    End Sub

#Region "func"
    Dim tx! = 0
    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        t = 0
        If e.Button = Windows.Forms.MouseButtons.Left Then
            mdown = 1
        End If
    End Sub
    Protected Overrides Sub OnClick(e As EventArgs)
        MyBase.OnClick(e)
        Checked = Not Checked
    End Sub
    Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
        MyBase.OnMouseUp(e)
        mdown = False
    End Sub
    Protected Overrides Sub OnMouseMove(ByVal e As MouseEventArgs)
        MyBase.OnMouseMove(e)
        If mdown Then
            t = 0
            Me.AccessibleDescription = ""
            x = Max(0, Min(22, e.X)) - tx
            If x - 9 > 20 AndAlso fd = 0 Then
                fd = 1
                Checked = True
                hb = hb2
            ElseIf x - 9 <= 20 AndAlso fd = 1 Then
                fd = 0
                Checked = False
                hb = hb1
            End If
            Invalidate()

        End If
    End Sub
    Protected Overrides Sub OnMouseLeave(ByVal e As EventArgs)
        MyBase.OnMouseLeave(e)
        mdown = False
        If x > 11 Then
            fd = 1
            Checked = True
            hb = hb2
        Else
            fd = 0
            Checked = 0
            hb = hb1
        End If
        Invalidate()
    End Sub
    Private Sub animlogic()
        If Not AccessibleDescription = "Animated Control" Then Exit Sub
        If t < 500 Then
            x = GetValue(x, fd * (Width - 18), t, 500, Interpolation.Type.EaseInOut, Interpolation.EasingMethods.Regular, 5)
            t += 1
        Else
            AccessibleDescription = ""
            t = 0
        End If
    End Sub
    Protected Overrides Sub OnCreateControl()
        MyBase.OnCreateControl()
        paintbits(0, hb1)
        paintbits(1, hb2)
        'Invalidate()
    End Sub
    Private Sub paintbits(fd%, ByRef hb As Bitmap)
        Dim bg As Graphics = Graphics.FromImage(hb)
        With bg
            .Clear(col(0, 0))
            .InterpolationMode = 7 : .SmoothingMode = 2 : .PixelOffsetMode = 2

            mb(col(48, 0), br)
            .FillEllipse(br, 4, 8, 18 * 8 - 4, 18 * 8)
            mb(col(40, 0), br)
            .FillEllipse(br, 4, 16, 18 * 8 - 4, 18 * 8)
            mb(col(24, 0), br)
            .FillEllipse(br, 4, 24, 18 * 8 - 4, 18 * 8)



            If fd = 1 Then mb(_fc, br) Else mb(col(120), br)
            .FillEllipse(br, 1, 1, 18 * 8 - 2, 18 * 8 - 2)
        End With
    End Sub
    Public Function getfd() As Integer
        Return fd
    End Function
#End Region

End Class
<ComponentModel.DefaultEvent("CheckedChanged")> Class customToggle : Inherits ThemeControl154
#Region "Declare"
    Public Event CheckedChanged(sender As Object)
    Private t As New toggler(_tc, _chk)
    Private tb As SolidBrush
    Private _tc As Color = col(0, 90, 190) : <Category("Appearance")> Public Property ToggleColor As Color
        Get
            Return _tc
        End Get
        Set(value As Color)
            _tc = value
            t.ForeColor = _tc
        End Set
    End Property
    Private _chk As Boolean = False : <Category("Behavior")> Public Property Checked As Boolean
        Get
            Return _chk
        End Get
        Set(value As Boolean)
            _chk = value
            t.Checked = _chk
            RaiseEvent CheckedChanged(Me)
        End Set
    End Property
#End Region
    Sub New()
        SetStyle(ControlStyles.ResizeRedraw, True)
        LockHeight = 30
        AccessibleDescription = "AnimateContents"
        Font = New Font("Segoe UI", 9)
    End Sub
    Protected Overrides Sub OnCreateControl()
        MyBase.OnCreateControl()
        t.Left = Width - 60
        Controls.Add(t)
        t.Show()
    End Sub
    Protected Overrides Sub PaintHook()
        G.Clear(BackColor)
        mb(col(_tc, t.getfd(), ForeColor, LimitingType.NotLessThan), tb)
        G.DrawString(Text, Font, tb, 5, 7)
        tb.Dispose()
    End Sub
    Protected Overrides Sub OnClick(e As EventArgs)
        MyBase.OnClick(e)
        Checked = Not Checked
    End Sub
End Class
#End Region