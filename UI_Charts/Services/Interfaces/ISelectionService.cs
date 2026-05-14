using System.Collections.ObjectModel;
using UICharts.Desktop.ViewModels;

namespace UICharts.Desktop.Services.Interfaces
{
    public interface ISelectionService
    {
        void BeginEdit(ObservableCollection<BlockViewModel> blocks, BlockViewModel block, out BlockViewModel selectedBlock);
        void ClearSelection(ObservableCollection<BlockViewModel> blocks);
        void EndEdit(BlockViewModel block);
        void EndEditingAll(ObservableCollection<BlockViewModel> blocks);
        bool IsEditingAny(ObservableCollection<BlockViewModel> blocks);
        void SelectBlock(ObservableCollection<BlockViewModel> blocks, BlockViewModel block, out BlockViewModel selectedBlock);
    }
}