using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using UICharts.Core.Enums;
using UICharts.Core.Factorys;
using UICharts.Core.Models;
using UICharts.Desktop.Interaction;
using UICharts.Desktop.Services;
using UICharts.Desktop.Services.Interfaces;

namespace UICharts.Desktop.ViewModels
{
    public class EditorViewModel : BindableBase
    {
        private readonly IBlockDragService dragService;

        private readonly IConnectionService connectionService;

        private readonly IDiagramMappingService mappingService;

        private readonly ISelectionService selectionService;

        private readonly IDeleteService deleteService;

        private readonly IBlockResizeService resizeService;

        private readonly IConnectionRoutingService routingService;

        private DiagramModel? currentDiagram;
        public DiagramModel? CurrentDiagram
        {
            get => currentDiagram;
            set
            {
                if (SetProperty(ref currentDiagram, value))
                {
                    LoadBlocksFromDiagram();
                }
            }
        }
        public ObservableCollection<BlockViewModel> Blocks => blocks;

        private BlockViewModel? selectedBlock;

        private readonly ObservableCollection<BlockViewModel> blocks = new();

        public BlockViewModel? SelectedBlock
        {
            get => selectedBlock;
            set => SetProperty(ref selectedBlock, value);
        }
        public ObservableCollection<ConnectionViewModel> Connections { get; } = new();

        private bool isConnectionMode;
        public bool IsConnectionMode
        {
            get => isConnectionMode;
            set => SetProperty(ref isConnectionMode, value);
        }

        private FigureItemModel? selectedFigure;

        public FigureItemModel? SelectedFigure
        {
            get => selectedFigure;
            set => SetProperty(ref selectedFigure, value);
        }

        private bool isConnectionPreviewVisible;

        public bool IsConnectionPreviewVisible
        {
            get => isConnectionPreviewVisible;
            set
            {
                if (SetProperty(ref isConnectionPreviewVisible, value))
                    RaisePropertyChanged(nameof(PreviewRoutePoints));
            }
        }

        private Point previewStartPoint;
        public Point PreviewStartPoint
        {
            get => previewStartPoint;
            set
            {
                if (SetProperty(ref previewStartPoint, value))
                    RaisePropertyChanged(nameof(PreviewRoutePoints));
            }
        }


        private Point previewEndPoint;
        public Point PreviewEndPoint
        {
            get => previewEndPoint;
            set
            {
                if (SetProperty(ref previewEndPoint, value))
                    RaisePropertyChanged(nameof(PreviewRoutePoints));
            }
        }
        private ConnectionViewModel? selectedConnection;

        public ConnectionViewModel? SelectedConnection
        {
            get => selectedConnection;
            set => SetProperty(ref selectedConnection, value);
        }

        public DelegateCommand<ConnectionViewModel> SelectConnectionCommand { get; }

        private BlockViewModel? previewStartBlock;
        private Size currentCanvasSize;

        private ConnectionViewModel? draggedConnection;
        private bool isDraggingConnectionEnd;

        public DelegateCommand<System.Windows.Point?> CanvasClickCommand { get; }
        public DelegateCommand<BlockViewModel> SelectBlockCommand { get; }
        public DelegateCommand<BlockViewModel> BeginEditBlockCommand { get; }
        public DelegateCommand<BlockViewModel> EndEditBlockCommand { get; }
        public DelegateCommand<BlockMouseEventArgs> BlockMouseDownCommand { get; }
        public DelegateCommand<Point?> BlockMouseMoveCommand { get; }
        public DelegateCommand BlockMouseUpCommand { get; }
        public DelegateCommand ToggleConnectionModeCommand { get; }
        public DelegateCommand DeleteSelectedCommand { get; }
        public DelegateCommand<BlockMouseEventArgs> BlockResizeMouseDownCommand { get; }
        public DelegateCommand<Point?> BlockResizeMouseMoveCommand { get; }
        public DelegateCommand BlockResizeMouseUpCommand { get; }
        public DelegateCommand<Point?> CanvasMouseMoveCommand { get; }

        public DelegateCommand<ConnectionViewModel> ConnectionEndMouseDownCommand { get; }
        public DelegateCommand<Point?> ConnectionEndMouseMoveCommand { get; }
        public DelegateCommand<Point?> ConnectionEndMouseUpCommand { get; }
        public EditorViewModel(
            IBlockDragService dragService,
            IConnectionService connectionService,
            IDiagramMappingService mappingService,
            ISelectionService selectionService,
            IDeleteService deleteService,
            IBlockResizeService resizeService,
            IConnectionRoutingService routingService
            )
        {
            this.dragService = dragService;
            this.connectionService = connectionService;
            this.mappingService = mappingService;
            this.selectionService = selectionService;
            this.deleteService = deleteService;
            this.resizeService = resizeService;
            this.routingService = routingService;

            CanvasClickCommand = new DelegateCommand<System.Windows.Point?>(OnCanvasClick);
            SelectBlockCommand = new DelegateCommand<BlockViewModel>(OnSelectBlock);
            BeginEditBlockCommand = new DelegateCommand<BlockViewModel>(OnBeginEditBlock);
            EndEditBlockCommand = new DelegateCommand<BlockViewModel>(OnEndEditBlock);
            DeleteSelectedCommand = new DelegateCommand(OnDeleteSelected);
            SelectConnectionCommand = new DelegateCommand<ConnectionViewModel>(OnSelectConnection);

            ToggleConnectionModeCommand = new DelegateCommand(() =>
            {
                IsConnectionMode = !IsConnectionMode;
                connectionService.Reset();
            });

            BlockMouseDownCommand = new DelegateCommand<BlockMouseEventArgs>(OnBlockMouseDown);
            BlockMouseMoveCommand = new DelegateCommand<Point?>(OnBlockMouseMove);
            BlockMouseUpCommand = new DelegateCommand(OnBlockMouseUp);

            BlockResizeMouseDownCommand = new DelegateCommand<BlockMouseEventArgs>(OnBlockResizeMouseDown);
            BlockResizeMouseMoveCommand = new DelegateCommand<Point?>(OnBlockResizeMouseMove);
            BlockResizeMouseUpCommand = new DelegateCommand(OnBlockResizeMouseUp);
            CanvasMouseMoveCommand = new DelegateCommand<Point?>(OnCanvasMouseMove);
            ConnectionEndMouseDownCommand = new DelegateCommand<ConnectionViewModel>(OnConnectionEndMouseDown);
            ConnectionEndMouseMoveCommand = new DelegateCommand<Point?>(OnConnectionEndMouseMove);
            ConnectionEndMouseUpCommand = new DelegateCommand<Point?>(OnConnectionEndMouseUp);
        }



        private void LoadBlocksFromDiagram()
        {
            connectionService.Reset();
            SelectedBlock = null;

            mappingService.Map(CurrentDiagram, Blocks, Connections);
        }

        private void OnCanvasMouseMove(Point? point)
        {
            if (point == null)
                return;

            if (!IsConnectionPreviewVisible || previewStartBlock == null)
                return;

            var mousePoint = point.Value;

            var targetBlock = Blocks
                .Where(block => block != previewStartBlock)
                .FirstOrDefault(block =>
                    mousePoint.X >= block.X &&
                    mousePoint.X <= block.X + block.Width &&
                    mousePoint.Y >= block.Y &&
                    mousePoint.Y <= block.Y + block.Height);
            var points = connectionService.GetPreviewPoints(
                previewStartBlock,
                targetBlock,
                mousePoint);
            PreviewStartPoint = points.Start;
            PreviewEndPoint = points.End;
        }
        private void OnCanvasClick(System.Windows.Point? point)
        {
            if (point == null || CurrentDiagram == null || SelectedFigure == null)
                return;

            if (selectionService.IsEditingAny(Blocks))
                return;

            var figure = new BlockFigureFactory()
            .GetFigure(SelectedFigure.Type);

            var model = new BlockModel
            {
                X = point.Value.X,
                Y = point.Value.Y,
                Width = figure.DefaultWidth,
                Height = figure.DefaultHeight,
                Text = SelectedFigure.Name,
                Type = SelectedFigure.Type
            };

            CurrentDiagram.Blocks.Add(model);

            var vm = new BlockViewModel(model);
            Blocks.Add(vm);

            selectionService.ClearSelection(Blocks);
            vm.IsSelected = true;
            SelectedBlock = vm;
            SelectedFigure = null;
           //selectionService.ClearSelection(Blocks);
        }

        private void OnSelectBlock(BlockViewModel? block)
        {
            if (block == null)
                return;

            selectionService.SelectBlock(Blocks, block, out var selected);
            SelectedBlock = selected;
        }

        private void OnBeginEditBlock(BlockViewModel? block)
        {
            if (block == null)
                return;

            selectionService.BeginEdit(Blocks, block, out var selected);
            SelectedBlock = selected;
        }

        private void OnEndEditBlock(BlockViewModel? block)
        {
            if (block == null)
                return;

            selectionService.EndEdit(block);
        }



        public void HandleConnectionClick(
            BlockViewModel block,
            Point mousePosition)
         {
            if (!IsConnectionPreviewVisible)
            {
                previewStartBlock = block;

                var points = connectionService.GetPreviewPoints(
                    previewStartBlock,
                    null,
                    mousePosition);

                PreviewStartPoint = points.Start;
                PreviewEndPoint = points.End;

                IsConnectionPreviewVisible = true;
            }
            else
            {
                previewStartBlock = null;
                IsConnectionPreviewVisible = false;
            }

            connectionService.HandleConnectionClick(
                block,
                mousePosition,
                CurrentDiagram,
                Connections);
        }

        private void OnBlockMouseDown(BlockMouseEventArgs args)
        {
            if (args == null)
                return;

            currentCanvasSize = args.CanvasSize;

            selectionService.EndEditingAll(Blocks);

            if (args.IsShiftPressed)
            {
                HandleConnectionClick(args.Block, args.MousePosition);
                return;
            }

            if (args.ClickCount == 2)
            {
                OnBeginEditBlock(args.Block);
                return;
            }

            OnSelectBlock(args.Block);

            dragService.StartDrag(args.Block, args.MousePosition);
        }

        private void OnBlockMouseMove(Point? point)
        {
            if (point == null)
                return;

            if(IsConnectionPreviewVisible)
            {
                previewEndPoint = point.Value;
            }
            dragService.DragTo(point.Value, currentCanvasSize);
        }

        private void OnBlockMouseUp()
        {
            dragService.EndDrag();
        }

        private void OnDeleteSelected()
        {
            if (SelectedConnection != null)
            {
                deleteService.DeleteConnection(
                    CurrentDiagram,
                    Connections,
                    SelectedConnection);

                SelectedConnection = null;
                return;
            }

            if (SelectedBlock != null)
            {
                deleteService.DeleteBlock(
                    CurrentDiagram,
                    Blocks,
                    Connections,
                    SelectedBlock);

                SelectedBlock = null;
            }
        }

        private void OnBlockResizeMouseDown(BlockMouseEventArgs args)
        {

            

            if (args == null)
                return;

            selectionService.SelectBlock(Blocks, args.Block, out var selected);
            SelectedBlock = selected;

            resizeService.StartResize(args.Block, args.MousePosition);
        }

        private void OnBlockResizeMouseMove(Point? point)
        {

            if (point == null)
                return;

            resizeService.ResizeTo(point.Value, currentCanvasSize);
        }

        private void OnBlockResizeMouseUp()
        {
            resizeService.EndResize();
        }
        public void ClearSelection()
        {
            selectionService.ClearSelection(Blocks);
            SelectedBlock = null;
        }

        private ConnectionSide GetPreviewSide(Point point, Point target)
        {
            var dx = target.X - point.X;
            var dy = target.Y - point.Y;

            if (Math.Abs(dx) > Math.Abs(dy))
            {
                return dx > 0
                    ? ConnectionSide.Right
                    : ConnectionSide.Left;
            }

            return dy > 0
                ? ConnectionSide.Bottom
                : ConnectionSide.Top;
        }

        public PointCollection PreviewRoutePoints
        {
            get
            {
                if (!IsConnectionPreviewVisible)
                    return new PointCollection();

                var startSide = GetPreviewSide(PreviewStartPoint, PreviewEndPoint);
                var endSide = GetPreviewSide(PreviewEndPoint, PreviewStartPoint);

                return new PointCollection(
                    routingService.BuildRoute(
                        PreviewStartPoint,
                        startSide,
                        PreviewEndPoint,
                        endSide));
            }
        }
        private void OnSelectConnection(ConnectionViewModel connection)
        {
            if (connection == null)
                return;

            if (SelectedBlock != null)
            {
                SelectedBlock.IsSelected = false;
                SelectedBlock = null;
            }

            if (SelectedConnection != null)
                SelectedConnection.IsSelected = false;

            SelectedConnection = connection;
            SelectedConnection.IsSelected = true;
        }

        private void OnConnectionEndMouseDown(ConnectionViewModel connection)
        {
            draggedConnection = connection;
            isDraggingConnectionEnd = true;

            OnSelectConnection(connection);
        }

        private void OnConnectionEndMouseMove(Point? point)
        {
            if (!isDraggingConnectionEnd || draggedConnection == null || point == null)
                return;

            PreviewStartPoint = draggedConnection.StartHandle;
            PreviewEndPoint = point.Value;
            IsConnectionPreviewVisible = true;
        }

        private void OnConnectionEndMouseUp(Point? point)
        {
            if (!isDraggingConnectionEnd || draggedConnection == null || point == null)
                return;

            var targetBlock = Blocks.FirstOrDefault(block =>
                point.Value.X >= block.X &&
                point.Value.X <= block.X + block.Width &&
                point.Value.Y >= block.Y &&
                point.Value.Y <= block.Y + block.Height);

            if (targetBlock != null && targetBlock != draggedConnection.From)
            {
                draggedConnection.ChangeTo(targetBlock);
            }

            draggedConnection = null;
            isDraggingConnectionEnd = false;
            IsConnectionPreviewVisible = false;
        }
    }
}
