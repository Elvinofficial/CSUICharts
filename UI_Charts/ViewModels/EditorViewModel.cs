using System.Collections.ObjectModel;
using System.Windows;
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
        public DelegateCommand<System.Windows.Point?> CanvasClickCommand { get; }
        public DelegateCommand<BlockViewModel> SelectBlockCommand { get; }
        public DelegateCommand<BlockViewModel> BeginEditBlockCommand { get; }
        public DelegateCommand<BlockViewModel> EndEditBlockCommand { get; }
        public DelegateCommand<BlockMouseEventArgs> BlockMouseDownCommand { get; }
        public DelegateCommand<Point?> BlockMouseMoveCommand { get; }
        public DelegateCommand BlockMouseUpCommand { get; }
        public DelegateCommand ToggleConnectionModeCommand { get; }

        public DelegateCommand DeleteSelectedCommand { get; }
        public EditorViewModel(
            IBlockDragService dragService,
            IConnectionService connectionService,
            IDiagramMappingService mappingService,
            ISelectionService selectionService,
            IDeleteService deleteService
            )
        {
            this.dragService = dragService;
            this.connectionService = connectionService;
            this.mappingService = mappingService;
            this.selectionService = selectionService;
            this.deleteService = deleteService;

            CanvasClickCommand = new DelegateCommand<System.Windows.Point?>(OnCanvasClick);
            SelectBlockCommand = new DelegateCommand<BlockViewModel>(OnSelectBlock);
            BeginEditBlockCommand = new DelegateCommand<BlockViewModel>(OnBeginEditBlock);
            EndEditBlockCommand = new DelegateCommand<BlockViewModel>(OnEndEditBlock);
            DeleteSelectedCommand = new DelegateCommand(OnDeleteSelected);

            ToggleConnectionModeCommand = new DelegateCommand(() =>
            {
                IsConnectionMode = !IsConnectionMode;
                connectionService.Reset();
            });

            BlockMouseDownCommand = new DelegateCommand<BlockMouseEventArgs>(OnBlockMouseDown);
            BlockMouseMoveCommand = new DelegateCommand<Point?>(OnBlockMouseMove);
            BlockMouseUpCommand = new DelegateCommand(OnBlockMouseUp);
        }

        private void LoadBlocksFromDiagram()
        {
            connectionService.Reset();
            SelectedBlock = null;

            mappingService.Map(CurrentDiagram, Blocks, Connections);
        }

        private void OnCanvasClick(System.Windows.Point? point)
        {
            if (point == null || CurrentDiagram == null || SelectedFigure == null)
                return;

            if (selectionService.IsEditingAny(Blocks))
                return;

            var model = new BlockModel
            {
                X = point.Value.X,
                Y = point.Value.Y,
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
        }

        private void OnSelectBlock(BlockViewModel? block)
        {
            if (block == null)
                return;
       

            if (IsConnectionMode)
            {
                HandleConnectionClick(block);
                return;
            }
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

        public void HandleConnectionClick(BlockViewModel block)
        {
            connectionService.HandleConnectionClick(block, CurrentDiagram,Connections);
        }

        private void OnBlockMouseDown(BlockMouseEventArgs args)
        {
            if (args == null)
                return;

            selectionService.EndEditingAll(Blocks);

            if (args.IsShiftPressed)
            {
                HandleConnectionClick(args.Block);
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

            dragService.DragTo(point.Value);
        }

        private void OnBlockMouseUp()
        {
            dragService.EndDrag();
        }

        private void OnDeleteSelected()
        {

            if (SelectedBlock == null)
                return;

            deleteService.DeleteBlock(CurrentDiagram, Blocks, Connections, SelectedBlock);

            SelectedBlock = null;
        }
    }
}
