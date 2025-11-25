using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;
using System.Windows;

namespace Tests.Wpf.DragDrop;

public class DragDropHandler(Collection<DragDropItem> items) : DefaultDropHandler
{
    public override void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo.DragInfo.SourceItem is DragDropItem model && Exists(model))
        {
            dropInfo.DropTargetHintAdorner = DropTargetAdorners.Hint;
            dropInfo.DropTargetHintState = DropHintState.Error;
            dropInfo.DropHintText = "Item already exists!";
            dropInfo.Effects = DragDropEffects.None;

            return;
        }

        var copyData = ShouldCopyData(dropInfo);

        dropInfo.Effects = copyData ? DragDropEffects.Copy : DragDropEffects.Move;
        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
        dropInfo.EffectText = "EffectText: Move";

        dropInfo.DropTargetHintAdorner = DropTargetAdorners.Hint;
        dropInfo.DropHintText = $"DropHintText: Move {((DragDropItem)(dropInfo.DragInfo.SourceItem)).Address}";
        dropInfo.DropTargetHintState = DropHintState.Active;
    }

    private bool Exists(DragDropItem vm)
    {
        var exists = items.Any(m =>
            m.Id == vm.Id
        );

        return exists;
    }
}
