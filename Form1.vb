'-- 2013.07.24 - modi123_1 w/ dream in code.
Public Class Form1
    Private WithEvents _timer As Timer = Nothing '-- the 'game engine'.
    Private _oUser As User '-- our user class

    '-- minor information on keeping the object moving.
    Private _bDidMove As Boolean = False
    Private _eLastMove As Entity.MovementDirection = Entity.MovementDirection.UP_LEFT


    Dim _bPause As Boolean = False '-- works in the 'paint/draw' portion.. if paused do not allow more state information to be fed to the user object.
    Dim _bGodModeOn As Boolean = False '-- signfies if god mode was selected

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.SetStyle(ControlStyles.UserPaint, True)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)

        Me.Height = 500
        Me.Width = 500


        '-- create our user to be drawn.
        _oUser = New User
        _oUser.ScreenHeight = Me.Height '-- to prevent moving off the screen!
        _oUser.ScreenWidth = Me.Width '-- to prevent moving off the screen!

        '-- Important piece here - before the game engine starts, check to see if a script exists or not!
        CheckForScript("myscript.txt")

        '-- setup our timer as the engine to process.
        _timer = New Timer
        _timer.Interval = 100
        _timer.Start()
    End Sub

    ''' <summary>
    ''' Check a specific file location for a script file.  If said script file exists read each line and run it through the 'script parser'.
    ''' </summary>
    ''' <param name="location"></param>
    ''' <remarks></remarks>
    Private Sub CheckForScript(ByVal location As String)
        Dim sr As IO.StreamReader
        Dim sTemp As String

        Try
            '-- always check to see if the file exists
            If IO.File.Exists(location) Then
                '-- set up the reader.
                sr = New IO.StreamReader(location)

                Do
                    '-- read each line (maybe multiple commands per line?)
                    sTemp = sr.ReadLine
                    '-- try to process that script line
                    ParseScript(sTemp)
                Loop Until sTemp Is Nothing
            Else
                Debug.WriteLine("File does not exist! " + location)
            End If
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Important key command here.  When the user hits the tilde (~) pause the game, ask for input to pump through the scripting engine, and resume game play.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Form1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case Keys.Escape
                Me.Close()
            Case Keys.Space
                _bPause = Not _bPause

            Case Keys.Oemtilde '-- ask for console input
                _bPause = True

                ParseScript(InputBox("Console command", "Console Command"))

                _bPause = False

        End Select
    End Sub

    ''' <summary>
    ''' ParseScript takes in a potential script line and tries to match it up with a known script interpretation rule.
    ''' </summary>
    ''' <param name="input"></param>
    ''' <remarks></remarks>
    Private Sub ParseScript(ByVal input As String)
        Debug.WriteLine(String.Format("ProcessConsoleInput: {0}", input))

        Dim sTokens() As String '-- in case there are multiple lines on one line

        If input Is Nothing OrElse input.Trim = String.Empty Then Exit Sub '-- if nothing then do nothing.

        input = input.Trim.ToLower '-- to keep uniformity - throw everything to lower case

        sTokens = input.Split(CChar(";")) '-- you can string multiple scripted actions together by using a semicolon.. break those a part.

        For Each temp As String In sTokens

            If temp.Trim.Length > 0 Then

                '-- this quick scripting engine checks for the first letter.  If it is a known first letter then check the rest of the line for the action.
                Select Case temp(0)
                    Case "d"c '-- direction change
                        '-- The thought was to have a <character>:<action>
                        '-- in this case any number that corresponds to Entity.MovementDirection's number system works.
                        If Char.IsNumber(temp(2)) Then
                            If CInt(temp(2).ToString) >= 0 AndAlso CInt(temp(2).ToString) <= 8 Then
                                _eLastMove = CType(temp(2).ToString, Entity.MovementDirection)
                            End If
                        End If
                    Case "p"c '-- print character info
                        Debug.WriteLine(_oUser.Print())
                    Case "c"c '-- change color
                        If temp.Contains("red") Then
                            _oUser.LoadNewImage("red.png")
                        ElseIf temp.Contains("blue") Then
                            _oUser.LoadNewImage("blue.png")
                        ElseIf temp.Contains("green") Then
                            _oUser.LoadNewImage("green.png")
                        End If
                    Case "g"c '-- god mode - display text to show you are in 'god mode'.. flip a boolean 
                        If Char.IsNumber(temp(2)) Then
                            If CInt(temp(2).ToString) = 1 Then
                                _bGodModeOn = True
                            Else
                                _bGodModeOn = False
                            End If
                        End If

                End Select

            End If
        Next
    End Sub

    '--- less than important function to keep the square moving.
    Private Sub CalculatNext()
        '-- When the engine ticks proces the input into something our user class can do.
        Dim eNextMove As Entity.MovementDirection = Entity.MovementDirection.NONE

        If Not _bDidMove Then
            Select Case _eLastMove
                Case Entity.MovementDirection.UP
                    eNextMove = Entity.MovementDirection.DOWN
                Case Entity.MovementDirection.DOWN
                    eNextMove = Entity.MovementDirection.UP
                Case Entity.MovementDirection.LEFT
                    eNextMove = Entity.MovementDirection.RIGHT
                Case Entity.MovementDirection.RIGHT
                    eNextMove = Entity.MovementDirection.LEFT
                Case Entity.MovementDirection.UP_LEFT
                    eNextMove = Entity.MovementDirection.DOWN_RIGHT
                Case Entity.MovementDirection.UP_RIGHT
                    eNextMove = Entity.MovementDirection.DOWN_LEFT
                Case Entity.MovementDirection.DOWN_RIGHT
                    eNextMove = Entity.MovementDirection.UP_LEFT
                Case Entity.MovementDirection.DOWN_LEFT
                    eNextMove = Entity.MovementDirection.UP_RIGHT
                Case Else
                    Dim r As New Random
                    Dim temp As Int32 = r.Next(1, 9)

                    eNextMove = CType(temp, Entity.MovementDirection)
            End Select
        Else
            eNextMove = _eLastMove
        End If

        _oUser.NextAction = eNextMove
        If eNextMove <> Entity.MovementDirection.NONE Then
            _eLastMove = eNextMove
        End If

    End Sub

    '-- the "game engine".  
    Private Sub _timer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles _timer.Tick
        '-- if we are in pause then skip over this.. if not find the next move and process it .
        If Not _bPause Then
            CalculatNext()

            _bDidMove = _oUser.ProcessState()
        End If

        '-- Redraw the scene
        Me.Refresh()
    End Sub


    Private Sub Form1_Paint(sender As Object, e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        '-- Paint the changes to the screen.

        '-- god mode text
        If _bGodModeOn Then
            e.Graphics.DrawString("GodMode: On", New Font("Arial", 16), Brushes.Black, New Point(20, 20))
        End If

        '-- user object
        _oUser.Paint(e.Graphics)
    End Sub

End Class
