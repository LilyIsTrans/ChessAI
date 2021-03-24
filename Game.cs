﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI
{

    public struct Position : IEquatable<Position>
    {
        public Position(byte row, byte column)
        {
            nrow = row;
            ncolumn = column;
        }
        private byte nrow;
        private byte ncolumn;
        public byte Row
        {
            get => nrow;
            set
            {
                if ((0 <= value) && (value < 8))
                {
                    ncolumn = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Row", "Row must be greater than or equal to 0 and less than 8");
                }
            }
        }
        public byte Column
        {
            get => ncolumn;
            set
            {
                if ((0 <= value) && (value < 8)) {
                    ncolumn = value; 
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Column", "Column must be greater than or equal to 0 and less than 8");
                }
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Position position && Equals(position);
        }

        public bool Equals(Position other)
        {
            return (Column == other.Column) && (Row == other.Row);
        }

        public override int GetHashCode()
        {
            int hashCode = 240067226;
            hashCode = hashCode * -1521134295 + Row.GetHashCode();
            hashCode = hashCode * -1521134295 + Column.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Position left, Position right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }

    }

    class Game
    {
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

    abstract class Piece //Defining a class as abstract tells the program that I intend to use this class to define general properties that many subclasses will inherit, but will not actually instantiate any instances of the base class
    {

        public Position position; //Declare that there will be a Position type variable named position, but don't define it
        public bool white; //Declare a bool (boolean, true/false) to keep track of whether a given piece is white
        public static int type; //Declare an integer named type. This will be set in each of the subclasses
        public byte Row //Define a property named Row. Properties are declared like variables, but defined sort of like functions with get and set methods that tell the code how to find the value.
                        //It's basically saying that I want to be able to treat this like a variable when I use it elsewhere, but run some code to actually determine the value
        {
            get => position.Row;
            set
            {
                if (value < 8) { position.Row = value; }
            }
        }
        public byte Column //Define a property named Column
        {
            get => position.Column;
            set
            {
                if (value < 8) { position.Column = value; }
            }
        }

        public abstract Position[] Moves(); //Define an abstract method named Moves(). An abstract method is a method declared in an abstract class but with no implementation, to indicate that all subclasses
                                            //must define an implementation of the method. In this case, Moves() generates the legal moves a piece can take, which must be defined for each piece but is also
                                            //different for each piece, so it is defined as an abstract method to indicate that I must define it for all pieces.

        public virtual Position[] Threaten() //Define a virtual method named Threaten(). A virtual method is like an abstract method, but it has a default implementation which can be overridden by subclasses
                                             //but doesn't need to be. In this case, most pieces simply threaten all of the positions which are legal moves, but pawns function differently, so I define
                                             //a virtual method for Threaten which defines it as simply returning Moves() for most pieces, but allows me to override it in Pawn where it works differently.
        {
            return Moves();
        }


    }

    class Pawn : Piece //Each of the piece types will be its own class which inherits from the abstract Piece class but with its own implementation of some things for the differences between pieces
    {
        new public static int type = Game.PAWN; 
        //I'm using static variables to be able to use a name for the types of different pieces in various functions without needing to pass strings or class instances around, which is much less efficient and
        //easy in C# than in python. Instead, I've named some variables with integer values that are more efficient and easy to handle but they still have names so I don't need to remember them.

        public Pawn(Position startPosition, bool isWhite) //The constructor method for Pawn is called when a Pawn is created. It takes in a position (the initial position of the pawn) and whether the pawn is white
        {
            position = startPosition; //Setting the internal position variable to the value of the temporary startPosition variable
            white = isWhite; //Setting the internal isWhite variable to the value of the temporary white variable
        }

        public override Position[] Moves()
        {
            throw new NotImplementedException();
            //This needs to be implemented later, but for now it just crashes the program
        }

        public override Position[] Threaten()
        {
            Position[] output;
            //Initialize the existance of output but don't define its size or values yet
            switch (Column)
            {
                default:
                    {
                        output = new Position[2];
                        output[0].Column = (byte)(Column - 1);
                        output[1].Column = (byte)(Column + 1);
                        break;
                    }
                case 0:
                    {
                        output = new Position[1];
                        output[0].Column = 1;
                        break;
                    }
                case 7:
                    {
                        output = new Position[1];
                        output[0].Column = 1;
                        break;
                    }
            }
            //Figure out how many positions should be in the output based on the current column of the pawn.
            if (white && Row > 0) {
                for (int i = 0; i < output.Length; i++)
                {
                    output[i].Row = (byte)(Row - 1);
                }
            }
            // Handle the case where the pawn is white and has not reached the end
            else if (!white && Row < 8) {
                for (int i = 0; i < output.Length; i++)
                {
                    output[i].Row = (byte)(Row + 1);
                }
            }
            // Handle the case where the pawn is black and has not reached the end
            else
            {
                return null; //If we are in the final row, there must be no positions left. Technically, this code should never run, since a pawn which reaches the end must promote,
                             //but in case that gets implemented later on I'll handle the case
            }
            // This code should never run, see comment inside
            return output;
            //Return the result
        }

    }
    

    class Knight : Piece
    {
        new public static int type = Game.KNIGHT; //Setting the type property of all knights to KNIGHT
        //I should explain what these keywords do:
        //new tells the compiler that I want to override the implementation of the same variable from the base class
        //public means the variable can be accessed by code from outside this class
        //static means this is the same for all instances of Knight
        //And int, predictably, means integer

        public Knight(Position startPosition, bool isWhite) //I don't think its necessary to explain these over and over again since they're the same for most of the pieces, so I'll just explain the differences
        {
            position = startPosition;
            white = isWhite;
        }

        public override Position[] Moves()
        {
            throw new NotImplementedException();
        }
    }

    class Bishop : Piece
    {
        new public static int type = Game.BISHOP;

        public Bishop(Position startPosition, bool isWhite)
        {
            position = startPosition;
            white = isWhite;
        }

        public override Position[] Moves()
        {
            throw new NotImplementedException();
        }
    }

    class Rook : Piece
    {
        new public static int type = Game.ROOK;
        public bool canCastle; //Since the rook needs to keep track of whether or not it has moved on account of being involved in castling, it get a bool called canCastle for that purpose
        //Confusingly, overriding properties from the base class requires the new keyword or the compiler complains (though it does actually still work), but creating new properties does not
        //require the new keyword

        public Rook(Position startPosition, bool isWhite, bool castle = true) //Much like python, default inputs for a function can be set by setting the variable in the function definition
        {
            position = startPosition;
            white = isWhite;
            canCastle = castle; //Also copying the castle value
        }

        public override Position[] Moves()
        {
            throw new NotImplementedException();
        }
    }

    class Queen : Piece
    {
        new public static int type = Game.QUEEN;

        public Queen(Position startPosition, bool isWhite)
        {
            position = startPosition;
            white = isWhite;
        }

        public override Position[] Moves()
        {
            throw new NotImplementedException();
        }
    }

    class King : Piece
    {
        new public static int type = Game.KING;
        public bool canCastle; //The king also needs to keep track of its eligibility to castle
        //The king pieces do not directly worry about check and checkmate, that's the game state's job

        public King(Position startPosition, bool isWhite, bool castle = true)
        {
            position = startPosition;
            white = isWhite;
            canCastle = castle;
        }

        public override Position[] Moves()
        {
            throw new NotImplementedException();
        }
    }
}
