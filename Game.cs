using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI {

    public struct Position : IEquatable<Position> {
        public Position(byte row, byte column) {
            nrow = row;
            ncolumn = column;
        }
        private byte nrow;
        private byte ncolumn;
        public byte Row { 
            get {
                return nrow;
            }
            set {
                if (value < 8) { nrow = value; }
            }
        }
        public byte Column {
            get {
                return ncolumn;
            }
            set {
                if (value < 8) { ncolumn = value; }
            }
        }

        public override bool Equals(object obj) {
            return obj is Position position && Equals(position);
        }

        public bool Equals(Position other) {
            return (Column == other.Column) && (Row == other.Row);
        }

        public override int GetHashCode() {
            int hashCode = 240067226;
            hashCode = hashCode * -1521134295 + Row.GetHashCode();
            hashCode = hashCode * -1521134295 + Column.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Position left, Position right) {
            return left.Equals(right);
        }

        public static bool operator !=(Position left, Position right) {
            return !(left == right);
        }

    }

    class Game {
        public const byte PAWN = 1, KNIGHT = 2, BISHOP = 3, ROOK = 4, QUEEN = 5, KING = 6;
        public const bool WHITE = true, BLACK = false;
        /* 1: Pawn
         * 2: Knight
         * 3: Bishop
         * 4: Rook
         * 5: Queen
         * 6: King
         */



    }

    abstract class Piece {

        public Position position;
        public bool white;
        public static int type;
        public byte Row {
            get { return position.Row; }
            set {
                if (value < 8) { position.Row = value; }
            }
        }
        public byte Column {
            get { return position.Column; }
            set {
                if (value < 8) { position.Column = value; }
            }
        }

        public abstract Position[] Moves();

        public virtual Position[] Threaten() {
            return Moves();
        }


    }

    class Pawn : Piece {
        new public static int type = Game.PAWN;

        public Pawn(Position startPosition, bool isWhite) {
            position = startPosition;
            white = isWhite;
        }

        public override Position[] Moves() {
            throw new NotImplementedException();
        }
    }
}
