using System.Collections.ObjectModel;
using UICharts.Desktop.Services.Interfaces;
using UICharts.Desktop.ViewModels;

namespace UICharts.Desktop.Services
{
    public class SelectionService : ISelectionService
    {
        public void ClearSelection(ObservableCollection<BlockViewModel> blocks)
        {
            foreach (var block in blocks)
            {
                block.IsSelected = false;
            }
        }

        public void SelectBlock(
            ObservableCollection<BlockViewModel> blocks,
            BlockViewModel block,
            out BlockViewModel selectedBlock)
        {
            ClearSelection(blocks);

            block.IsSelected = true;
            selectedBlock = block;
        }

        public void BeginEdit(
            ObservableCollection<BlockViewModel> blocks,
            BlockViewModel block,
            out BlockViewModel selectedBlock)
        {
            ClearSelection(blocks);

            block.IsSelected = true;
            block.IsEditing = true;

            selectedBlock = block;
        }

        public void EndEdit(BlockViewModel block)
        {
            block.IsEditing = false;
        }

        public void EndEditingAll(ObservableCollection<BlockViewModel> blocks)
        {
            foreach (var block in blocks)
            {
                block.IsEditing = false;
            }
        }

        public bool IsEditingAny(ObservableCollection<BlockViewModel> blocks)
        {
            return blocks.Any(b => b.IsEditing);
        }
    }
}