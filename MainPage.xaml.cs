using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Graphics.Canvas.Svg;
using Microsoft.Graphics.Canvas.UI.Xaml;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ChessAI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        CanvasSvgDocument Board;
        CanvasSvgDocument PawnBlack;
        CanvasSvgDocument PawnWhite;
        CanvasSvgDocument KnightBlack;
        CanvasSvgDocument KnightWhite;
        CanvasSvgDocument BishopBlack;
        CanvasSvgDocument BishopWhite;
        CanvasSvgDocument RookBlack;
        CanvasSvgDocument RookWhite;
        CanvasSvgDocument QueenBlack;
        CanvasSvgDocument QueenWhite;
        CanvasSvgDocument KingBlack;
        CanvasSvgDocument KingWhite;

        public MainPage()
        {
            this.InitializeComponent();
        }
        // The actual renderer (currently Win2D)
        private void canvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args) {
            args.DrawingSession.DrawSvg(Board, sender.Size);

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e) {
            this.canvas.RemoveFromVisualTree();
            this.canvas = null;
        }

        private void canvas_CreateResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args) {
            Board = LoadAsset(sender, "Chess Board");
            PawnBlack = LoadAsset(sender, "PawnBlack");
            PawnWhite = LoadAsset(sender, "PawnWhite");
            KnightBlack = LoadAsset(sender, "KnightBlack");
            KnightWhite = LoadAsset(sender, "KnightWhite");
            BishopBlack = LoadAsset(sender, "BishopBlack");
            BishopWhite = LoadAsset(sender, "BishopWhite");
            RookBlack = LoadAsset(sender, "RookBlack");
            RookWhite = LoadAsset(sender, "RookWhite");
            QueenBlack = LoadAsset(sender, "QueenBlack");
            QueenWhite = LoadAsset(sender, "QueenWhite");
            KingBlack = LoadAsset(sender, "KingBlack");
            KingWhite = LoadAsset(sender, "KingWhite");

        }

        private CanvasSvgDocument LoadAsset(CanvasControl sender, string fileName) {
            var file = File.ReadAllText($"Assets/{fileName}.svg");
            return CanvasSvgDocument.LoadFromXml(sender, file);
        }
    }
}
