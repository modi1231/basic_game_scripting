Public MustInherit Class Entity

    Public Enum MovementDirection
        NONE = 0
        UP = 1
        RIGHT = 2
        DOWN = 3
        LEFT = 4
        UP_LEFT = 5
        UP_RIGHT = 6
        DOWN_RIGHT = 7
        DOWN_LEFT = 8
    End Enum

    Protected _bitmapImage As Bitmap = Nothing '-- the image to be drawn.
    Protected _pointLocation As Point = Nothing '-- where on the map is the user.
    Protected _sID As String = String.Empty '-- ids are usually important
    Protected _lMovementRate As Int32 = 0 '-- how fast the location changes when a move command is issued.
    Protected _lScreenWidth As Int32 = 0 '-- used to determine the boundaries so the location is still visible.
    Property _lScreenHeight As Int32 = 0 '-- used to determine the boundaries so the location is still visible.

    Public WriteOnly Property ScreenHeight As Int32
        Set(ByVal value As Int32)
            _lScreenHeight = value
        End Set
    End Property

    Public WriteOnly Property ScreenWidth As Int32
        Set(ByVal value As Int32)
            _lScreenWidth = value
        End Set
    End Property

    ''' <summary>
    ''' Print pertinent information
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Print() As String
        Return String.Format("{0}: x={1}, y={2}", _sID, _pointLocation.X, _pointLocation.Y)
    End Function

    '-- Everything must be drawn!
    Public MustOverride Sub Paint(ByVal e As Graphics)

    '-- Everything must be provided a consistant entry point to process that state for that given time slice.
    Public MustOverride Function ProcessState() As Boolean
End Class

