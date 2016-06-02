Imports System.Drawing.Drawing2D

Class custom_Material_Button
    Inherits ThemeControl154
    Private x As Single = 75.0
    Private y As Single = 75.0
    Private isd As Integer = 0
    Dim total As Single
    Dim d(8) As Single
    Dim t% = 0
    Dim hc As Color = col(0, 0)
    Private w! = 0.0
    Dim iso! = 0
    Private fc As Color = col(100, 0) : <System.ComponentModel.Category("Appearance")> Public Property FloodColor As Color
        Get
            Return fc
        End Get
        Set(value As Color)
            fc = value
            Invalidate()
        End Set
    End Property

    Sub New()
        Height = 150
        Width = 150
    End Sub


    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        x = e.X
        y = e.Y
        d = {x,
             y,
             Width - x,
             Height - y,
             (x ^ 2 + y ^ 2) ^ (1 / 2),
             ((Width - x) ^ 2 + y ^ 2) ^ (1 / 2),
             (x ^ 2 + (Height - y) ^ 2) ^ (1 / 2),
             ((Width - x) ^ 2 + (Height - y) ^ 2) ^ (1 / 2)}
        Dim bigi As Byte = 0
        For i = 0 To 7
            If d(i) > d(bigi) Then bigi = i
        Next
        total = d(bigi) + 2
        isd = 1
        Me.AccessibleDescription = "Animated Control"
    End Sub
    Protected Overrides Sub PaintHook()

        calc()

        G.SmoothingMode = 2 : G.InterpolationMode = 7 : G.TextRenderingHint = 5 : G.PixelOffsetMode = 2
        G.Clear(BackColor)

        G.FillRectangle(mb(hc), rct(Me))

        G.FillEllipse(mb(col(Math.Abs(150 * (1 - (w / (total + 0.001)))), fc)), rct(x - w, y - w, w * 2, w * 2))
        DrawText(mb(ForeColor), HorizontalAlignment.Center, 0, 0)
    End Sub


    Sub calc()

        If isd = 1 Then
            If Not t >= 1000 Then
                t += 1
                w = GetValue(0, total, t, 1000, 1, 0)
            Else
                t = 0
                w = 0.0
                isd = 0
                If iso = 0 Then Me.AccessibleDescription = "Animated Controll"
            End If
        End If

        If iso = 1 Then
            If t1 < 500 Then
                t1 += 1
                hc = col(GetValue(0, 30, t1, 500, Type.SmoothStep, EasingMethods.Critical_Damping, 1), rescol(BackColor))
            Else
                t1 = 0 : iso = 0
                If isd = 0 Then Me.AccessibleDescription = "Animated Controll"
            End If
        ElseIf iso = 2 Then
            If t1 < 500 Then
                t1 += 1
                hc = col(GetValue(30, 0, t1, 500, Type.SmoothStep, EasingMethods.Critical_Damping, 1), rescol(BackColor))
            Else
                t1 = 0 : iso = 0
                If isd = 0 Then Me.AccessibleDescription = "Animated Controll"
            End If
        End If

    End Sub

    Dim t1% = 0

    Protected Overrides Sub OnMouseEnter(e As EventArgs)
        MyBase.OnMouseEnter(e)
        iso = 1
        Me.AccessibleDescription = "Animated Control"
    End Sub
    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        iso = 2
        Me.AccessibleDescription = "Animated Control"
    End Sub
End Class






Class CustomButton
    Inherits ThemeControl154
    Dim lgb As Brush
    Dim p As Pen
    Private O As _Options
    Private backc As Color
    <Flags()>
    Enum _Options
        Original
        Soft
        Rounded
    End Enum
    <System.ComponentModel.Category("Behavior")>
    Public Property Style As _Options
        Get
            Return O
        End Get
        Set(value As _Options)
            O = value
            Invalidate()
        End Set
    End Property
    Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
        Font = New Font("Segoe UI", 9)
    End Sub
    Protected Overrides Sub OnCreateControl()
        MyBase.OnCreateControl()
        backc = BackColor
    End Sub

    Protected Overrides Sub PaintHook()
        G.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit

        If O = _Options.Original Then
            ori()
            DrawText(New SolidBrush(ForeColor), HorizontalAlignment.Center, 0, 0)
        ElseIf O = _Options.Rounded Then
            rnd()
            DrawText(New SolidBrush(ForeColor), HorizontalAlignment.Center, 0, d)
        Else : soft()
        End If
    End Sub





    Private Sub ori()


        With G
            .Clear(backc)

            .SmoothingMode = 2 : .PixelOffsetMode = 2

            If rescol(Parent.BackColor) = Color.Black Then
                mp(col(100, Color.Black))
                .DrawRectangle(p, New Rectangle(0, 0, Width, Height - 0))
                mp(col(60, Color.Black))
                .DrawLine(p, 2, Height, Width - 2, Height)
                .DrawLine(p, 1, Height - 1, Width - 1, Height - 1)
                mp(col(100, Color.Black))
                .DrawLine(p, 1, Height - 1, Width - 1, Height - 1)
            Else
                mp(col(205, Color.Black))
                .DrawRectangle(p, New Rectangle(0, 0, Width, Height - 0))
                mp(col(65, Color.Black))
                .DrawLine(p, 2, Height, Width - 2, Height)
                .DrawLine(p, 1, Height - 1, Width - 1, Height - 1)
                mp(col(100, Color.Black))
                .DrawLine(p, 1, Height - 1, Width - 1, Height - 1)
            End If

            px(Parent.BackColor, 0, 0, G)
            px(Parent.BackColor, Width - 1, 0, G)
            px(Parent.BackColor, 0, Height - 1, G)
            px(Parent.BackColor, Width - 1, Height - 1, G)

        End With
    End Sub


    Sub mp(c As Byte)
        p = New Pen(col(c))
    End Sub
    Sub mp(c As Color)
        p = New Pen(c)
    End Sub
    Sub mp(a As Byte, c As Color)
        p = New Pen(col(a, c))
    End Sub
    Function bwp(c As Byte) As Color
        Dim a As Color() = {col(c), BackColor}
        Return blend(a)
    End Function
    Sub px(c As Color, x!, y!, g As Graphics)
        lgb = New SolidBrush(c)
        g.FillRectangle(lgb, x, y, 1, 1)
        lgb.Dispose()
    End Sub



    Private Sub soft()
        G.Clear(BackColor)
        G.SmoothingMode = SmoothingMode.AntiAlias
        G.InterpolationMode = InterpolationMode.HighQualityBicubic
        G.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit

        G.SetClip(CreateRound(New Rectangle(2, 0, Width - 4, Height - 3), 6), CombineMode.Exclude)
        lgb = New SolidBrush(Color.FromArgb(80, Color.Black))
        G.FillPath(lgb, CreateRound(New Rectangle(0, 0, Width - 1, Height - 1), 10))
        lgb = New SolidBrush(Color.FromArgb(20, Color.Black))
        G.FillPath(lgb, CreateRound(New Rectangle(0, 0, Width - 1, Height), 10))
        G.ResetClip()

        Select Case State
            Case MouseState.None

                G.FillPath(Brushes.White, CreateRound(New Rectangle(1, 0, Width - 3, Height - 3), 6))
                p = New Pen(Color.FromArgb(140, 140, 140), 1)
                G.DrawPath(p, CreateRound(New Rectangle(1, 0, Width - 3, Height - 3), 6))

            Case MouseState.Over

                G.DrawPath(Pens.Black, CreateRound(New Rectangle(1, 0, Width - 3, Height - 3), 6))

                lgb = New LinearGradientBrush(New Rectangle(1, 1, Width - 3, Height - 4), Color.FromArgb(70, 70, 70), Color.FromArgb(5, 5, 5), 90.0F)
                G.FillPath(lgb, CreateRound(New Rectangle(1, 1, Width - 3, Height - 4), 6))

            Case MouseState.Down
                G.FillPath(Brushes.Black, CreateRound(New Rectangle(1, 0, Width - 3, Height - 3), 6))

        End Select

        If Enabled Then
            Select Case State
                Case MouseState.None
                    DrawText(New SolidBrush(Color.Black), HorizontalAlignment.Center, 0, 0)
                Case Else
                    DrawText(New SolidBrush(Color.White), HorizontalAlignment.Center, 0, 0)
            End Select
        Else
            DrawText(New SolidBrush(Color.Gray), HorizontalAlignment.Center, 0, 0)
        End If

    End Sub

    Private Sub rnd()
        G.Clear(Parent.BackColor)
        G.InterpolationMode = 7
        G.SmoothingMode = 2
        G.PixelOffsetMode = PixelOffsetMode.HighQuality
        'G.CompositingMode = CompositingMode.SourceOver

        G.DrawImage(DrawRoundRectangle(New Rectangle(0, 0, Width, Height), 13, 2, Color.FromArgb(40, Color.Black), Color.FromArgb(0, 63, 63, 63), -0.5), 6, 6 + d, Width - 11, Height - 8 - d)
        G.DrawImage(DrawRoundRectangle(New Rectangle(0, 0, Width, Height), 13, 2, Color.FromArgb(100, Color.Black), Color.FromArgb(0, 63, 63, 63), -0.5), 7, 6 + d, Width - 12, Height - 9 - d)

        G.DrawImage(DrawRoundRectangle(New Rectangle(5, 5, Width - 10, Height - 10), 8, 4, backc, Color.FromArgb(0, 63, 63, 63), 0), 5, 5 + d, Width - 10, Height - 10)

        G.PixelOffsetMode = PixelOffsetMode.None
        'G.DrawLine(New Pen(Color.FromArgb(20, Color.White)), 10, 6, Width - 10, 6)
        'G.DrawLine(New Pen(Color.FromArgb(100, Invert(rescol(Parent.BackColor)))), 9, 5, Width - 9, 5)

        ' G.DrawLine(Pens.Green, 8, 32 + 5, Width - 8, 32 + 5)
        'G.DrawLine(New Pen(Color.FromArgb(15, rescol(Parent.BackColor))), 10, Height - 10, Width - 10, Height - 10)

        G.DrawLine(New Pen(Color.FromArgb(14, rescol(Parent.BackColor))), 9, 6 + d, Width - 10, 6 + d)
        G.DrawLine(New Pen(col(200, backc)), 8, 7 + d, 8, Height - 13 + d)
        G.DrawLine(New Pen(col(200, backc)), Width - 9, 7 + d, Width - 9, Height - 13 + d)
        G.DrawLine(New Pen(col(30, Color.Black)), 6, 8 + d, 6, Height - 9 + d)


    End Sub
    Dim d! = 0

    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
        MyBase.OnMouseMove(e)
        If State = MouseState.Down Then Exit Sub
        If rescol(BackColor) = Color.Black Then
            backc = bwp(240)
        Else
            backc = bwp(100)
        End If
        d = 0.4
        Invalidate()
    End Sub
    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        MyBase.OnMouseLeave(e)
        backc = BackColor
        d = 0
        Invalidate()
    End Sub
    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        If O = _Options.Original Then
            If rescol(BackColor) = Color.Black Then
                backc = bwp(100)
            Else
                backc = bwp(0)
            End If
        End If
        d = 2
        Invalidate()
    End Sub
    Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
        MyBase.OnMouseUp(e)
        d = 0
        Invalidate()
    End Sub

End Class 'DISPOSE done
































'G.Clear(Parent.BackColor)
'G.SmoothingMode = SmoothingMode.AntiAlias
'G.InterpolationMode = InterpolationMode.HighQualityBicubic
'G.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit


'Select Case State
'	Case MouseState.None
'              LGB = New LinearGradientBrush(New Rectangle(0, 0, Width - 1, Height - 2), gtn, GBN, 90.0F)
'              'LGB = New SolidBrush(GBN)
'          Case MouseState.Over
'              LGB = New SolidBrush(GBO)
'              'LGB = New LinearGradientBrush(New Rectangle(0, 0, Width - 1, Height - 2), GTO, GBO, 90.0F)
'          Case Else
'              LGB = New SolidBrush(GBD)
'              'LGB = New LinearGradientBrush(New Rectangle(0, 0, Width - 1, Height - 2), GTD, GBD, 90.0F)
'      End Select


'If Not Enabled Then
'	LGB = New LinearGradientBrush(New Rectangle(0, 0, Width - 1, Height - 1), gtn, GBN, 90.0F)
'End If
'Dim buttonpath As GraphicsPath = CreateRound(Rectangle.Round(New Rectangle(0, 0, Width - 1, Height - 1)), 3)
''Dim buttonpath As GraphicsPath = createround(Rectangle.Round(LGB.Rectangle), 3)
'Dim buttonpath2 As GraphicsPath = CreateRound(Rectangle.Round(New Rectangle(1, 1, Width - 3, Height - 4)), 3)
'Dim bog = New LinearGradientBrush(New Rectangle(0, 0, Width, Height), Color.FromArgb(120, 120, 120), Color.FromArgb(60, 60, 60), 90.0F)


'G.FillPath(bog, buttonpath)
''G.FillPath(LGB, createround(Rectangle.Round(LGB.Rectangle), 3))
'G.FillPath(LGB, CreateRound(Rectangle.Round(New Rectangle(0, 0, Width - 1, Height - 2)), 3))
'If Not Enabled Then G.FillPath(New SolidBrush(Color.FromArgb(50, Color.White)), CreateRound(Rectangle.Round(New Rectangle(0, 0, Width - 1, Height - 2)), 3))

'G.SetClip(buttonpath)

'LGB = New LinearGradientBrush(New Rectangle(0, 0, Width, Height / 6), Color.FromArgb(80, Color.White), Color.Transparent, 90.0F)
''G.FillRectangle(LGB, Rectangle.Round(LGB.Rectangle))
'G.ResetClip()


''G.DrawPath(New Pen(Bo), buttonpath)
''G.DrawPath(New Pen(Parent.BackColor), buttonpath2)


'If Enabled Then
'	Select Case State
'		Case MouseState.None
'			DrawText(New SolidBrush(TN), HorizontalAlignment.Center, 1, 0)
'		Case Else
'			DrawText(New SolidBrush(TDO), HorizontalAlignment.Center, 1, 0)
'	End Select
'Else
'	DrawText(New SolidBrush(TD), HorizontalAlignment.Center, 1, 0)
'End If