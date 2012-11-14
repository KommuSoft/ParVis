//  
//  DisplayWindow.cs
//  
//  Author:
//       Willem Van Onsem <vanonsem.willem@gmail.com>
// 
//  Copyright (c) 2012 Willem Van Onsem
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Threading;
using Gtk;
using System.Xml.Schema;

namespace ParallelVisualizer {

	public class DisplayWindow : Window {
		
		private BlueprintParallelStatePainter bpsp;
		private BlueprintTabControl bs;
		private BlueprintMediabar bm;
		private SimulatorResult sr;
		private SimulationServer ss = new SimulationServer();
		private VBox vb = new VBox(false, 0);
		
		public DisplayWindow () : base(WindowType.Toplevel) {
			MenuBar mb = new MenuBar();
			MenuItem mi_file = new MenuItem("File");
			Menu m_file = new Menu();
			MenuItem mi_file_opendll = new MenuItem("Connect with algorithm library...");
			mi_file_opendll.Activated += openLibFile;
			MenuItem mi_file_openxml = new MenuItem("Open configuration file...");
			mi_file_openxml.Activated += openConfigFile;
			MenuItem mi_file_quit = new MenuItem("Quit");
			mi_file_quit.Activated += delegate {
				Application.Quit();
			};
			MenuItem mi_edit = new MenuItem("Edit");
			Menu m_edit = new Menu();
			RadioMenuItem mi_edit_move = new RadioMenuItem("Move nodes");
			RadioMenuItem mi_edit_insp = new RadioMenuItem(mi_edit_move, "Inspect node");
			MenuItem mi_edit_play = new ImageMenuItem(Stock.MediaPlay, null);
			MenuItem mi_edit_pause = new ImageMenuItem(Stock.MediaPause, null);
			mb.Add(mi_file);
			mi_file.Submenu = m_file;
			m_file.Add(mi_file_opendll);
			m_file.Add(mi_file_openxml);
			m_file.Add(new SeparatorMenuItem());
			m_file.Add(mi_file_quit);
			mi_edit.Submenu = m_edit;
			m_edit.Add(mi_edit_move);
			m_edit.Add(mi_edit_insp);
			m_edit.Add(new SeparatorMenuItem());
			m_edit.Add(mi_edit_play);
			m_edit.Add(mi_edit_pause);
			mb.Add(mi_edit);
			Toolbar tb = new Toolbar();
			StockItem si;
			StockManager.Lookup(Stock.MediaPlay, ref si);
			ToolButton tb_opendll = new ToolButton(Stock.Connect);
			tb_opendll.Clicked += openLibFile;
			ToolButton tb_openxml = new ToolButton(Stock.Open);
			tb_openxml.Clicked += openConfigFile;
			ToolButton tb_move = new ToggleToolButton(Stock.Preferences);
			ToolButton tb_insp = new ToggleToolButton(Stock.ZoomIn);
			ToolButton tb_play = new ToolButton(Stock.MediaPlay);
			ToolButton tb_pause = new ToolButton(Stock.MediaPause);
			tb.Add(tb_opendll);
			tb.Add(tb_openxml);
			tb.Add(new SeparatorToolItem());
			tb.Add(tb_move);
			tb.Add(tb_insp);
			tb.Add(new SeparatorToolItem());
			tb.Add(tb_play);
			tb.Add(tb_pause);
			this.bpsp = new BlueprintParallelStatePainter();
			this.bm = new BlueprintMediabar();
			this.bm.CurrentChanged += HandleBmhandleCurrentChanged;
			this.bm.CurrentChanged += this.bpsp.RepaintEdges;
			this.bs = new BlueprintTabControl(1, 1, 1);
			this.bs.CurrentChanged += HandleBshandleCurrentChanged;
			;
			vb.PackStart(mb, false, false, 0x00);
			vb.PackStart(tb, false, false, 0x00);
			vb.PackStart(this.bpsp, true, true, 0x00);
			vb.PackStart(this.bm, false, false, 0x00);
			vb.PackStart(this.bs, false, false, 0x00);
			this.Title = "Parallel Visualizer";
			this.Resize(640, 480);
			this.Add(vb);
			this.ShowAll();
		}

		void HandleBshandleCurrentChanged (object sender, EventArgs e) {
			if(this.sr != null) {
				int m, M;
				this.sr.GetBounds(this.bs.Current-1, out m, out M);
				this.bm.SetMinCurrentMax(m, m, M);
			}
		}

		void HandleBmhandleCurrentChanged (object sender, EventArgs e) {
			//double current = this.bm.Current;
			//this.ss.Simulator.ForwardTo((int)Math.Floor(current));
			//this.bpsp.
			this.bpsp.Time = this.bm.Current;
		}
		
		private void openConfigFile (object s, EventArgs e) {
			FileChooserDialog fcd = new FileChooserDialog("Open Config File...", this, FileChooserAction.Open);
			fcd.TransientFor = this;
			fcd.AddButton(Stock.Cancel, ResponseType.Cancel);
			fcd.AddButton(Stock.Ok, ResponseType.Ok);
			FileFilter ff = new FileFilter();
			ff.Name = "Config file (.xml)";
			ff.AddMimeType("text/xml");
			ff.AddMimeType("application/xml");
			fcd.AddFilter(ff);
			int result = fcd.Run();
			fcd.HideAll();
			if(result == (int)ResponseType.Ok) {
				try {
					ss.ReadConfigFile(fcd.Filename);
					this.sr = ss.Simulator.CollectResults();
					this.bs.SetMinCurrentMax(1, 1, this.sr.ChapterCount);
					this.bpsp.SimulatorResult = sr;
				}
				catch(Exception ex) {
					Console.Error.WriteLine(ex);
					MessageDialog md = new MessageDialog(this, DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Ok, true, ex.Message);
					md.Run();
					md.Destroy();
				}
				this.bpsp.Reload();
			}
			fcd.Dispose();
		}
		protected override void OnHidden () {
			base.OnHidden();
			this.Destroy();
		}
		public override void Destroy () {
			base.Destroy();
			Gtk.Main.Quit();
		}
		private void openLibFile (object s, EventArgs e) {
			FileChooserDialog fcd = new FileChooserDialog("Open Library File...", this, FileChooserAction.Open);
			fcd.TransientFor = this;
			fcd.AddButton(Stock.Cancel, ResponseType.Cancel);
			fcd.AddButton(Stock.Ok, ResponseType.Ok);
			FileFilter ff = new FileFilter();
			ff.Name = "Dynamic Link Library (.dll,.exe)";
			ff.AddPattern(@"*.dll");
			ff.AddPattern(@"*.exe");
			fcd.AddFilter(ff);
			int result = fcd.Run();
			fcd.HideAll();
			if(result == (int)ResponseType.Ok) {
				ss.ConnectAlgorithmLibrary(fcd.Filename);
			}
			fcd.Dispose();
		}
		
		public static void Main (string[] args) {
			Application.Init();
			Gdk.Threads.Init();
			Gdk.Threads.Enter();
			using(DisplayWindow dw = new DisplayWindow ()) {
				
				Application.Run();
			}
			Gdk.Threads.Leave();
		}
	}
}

/*
SampleParallelAlgorithm spa0 = new SampleParallelAlgorithm (null);
SampleParallelAlgorithm spa1 = new SampleParallelAlgorithm (spa0);
SampleParallelAlgorithm spa2 = new SampleParallelAlgorithm (spa1);
SampleParallelAlgorithm spa3 = new SampleParallelAlgorithm (spa2);
SampleParallelAlgorithm spa4 = new SampleParallelAlgorithm (spa3);
SampleParallelAlgorithm spa5 = new SampleParallelAlgorithm (spa1);
SampleParallelAlgorithm spa6 = new SampleParallelAlgorithm (spa5);
SampleParallelAlgorithm spa7 = new SampleParallelAlgorithm (spa6);
SampleParallelAlgorithm spa8 = new SampleParallelAlgorithm (spa7);
SampleParallelAlgorithm spa9 = new SampleParallelAlgorithm (spa8);
SampleParallelAlgorithm spa10 = new SampleParallelAlgorithm (spa6);
SampleParallelAlgorithm spa11 = new SampleParallelAlgorithm (spa10);
SampleParallelAlgorithm spa12 = new SampleParallelAlgorithm (spa11);
SampleParallelAlgorithm spa13 = new SampleParallelAlgorithm (spa12);
ParallelSimulation ps = new ParallelSimulation (spa0, spa1, spa2, spa3, spa4, spa5, spa6, spa7, spa8, spa9,
spa10, spa11, spa12, spa13);
ps.AddEdgeSequence (spa0, spa1, spa2, spa3, spa4);
ps.AddEdgeSequence (spa1, spa5, spa6, spa7, spa8, spa9);
ps.AddEdgeSequence (spa6, spa10, spa11, spa12, spa13); 
*/