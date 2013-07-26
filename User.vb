Public Class User
    Inherits Entity

    Private _enumNextAction As MovementDirection = MovementDirection.NONE
    Private _lHealth As Int32 = 1

    Public Property NextAction As MovementDirection
        Get
            Return _enumNextAction
        End Get
        Set(ByVal value As MovementDirection)
            _enumNextAction = value
        End Set
    End Property

    Public Sub New()
        _pointLocation = New Point(50, 50)
        _lMovementRate = 10
        _sID = "User"

        Try
            '-- starting color is blue
            _bitmapImage = New Bitmap("blue.png")
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OkOnly, String.Format("Class: {0}", Me.GetType().Name))
            _bitmapImage = New Bitmap(10, 10)
        End Try
    End Sub

    ''' <summary>
    ''' A constructor that takes in all the important bits of information to show the image on the screen.
    ''' </summary>
    ''' <param name="startingLocation"></param>
    ''' <param name="movementRate"></param>
    ''' <param name="imageLocation"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal startingLocation As Point, ByVal movementRate As Int32, ByVal imageLocation As String)
        _pointLocation = startingLocation
        _lMovementRate = movementRate
        _bitmapImage = New Bitmap(imageLocation)
        _sID = "User"
    End Sub

    '-- Sets up drawing the image.  
    Public Overrides Sub Paint(ByVal e As System.Drawing.Graphics)
        e.DrawImage(_bitmapImage, _pointLocation)
    End Sub

    Public Overrides Function ProcessState() As Boolean
        '-- The only state the user needs to worry about is which direction the user wants to move.
        Return DoMovement(_enumNextAction)
    End Function

    ''' <summary>
    ''' Takes the information from the user's key press and update the location.
    ''' </summary>
    ''' <param name="direction"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function DoMovement(ByVal direction As MovementDirection) As Boolean
        Dim bMoveOccured As Boolean = False

        '-- expanded directions w/ diagonals
        If _enumNextAction <> MovementDirection.NONE Then
            Select Case direction
                Case MovementDirection.UP
                    If _pointLocation.Y - _lMovementRate >= 0 Then '-- make sure user is not moving off the screen 
                        _pointLocation.Y -= _lMovementRate
                        bMoveOccured = True
                    Else
                        _enumNextAction = MovementDirection.NONE
                    End If
                Case MovementDirection.DOWN
                    If _pointLocation.Y + _bitmapImage.Height + _lMovementRate + 20 < _lScreenHeight Then '-- make sure user is not moving off the screen 
                        _pointLocation.Y += _lMovementRate
                        bMoveOccured = True
                    Else
                        _enumNextAction = MovementDirection.NONE
                    End If
                Case MovementDirection.RIGHT
                    If _pointLocation.X + _bitmapImage.Width + _lMovementRate < _lScreenWidth Then '-- make sure user is not moving off the screen 
                        _pointLocation.X += _lMovementRate
                        bMoveOccured = True
                    Else
                        _enumNextAction = MovementDirection.NONE
                    End If
                Case MovementDirection.LEFT
                    If _pointLocation.X - _lMovementRate >= 0 Then '-- make sure user is not moving off the screen 
                        _pointLocation.X -= _lMovementRate
                        bMoveOccured = True
                    Else
                        _enumNextAction = MovementDirection.NONE
                    End If

                Case MovementDirection.UP_LEFT
                    If (_pointLocation.Y - _lMovementRate >= 0) AndAlso (_pointLocation.X - _lMovementRate >= 0) Then
                        _pointLocation.Y -= _lMovementRate
                        _pointLocation.X -= _lMovementRate
                        bMoveOccured = True
                    Else
                        _enumNextAction = MovementDirection.NONE
                    End If

                Case MovementDirection.UP_RIGHT
                    If (_pointLocation.Y - _lMovementRate >= 0) AndAlso (_pointLocation.X + _bitmapImage.Width + _lMovementRate < _lScreenWidth) Then
                        _pointLocation.Y -= _lMovementRate
                        _pointLocation.X += _lMovementRate
                        bMoveOccured = True
                    Else
                        _enumNextAction = MovementDirection.NONE
                    End If
                Case MovementDirection.DOWN_RIGHT
                    If (_pointLocation.Y + _bitmapImage.Height + _lMovementRate + 20 < _lScreenHeight) AndAlso (_pointLocation.X + _bitmapImage.Width + _lMovementRate < _lScreenWidth) Then
                        _pointLocation.Y += _lMovementRate
                        _pointLocation.X += _lMovementRate
                        bMoveOccured = True
                    Else
                        _enumNextAction = MovementDirection.NONE
                    End If
                Case MovementDirection.DOWN_LEFT
                    If (_pointLocation.Y + _bitmapImage.Height + _lMovementRate + 20 < _lScreenHeight) AndAlso (_pointLocation.X - _lMovementRate >= 0) Then
                        _pointLocation.Y += _lMovementRate
                        _pointLocation.X -= _lMovementRate
                        bMoveOccured = True
                    Else
                        _enumNextAction = MovementDirection.NONE
                    End If
                Case Else
                    bMoveOccured = False
            End Select

            If bMoveOccured Then _enumNextAction = MovementDirection.NONE

        End If
        Return bMoveOccured
    End Function

    ''' <summary>
    ''' This shows we can tack more information as we add variables to the print method already in the abstract class.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Print() As String
        Return MyBase.Print + String.Format(" Health: {0}", _lHealth)
    End Function


    Public Sub LoadNewImage(ByVal location As String)
        _bitmapImage = New Bitmap(location)
    End Sub
End Class

