﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using BExplorer.Shell;
using BExplorer.Shell.Interop;
using Fluent;

namespace BetterExplorerControls {

	/// <summary>
	/// Interaction logic for BreadcrumbBarItem.xaml
	/// </summary>
	public partial class BreadcrumbBarItem : UserControl {
		private string path = "";
		private Fluent.ContextMenu DropDownMenu = new Fluent.ContextMenu();
		private bool children = true;
		private ShellItem so;

		public bool HasDropDownMenu {
			get {
				return children;
			}
		}

		public ShellItem ShellItem {
			get {
				return so;
			}
			set {
				LoadDirectory(value);
			}
		}

		public BreadcrumbBarItem() {
			InitializeComponent();
			DropDownMenu.Opened += new RoutedEventHandler(DropDownMenu_Opened);
			DropDownMenu.Closed += new RoutedEventHandler(DropDownMenu_Closed);
		}

		private void DropDownMenu_Closed(object sender, RoutedEventArgs e) {
			OnMenuClosed(EventArgs.Empty);
			expandArrow.Visibility = System.Windows.Visibility.Visible;
			ddArrow.Visibility = System.Windows.Visibility.Hidden;
			HideSelectedColors();
		}

		private void DropDownMenu_Opened(object sender, RoutedEventArgs e) {
			OnMenuOpened(EventArgs.Empty);
			expandArrow.Visibility = System.Windows.Visibility.Hidden;
			ddArrow.Visibility = System.Windows.Visibility.Visible;
			ShowSelectedColors();
		}

		public void LoadDirectory(ShellItem obj) {
			obj.Thumbnail.FormatOption = ShellThumbnailFormatOption.IconOnly;
			obj.Thumbnail.CurrentSize = new System.Windows.Size(16, 16);
			this.PathImage.Source = obj.Thumbnail.BitmapSource;
			this.pathName.Text = obj.GetDisplayName(SIGDN.NORMALDISPLAY);
			this.so = obj;
			path = obj.ParsingName;

			Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)(() => {
				if (obj.ParsingName == KnownFolders.Network.ParsingName || obj.ParsingName.StartsWith(@"\\")) {
					SetChildren(true);
					grid1.Visibility = System.Windows.Visibility.Visible;
					MenuBorder.Visibility = System.Windows.Visibility.Visible;
				}
				else {
					try {
						ShellItem con = obj;
						List<ShellItem> joe = new List<ShellItem>();
						foreach (ShellItem item in con) {
							if (item.IsFolder == true) {
								if (item.ParsingName.ToLower().EndsWith(".zip") == false && item.ParsingName.ToLower().EndsWith(".cab") == false) {
									joe.Add(item);
								}
							}
						}
						SetChildren(joe.Count > 0);
					}
					catch {
						SetChildren(false);
					}
				}
			}));
		}

		public void SetChildren(bool isON) {
			expandArrow.Visibility = isON ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
			grid1.Visibility = isON ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
			children = isON;
		}

		public delegate void PathEventHandler(object sender, PathEventArgs e);

		// An event that clients can use to be notified whenever the
		// elements of the list change:
		public event PathEventHandler NavigateRequested;

		//public event EventHandler MouseDoubleClick;

		// Invoke the Changed event; called whenever list changes:
		protected virtual void OnNavigateRequested(PathEventArgs e) {
			if (NavigateRequested != null)
				NavigateRequested(this, e);
		}

		// An event that clients can use to be notified whenever the
		// elements of the list change:
		public event PathEventHandler ContextMenuRequested;

		//public event EventHandler MouseDoubleClick;

		// Invoke the Changed event; called whenever list changes:
		protected virtual void OnContextMenuRequested(PathEventArgs e) {
			if (ContextMenuRequested != null)
				ContextMenuRequested(this, e);
		}

		// An event that clients can use to be notified whenever the
		// elements of the list change:
		public event MouseButtonEventHandler ButtonClick;

		//public event EventHandler MouseDoubleClick;

		// Invoke the Changed event; called whenever list changes:
		protected virtual void OnButtonClick(MouseButtonEventArgs e) {
			if (ButtonClick != null)
				ButtonClick(this, e);
		}

		// An event that clients can use to be notified whenever the
		// elements of the list change:
		public event EventHandler MenuOpened;

		// Invoke the Changed event; called whenever list changes:
		protected virtual void OnMenuOpened(EventArgs e) {
			if (MenuOpened != null)
				MenuOpened(this, e);
		}

		// An event that clients can use to be notified whenever the
		// elements of the list change:
		public event EventHandler MenuClosed;

		// Invoke the Changed event; called whenever list changes:
		protected virtual void OnMenuClosed(EventArgs e) {
			if (MenuClosed != null)
				MenuClosed(this, e);
		}

		private string IncludeTrailingBackslash(string Path) {
			string CharToInsert = "";
			if (Path[Path.Length - 1] != Char.Parse(@"\")) {
				CharToInsert = @"\";
			}
			return Path + CharToInsert;
		}

		private void MenuItemClicked(object sender, RoutedEventArgs e) {
			Fluent.MenuItem pan = (Fluent.MenuItem)sender;
			OnNavigateRequested(new PathEventArgs(pan.Tag as ShellItem));
		}

		private void ShowSelectedColors() {
			this.SelectionBackground.Visibility = System.Windows.Visibility.Visible;
			this.PathBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 60, 127, 177));

			if (HasDropDownMenu == true) {
				this.MenuBackground.Visibility = System.Windows.Visibility.Visible;
				this.MenuBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 60, 127, 177));
			}
		}

		private void HideSelectedColors() {
			this.SelectionBackground.Visibility = System.Windows.Visibility.Hidden;
			this.PathBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(1, 60, 127, 177));

			if (HasDropDownMenu == true) {
				this.MenuBackground.Visibility = System.Windows.Visibility.Hidden;
				this.MenuBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(1, 60, 127, 177));
			}
		}

		private void UserControl_MouseEnter(object sender, MouseEventArgs e) {
			ShowSelectedColors();
		}

		private void UserControl_MouseLeave(object sender, MouseEventArgs e) {
			HideSelectedColors();
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			e.Handled = true;
			ShellItem con = this.ShellItem;
			List<ShellItem> joe = new List<ShellItem>();
			foreach (ShellItem item in con) {
				if (item.IsFolder) {
					if (!item.ParsingName.EndsWith(".zip")) {
						joe.Add(item);
					}
				}
			}

			joe.Sort(delegate(ShellItem j1, ShellItem j2) { return j1.GetDisplayName(SIGDN.NORMALDISPLAY).CompareTo(j2.GetDisplayName(SIGDN.NORMALDISPLAY)); });
			DropDownMenu.Items.Clear();
			foreach (ShellItem thing in joe) {
				Fluent.MenuItem pan = new Fluent.MenuItem();
				thing.Thumbnail.FormatOption = ShellThumbnailFormatOption.IconOnly;
				thing.Thumbnail.CurrentSize = new System.Windows.Size(16, 16);
				pan.Icon = thing.Thumbnail.BitmapSource;
				var header = thing.GetDisplayName(SIGDN.NORMALDISPLAY);
				var posunderscore = header.IndexOf("_");
				if (posunderscore != -1)
					header = header.Insert(posunderscore, "_");
				pan.Header = header;
				pan.Height = 23;
				pan.Tag = thing;

				pan.Click += new RoutedEventHandler(MenuItemClicked);
				this.DropDownMenu.Items.Add(pan);
			}
			if (DropDownMenu.Items.Count > 0) {
				DropDownMenu.Placement = PlacementMode.Bottom;
				DropDownMenu.PlacementTarget = Base;
				DropDownMenu.IsOpen = true;
			}
		}

		private void Grid_MouseUp(object sender, MouseButtonEventArgs e) {
			//e.Handled = true;
			OnButtonClick(e);
			if (e.ChangedButton == MouseButton.Left) {
				OnNavigateRequested(new PathEventArgs(this.ShellItem));
			}
			else if (e.ChangedButton == MouseButton.Right) {
				OnContextMenuRequested(new PathEventArgs(this.ShellItem));
			}
		}

		private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
			//e.Handled = true;
		}

		private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
			//e.Handled = true;
			//if (e.ClickCount == 2)
			//{
			//    if (MouseDoubleClick != null)
			//    {
			//        MouseDoubleClick(this, EventArgs.Empty);
			//    }

			//}
		}

		private void btnDropDown_MouseDown(object sender, MouseButtonEventArgs e) {
			e.Handled = true;
		}

		private void btnDropDown_MouseUp(object sender, MouseButtonEventArgs e) {
			e.Handled = true;
		}
	}
}

public class PathEventArgs {
	public string Path { get; private set; }
	public ShellItem ShellItem { get; private set; }

	public PathEventArgs(ShellItem obj = null) {
		ShellItem = obj;
		Path = obj.ParsingName;
	}
}