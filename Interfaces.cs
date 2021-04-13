using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Svg;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ChessAI {
    public class RenderTranslator {
        internal Dictionary<byte, CanvasSvgDocument> map = new Dictionary<byte, CanvasSvgDocument>(); //Define a dictionary which maps bytes to Svg documents named map which can only be directly accessed through instances of this class
        public RenderTranslator(
            CanvasSvgDocument PawnBlack, CanvasSvgDocument PawnWhite, CanvasSvgDocument KnightBlack, CanvasSvgDocument KnightWhite, CanvasSvgDocument BishopBlack, CanvasSvgDocument BishopWhite, CanvasSvgDocument RookBlack, CanvasSvgDocument RookWhite, CanvasSvgDocument QueenBlack, CanvasSvgDocument QueenWhite, CanvasSvgDocument KingBlack, CanvasSvgDocument KingWhite, CanvasSvgDocument PlaceholderBlack, CanvasSvgDocument PlaceholderWhite) {


            map.Add(0b0001, PawnBlack); //Map the RenderID of any given piece to the corresponding SVG object.
            map.Add(0b1001, PawnWhite); //The way the encoding works is simple; it is the type of the piece, with 16 added if the piece is white.
            map.Add(0b0010, KnightBlack); //This means that in binary, the RenderID is 0 or 1 depending on the colour of the piece, followed by the piece's type ID in binary.
            map.Add(0b1010, KnightWhite); //The 0b prefix on an integer literal indicates that I want to work with the integer in binary.
            map.Add(0b0011, BishopBlack); //The reason for doing it this way is that the hashing for a byte is unbelievably fast, so the dictionary lookup will take so close to no time as to not make a difference.
            map.Add(0b1011, BishopWhite);
            map.Add(0b0100, RookBlack);
            map.Add(0b1100, RookWhite);
            map.Add(0b0101, QueenBlack);
            map.Add(0b1101, QueenWhite);
            map.Add(0b0110, KingBlack);
            map.Add(0b1110, KingWhite);
            map.Add(0b0111, PlaceholderBlack);
            map.Add(0b1111, PlaceholderWhite);
        }
    }

    public class GameState //Temporarily putting this here while it is a dummy object for testing the other systems, will later be moved to it's own file
    {
        public Dictionary<Position, Piece> state;
        private Piece lastCapturedPiece;
        private Position[] lastMadeMove = new Position[2];
        public List<Tuple<Position, byte>> RenderInterface() {
            //Later, code will go here that will find all of the pieces and their locations, then return a list of tuples of all of the positions with a piece and the RenderID of that piece.
            //For now, it just returns a hard coded list of tuples for testing the renderer
            List<Tuple<Position, byte>> pieces = new List<Tuple<Position, byte>>(state.Count);
            foreach (Position key in state.Keys) {
                pieces.Add(new Tuple<Position, byte>(key, state[key].RenderID));
            }

            return pieces;
        }

        public GameState(Dictionary<Position, Piece> startState) {
            state = startState;

        }

        public GameState() {
            state = new Dictionary<Position, Piece>();
            //Initialize state
            for (byte column = 0; column < 8; column++) {
                Position BlackSpot = new Position((byte)1, column);
                Position WhiteSpot = new Position((byte)6, column);
                state.Add(WhiteSpot, new Pawn(WhiteSpot, Game.WHITE));
                state.Add(BlackSpot, new Pawn(BlackSpot, Game.BLACK));

            }
            //Add pawns
            state.Add(new Position(0, 0), new Rook(new Position(0, 0), false));
            state.Add(new Position(0, 7), new Rook(new Position(0, 7), false));
            state.Add(new Position(7, 0), new Rook(new Position(7, 0), true));
            state.Add(new Position(7, 7), new Rook(new Position(7, 7), true));
            //Add rooks
            state.Add(new Position(0, 1), new Knight(new Position(0, 1), false));
            state.Add(new Position(0, 6), new Knight(new Position(0, 6), false));
            state.Add(new Position(7, 1), new Knight(new Position(7, 1), true));
            state.Add(new Position(7, 6), new Knight(new Position(7, 6), true));
            //Add knights
            state.Add(new Position(0, 2), new Bishop(new Position(0, 2), false));
            state.Add(new Position(0, 5), new Bishop(new Position(0, 5), false));
            state.Add(new Position(7, 2), new Bishop(new Position(7, 2), true));
            state.Add(new Position(7, 5), new Bishop(new Position(7, 5), true));
            //Add bishops
            state.Add(new Position(0, 3), new Queen(new Position(0, 3), false));
            state.Add(new Position(7, 3), new Queen(new Position(7, 3), true));
            //Add Queens
            state.Add(new Position(0, 4), new King(new Position(0, 4), false));
            state.Add(new Position(7, 4), new King(new Position(7, 4), true));
            //Add Kings

        }

        public List<Position> Moves = new List<Position>();
        private Position selected;

        public void Select(Position position) {
            if (Moves.Count == 0) { //Check if moves is empty
                try {
                    Moves = state[position].Moves(this); //Set moves to all the possible moves of the piece at the position that was clicked
                }
                catch (System.Collections.Generic.KeyNotFoundException) { //Catch if they click a blank tile
                    Moves = new List<Position>(); //Reset Moves
                }
                selected = position; //Remember the position that was selected
            }
            else {

                if (Moves.Contains(position)) {
                    MakeMove(selected, position);
                }

                Moves = new List<Position>(); //Reset moves to nothing
            }

        }

        public event EventHandler<GameState> TurnCompleted; //Declare an event called TurnCompleted

        protected virtual void OnTurnCompleted(EventArgs args) { //Create a new event method for TurnCompleted
            EventHandler<GameState> handler = TurnCompleted; //Set the event method to be for the TurnCompleted event
            handler?.Invoke(this, this); //Invoke the event (cause all subscribers to the event to handle the event)
        }

        public virtual void OnForcedTurn(EventArgs args) {
            EventHandler<GameState> handler = TurnCompleted; //Set the event method to be for the TurnCompleted event
            handler?.Invoke(this, this); //Invoke the event (cause all subscribers to the event to handle the event)
        }


        public bool Threatened(Position position, bool white, Piece mover) {
            state.Remove(mover.position);
            Queen virtualQueen = new Queen(position, white);
            Knight virtualKnight = new Knight(position, white);
            List<Position> candidates = new List<Position>(virtualQueen.Threaten(this));
            candidates.AddRange(virtualKnight.Threaten(this));
            
            IEnumerable<Position> Pieces = candidates.Intersect<Position>(state.Keys);
            
            Pieces = Pieces.Where((Position n) => (state[n].IsWhite != white) && state[n].Threaten(this).Contains(position));
            state.Add(mover.position, mover);
            return Pieces.Count() != 0;
        }

        public void MakeMove(Position startPosition, Position endPosition) {
            lastCapturedPiece = null;
            Piece piece = state[startPosition]; //Get a reference to the piece being moved before removing it from the dictionary so it isn't destroyed
            state.Remove(startPosition); //Remove the piece being moved from its current board position
            if (state.TryGetValue(endPosition, out lastCapturedPiece)) 
            {
                state.Remove(endPosition); //Capture any pieces at the location the piece is moving to (it is the piece's job to not add positions they cannot capture such as allies to the move list)
            }
            state.Add(endPosition, piece); //Add the piece back to the board in its new position
            OnTurnCompleted(EventArgs.Empty); //Raise the TurnCompleted event, so that any logic that needs to happen at the end of a turn can do so
            piece.Moved(endPosition, this); //Tell the piece that it moved and where
            lastMadeMove[0] = startPosition; //Save the last made move
            lastMadeMove[1] = endPosition; //Save the last made move
        }

        public void UnMakeMove() {
            Piece temp = null; //Set to a null reference or the compiler has issues with using it in the second if statement even though it will always be initialized if that if staement runs
            if (lastCapturedPiece != null) { //If a piece was captured last move
                temp = lastCapturedPiece;
            }
            MakeMove(lastMadeMove[1], lastMadeMove[0]); //Make the inverse of the last move (this will sometimes make an invalid move like moving a pawn backwards, but that's fine because MakeMove doesn't check for that)
            if (lastCapturedPiece != null) { //If a piece was captured last move
                state.Add(temp.position, temp);
            }
        }

    }


    partial class MainPage { //Add stuff to the MainPage class from this file. This allows the code to run from within MainPage to easily get input data, but still be logically separated as the input from the renderer
        public void input_MouseClicked(object sender, PointerRoutedEventArgs args) {
            //Handles mouse click
            canvas.CapturePointer(args.Pointer); //Necessary for this event because Microsoft declared it to be so and thus it was and as far as I can tell for no other reason,
                                                 //all it does is tell every other object to not handle this event.
            args.Handled = true; //This also tells other objects not to handle the event. I don't know why the last thing wasn't enough but the docs say this is necessary so I guess we're keeping it
            //And that's the end of the stuff that is necessary because it was declared from on high, everything else in this function is the actual logic

            PointerPoint rawPoint = args.GetCurrentPoint(canvas); //Figure out where the pointer is relative to the canvas
            byte row = (byte)(rawPoint.Position.Y / pieceSize.Width);
            byte column = (byte)(rawPoint.Position.X / pieceSize.Height);
            game.Select(new Position(row, column));



            canvas.Invalidate(); //Indicates that the canvas should be redrawn
        }

        public void OnCtrlZInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) {
            game.UnMakeMove();

            canvas.Invalidate();
        }
        public void OnCtrlTInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) {
            game.OnForcedTurn(EventArgs.Empty);
            canvas.Invalidate();
        }
    }
}
