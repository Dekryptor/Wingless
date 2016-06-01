Imports System.Drawing.Drawing2D
Imports System.Drawing.Text
Imports System.Math
Imports System.ComponentModel

Class customCircularProgress : Inherits ThemeControl154
#Region "Declare"
	Private arcp As Pen
	Private _Maximum As Double = 100
	Private _Value As Double = 0
#End Region
#Region "prop"
	<Category("Behavior")>
	Public Property Value As Double
		Get
			Return _Value#
		End Get
		Set(ByVal V As Double)
			If V > _Maximum Then
				_Value = _Maximum
			End If
			If V < 0 Then
				_Value = 0
			End If

			If _Value <> V Then
				'				tv = V
				'				tmr1.Start()
				_Value = V
			End If

			Invalidate()
		End Set
	End Property
	<Category("Behavior")> _
	Public Property Maximum As Integer
		Get
			Return _Maximum
		End Get
		Set(ByVal V As Integer)
			If V < 1 Then V = 1
			If V < _Value Then _Value = V
			_Maximum = V
			Invalidate()
		End Set
	End Property
#End Region
    Protected Overrides Sub PaintHook()
		G.SmoothingMode = 2
		G.InterpolationMode = InterpolationMode.HighQualityBicubic
		G.Clear(Parent.BackColor)

		simplev()
		arcp.Dispose()
	End Sub


	Sub filledm()

	End Sub
	Sub New()
		SetStyle(ControlStyles.ResizeRedraw Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
		DoubleBuffered = True
		ForeColor = Color.FromArgb(255, 128, 0)
	End Sub
	Sub simplev()
		G.SmoothingMode = 2
		G.InterpolationMode = 7

		Dim b As Bitmap = New Bitmap(Width * 2, Height * 2)
		Dim gr = Graphics.FromImage(b)
		gr.SmoothingMode = SmoothingMode.HighQuality
		gr.InterpolationMode = 7

		For h = 1 To 1
			arcp = New Pen(Color.FromArgb(40 * h, 255 - Parent.BackColor.R, 255 - Parent.BackColor.G, 255 - Parent.BackColor.B), (2 - h) * 2.0!)
			gr.DrawArc(arcp, New Rectangle(5.0! * 2.0!, 5.0! * 2.0!, CSng(Width - 10) * 2.0!, CSng(Height - 10) * 2.0!), -90, 360)
		Next

		If Value > 0.0# Then
			For i = 1 To 1
				Dim ab1 = Color.FromArgb(260 - (13 * i), ForeColor)
				arcp = New Pen(ab1, i * 2)
				gr.DrawArc(arcp, New Rectangle(5.0! * 2.0!, 5.0! * 2.0!, CSng(Width - 10) * 2.0!, CSng(Height - 10) * 2.0!), -90 - 1, CSng(Value / Maximum * 360) + 2)
			Next



			'Dim ab2 = Color.FromArgb(128, 255 - BackColor.B, 255 - BackColor.B, 255 - BackColor.B)
			'arcp = New Pen(ab2, 1)
			'gr.DrawArc(arcp, new rectangle(5, 5, Width - 10, Height - 10), sa - 100, ea + 200)


			arcp = New Pen(ForeColor, 3)
			'gr.DrawArc(arcp, new rectangle(5, 5, Width - 10, Height - 10), 0, CSng(Value / Maximum * 360))
		End If
		G.DrawImage(b, 0, 0, Width, Height)

	End Sub

End Class

Class customLinearProgress

#Region "declare"
	Inherits ThemeControl154

#Region "global"
	Private _Maximum As Double = 100
	Private _Value As Double = 0
	Private pw As Integer
	Private hoff As Double
	Dim rev As Boolean
	Dim op As Double = 2
	Dim hz As Integer
	Private O As _Options
	Private t As _type
#End Region

#Region "for fancy"
	Dim pts As Point()
	Dim sshad As Pen

	Dim blb As LinearGradientBrush
	Dim tf1 As LinearGradientBrush
	Dim tf2 As LinearGradientBrush

	Dim bf As SolidBrush
	Dim backb As SolidBrush
#End Region

#End Region

#Region "property"
	Public Sub New()
		DoubleBuffered = True
		SetStyle(ControlStyles.ResizeRedraw Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
		Maximum = 100
		Value = 0
		If O = _Options.Simple Then LockHeight = 10
		AccessibleDescription = "Animated Control"
	End Sub


    <Category("Behavior")>
	Public Property Maximum As Integer
		Get
			Return _Maximum
		End Get
		Set(ByVal V As Integer)
			If V < 1 Then V = 1
			If V < _Value Then _Value = V
			_Maximum = V
			Invalidate()
		End Set
	End Property
	<Category("Behavior")>
	Public Property Value As Double
		Get
			Return _Value#
		End Get
		Set(ByVal V As Double)
			If V > _Maximum Then
				_Value = _Maximum
			End If
			If V < 0 Then
				_Value = 0
			End If

			If _Value <> V Then
				_Value = V
			End If

			Invalidate()
		End Set
	End Property

    <Flags()>
	Enum _Options
		Fancy
		Simple
		'Style3
	End Enum
	<Category("Behavior")>
	Public Property Style As _Options
		Get
			Return O
		End Get
		Set(value As _Options)
			O = value
			If O = _Options.Simple Then
				LockHeight = 10
				hz = 35
			Else : LockHeight = 0 : hz = Height * 2
			End If
			Invalidate()
		End Set
	End Property
	<Flags()>
	Enum _type
		Valued
		Marquee
	End Enum
	<Category("Behavior")>
	Public Property ProgressType As _type
		Get
			Return t
		End Get
		Set(value As _type)
			t = value
			Invalidate()
		End Set
	End Property

#End Region

#Region "draw"

	Protected Overrides Sub PaintHook()
		If ctr > 1.5 Then ctr = -1.6 Else ctr += 0.01
		If rev = False Then
			If op < 81 Then op += 0.1 Else rev = True
		Else
			If op > 10 Then op -= 0.1 Else rev = False
		End If
		If hoff >= hz Then hoff = 0
		hoff += 0.08

		If t = _type.Valued Then
			If O = _Options.Fancy Then
				fancy()
			ElseIf O = _Options.Simple Then
				simple()
			End If
		Else
			marquee()
		End If
	End Sub
	Private Sub simple()

		'decor
		G.InterpolationMode = 7
		G.SmoothingMode = 2
		pw = CInt((Value / Maximum) * (Width))
		G.Clear(Parent.BackColor)
		hz = 35
		DrawBorders(New Pen(Color.FromArgb(100, 255 - Parent.BackColor.R, 255 - Parent.BackColor.G, 255 - Parent.BackColor.B)))
		'---------------------------------------------------------------------------------------------------------------------




		bf = New SolidBrush(BackColor)
		G.FillRectangle(bf, pw, 0, Width - pw - 1, Height - 1)
		'----------------------------------------------------------------------------------------------------------------------

		If Not Value = 0 Then

			bf = New SolidBrush(Parent.BackColor)
			G.FillRectangle(bf, New Rectangle(-1, -1, pw + 1, Height + 1))
			'bf = New SolidBrush(Color.FromArgb(128, forecolor))
			bf = New SolidBrush(Color.FromArgb(ForeColor.A, Abs(ForeColor.R - 30), Abs(ForeColor.G - 30), Abs(ForeColor.B - 30)))
			G.FillRectangle(bf, New Rectangle(-1, -1, pw + 1, Height + 1))
			'----------------------------------------------------------------------------------------------------------------------


			G.SetClip(New Rectangle(1, 1, pw, Height - 2))
			bf = New SolidBrush(ForeColor)
			G.FillRectangle(bf, New Rectangle(0, 0, pw, Height))

			G.SetClip(New Rectangle(1, 1, pw - 1, Height - 2))
			For i = -40 To pw + 40 Step 35
				pts = {New Point(i + hoff, 0),
					   New Point(i + 20 + hoff, 0),
					   New Point(i + 30 + hoff, Height),
					   New Point(i + 10 + hoff, Height)
					}
				bf = New SolidBrush(Color.FromArgb(70, Color.White))
				G.FillPolygon(bf, pts)
			Next
			G.ResetClip()

			bf = New SolidBrush(Color.FromArgb(ForeColor.A, Abs(ForeColor.R - 30), Abs(ForeColor.G - 30), Abs(ForeColor.B - 30)))
			G.FillRectangle(bf, New Rectangle(pw - 1, 0, 1, Height))
			G.ResetClip()
			'-----------------------------------------------------------------------------------------------------------------------

			sshad = New Pen(Color.FromArgb(100, Color.White), 1)
			If Not Value = 100 Then
				G.DrawLine(sshad, 1, 1, pw - 1, 1)
			Else
				G.DrawLine(sshad, 1, 1, pw - 2, 1)
			End If
			sshad.Dispose()
		End If
		'-----------------------------------------------------------------------------------------------------------------------


		bf.Dispose()
	End Sub
	Private Sub fancy()


		'decor
		G.InterpolationMode = InterpolationMode.HighQualityBicubic
		G.SmoothingMode = SmoothingMode.AntiAlias
		G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit

		'declare
		pw = CInt((Value / Maximum) * (Width))
		hz = Height * 2
		If pw = 0 Then pw = 1
		bf = New SolidBrush(Color.FromArgb(41 - CInt(op / 2), Color.White))
		tf1 = New LinearGradientBrush(New Rectangle(0, 0, Height, CInt(Height / 2)), Color.FromArgb(0, 90, 174), Color.FromArgb(0, 120, 204), 90.0F)
		tf2 = New LinearGradientBrush(New Rectangle(0, 0, Height, CInt(Height / 2)), Color.FromArgb(0, 120, 204), Color.FromArgb(0, 90, 174), 90.0F)
		sshad = New Pen(Color.FromArgb(50, Color.Black), 2)
		backb = New SolidBrush(BackColor)
		blb = New LinearGradientBrush(New Rectangle(0, 0, pw, Height), Color.FromArgb(51, 51, 51), Color.FromArgb(28, 28, 28), 90.0F)

		'draw
		G.Clear(Parent.BackColor)

		G.DrawRectangle(New Pen(Color.FromArgb(100, Color.Black)), New Rectangle(pw, 1, Width - pw - 1, Height - 3))

		G.FillRectangle(backb, pw, 1, Width - pw - 1, Height - 3)
		G.SetClip(New Rectangle(0, 0, pw, Height))

		If Not Value = 0.0 Then
			G.FillRectangle(blb, 0, 1, pw, Height - 3)

			For i = pw + 2 * Height To -2 * Height Step -Height * 2
				pts = {New Point(i + Height * 2 + 2 - hoff, -2), New Point(i + Height + 2 - hoff, -2), New Point(i - 2 - hoff, Height + 2), New Point(-hoff + Height + i - 2, Height + 2)}
				G.FillPolygon(bf, pts)
			Next


			For i = -2 * Height To pw + 2 * Height Step Height * 2
				'pts = {New Point(hoff + i - 2, -2), New Point(hoff + Height + i - 2, -2), New Point(i + hoff + Height * 2 + 2, Height + 2), New Point(i + hoff + Height + 2, Height + 2)}
				pts = {New Point(hoff + i - 2, -2),
					   New Point(hoff + Height + i - 2, -2),
					   New Point(i + hoff + Height + CInt(Height / 2), Height / 2),
					   New Point(i + hoff + Height / 2, Height / 2)}
				G.FillPolygon(tf1, pts)

				pts = {New Point(i + hoff + Height / 2, Height / 2),
					   New Point(i + hoff + Height + CInt(Height / 2), Height / 2),
					   New Point(i + hoff + Height * 2, Height),
					   New Point(i + hoff + Height, Height)}
				G.FillPolygon(tf2, pts)

				G.DrawLine(New Pen(Color.FromArgb(0, 120, 204)), New Point(i + hoff + Height / 2 + 1, Height / 2), New Point(i + hoff + Height + CInt(Height / 2) - 1, Height / 2))

				G.DrawLine(sshad, New Point(hoff + i - 2, -2), New Point(i + hoff + Height + 2, Height + 2))
				G.DrawLine(sshad, New Point(hoff + Height + i - 2, -2), New Point(i + hoff + Height * 2 + 2, Height + 2))
			Next

			G.ResetClip()
			G.DrawLine(sshad, pw, 0, pw, Height)

		End If

		'dispose
		tf1.Dispose() : tf2.Dispose()
		blb.Dispose() : backb.Dispose()
		sshad.Dispose() : bf.Dispose()
	End Sub
	Private Sub marquee()
		G.InterpolationMode = InterpolationMode.HighQualityBicubic
		G.SmoothingMode = SmoothingMode.AntiAlias
		bf = New SolidBrush(ForeColor)
		G.Clear(BackColor)

		'G.FillEllipse(bf, x + 3, Height / 2.0! - 3.0!, 6, 6)

		'G.DrawLine(Pens.Gray, Width / 2.0!, -CSng(Height / 2), Width / 2.0!, Height)
		'G.DrawString(x, Font, Brushes.Gray, New Point(Width / 2, Height - 15))
		bf.Dispose()
	End Sub
	Dim ctr As Double = 0.0

#End Region

End Class

Class customCircularMarquee
	Inherits Control
	Dim t! = 0
	Dim t1! = 0
	Dim p As Pen
	Sub New()
		SetStyle(ControlStyles.ResizeRedraw Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
		UpdateStyles()
		DoubleBuffered = True
		Size = New Size(200, 200)
		AccessibleDescription = "Animated Control"
	End Sub

	Protected Overrides Sub OnPaint(e As PaintEventArgs)
		MyBase.OnPaint(e)
		e.Graphics.SmoothingMode = 2 : e.Graphics.InterpolationMode = 7

		Dim rev As Boolean = False
		If t < 3000 Then t += 1 Else t = 0
		If t1 < 360 Then t1 += 0.25 Else t1 = 0

		Dim i As New Interpolation
		Dim v!, vs!



		'v! = i.GetValue(0, 360, t, 3000, Interpolation.Type.EaseInOut, Interpolation.EasingMethods.Jumpback, 2)
		v! = i.GetValue(0, 360, t, 3000, 7, -5)
		vs! = i.GetValue(0, 360 - v, t * 2, 3000, Interpolation.Type.EaseInOut, Interpolation.EasingMethods.Exponent, 1)
		Dim vs1! = i.GetValue(0, 360 - v, t, 3000, Interpolation.Type.EaseInOut, Interpolation.EasingMethods.Exponent, 1)

		Dim r As New Rectangle(5, 5, Width - 10, Height - 10)
		p = New Pen(Color.FromArgb(100, ForeColor), 0)  'ThemeControl154.rescol(Parent.BackColor)))
		e.Graphics.DrawArc(p, r, t1 + v, vs)


		p = New Pen(Color.FromArgb(50, Color.Black), 1)
		e.Graphics.DrawArc(p, New Rectangle(5 - 1, 5 - 1, Width - 8, Height - 8), t1, vs1 * 1.2!)



		p = New Pen(Color.FromArgb(255, ForeColor), 2)
		e.Graphics.DrawArc(p, r, t1, vs1 * 1.2!)


	End Sub
End Class


















