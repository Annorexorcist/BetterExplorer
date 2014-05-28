﻿using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.WindowsAPICodePack.Shell;

namespace BetterExplorer {
	/// <summary>Must inherit Control, not Component, in order to have Handle</summary>
	[DefaultEvent("ClipboardChanged")]
	public partial class ClipboardMonitor : Control {

		#region Properties/Events/DllImport

		IntPtr nextClipboardViewer;

		/// <summary>Clipboard contents changed.</summary>
		public event EventHandler<Tuple<IDataObject>> ClipboardChanged;

		[DllImport("user32.dll")]
		protected static extern int SetClipboardViewer(int hWndNewViewer);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

		#endregion

		public ClipboardMonitor() {
			this.BackColor = Color.Red;
			this.Visible = false;

			nextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.Handle);
		}

		protected override void Dispose(bool disposing) {
			try {
				if (nextClipboardViewer != null)
					ChangeClipboardChain(this.Handle, nextClipboardViewer);
			}
			catch (Exception) {

			}
		}

		protected override void WndProc(ref Message m) {
			// defined in winuser.h
			const int WM_DRAWCLIPBOARD = 0x308;
			const int WM_CHANGECBCHAIN = 0x030D;

			switch (m.Msg) {
				case WM_DRAWCLIPBOARD:
					OnClipboardChanged();
					SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
					break;

				case WM_CHANGECBCHAIN:
					if (m.WParam == nextClipboardViewer)
						nextClipboardViewer = m.LParam;
					else
						SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
					break;

				default:
					base.WndProc(ref m);
					break;
			}
		}

		/// <summary>Call when content content of clipboard is changed</summary>
		void OnClipboardChanged() {
			try {
				IDataObject iData = Clipboard.GetDataObject();

				if (ClipboardChanged != null) {
					ClipboardChanged(this, new Tuple<IDataObject>(iData));
				}

			}
			//catch (Exception e)
			catch (Exception) {
				// Swallow or pop-up, not sure
				// Trace.Write(e.ToString());
				//MessageBox.Show(e.ToString());
			}
		}
	}

	/// <summary>Class for recording events of ClipboardChanges</summary>
	[Obsolete("Being changed into a Tuple<IDataObject>", true)]
	public class ClipboardChangedEventArgs : EventArgs {
		public readonly IDataObject DataObject;

		public ClipboardChangedEventArgs(IDataObject dataObject) {
			DataObject = dataObject;
		}
	}
}