using CommunityToolkit.Mvvm.ComponentModel;
using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;

namespace DragDropEmptyGroupTest
{
    public partial class Item : ObservableObject
    {
        [ObservableProperty]
        private string _name, _group;

        public Item( string name, string group ) => (Name, Group) = (name, group);
    }

    public class ViewModel : IDropTarget
    {
        private readonly ObservableCollection<Item> items = new( new Item[] {
            new( "Item 1", "Group 2" ),
            new( "Item 2", "Group 2" ),
            new( "Item 3", "Group 3" ),
        } );

        public ListCollectionView ItemsCollection { get; }

        public ViewModel()
        {
            ItemsCollection = new ListCollectionView( items );
            ItemsCollection.IsLiveGrouping = true;

            var groupDescription = new PropertyGroupDescription( nameof( Item.Group ) );
            groupDescription.GroupNames.Add( "Group 1" );

            ItemsCollection.GroupDescriptions.Add( groupDescription );
        }

        public void DragOver( IDropInfo dropInfo )
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            dropInfo.Effects = DragDropEffects.Move;
        }

        public void Drop( IDropInfo dropInfo )
        {
            if( dropInfo.Data is Item source && dropInfo.TargetGroup is not null )
            {
                if( source == dropInfo.TargetItem ) return;

                int sourceIdx = items.IndexOf( source ),
                    targetIdx = dropInfo.TargetItem is Item target ? items.IndexOf( target ) : 0;
                if( sourceIdx < targetIdx ) --targetIdx;
                if( dropInfo.InsertPosition.HasFlag( RelativeInsertPosition.AfterTargetItem ) ) ++targetIdx;

                source.Group = (dropInfo.TargetGroup.Name as string)!;
                
                if( sourceIdx != targetIdx )
                    items.Move( sourceIdx, targetIdx );
            }
        }
    }

    public partial class MainWindow : Window
    {
        public ViewModel Model { get; } = new();

        public MainWindow() => InitializeComponent();
    }
}
