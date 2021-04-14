﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI {

    public struct Position : IEquatable<Position> //Define a struct named Position. A struct is a custom datatype. Inheriting IEquatable tells the code that instances of Position can be checked for equality with each other
    {
        public Position(byte row, byte column) //Structs have class-like constructor methods. A byte is just an int that only spans one byte in memory.
                                               //Since the positions on a chess board are small numbers, I can use them here, and they are a little faster and more memory efficient.
        {
            index = (byte)(row * 8 + column);
            if (index >= 64) {
                throw new ArgumentOutOfRangeException("Both row and column must be less than 8");
            }
        }
        public Position(byte Index) {
            index = Index;
            if (index >= 64) {
                throw new ArgumentOutOfRangeException("Index must be less than 64");
            }
        }
        public Position(int Index) {
            index = (byte)Index;
            if (index >= 64 || index < 0) {
                throw new ArgumentOutOfRangeException("Index must be an integer which is greater than or equal to 0 and less than 64");
            }
        }
        public Position(int row, int column) {
            index = (byte)(row * 8 + column);
            if (index >= 64 || index < 0) {
                throw new ArgumentOutOfRangeException("Both row and column must be integers which are greater than or equal to 0 and less than 8");
            }
        }


        private byte index;
        public byte Row //Define a property named Row.
        {
            get => (byte)(index >> 3); //This => operator just tells it to define the function on the left to return the expression on the right. It's a lot like lambda in python.
            //Return index bit shifted right 3, getting just the first 3 bits (if we pretend it is 6 bits not 32) which hold the row
            set //The set method of a property tells the code what to do when code tries to set the value of the property. Inside the set method, there is a special variable named value which is just whatever the property is being set to.
            {
                if ((0 <= value) && (value < 8)) //Check if the value is 0, 1, 2, 3, 4, 5, 6, or 7
                {
                    index = (byte)(Column + value); //Set the internal value
                }
                else {
                    throw new ArgumentOutOfRangeException("Row", "Row must be greater than or equal to 0 and less than 8");
                    //Crash the program with an error message. Functions setting a position should be doing their own checks to make sure the value makes sense; clamping it could lead to weird, hard to track down bugs.
                    //It's best to just crash and let me know immediately what's going wrong.
                }
            }
        }
        public byte Column //Define property named Column. Identical to Row except that it's the columns.
        {
            get => (byte)(index & 0b00000111);
            //Return the last three bits of the index using bitwise and comparions. I have since learned that the number I'm anding with is called the mask, but it just seemed like a good way to retrieve the last three bits to me
            set {
                if ((0 <= value) && (value < 8)) {
                    index = (byte)(Row + value);
                }
                else {
                    throw new ArgumentOutOfRangeException("Column", "Column must be greater than or equal to 0 and less than 8");
                }
            }
        }

        public override bool Equals(object obj) //This Equals method was auto-generated when I told it to inherit IEquatable, I don't fully understand what it's doing but I will explain to the best of my ability
        {
            return obj is Position position && Equals(position); //I think it is saying that if the other object is of type position, run the other Equals method to check if they are equal
        }

        public bool Equals(Position other) //This is the one that I defined, it takes in another Position and returns true if both the row and column of the other are equal to the row and column of this position.
        {
            return (index == other.index);
        }

        public override int GetHashCode() //This is part of IEquatable, it is supposed to return an integer representation of any given instance of Position which is consistent (two equal Positions will return the same hash) but not necessarily unique (two different Positions do not necessarily need to return different hashes). 
        {
            return index; //Since the core of the struct is just a wrapper over an integer, we can just return that
        }

        public static bool operator ==(Position left, Position right) //This was autogenerated, but I understand it fully. It defines how the == operator behaves between two positons, returning the value of the Equals method
        {
            return left.Equals(right);
        }

        public static bool operator !=(Position left, Position right) //This is the same as the above except it's for != (not equals), and works the same way but inverts the output
        {
            return !(left == right);
        }

        public static implicit operator byte(Position position) => position.index;
        public static implicit operator Position(byte index) => new Position(index);
        public static implicit operator int(Position position) => position.index;
        public static implicit operator Position(int index) => new Position((byte)index);

        public static int operator +(Position position) => position.index;
        public static int operator -(Position position) => position.index;
        public static int operator +(Position a, Position b) => a.index + b.index;
        public static int operator -(Position a, Position b) => a.index - b.index;
        public static int operator +(Position a, int b) => a.index + b;
        public static int operator +(int a, Position b) => a + b.index;
        public static int operator -(Position a, int b) => a.index - b;
        public static int operator -(int a, Position b) => a - b.index;
        public static bool operator ==(int a, Position b) => (a == b.index);
        public static bool operator !=(int a, Position b) => (a != b.index);
        public static bool operator ==(Position a, int b) => (a.index == b);
        public static bool operator !=(Position a, int b) => (a.index != b);
        public static bool operator <(Position a, Position b) => (a.index < b.index);
        public static bool operator >(Position a, Position b) => (a.index > b.index);
        public static bool operator <=(Position a, Position b) => (a.index <= b.index);
        public static bool operator >=(Position a, Position b) => (a.index >= b.index);
        public static bool operator < (int a, Position b) => (a < b.index);
        public static bool operator > (int a, Position b) => (a > b.index);
        public static bool operator <=(int a, Position b) => (a <= b.index);
        public static bool operator >=(int a, Position b) => (a >= b.index);
        public static bool operator < (Position a, int b) => (a.index < b);
        public static bool operator > (Position a, int b) => (a.index > b);
        public static bool operator <=(Position a, int b) => (a.index <= b);
        public static bool operator >=(Position a, int b) => (a.index >= b);//Operator definitions


    }

    static class Game //Define a class called Game. I am going to use this class to hold various information that may need to be accessed in multiple places but which does not necessarily belong anywhere else.
               //It will mostly hold constant values that I may want to give names to but that are actually just aliases for simple constants, but it may also contain the gamestate later, I am yet to decide
    {
        public const byte PAWN = 1, KNIGHT = 2, BISHOP = 3, ROOK = 4, QUEEN = 5, KING = 6, PASSANT = 7;
        public const bool WHITE = true, BLACK = false;
        public static readonly int[] diagonals = { 9, 7, -7, -9 };
        public static readonly int[] laterals = { 8, 1, -1, -8 };
        public static readonly int[] adjacents = { 9, 8, 7, 1, -1, -7, -8, -9 };
        public static readonly int[] knightMoves = { 6, 15, 17, 10, -6, -15, -17, -10 };
        /* 1: Pawn
         * 2: Knight
         * 3: Bishop
         * 4: Rook
         * 5: Queen
         * 6: King
         */
        public static List<Position> SlideMoves(GameState board, int[] directions, Piece mover) {

            List<Position> output = new List<Position>();
            foreach (int direction in directions) {
                for (int dist = direction; 0 <= (mover.position + dist) && (mover.position + dist) < 64; dist += direction) {
                    if (mover.CanMoveTo(mover.position + dist, board)) {
                        output.Add(mover.position + dist);
                    }
                    if (board.state.ContainsKey(mover.position + dist)) {
                        switch (board.state[mover.position + dist].RenderID) {
                            case 0b1111:
                            case 0b0111:
                                continue;
                            default:
                                goto NextDirection;
                        }
                    }
                }
                NextDirection: { }
            }
            return output;

        }

        public static bool GetBit(this byte self, byte index) {
            return (self & (1 << index)) == 1;
        }
        public static bool GetBit(this int self, byte index) {
            return (self & (1 << index)) == 1;
        }

        public static byte BitSum(this byte self) {
            byte count; // c accumulates the total bits set in v
            for (count = 0; self != 0; count++) {
                self = (byte)(self & (self - 1)); // clear the least significant bit set
            }
            return count;
        }
    }

    public abstract class Piece {
        public Position position;
        public bool IsWhite;
        public bool hasMoved = false;
        public virtual byte type { get; }

        public byte RenderID {
            get {
                return (byte)(type + (Convert.ToInt32(IsWhite) << 3));
            }
        }

        public abstract List<Position> Moves(GameState board);
        public virtual List<Position> Threaten(GameState board) => Moves(board);

        public override int GetHashCode() {
            return (RenderID + (Convert.ToInt32(hasMoved) << 4) + (position << 10));
        }
        public static explicit operator int(Piece piece) => piece.GetHashCode();
        

        

        public virtual void Moved(Position move, GameState board) {
            hasMoved = true;
            position = move;
        }

        public bool CanCapture(Piece other) {
            return other.CanBeCaptured(this);
        }

        public virtual bool CanMoveTo(Position position, GameState board) {
            Piece target;
            if (board.state.TryGetValue(position, out target)) {
                return CanCapture(target);
            }
            else {
                return true;
            }
        }

        public virtual bool CanBeCaptured(Piece other) {
            return (other.IsWhite != IsWhite);
        }
    }

    class Pawn : Piece {
        public override byte type => Game.PAWN;
        public override List<Position> Moves(GameState board) {
            List<Position> output = new List<Position>();
            int rowDifference = IsWhite ? -8 : 8; //Set rowDifference to -8 if IsWhite, 8 otherwise
            if (!board.state.ContainsKey(position + rowDifference)) { //If there is no piece directly in front of the pawn
                output.Add(position + rowDifference); //Add the move directly in front of the pawn to the output list
            }
            for (int i = position.Column == 0 ? 1 : -1; i <= (position.Column == 7 ? -1 : 1); i += 2) { //Loop through the possible column offsets using inline conditionals to filter out nonexistant columns
                Piece target;
                if (board.state.TryGetValue(position + rowDifference + i, out target)) { //Check if the position has a piece at that location and if so what it is
                    if (CanCapture(target)) {
                        output.Add(position + rowDifference + i);
                    }
                }
            }
            if (!((hasMoved) || (board.state.ContainsKey(position + rowDifference)) || (board.state.ContainsKey(position + rowDifference + rowDifference)))) { //If the piece hasn't moved and there are no pieces in the first two positions in front of it, add double pawn move to the output
                output.Add(position + rowDifference * 2);
            }
            return output;
        }

        public override List<Position> Threaten(GameState board) {
            int rowDifference = IsWhite ? -8 : 8; //Set rowDifference to -8 if IsWhite, 8 otherwise
            
            List<Position> output = new List<Position>();
            for (int i = position.Column == 0 ? 1 : -1; i <= (position.Column == 7 ? -1 : 1); i++) { //Loop through the possible column offsets using inline conditionals to filter out nonexistant columns
                output.Add(position + rowDifference + i);
            }
            return output;
        }

        public override void Moved(Position move, GameState board) {
            hasMoved = true;
            if (Math.Abs(move - position) == 16) {
                int rowOffset = IsWhite ? -8 : 8;
                if (move - position == 2 * rowOffset) {
                    board.state.Add(position + rowOffset, new EnPassantPlaceholder(position + rowOffset, IsWhite, board));
                }
                else if (move - position == -2 * rowOffset) {
                    hasMoved = false;
                }
            }
            position = move;
        }

        public Pawn(Position startPosition, bool isWhite) {
            position = startPosition;
            IsWhite = isWhite;
        }
    }

    class EnPassantPlaceholder : Piece {
        public override byte type => Game.PASSANT;
        private bool extant = true;
        public override List<Position> Moves(GameState board) {
            return new List<Position>(); //Return an empty list, the placeholder can't move anywhere
        }

        public override List<Position> Threaten(GameState board) {
            return new List<Position>();
        }

        public override bool CanBeCaptured(Piece other) {
            return (other.IsWhite != IsWhite) || (other.type != Game.PAWN);
        }

        public EnPassantPlaceholder(Position Position, bool isWhite, GameState game) {
            position = Position;
            IsWhite = isWhite;
            game.TurnCompleted += placeholder_TurnCompleted;

        }

        void placeholder_TurnCompleted(object sender, GameState args) {
            if (extant)
            {
                Piece piece;
                if (args.state.TryGetValue(position, out piece)) {
                    if (piece == this) {
                        args.state.Remove(position);
                    }
                    else if (IsWhite && !piece.IsWhite && piece.type == Game.PAWN) {
                        args.state.Remove(new Position((byte)(position.Row - 1), position.Column));
                    }
                    else if (!IsWhite && piece.IsWhite && piece.type == Game.PAWN) {
                        args.state.Remove(new Position((byte)(position.Row + 1), position.Column));
                    }
                }
                extant = false;
                args.TurnCompleted -= placeholder_TurnCompleted;
                args.Undo += placeholder_Undo;
            }
            else {
                args.Undo -= placeholder_Undo;
            }

        }

        void placeholder_Undo(object sender, GameState args) {
            if (!extant) {
                extant = true;
                args.state.Add(position, this);
                args.Undo -= placeholder_Undo;
                args.TurnCompleted += placeholder_TurnCompleted;
            }
        }
    }


    class Knight : Piece {
        public override byte type { get => Game.KNIGHT; } //Setting the type property of all knights to KNIGHT
        //I should explain what these keywords do:
        //new tells the compiler that I want to override the implementation of the same variable from the base class
        //public means the variable can be accessed by code from outside this class
        //static means this is the same for all instances of Knight
        //And int, predictably, means integer

        public Knight(Position startPosition, bool isWhite) //I don't think its necessary to explain these over and over again since they're the same for most of the pieces, so I'll just explain the differences
        {
            position = startPosition;
            IsWhite = isWhite;
        }

        public override bool CanMoveTo(Position position, GameState board) {
            return base.CanMoveTo(position, board) && ((position.Row.GetBit(0) == position.Column.GetBit(0)) != (this.position.Row.GetBit(0) == this.position.Column.GetBit(0)));
        }

        public override List<Position> Moves(GameState board) {
            List<Position> output = new List<Position>();
            
            foreach (int offSet in Game.knightMoves) {
                int move = position + offSet;
                if (0 <= move && move < 64) {
                    if (CanMoveTo(move, board)) {
                        output.Add(move);
                    }
                }
            }
            return output;
        }
    }

    class Bishop : Piece {
        public override byte type { get => Game.BISHOP; }

        public Bishop(Position startPosition, bool isWhite) {
            position = startPosition;
            IsWhite = isWhite;
        }

        public override bool CanMoveTo(Position position, GameState board) {
            return base.CanMoveTo(position, board) && ((position.Row.GetBit(0) == position.Column.GetBit(0)) == (this.position.Row.GetBit(0) == this.position.Column.GetBit(0)));
        }

        public override List<Position> Moves(GameState board) {
            return Game.SlideMoves(board, Game.diagonals, this);
        }
    }

    class Rook : Piece {
        public override byte type { get => Game.ROOK; }

        public Rook(Position startPosition, bool isWhite, bool canCastle = true) {
            position = startPosition;
            IsWhite = isWhite;
            hasMoved = !canCastle;
        }

        public override bool CanMoveTo(Position position, GameState board) {
            return base.CanMoveTo(position, board) && ((position.Row == this.position.Row) != (position.Column == this.position.Column));
        }

        public override List<Position> Moves(GameState board) {
            return Game.SlideMoves(board, Game.laterals, this);
        }
    }

    class Queen : Piece {
        public override byte type { get => Game.QUEEN; }
        private bool moveMode = false;

        public Queen(Position startPosition, bool isWhite) {
            position = startPosition;
            IsWhite = isWhite;
        }

        public override bool CanMoveTo(Position position, GameState board) {
            bool bishopCould = ((position.Row.GetBit(0) == position.Column.GetBit(0)) == (this.position.Row.GetBit(0) == this.position.Column.GetBit(0)));
            bool rookCould = ((position.Row == this.position.Row) != (position.Column == this.position.Column));
            return base.CanMoveTo(position, board) && (moveMode ? bishopCould : rookCould);
        }

        public override List<Position> Moves(GameState board) {
            List<Position> output = Game.SlideMoves(board, Game.laterals, this);
            moveMode = true;
            output.AddRange(Game.SlideMoves(board, Game.diagonals, this));
            moveMode = false;
            return output;
        }
    }

    class King : Piece {
        public override byte type { get => Game.KING; }

        public King(Position startPosition, bool isWhite, bool canCastle = true) {
            position = startPosition;
            IsWhite = isWhite;
            hasMoved = !canCastle;
        }

        public override List<Position> Moves(GameState board) {
            List<Position> output = new List<Position>();
            foreach (int adjacent in Game.adjacents) {
                try {
                    if (CanMoveTo(position + adjacent, board)) {
                        output.Add(position + adjacent);
                    }
                }
                catch (System.ArgumentOutOfRangeException) { }
            }

            return output;
        }

        public override bool CanMoveTo(Position position, GameState board) {
            return base.CanMoveTo(position, board) && !((((this.position & 0b111) == 0) && ((position & 0b111) == 7)) || (((this.position & 0b111) == 7) && ((position & 0b111) == 0))) && !board.Threatened(position, IsWhite, this);
        }

        public override List<Position> Threaten(GameState board) {
            List<Position> output = new List<Position>();
            foreach (int adjacent in Game.adjacents) {
                    output.Add(position + adjacent);
            }
            return output;
        }
    }
}
