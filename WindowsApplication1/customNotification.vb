Imports System.Drawing.Drawing2D
Imports System.Runtime.InteropServices

Public Class LayeredForm
    Inherits Form

#Region "DECLARE"
    Private Const BorderWidth As Integer = 14
    Private Const WM_NCLBUTTONDOWN As Integer = &HA1
    Private Const HTCAPTION As Integer = 2
#End Region

#Region " API Stuff "
    Private Const AC_SRC_OVER As Byte = 0
    Private Const AC_SRC_ALPHA As Byte = 1
    Private Const ULW_ALPHA As Int32 = 2

    Private Declare Auto Function CreateCompatibleDC Lib "gdi32.dll" (hDC As IntPtr) As IntPtr
    Private Declare Auto Function GetDC Lib "user32.dll" (hWnd As IntPtr) As IntPtr

    <DllImport("gdi32.dll", ExactSpelling:=True)>
    Private Shared Function SelectObject(hDC As IntPtr, hObj As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", ExactSpelling:=True)>
    Private Shared Function ReleaseDC(hWnd As IntPtr, hDC As IntPtr) As Integer
    End Function

    Private Declare Auto Function DeleteDC Lib "gdi32.dll" (hDC As IntPtr) As Integer
    Private Declare Auto Function DeleteObject Lib "gdi32.dll" (hObj As IntPtr) As Integer
    Private Declare Auto Function UpdateLayeredWindow Lib "user32.dll" (hwnd As IntPtr, hdcDst As IntPtr, ByRef pptDst As Point, ByRef psize As Size, hdcSrc As IntPtr, ByRef pptSrc As Point, crKey As Int32, ByRef pblend As BLENDFUNCTION, dwFlags As Int32) As Integer
    Private Declare Auto Function ExtCreateRegion Lib "gdi32.dll" (lpXform As IntPtr, nCount As UInteger, rgnData As IntPtr) As IntPtr

    <StructLayout(LayoutKind.Sequential)>
    Private Structure Size
        Public cx As Int32
        Public cy As Int32

        Public Sub New(x As Int32, y As Int32)
            cx = x
            cy = y
        End Sub
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Private Structure BLENDFUNCTION
        Public BlendOp As Byte
        Public BlendFlags As Byte
        Public SourceConstantAlpha As Byte
        Public AlphaFormat As Byte
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure Point
        Public x As Int32
        Public y As Int32

        Public Sub New(x As Int32, y As Int32)
            Me.x = x
            Me.y = y
        End Sub

        Public Shared Operator +(x As Point, y As Point) As Point
            Return New Point(x.x + y.x, x.y + y.y)
        End Operator

    End Structure
#End Region

#Region " Properties "
    Public Property CanClickThrought As Boolean = False

    Private _Opacity As Byte = 255
    Public Shadows Property Opacity As Byte
        Get
            Return _Opacity
        End Get
        Set(value As Byte)
            If value = _Opacity Then Exit Property

            _Opacity = value
            Invalidate()
        End Set
    End Property




#End Region


    Protected Overrides Sub OnShown(e As EventArgs)
        MyBase.OnShown(e) : Invalidate()
    End Sub

    'WS_EX_LAYERED, WS_EX_TRANSPARENT
    Protected Overrides ReadOnly Property CreateParams() As CreateParams
        Get
            Dim cParms As CreateParams = MyBase.CreateParams

            If Not DesignMode Then
                cParms.ExStyle = cParms.ExStyle Or &H80000
                If CanClickThrought Then cParms.ExStyle = cParms.ExStyle Or &H20
            End If

            Return cParms
        End Get
    End Property

    Public Sub SetBits(B As Drawing.Bitmap)
        If Not IsHandleCreated Or DesignMode Then Exit Sub

        Dim oldBits As IntPtr = IntPtr.Zero
        Dim screenDC As IntPtr = GetDC(IntPtr.Zero)
        Dim hBitmap As IntPtr = IntPtr.Zero
        Dim memDc As IntPtr = CreateCompatibleDC(screenDC)

        Try
            Dim topLoc As New Point(Left, Top)
            Dim bitMapSize As New Size(B.Width, B.Height)
            Dim blendFunc As New BLENDFUNCTION()
            Dim srcLoc As New Point(0, 0)

            hBitmap = B.GetHbitmap(Drawing.Color.FromArgb(0))
            oldBits = SelectObject(memDc, hBitmap)
            blendFunc.BlendOp = AC_SRC_OVER
            blendFunc.SourceConstantAlpha = Opacity
            blendFunc.AlphaFormat = AC_SRC_ALPHA
            blendFunc.BlendFlags = 0

            UpdateLayeredWindow(Handle, screenDC, topLoc, bitMapSize, memDc, srcLoc, 0, blendFunc, ULW_ALPHA)
        Finally
            If hBitmap <> IntPtr.Zero Then
                SelectObject(memDc, oldBits)
                DeleteObject(hBitmap)
            End If
            ReleaseDC(IntPtr.Zero, screenDC)
            DeleteDC(memDc)
        End Try
    End Sub

    Public Shadows Sub Invalidate()
        If DesignMode Then
            MyBase.Invalidate()
        Else
            Dim B As New Bitmap(ClientSize.Width, ClientSize.Height)
            Dim G As Graphics = Graphics.FromImage(B)

            Draw(G)

            G.Dispose()
            SetBits(B)
            B.Dispose()
        End If
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim B As New Drawing.Bitmap(ClientSize.Width, ClientSize.Height)
        Dim G As Drawing.Graphics = Drawing.Graphics.FromImage(B)

        G.Clear(Color.White)
        Draw(G)


        G.Dispose()
        e.Graphics.DrawImage(B, 0, 0)
        B.Dispose()
    End Sub

    Overridable Sub Draw(G As Drawing.Graphics)
    End Sub



#Region "DWM"
    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
        If DesignMode Then Exit Sub

        If e.Button = Windows.Forms.MouseButtons.Left Then


            If Me.Width - BorderWidth > e.Location.X AndAlso
                    e.Location.X > BorderWidth AndAlso e.Location.Y > BorderWidth Then
                MoveControl(Me.Handle)
            End If
        End If

        MyBase.OnMouseDown(e)
    End Sub
    Private Sub MoveControl(ByVal hWnd As IntPtr)
        If DesignMode Then Exit Sub
        ReleaseCapture()
        SendMessage(hWnd, WM_NCLBUTTONDOWN, HTCAPTION, 0)
    End Sub
    <DllImport("user32.dll")>
    Public Shared Function ReleaseCapture() As Boolean
    End Function
    <DllImport("user32.dll")>
    Public Shared Function SendMessage(ByVal hWnd As IntPtr,
            ByVal Msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) _
            As Integer
    End Function
#End Region





    Private Sub LayeredForm_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Invalidate()
    End Sub

    Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'LayeredForm
        '
        Me.ClientSize = New System.Drawing.Size(288, 246)
        Me.Name = "LayeredForm"
        Me.ResumeLayout(False)

    End Sub
End Class


Public Class customNotification
    Inherits LayeredForm
    Dim r As New Rectangle(0, 0, 10, 10)
    Dim t As String

    Sub New(t As String)
        Me.t = t
        FormBorderStyle = FormBorderStyle.None
        WindowState = FormWindowState.Normal
        CanClickThrought = False
        Visible = True
        Height = 120 : Width = 400
        MaximumSize = Size
        MinimumSize = Size
        Top = Screen.PrimaryScreen.WorkingArea.Height - ctr * (Height - 25) - 10 - 14 : Left = Screen.PrimaryScreen.WorkingArea.Width - Width
        Show()
    End Sub
    Dim br As SolidBrush
    Dim p As Pen
    Public Overrides Sub Draw(G As Graphics)
        If DesignMode Then Exit Sub
        With G
            .SmoothingMode = 2 : .InterpolationMode = 7 : .PixelOffsetMode = 2 : .TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit
            .Clear(col(0, 0))
            Dim br As SolidBrush
            Dim s = 14
            .SetClip(New Rectangle(s, s, Width - 2 * s, Height - 2 * s), CombineMode.Exclude)
            For i = s To 0 Step -1
                Dim Rounding = 5 + s - i
                Dim pth As GraphicsPath = CustomWindow.DM.CreateRoundRectangle(i, i, Width - (i * 2), Height - (i * 2), Rounding, , , , )
                mb(col(i ^ (0.111 * i), Color.Black), br)
                .FillPath(br, pth)
                pth.Dispose()
            Next
            .ResetClip()

            mp(col(30, 255), p)
            mb(col(245), br)
            .FillRectangle(br, rct(s, s, Width - 2 * s, Height - 2 * s))
            .DrawRectangle(p, rct(s, s, Width - 2 * s, Height - 2 * s))

            .DrawIcon(Me.Icon, rct(s + 5, s + 5, 42, 42))

            .DrawString(t, New Font("Segoe UI", 18), Brushes.Gray, pt(s + 5 + 42 + 30, s + 5))

            br.Dispose()
            p.Dispose()
        End With
    End Sub
End Class



Module notif
    Public ctr% = 0
    Public Sub notify(Text As String)
        ctr += 1
        Dim n As New customNotification(Text)
    End Sub
End Module