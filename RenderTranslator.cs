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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ChessAI
{
    public class RenderTranslator
    {
        internal Dictionary<byte, CanvasSvgDocument> map = new Dictionary<byte, CanvasSvgDocument>();
        public RenderTranslator(
            CanvasSvgDocument PawnBlack, CanvasSvgDocument PawnWhite, CanvasSvgDocument KnightBlack, CanvasSvgDocument KnightWhite, CanvasSvgDocument BishopBlack, CanvasSvgDocument BishopWhite, CanvasSvgDocument RookBlack, CanvasSvgDocument RookWhite, CanvasSvgDocument QueenBlack, CanvasSvgDocument QueenWhite, CanvasSvgDocument KingBlack, CanvasSvgDocument KingWhite)
        {
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
        }
    }

    public class GameState //Temporarily putting this here while it is a dummy object for testing the other systems
    {
        public List<Tuple<Position, byte>> RenderInterface()
        {
            //Later, code will go here that will find all of the pieces and their locations, then return a list of tuples of all of the positions with a piece and the RenderID of that piece.
            //For now, it just returns a hard coded list of tuples for testing the renderer
            List<Tuple<Position, byte>> pieces = new List<Tuple<Position, byte>>(16);
            for (byte column = 0; column < 8; column++)
            {
                pieces.Add(new Tuple<Position, byte>(new Position(1, column), 0b0011)); //Adding all the pawns
                pieces.Add(new Tuple<Position, byte>(new Position(6, column), 0b1011));

            }
            if (pieces == null)
            {
                throw new Exception("The problem was GameState");
            }


            return pieces;
        }

        
    }
}
