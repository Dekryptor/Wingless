
#Region "IMPORTS"
Imports System.Runtime.InteropServices
Imports System.Drawing.Drawing2D


Public Class CustomWindow : Inherits Form
#End Region



#Region "DECLARE"
    Private Const BorderWidth As Integer = 6

    WithEvents cb As customControlBox
    Dim tp As Pen = Pens.Black : Dim tb As SolidBrush = Brushes.Black
    Dim bg1, bg2, bgx1, bgx2, bgd1, bgd2 As LinearGradientBrush
    Dim sst As Boolean = False
    Dim sht As Double = 23
    Dim bv1, bh, bh1 As LinearGradientBrush
    Public Shared T# = 0
    Dim ot% = 0
    WithEvents tmr As New Multimedia.Timer With {.Period = 1, .Resolution = 1, .Mode = 1}
    Dim fxt% = 0
#End Region

#Region "DWM"


#Region "Consts"
    Private Const WM_NCLBUTTONDOWN As Integer = &HA1
    Private HTNOWHERE As Integer = 0
    Private HTCLIENT As Integer = 1
    Private HTCAPTION As Integer = 2
    Private HTGROWBOX As Integer = 4
    Private HTSIZE As Integer = HTGROWBOX
    Private HTMINBUTTON As Integer = 8
    Private HTMAXBUTTON As Integer = 9
    Private HTLEFT As Integer = 10
    Private HTRIGHT As Integer = 11
    Private HTTOP As Integer = 12
    Private HTTOPLEFT As Integer = 13
    Private HTTOPRIGHT As Integer = 14
    Private HTBOTTOM As Integer = 15
    Private HTBOTTOMLEFT As Integer = 16
    Private HTBOTTOMRIGHT As Integer = 17
    Private HTREDUCE As Integer = HTMINBUTTON
    Private HTZOOM As Integer = HTMAXBUTTON
    Private HTSIZEFIRST As Integer = HTLEFT
    Private HTSIZELAST As Integer = HTBOTTOMRIGHT
    Dim WM_NCCALCSIZE As Integer = &H83
    Dim WM_NCHITTEST As Integer = &H84
#End Region
#Region "Fields"
    <StructLayout(LayoutKind.Sequential)>
    Public Structure MARGINS
        Public cxLeftWidth As Integer
        Public cxRightWidth As Integer
        Public cyTopHeight As Integer
        Public cyBottomHeight As Integer
        Public Sub New(ByVal Left As Integer, ByVal Right As Integer, ByVal Top As Integer, ByVal Bottom As Integer)
            Me.cxLeftWidth = Left
            Me.cxRightWidth = Right
            Me.cyTopHeight = Top
            Me.cyBottomHeight = Bottom
        End Sub
    End Structure

    Private dwmMargins As MARGINS
    Private _marginOk As Boolean
    Private _aeroEnabled As Boolean = False
#End Region
#Region "Methods"
    Public Shared Function LoWord(ByVal dwValue As Integer) As Integer
        Return dwValue And &HFFFF
    End Function
    ''' <summary>
    ''' Equivalent to the HiWord C Macro
    ''' </summary>
    ''' <param name="dwValue"></param>
    ''' <returns></returns>
    Public Shared Function HiWord(ByVal dwValue As Integer) As Integer
        Return (dwValue >> 16) And &HFFFF
    End Function
    <StructLayout(LayoutKind.Explicit)>
    Public Structure RECT
        ' Fields
        <FieldOffset(12)>
        Public bottom As Integer
        <FieldOffset(0)>
        Public left As Integer
        <FieldOffset(8)>
        Public right As Integer
        <FieldOffset(4)>
        Public top As Integer

        ' Methods
        Public Sub New(ByVal rect As Rectangle)
            Me.left = rect.Left
            Me.top = rect.Top
            Me.right = rect.Right
            Me.bottom = rect.Bottom
        End Sub

        Public Sub New(ByVal left As Integer, ByVal top As Integer, ByVal right As Integer, ByVal bottom As Integer)
            Me.left = left
            Me.top = top
            Me.right = right
            Me.bottom = bottom
        End Sub

        Public Sub [Set]()
            Me.left = InlineAssignHelper(Me.top, InlineAssignHelper(Me.right, InlineAssignHelper(Me.bottom, 0)))
        End Sub

        Public Sub [Set](ByVal rect As Rectangle)
            Me.left = rect.Left
            Me.top = rect.Top
            Me.right = rect.Right
            Me.bottom = rect.Bottom
        End Sub

        Public Sub [Set](ByVal left As Integer, ByVal top As Integer, ByVal right As Integer, ByVal bottom As Integer)
            Me.left = left
            Me.top = top
            Me.right = right
            Me.bottom = bottom
        End Sub

        Public Function ToRectangle() As Rectangle
            Return New Rectangle(Me.left, Me.top, Me.right - Me.left, Me.bottom - Me.top)
        End Function

        ' Properties
        Public ReadOnly Property Height() As Integer
            Get
                Return (Me.bottom - Me.top)
            End Get
        End Property

        Public ReadOnly Property Size() As Size
            Get
                Return New Size(Me.Width, Me.Height)
            End Get
        End Property

        Public ReadOnly Property Width() As Integer
            Get
                Return (Me.right - Me.left)
            End Get
        End Property
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
            target = value
            Return value
        End Function
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure NCCALCSIZE_PARAMS
        Public rect0 As RECT, rect1 As RECT, rect2 As RECT
        ' Can't use an array here so simulate one
        Private lppos As IntPtr
    End Structure
    <DllImport("dwmapi.dll")>
    Public Shared Function DwmDefWindowProc(ByVal hwnd As IntPtr, ByVal msg As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr, ByRef result As IntPtr) As Integer
    End Function

    <DllImport("user32.dll")>
    Public Shared Function ReleaseCapture() As Boolean
    End Function
    <DllImport("user32.dll")>
    Public Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer
    End Function

    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
        If DesignMode Then Exit Sub
        If Me.Width - BorderWidth > e.Location.X AndAlso
                    e.Location.X > BorderWidth AndAlso e.Location.Y > BorderWidth Then
            MoveControl(Me.Handle)
        End If
        MyBase.OnMouseDown(e)
    End Sub
    Private Sub MoveControl(ByVal hWnd As IntPtr)
        If DesignMode Then Exit Sub
        ReleaseCapture()
        SendMessage(hWnd, WM_NCLBUTTONDOWN, HTCAPTION, 0)
    End Sub
#End Region
#Region "Ctor"
    Public Sub New()
        Opacity = 0
        SetStyle(ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint, True)
        DoubleBuffered = True
        FormBorderStyle = FormBorderStyle.None
    End Sub
#End Region
    Protected Overloads Overrides Sub WndProc(ByRef m As Message)


        Dim result As IntPtr
        Dim dwmHandled As Integer = DwmDefWindowProc(m.HWnd, m.Msg, m.WParam, m.LParam, result)

        If dwmHandled = 1 Then
            m.Result = result
            Exit Sub
        End If

        If m.Msg = WM_NCCALCSIZE AndAlso CInt(m.WParam) = 1 Then
            Dim nccsp As NCCALCSIZE_PARAMS =
              DirectCast(Marshal.PtrToStructure(m.LParam,
              GetType(NCCALCSIZE_PARAMS)), NCCALCSIZE_PARAMS)

            ' Adjust (shrink) the client rectangle to accommodate the border:
            nccsp.rect0.top += 0
            nccsp.rect0.bottom += 0
            nccsp.rect0.left += 0
            nccsp.rect0.right += 0

            If Not _marginOk Then
                'Set what client area would be for passing to 
                'DwmExtendIntoClientArea. Also remember that at least 
                'one of these values NEEDS TO BE > 1, else it won't work.
                dwmMargins.cyTopHeight = 6
                dwmMargins.cxLeftWidth = 6
                dwmMargins.cyBottomHeight = 6
                dwmMargins.cxRightWidth = 6
                _marginOk = True
            End If

            Marshal.StructureToPtr(nccsp, m.LParam, False)

            m.Result = IntPtr.Zero


        ElseIf m.Msg = WM_NCHITTEST AndAlso CInt(m.Result) = 0 Then
            m.Result = HitTestNCA(m.HWnd, m.WParam, m.LParam)
        Else : MyBase.WndProc(m)
        End If
    End Sub
    Private Function HitTestNCA(ByVal hwnd As IntPtr, ByVal wparam _
                                      As IntPtr, ByVal lparam As IntPtr) As IntPtr

        Dim p As New Point(LoWord(CInt(lparam)), HiWord(CInt(lparam)))

        Dim topleft As Rectangle = RectangleToScreen(New Rectangle(0, 0, dwmMargins.cxLeftWidth, dwmMargins.cxLeftWidth))
        Dim topright As Rectangle = RectangleToScreen(New Rectangle(Width - dwmMargins.cxRightWidth, 0, dwmMargins.cxRightWidth, dwmMargins.cxRightWidth))
        Dim botleft As Rectangle = RectangleToScreen(New Rectangle(0, Height - dwmMargins.cyBottomHeight, dwmMargins.cxLeftWidth, dwmMargins.cyBottomHeight))
        Dim botright As Rectangle = RectangleToScreen(New Rectangle(Width - dwmMargins.cxRightWidth, Height - dwmMargins.cyBottomHeight, dwmMargins.cxRightWidth, dwmMargins.cyBottomHeight))
        Dim top As Rectangle = RectangleToScreen(New Rectangle(0, 0, Width, dwmMargins.cxLeftWidth))
        Dim cap As Rectangle = RectangleToScreen(New Rectangle(0, dwmMargins.cxLeftWidth, Width, dwmMargins.cyTopHeight - dwmMargins.cxLeftWidth))
        Dim left As Rectangle = RectangleToScreen(New Rectangle(0, 0, dwmMargins.cxLeftWidth, Height))
        Dim right As Rectangle = RectangleToScreen(New Rectangle(Width - dwmMargins.cxRightWidth, 0, dwmMargins.cxRightWidth, Height))
        Dim bottom As Rectangle = RectangleToScreen(New Rectangle(0, Height - dwmMargins.cyBottomHeight, Width, dwmMargins.cyBottomHeight))


        If topleft.Contains(p) Then Return New IntPtr(HTTOPLEFT)
        If topright.Contains(p) Then Return New IntPtr(HTTOPRIGHT)
        If botleft.Contains(p) Then Return New IntPtr(HTBOTTOMLEFT)
        If botright.Contains(p) Then Return New IntPtr(HTBOTTOMRIGHT)
        If top.Contains(p) Then Return New IntPtr(HTTOP)
        If cap.Contains(p) Then Return New IntPtr(HTCAPTION)
        If left.Contains(p) Then Return New IntPtr(HTLEFT)
        If right.Contains(p) Then Return New IntPtr(HTRIGHT)
        If bottom.Contains(p) Then Return New IntPtr(HTBOTTOM)

        Return New IntPtr(HTCLIENT)
    End Function
    Protected Overrides Sub SetBoundsCore(x As Integer, y As Integer, width As Integer, height As Integer, specified As BoundsSpecified)
        If DesignMode Then MyBase.SetBoundsCore(x, y, width, height, specified)
    End Sub
#End Region

#Region "VISUALS"

    Dim stb As Bitmap

    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
        If DesignMode Then
            If tmr.IsRunning = False Then tmr.Start()
        End If
        If fxt = 0 Then loadfx() Else closefx()
        Dim g = e.Graphics
        g.Clear(BackColor)

        bv1 = New LinearGradientBrush(New Rectangle(0, Height - CInt(sht), Width, CInt(sht)), Color.Transparent, Color.FromArgb(50, Color.Black), 90.0F)
        bh = New LinearGradientBrush(New Rectangle(0, Height - CInt(sht), Width, CInt(sht)), Color.FromArgb(60, Color.Black), Color.Transparent, 0.0F)
        bh1 = New LinearGradientBrush(New Rectangle(0, Height - CInt(sht), Width, CInt(sht)), Color.Transparent, Color.FromArgb(60, Color.Black), 0.0F)

        If rescol(BackColor) = Color.White Then
            tp = New Pen(Color.FromArgb(20, rescol(BackColor)), 0)
            g.DrawRectangle(tp, 0, 0, Width - 1, Height - 1)
        Else
            tp = New Pen(Color.FromArgb(50, rescol(BackColor)), 0)
            g.DrawRectangle(tp, 0, 0, Width - 1, Height - 1)
        End If

        If sst Then
            tb = New SolidBrush(ForeColor)
            If sst = True Then
                g.FillRectangle(tb, New Rectangle(0, Height - CInt(sht), Width, CInt(sht)))
                g.FillRectangle(bv1, New Rectangle(0, Height - CInt(sht), Width, CInt(sht)))
                g.FillRectangle(bh, New Rectangle(0, Height - CInt(sht), CInt(Width / 2) - 1, CInt(sht)))
                g.FillRectangle(bh1, New Rectangle(CInt(Width / 2) - 1, Height - CInt(sht), CInt(Width / 2) + 2, CInt(sht)))
            End If


            Dim px As Integer() = {13, 11, 9, 7}
            Dim py As Integer() = {7, 9, 11, 13}

            For i = 0 To 3
                For j = 0 To i
                    mb(col(180, Color.Black), tb)
                    g.FillRectangle(tb, Width - px(i), Height - py(j), 1, 1)
                    mb(col(80, Color.White), tb)
                    g.FillRectangle(tb, Width - px(i) - 1, Height - py(j) - 1, 1, 1)
                Next
            Next

            'dg.SmoothingMode = 2
            'g.PixelOffsetMode = 2
            g.InterpolationMode = 7

            mp(col(60, Color.Black), tp)
            g.DrawLine(tp, 1, Height - CInt(sht), Width - 2, Height - CInt(sht))
            mp(col(30, Color.Black), tp)
            g.DrawLine(tp, 1, Height - CInt(sht) + 1, Width - 2, Height - CInt(sht) + 1)


            mp(col(50, Color.Black), tp)

            mp(col(13, Color.White), tp)
            g.DrawLine(tp, 0, Height - 1, Width - 2, Height - 1)

            g.DrawLine(tp, 0, Height - CInt(sht), 0, Height - 2)
            g.DrawLine(tp, Width - 1, Height - CInt(sht), Width - 1, Height - 1)

            bv1.Dispose() : bh.Dispose() : bh1.Dispose()
            tp.Dispose() : tb.Dispose()

        End If

        If DesignMode Then
            g.DrawLine(Pens.Black, pt(Width - 103, 0), pt(Width - 103, 33))
            g.DrawLine(Pens.Black, pt(Width - 103, 33), pt(Width, 33))

        End If

    End Sub
    Sub loadfx()
        Static lf As New Interpolation
        If Not fxt = 0 Then Exit Sub
        If Opacity < 1 Then
            If ot < 500 Then
                Opacity = lf.GetValue(0, 1, ot, 500, Interpolation.Type.EaseOut, Interpolation.EasingMethods.Exponent, 1)
                ot += 1
            Else
                AccessibleDescription = ""
                Opacity = 1
                fxt = -1
                ot = 0
            End If
        End If
    End Sub
    Sub closefx()
        Static lf1 As New Interpolation
        If Not fxt = 1 Then Exit Sub
        If Opacity > 0 Then
            If ot < 500 Then
                Opacity = lf1.GetValue(1, 0, ot, 500, Interpolation.Type.EaseOut, Interpolation.EasingMethods.Exponent, 1.5)
                ot += 1
            Else
                AccessibleDescription = ""
                Opacity = 0
                fxt = -1
                ot = 0
                End
            End If
        End If
    End Sub
    Sub paintstatusbar()
        Dim g As Graphics = Graphics.FromImage(stb)

    End Sub
#End Region

#Region "FORM LOADING and CLOSING"
    Private Sub CustomWindow_BackColorChanged(sender As Object, e As EventArgs) Handles Me.BackColorChanged
        If Controls.Contains(cb) Then cb.BackColor = BackColor
    End Sub
    Private Sub me_Load(sender As Object, e As EventArgs) Handles Me.Load
        stb = New Bitmap(CInt(Width), CInt(sht))
        Opacity = 0
        If DesignMode Then Exit Sub
        fxt = 0
        AccessibleDescription = "Animated Form"
        tmr.Start()
        If DesignMode Then
            FormBorderStyle = FormBorderStyle.None
        Else FormBorderStyle = FormBorderStyle.Sizable
        End If
        MinimumSize = New Size(100, 27 + sht)
            Shadow.BackColor = Color.Black
        Shadow.Visible = True
        TransparencyKey = Color.Fuchsia
        cb = New customControlBox
        Top = (Screen.PrimaryScreen.WorkingArea.Height - Height)
        StartPosition = FormStartPosition.CenterScreen
        sst = True
        With cb
            .AccessibleDescription = "AnimateOnce"
            .Location = New Point(Width - 104, 1)
            .BackColor = Me.BackColor
        End With
        Me.Controls.Add(cb)

    End Sub
    Private Sub CustomWindow_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
        Shadow.Hide()
        Shadow.Visible = False
        'Shadow.Close()
        fxt = 1
        '_________________̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲
        'End the process  ͟___________________________________________________̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲ 
        '‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾																													
        Dim PR() As Process = Process.GetProcessesByName("magnify") '            
        For Each Process As Process In PR '                                                    
            On Error GoTo e  '    '                                                                
            Process.Kill() '                                                                                 
        Next '																 ̲ ̲  FUCK YOU MAGNIFYIER ̲͟͟͟͟͟͟͟͟͟͟͟͟͟͟͟͟͟͟͟͟͟͟ ̲̲̲̲ 
        '																	  ‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾	 
        '̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅‾̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅‾̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅‾̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅‾̅̅‾̅̅‾̅̅‾̅̅‾̅̅‾̅̅‾̅̅‾̅̅‾̅̅‾̅̅‾̅̅‾̅̅‾̅̅̅ ̅̅̅̅̅̅̅̅̅̅̅̅̅̅̅̅̅̅̅

e:
        Me.AccessibleDescription = "Animated Form"
    End Sub
    Sub cbmm() Handles cb.MouseMove
        cb.AccessibleDescription = "Animated Control"
    End Sub
    Sub cbml() Handles cb.MouseLeave
        cb.AccessibleDescription = ""
    End Sub
#End Region

#Region "SHADOW"
#Region "declare"
    Private Shadow As New ShadowForm(Me, 14)
    Private Rounding!
    Private TopHeight As Integer = 25
    Private sysbtnsW As Integer = 60
    Private BarY As Integer = 4
    Private LeftCornerW As Integer = 60
    Private RigthCornerW As Integer = 100
#End Region

    Private Sub DrawShadow() Handles Me.SizeChanged
        If DesignMode Or IsHandleCreated = False Then Exit Sub
        Dim B As New Bitmap(CInt(Shadow.Size.Width), CInt(Shadow.Size.Height))
        Dim G As Graphics = Graphics.FromImage(B)
        G.InterpolationMode = 7
        G.SmoothingMode = 2 : G.PixelOffsetMode = 2
        Shadow.BackColor = Color.Black
        Static br As SolidBrush
        With G
            Dim s = Shadow.ShadowSize
            .SetClip(New Rectangle(Shadow.ShadowSize, Shadow.ShadowSize, Width, Height - 5), CombineMode.Exclude)
            For i = s To 0 Step -1
                Rounding = 5 + s - i
                Dim pth As GraphicsPath = DM.CreateRoundRectangle(i, i, Shadow.Width - 1 - (i * 2), Shadow.Height - 1 - (i * 2) - 3, Rounding, , , , )
                mb(col(i ^ (0.111 * i), Shadow.BackColor), br)
                .FillPath(br, pth)
                pth.Dispose()
            Next


            ''(i ^ (i / 9))          Rounding +  (Shadow.ShadowSize - i)
            'Rounding = 1.5 * (Shadow.ShadowSize)
            ''(Shadow.ShadowSize + i) ^ 1.6
            'mb(col((i ^ (0.117647 * i)), Shadow.BackColor), br)




        End With

        G.Dispose()
        Shadow.SetBits(B)
        B.Dispose()

    End Sub

    Friend Class DM

        Public Shared Function CreateRoundRectangle(ByVal rectangle As Rectangle, ByVal radius As Integer, Optional ByVal TopLeft As Boolean = True, Optional ByVal TopRigth As Boolean = True, Optional ByVal BottomRigth As Boolean = True, Optional ByVal BottomLeft As Boolean = True) As GraphicsPath
            Dim path As New GraphicsPath()
            Dim l As Integer = rectangle.Left
            Dim t As Integer = rectangle.Top
            Dim w As Integer = rectangle.Width
            Dim h As Integer = rectangle.Height
            Dim d As Integer = radius << 1

            If radius <= 0 Then
                path.AddRectangle(rectangle)
                Return path
            End If

            If TopLeft Then
                path.AddArc(l, t, d, d, 180, 90)
                If TopRigth Then path.AddLine(l + radius, t, l + w - radius, t) Else path.AddLine(l + radius, t, l + w, t)
            Else
                If TopRigth Then path.AddLine(l, t, l + w - radius, t) Else path.AddLine(l, t, l + w, t)
            End If

            If TopRigth Then
                path.AddArc(l + w - d, t, d, d, 270, 90)
                If BottomRigth Then path.AddLine(l + w, t + radius, l + w, t + h - radius) Else path.AddLine(l + w, t + radius, l + w, t + h)
            Else
                If BottomRigth Then path.AddLine(l + w, t, l + w, t + h - radius) Else path.AddLine(l + w, t, l + w, t + h)
            End If

            If BottomRigth Then
                path.AddArc(l + w - d, t + h - d, d, d, 0, 90)
                If BottomLeft Then path.AddLine(l + w - radius, t + h, l + radius, t + h) Else path.AddLine(l + w - radius, t + h, l, t + h)
            Else
                If BottomLeft Then path.AddLine(l + w, t + h, l + radius, t + h) Else path.AddLine(l + w, t + h, l, t + h)
            End If

            If BottomLeft Then
                path.AddArc(l, t + h - d, d, d, 90, 90)
                If TopLeft Then path.AddLine(l, t + h - radius, l, t + radius) Else path.AddLine(l, t + h - radius, l, t)
            Else
                If TopLeft Then path.AddLine(l, t + h, l, t + radius) Else path.AddLine(l, t + h, l, t)
            End If

            path.CloseFigure()
            Return path
        End Function
        Public Shared Function CreateRoundRectangle(x As Integer, y As Integer, w As Integer, h As Integer, radius As Integer, Optional ByVal TopLeft As Boolean = True, Optional ByVal TopRigth As Boolean = True, Optional ByVal BottomRigth As Boolean = True, Optional ByVal BottomLeft As Boolean = True) As GraphicsPath
            Return CreateRoundRectangle(New Rectangle(x, y, w, h), radius, TopLeft, TopRigth, BottomRigth, BottomLeft)
        End Function

    End Class

#Region " Methods "
    ' The idea of this is not to create a outline, but to have a clipping path, to be sure to get a perfect transparent effect
    Private Function CreateOutline(Optional add As Integer = -1) As GraphicsPath
        Dim Pth As New GraphicsPath
        Dim d As Integer = 5 << 1
        Dim w% = ClientSize.Width + add, h% = ClientSize.Height + add

        If WindowState = FormWindowState.Normal Then
            With Pth
                .StartFigure()

                .AddArc(0, 0, d, d, 180, 90)
                .AddLines({New Point(LeftCornerW - TopHeight + add, 0), New Point(LeftCornerW - TopHeight + BarY + add, BarY),
                           New Point(w - RigthCornerW + TopHeight - BarY + add - 1, BarY), New Point(w - RigthCornerW + TopHeight + add - 1, 0),
                           New Point(w - Rounding, 0)})
                .AddArc(w - d, 0, d, d, 270, 90)
                .AddLines({New Point(w, h), New Point(0, h)})

                .CloseFigure()
            End With
        Else
            Pth.AddRectangle(New Rectangle(0, 0, w, h))
        End If

        Return Pth
    End Function

    Private Function CreateOutline(x As Integer, y As Integer, w As Integer, h As Integer, r As Integer) As GraphicsPath
        Dim Pth As New GraphicsPath
        Dim d As Integer = r << 1
        Dim dif As Integer = w - Me.Width

        If WindowState = FormWindowState.Normal AndAlso r > 0 Then
            With Pth
                .StartFigure()

                .AddArc(x, y, d, d, 180, 90)

                '.AddLines({ _
                '                New Point(x + LeftCornerW - TopHeight + dif, y), _
                '                New Point(x + LeftCornerW - TopHeight + BarY + dif, y + BarY), _
                '                New Point(x + w - RigthCornerW + TopHeight - BarY - 1 - dif, y + BarY), _
                '                New Point(x + w - RigthCornerW + TopHeight - 1 - dif, y), _
                '                New Point(x + w - r, y) _
                '           })

                .AddArc(x + w - d, y, d, d, 270, 90)
                .AddLines({New Point(x + w, y + h), New Point(x, y + h)})

                .CloseFigure()
            End With
        Else
            Pth.AddRectangle(New Rectangle(x, y, x + w, y + h))
        End If

        Return Pth
    End Function

    Private Function CreateLeftCorner(x%, y%, w%, h%, r%) As GraphicsPath
        Dim Pth As New GraphicsPath
        Dim d As Integer = r << 1

        With Pth
            .StartFigure()

            If r > 0 Then .AddArc(x, y, d, d, 180, 90)
            .AddLine(x + r, y, w - h, y)
            .AddLine(w - h, y, w, h)
            .AddLine(w, h, x, h)

            .CloseFigure()
        End With

        Return Pth
    End Function

    Private Function CreateRightCorner(x%, y%, w%, h%, r%) As GraphicsPath
        Dim Pth As New GraphicsPath
        Dim d As Integer = r << 1

        With Pth
            .StartFigure()

            If r > 0 Then .AddArc(x + w - d, y, d, d, 270, 90)
            .AddLines({New Point(x + w, r), New Point(x + w, h), New Point(x, h), New Point(x + h, y)})

            .CloseFigure()
        End With

        Return Pth
    End Function

    Private Function MeasurePathString(ByVal str As String, FF As FontFamily, s As Single) As Size
        Dim Pth As New GraphicsPath, Rect As RectangleF
        Pth.AddString(str, FF, FontStyle.Bold, s, New Point, StringFormat.GenericTypographic)
        Rect = Pth.GetBounds
        Pth.Dispose()

        Return New Size(Rect.Width, Rect.Height)
    End Function
#End Region ' Methods
    Public Class ShadowForm
        Inherits Form

        Private Shadows ParentForm As Form
        Public ShadowSize As Integer

#Region "Ctor"
        Public Sub New(ByVal Parent As Form, ByVal ShadowSize As Integer)

            SetStyle(ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ContainerControl Or ControlStyles.SupportsTransparentBackColor Or ControlStyles.UserMouse Or ControlStyles.ResizeRedraw Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint, True)
            SetStyle(ControlStyles.Selectable Or ControlStyles.StandardClick Or ControlStyles.StandardDoubleClick Or ControlStyles.Opaque, False)
            DoubleBuffered = True

            Me.ParentForm = Parent
            Me.ShadowSize = ShadowSize

            FormBorderStyle = Windows.Forms.FormBorderStyle.None
            ShowInTaskbar = False
            ControlBox = False
            Text = ""


            AddHandler Parent.Resize, Sub() UpdateBounds()
            AddHandler Parent.Move, Sub() UpdateBounds()
            AddHandler Parent.Layout, Sub() UpdateBounds()

            AddOwnedForm(ParentForm)


        End Sub
#End Region
        Protected Overrides Sub OnPaintBackground(e As System.Windows.Forms.PaintEventArgs)
        End Sub
        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            MyBase.OnPaint(e)
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        End Sub


        Private Shadows Sub UpdateBounds()
            Location = ParentForm.Location - New Point(ShadowSize, ShadowSize - 5)
            MyBase.Size = ParentForm.ClientSize + New Size(ShadowSize * 2 + 1, ShadowSize * 2 + 0)
        End Sub

        Protected Overrides ReadOnly Property CreateParams() As CreateParams
            Get
                Dim cParms As CreateParams = MyBase.CreateParams
                cParms.ExStyle = cParms.ExStyle Or &H80000
                'cParms.ExStyle = cParms.ExStyle Or CInt(Fix(Win32.WS_EX_TOOLWINDOW))
                'cParms.ExStyle -= CInt(Fix(Win32.WS_EX_APPWINDOW))
                Return cParms

            End Get
        End Property

        Public Sub SetBits(B As Bitmap)
            If Not IsHandleCreated Or DesignMode Then Exit Sub

            If Not Bitmap.IsCanonicalPixelFormat(B.PixelFormat) OrElse Not Bitmap.IsAlphaPixelFormat(B.PixelFormat) Then
                Throw New ApplicationException("The picture must be 32bit picture with alpha channel.")
            End If

            Dim oldBits As IntPtr = IntPtr.Zero
            Dim screenDC As IntPtr = Win32.GetDC(IntPtr.Zero)
            Dim hBitmap As IntPtr = IntPtr.Zero
            Dim memDc As IntPtr = Win32.CreateCompatibleDC(screenDC)

            Try
                Dim topLoc As New Win32.Point(Left, Top)
                Dim bitMapSize As New Win32.Size(B.Width, B.Height)
                Dim blendFunc As New Win32.BLENDFUNCTION()
                Dim srcLoc As New Win32.Point(0, 0)

                hBitmap = B.GetHbitmap(Color.FromArgb(0))
                oldBits = Win32.SelectObject(memDc, hBitmap)

                blendFunc.BlendOp = Win32.AC_SRC_OVER
                blendFunc.SourceConstantAlpha = 255
                blendFunc.AlphaFormat = Win32.AC_SRC_ALPHA
                blendFunc.BlendFlags = 0

                Win32.UpdateLayeredWindow(Handle, screenDC, topLoc, bitMapSize, memDc, srcLoc,
                 0, blendFunc, Win32.ULW_ALPHA)
            Finally
                If hBitmap <> IntPtr.Zero Then
                    Win32.SelectObject(memDc, oldBits)
                    Win32.DeleteObject(hBitmap)
                End If
                Win32.ReleaseDC(IntPtr.Zero, screenDC)
                Win32.DeleteDC(memDc)
            End Try
        End Sub
    End Class

    Friend Class Win32

        'Public Const WS_EX_TOOLWINDOW As Long = &H80L
        'Public Const WS_EX_APPWINDOW As Long = &H40000L
        Public Const AC_SRC_OVER As Byte = 0
        Public Const AC_SRC_ALPHA As Byte = 1
        Public Const ULW_ALPHA As Int32 = 2

        Public Declare Auto Function CreateCompatibleDC Lib "gdi32.dll" (hDC As IntPtr) As IntPtr
        Public Declare Auto Function GetDC Lib "user32.dll" (hWnd As IntPtr) As IntPtr

        <DllImport("gdi32.dll", ExactSpelling:=True)>
        Public Shared Function SelectObject(hDC As IntPtr, hObj As IntPtr) As IntPtr
        End Function

        <DllImport("user32.dll", ExactSpelling:=True)>
        Public Shared Function ReleaseDC(hWnd As IntPtr, hDC As IntPtr) As Integer
        End Function

        Public Declare Auto Function DeleteDC Lib "gdi32.dll" (hDC As IntPtr) As Integer
        Public Declare Auto Function DeleteObject Lib "gdi32.dll" (hObj As IntPtr) As Integer
        Public Declare Auto Function UpdateLayeredWindow Lib "user32.dll" (hwnd As IntPtr, hdcDst As IntPtr, ByRef pptDst As Point, ByRef psize As Size, hdcSrc As IntPtr, ByRef pptSrc As Point, crKey As Int32, ByRef pblend As BLENDFUNCTION, dwFlags As Int32) As Integer
        Public Declare Auto Function ExtCreateRegion Lib "gdi32.dll" (lpXform As IntPtr, nCount As UInteger, rgnData As IntPtr) As IntPtr

        <StructLayout(LayoutKind.Sequential)>
        Public Structure Size
            Public cx As Int32
            Public cy As Int32

            Public Sub New(x As Int32, y As Int32)
                cx = x
                cy = y
            End Sub
        End Structure

        <StructLayout(LayoutKind.Sequential, Pack:=1)>
        Public Structure BLENDFUNCTION
            Public BlendOp As Byte
            Public BlendFlags As Byte
            Public SourceConstantAlpha As Byte
            Public AlphaFormat As Byte
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Public Structure Point
            Public x As Int32
            Public y As Int32

            Public Sub New(x As Int32, y As Int32)
                Me.x = x
                Me.y = y
            End Sub
        End Structure
    End Class

#End Region

#Region "TIMER"
    Sub tck() Handles tmr.Tick
        T += 1
        If T >= 25000 Then T = 0
        If AccessibleDescription = "Animated Form" Then Invalidate()

        For Each c As Control In Me.Controls
            If c.AccessibleDescription = "Animated Control" Then c.Invalidate()
            If c.AccessibleDescription = "AnimateOnce" Then
                c.Invalidate()
                c.AccessibleDescription = ""
            End If

            If TypeOf c Is Panel Or c.AccessibleDescription = "AnimateContents" Then invcon(c)
        Next
    End Sub
    Sub invcon(pc As Control)
        For Each c As Control In pc.Controls
            ' MsgBox("dw")
            If c.AccessibleDescription = "Animated Control" Then c.Invalidate()
            If TypeOf c Is Panel Then invcon(c)
        Next
    End Sub

End Class ' DISPOSE done
#End Region



















































'Dim g = e.Graphics
'		bv1 = New LinearGradientBrush(new rectangle(0, Height - CInt(sht), Width, CInt(sht)), Color.Transparent, Color.FromArgb(50, Color.Black), 90.0F)
'		bh = New LinearGradientBrush(new rectangle(0, Height - CInt(sht), Width, CInt(sht)), Color.FromArgb(60, Color.Black), Color.Transparent, 0.0F)
'		bh1 = New LinearGradientBrush(new rectangle(0, Height - CInt(sht), Width, CInt(sht)), Color.Transparent, Color.FromArgb(60, Color.Black), 0.0F)

'		If rescol(BackColor) = Color.White Then
'			tp = New Pen(Color.FromArgb(20, rescol(BackColor)), 0)
''		g.DrawRectangle(tp, 0, 0, Width - 1, Height - 1)
'		Else
'			tp = New Pen(Color.FromArgb(60, rescol(BackColor)), 0)
''		g.DrawRectangle(tp, 0, 0, Width - 1, Height - 1)
'			tp = New Pen(DM.Invert(rescol(BackColor)), 0)
''		g.DrawRectangle(tp, 1, 1, Width - 3, Height - 2)
'		End If




'		If sst Then
'			tb = New SolidBrush(ForeColor)
'			If sst = True Then
'				g.FillRectangle(tb, new rectangle(0, Height - CInt(sht), Width, CInt(sht)))
'				g.FillRectangle(bv1, new rectangle(0, Height - CInt(sht), Width, CInt(sht)))
'				g.FillRectangle(bh, new rectangle(0, Height - CInt(sht), CInt(Width / 2) - 1, CInt(sht)))
'				g.FillRectangle(bh1, new rectangle(CInt(Width / 2) - 1, Height - CInt(sht), CInt(Width / 2) + 2, CInt(sht)))
'			End If


'Dim px As Integer() = {13, 11, 9, 7}
'Dim py As Integer() = {7, 9, 11, 13}

'			For i = 0 To 3
'				For j = 0 To i
'					tb = New SolidBrush(Color.FromArgb(180, Color.Black))
'					g.FillRectangle(tb, Width - px(i), Height - py(j), 1, 1)
'					tb = New SolidBrush(Color.FromArgb(80, Color.White))
'					g.FillRectangle(tb, Width - px(i) - 1, Height - py(j) - 1, 1, 1)
'				Next
'			Next




'			tp = New Pen(Color.FromArgb(40, Color.Black))
'			g.DrawLine(tp, 0, Height - CInt(sht), Width - 1, Height - CInt(sht))
'			tp = New Pen(Color.FromArgb(30, Color.Black))
'			g.DrawLine(tp, 0, Height - CInt(sht) + 1, Width - 1, Height - CInt(sht) + 1)


'			tp = New Pen(Color.FromArgb(13, Color.White), 0)
'			g.DrawLine(tp, 0, Height - 1, Width - 2, Height - 1)
'			g.DrawLine(tp, 0, Height - CInt(sht), 0, Height - 2)
'			g.DrawLine(tp, Width - 1, Height - CInt(sht), Width - 1, Height - 1)

'			bv1.Dispose() : bh.Dispose() : bh1.Dispose()
'			tp.Dispose() : tb.Dispose()

'		End If




'		tp = New Pen(ForeColor, 2)
'		If DesignMode Then Exit Sub
'		Select Case cb.l
'			Case 0
'				g.DrawLine(tp, Width - 86, 1, Width - 85 + 28 - 1, 1)
'			Case 1
'				g.DrawLine(tp, Width - 86 + 28 + 1, 1, Width - 85 + 28 + 28, 1)
'			Case 2
'				g.DrawLine(tp, Width - 86 + 28 + 28, 1, Width - 85 + 28 + 28 + 28 - 1, 1)
'			Case 3
'		End Select
