using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UICharts.Core.Enums;
using UICharts.Core.Models;

namespace UICharts.Desktop.ViewModels
{
    public class EditorViewModel : BindableBase
    {
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

        public ToolboxViewModel? Toolbox { get; set; } // Убрать ! Здесь должна быть только фигура а не тулбокс

        public DelegateCommand<System.Windows.Point?> CanvasClickCommand { get; }
        public DelegateCommand<BlockViewModel> SelectBlockCommand { get; }
        public DelegateCommand<BlockViewModel> BeginEditBlockCommand { get; }
        public DelegateCommand<BlockViewModel> EndEditBlockCommand { get; }

        public EditorViewModel()
        {
            CanvasClickCommand = new DelegateCommand<System.Windows.Point?>(OnCanvasClick);
            SelectBlockCommand = new DelegateCommand<BlockViewModel>(OnSelectBlock);
            BeginEditBlockCommand = new DelegateCommand<BlockViewModel>(OnBeginEditBlock);
            EndEditBlockCommand = new DelegateCommand<BlockViewModel>(OnEndEditBlock);
        }

        private void LoadBlocksFromDiagram()
        {
            Blocks.Clear();

            if (CurrentDiagram == null)
                return;

            foreach (var block in CurrentDiagram.Blocks)
            {
                Blocks.Add(new BlockViewModel(block));
            }
        }

        private void OnCanvasClick(System.Windows.Point? point)
        {
            if (point == null || CurrentDiagram == null || Toolbox?.SelectedFigure == null)
                return;

            if (IsEditingAnyBlock())
                return;

            var model = new BlockModel
            {
                X = point.Value.X,
                Y = point.Value.Y,
                Text = Toolbox.SelectedFigure.Name,
                Type = Toolbox.SelectedFigure.Type
            };

            CurrentDiagram.Blocks.Add(model);

            var vm = new BlockViewModel(model);
            Blocks.Add(vm);

            ClearSelection();
            vm.IsSelected = true;
            SelectedBlock = vm;
            this.Toolbox.SelectedFigure = null;
        }

        private void OnSelectBlock(BlockViewModel? block)
        {
            if (block == null)
                return;
            ClearSelection();
            block.IsSelected = true;
            SelectedBlock = block;
        }

        private void OnBeginEditBlock(BlockViewModel? block)
        {
            if(block == null)
                return;

            ClearSelection();
            block.IsSelected = true;
            block.IsEditing = true;
            SelectedBlock = block;
        }

        private void OnEndEditBlock(BlockViewModel? block)
        {
            if (block == null)
                return;
            block.IsEditing = false;
        }

        private bool IsEditingAnyBlock()
        {
            return Blocks.Any(b => b.IsEditing);
        }

        private void ClearSelection()
        {
            foreach (var block in Blocks)
            {
                block.IsSelected = false;
            }

            SelectedBlock = null;
        }
    }
}
