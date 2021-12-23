basic_game_scripting
====================

A quick VB.NET project that highlights two different ways to add quick and dirty scripting to your game.

The gist is you have an entity in a game that is bouncing from edge to edge. By either hitting the tilde (~) key to bring up and input box to add a scripted command, or have a specific script in a set location, a dev (or user) can alter the game's properties without having to recompile!

This is just a proof of concept to help nudge folks along in the right direction.


=================
dreamincode.net tutorial backup ahead of decommissioning


 Posted 26 July 2013 - 03:34 PM 

I recently found myself explaining to different pods of folks how to add 'scripting' to their games.  The consistent response was confusion and bemoaning the difficulty of the task.  Certainly there is some work to be done (if you want to add Lua, Javascript, Perl, Python etc) to your engine but it doesn't need to go _that_ far.  With planned access points and a 'ninja' method to process what was entered you can have functional scripting with little hassle.  

[u]Topics covered:[/u]
- conceptually how to add scripting/modding ability to your game
- expanding on object interaction
- game loops

[u]Technology used:[/u]
- VB.NET 4.0 framework

[i]Notes[/i]
- Keep the images in the execution directory of the executable.
- the startup script file name is 'myscript.txt' - that also needs to be in the executable's directory.

[b][u]What is scripting?[/u][/b]

Scripting, in respects to a videogame, is a way to provide commands to the game engine that are interpreted and modify the state, properties of the actors, or generate new objects per their interpretation.  If you make your game flexible enough you can enjoy a controlled avenue rich with development, content creation, and happy users without needing to recompile the game!  

The gist is you review your game, determine what you would like to have affected by scripts, any actions for those affected areas, and then gin up some rules on matching unique command names and actions!  It sounds complex, but it is not that bad.

Examples of games with scripting:
https://en.wikipedia.org/wiki/Category:Lua-scripted_video_games
http://nwn2.wikia.com/wiki/Neverwinter2_Script_directory

[b][u]How does it relate to a video game?[/u][/b]

When folks write their game (or game engine and game) there is a sort of tunnel vision.  They see the game loop as a closed system, written in a specific language, and with specific states.  There is no room to shoe horn in an 'on the fly' debug mode, scripts to modify game play (or test cases), or allow post modding of their game.  All of this can happen, without the need to recompile, and on the fly!  

[b][u]What are common scripting points?[/u][/b]
Typically there are two main script entry points into your game loop - at the start, or through a 'console input' by the user.

One would typically see a start-up script modify a large number of game environment options (such as level design, background, object start locations, etc) and is pretty complex.  

A 'console input' is often for quick changes and are one liners.  This would be things like turning on 'god mode' or 'no clip', unlocking a door, or advancing milestones in a preset quest.  

People get bogged down in their game's complex states and ultimately are too consumed by the details that it seems impossible to wedge some sort of fancy script insert point into their loop, but this is not true.

[img]https://farm3.staticflickr.com/2881/9371086231_360d1d0035_o.jpg[/img]

If you look at a typical game loop there really only four major parts
- the start up
- the update
- the draw
- the exit

We know the start up scripts would be best in the first option, typically the check is best when the game window and objects are made, but before you kick off the timing engine.  If the script needs to interact with objects (set locations, image, inventory, etc) it is best to change that before the engine starts processing updates.

The 'console input' is just a trigger off the key press event, right?  A quick call to pause the game's update, input of the script command, process the input, and resume game play!  

Conceptually it is difficult for people to get out of their engine's rut and see they can dynamically affect their game states, and objects, with little fuss.

[b][u]The example:[/u][/b]
Programmatically I am using my old game setup from this tutorial ([url="http://www.dreamincode.net/forums/topic/245268-oop-with-video-game-basics-part-1/"] OOP with Video Game Basics Part 1 [/url]).  I modified the Entity class to have a few more directions of movement, and the user class to use those movements and have a method open to changing its color.  The real magic is in the engine modifications.

The gist is the game starts up, loads an image, and moves the blue block around the screen without user input.  I would like to change the path of the movement, color of the object, display information about the object, and turn on a universal boolean to illustrate affecting larger portions of the game.

Conceptually the format for the scripts were to be:
[i]<action>:<modifier>[/i]

The action would be a single character would direct my interpreter to what action to perform, and the modifier is needed to say how much or to what.

The actions I would like to be able to access is:
d - direction of the block's movement.   
-- -- Modifier is a value 0 through 8 representing one of the directions in the Entity.MovementDirection

c - color change.
-- -- Modifier is one of the strings: red, blue, or green.  This would have the user object change the image/color accordingly.

p - print user information.  Useful for debugging.
-- -- modifier: none.  

g - 'god mode'.  Use to be impervious to damage, enemies, etc.  Functionally vacant with this demo, but it illustrates the global 'switch' system that can be built to react to user input.
-- -- modifier: 1 to turn on, anything else to turn off

I also envisioned stringing multiple script commands together separated by semicolons ( ; )

An example of: 
c:red;d:1
means change the block's color to red, and its direction to 'up'.

A decent representation of in game object changes, visual changes, and game state changes!  


[b][u]Code[/u][/b]
In the form's class here is the interpreter engine.  It takes in a string, cleans it up, and attempts to fit it against the known scripting rules.  Going forward I can add more interpreter rules here and not need to make crazy changes elsewhere!  

To be clear - the console script commands and the 'start-up script' commands run on the same rules.  They are just two different penetration points into the game loop but provide the same information!  


[code]
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
[/code]

Clearly some of that code can be tightened up, but you see how easy it is (with a switch statement and some if statements) to take gibberish input and enact sweeping changes under the game hood!


With respects to the penetration points there are two.  

The first is the 'console input'.  When the Keys.Oemtilde is pressed the game pauses, puts up an input box to ask for script input, processes the input found, and returns the game play.  This means I can change mechanics on the fly!

[code]
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

            Case Keys.Oemtilde'-- ask for console input
                _bPause = True

                ParseScript(InputBox("Console command", "Console Command"))

                _bPause = False

        End Select
    End Sub
[/code]

The second input point is the startup.  I am cleverly looking for a single script file called "myscript.txt" in the execution location of the game.  If nothing is found then move on, but if it is found start reading each line and see what suggested changes can be made.

[code]
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

[/code]


[b][u]Example screen shots:[/u][/b]

Running normally

[img]https://farm6.staticflickr.com/5491/9370291577_cabfa19c86_o.jpg[/img]

Console input of: c:green;d:4
Changes the color to green, and the direction.
[img]https://farm3.staticflickr.com/2814/9370291495_8ebab2986f_o.jpg[/img]

Start script: g:1
starts game with 'god mode' on.
[img]https://farm8.staticflickr.com/7360/9370291687_338d5d0322_o.png[/img]

At the end - it is just this simple to get a script interpreter up and running in your game.  Recognize the basic game loop, pick the two obvious entry points (script file or 'console input'), and structure your script rules.  The rules are just products of thinking what you want to have external access to (images, object properties, levels, etc), and writing a quick rule-to-action interpreter for them!

[b][u]Advanced topics:[/u][/b]
- try adding python or lua to the mix
- expand the rule set to alter other game aspects
- try using multiple objects (with unique ids) and add to the interpreter function the ability to specify a single object (by its id) and have changes only affect that instance

The code behind of the form/game window.
[spoiler]
[code]'-- 2013.07.24 - modi123_1 w/ dream in code.
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

[/code]
[/spoiler]


Taking the advice of [member='BetaWar'] (from [url="http://www.dreamincode.net/forums/topic/323844-fork-me-on-github/"]this thread[/url] I decided to put the entire project on github.  You can find it here: https://github.com/modi1231/basic_game_scripting
