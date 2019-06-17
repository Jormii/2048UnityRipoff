public class Enums {

    /// <summary>
    /// Enum which stores all the possible directions the tiles can move.
    /// <see cref="Direction.None"/> value is used to indicate no movement.
    /// </summary>
    public enum Direction {
        Up,
        Down,
        Left,
        Right,
        None
        };

        [System.Serializable]
        public enum Mode {
        Mode3x3,
        Mode4x4,
        Mode5x5,
        Mode6x6
    }
}