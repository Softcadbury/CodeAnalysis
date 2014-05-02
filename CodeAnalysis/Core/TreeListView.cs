namespace CodeAnalysis.Core
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// Represents a control that displays hierarchical data in a tree structure that has items that can expand and collapse.
    /// Downloaded from : http://www.codeproject.com/Articles/24973/TreeListView
    /// </summary>
    public class TreeListView : TreeView
    {
        /// <summary>
        /// Override the default style and the default control template
        /// </summary>
        static TreeListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListView), new FrameworkPropertyMetadata(typeof(TreeListView)));
        }

        /// <summary>
        /// Initialize a new instance of TreeListView.
        /// </summary>
        public TreeListView()
        {
            Columns = new GridViewColumnCollection();
        }

        /// <summary>
        /// Gets or sets the collection of System.Windows.Controls.GridViewColumn
        /// objects that is defined for this TreeListView.
        /// </summary>
        public GridViewColumnCollection Columns
        {
            get { return (GridViewColumnCollection)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether columns in a TreeListView can be
        /// reordered by a drag-and-drop operation. This is a dependency property.
        /// </summary>
        public bool AllowsColumnReorder
        {
            get { return (bool)GetValue(AllowsColumnReorderProperty); }
            set { SetValue(AllowsColumnReorderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowsColumnReorder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowsColumnReorderProperty =
            DependencyProperty.Register("AllowsColumnReorder", typeof(bool), typeof(TreeListView), new UIPropertyMetadata(null));

        // Using a DependencyProperty as the backing store for Columns.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(GridViewColumnCollection),
            typeof(TreeListView),
            new UIPropertyMetadata(null));
    }

    /// <summary>
    /// Represents a control that can switch states in order to expand a node of a TreeListView.
    /// </summary>
    public class TreeListViewExpander : ToggleButton { }

    /// <summary>
    /// Represents a control that can generate spaces depending of the node of a TreeListView.
    /// </summary>
    public class TreeListViewSpace : ToggleButton { }

    /// <summary>
    /// Represents a convert that can calculate the indentation of any element in a class derived from TreeView.
    /// </summary>
    public class TreeListViewConverter : IValueConverter
    {
        public const double Indentation = 10;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //If the value is null, don't return anything
            if (value == null) return null;
            {
                //Convert the item to a double
                if (targetType == typeof(double) && value is DependencyObject)
                {
                    //Cast the item as a DependencyObject
                    var element = value as DependencyObject;

                    //Create a level counter with value set to -1
                    int level = -1;

                    //Move up the visual tree and count the number of TreeViewItem's.
                    for (; element != null; element = VisualTreeHelper.GetParent(element)) //Check whether the current elemeent is a TreeViewItem
                        if (element is TreeViewItem) //Increase the level counter
                            level++;

                    //Return the indentation as a double
                    return Indentation * level;
                }
            }

            //Type conversion is not supported
            throw new NotSupportedException(
                string.Format("Cannot convert from <{0}> to <{1}> using <TreeListViewConverter>.",
                value.GetType(), targetType));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("This method is not supported.");
        }
    }
}