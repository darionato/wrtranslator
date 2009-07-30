// MainWindow.cs created with MonoDevelop
// User: dario at 4:51 PM 3/28/2009
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//
using System;
using Gtk;
using Badlydone.WRTranslator;

public partial class MainWindow: Gtk.Window
{	
	private lib_wrtranslation m_WRTrans = null;
	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		
		this.txtFrom.PasteClipboard();
		this.txtFrom.IsFocus = true;
		
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected virtual void OnBtnCloseClicked (object sender, System.EventArgs e)
	{
		Application.Quit();
	}
	/*
	private enumLangFromTo getLangFromTo()
	{
		
		return enumLangFromTo.English_Italian;
		
	}
	 */
	protected virtual void OnButton1Clicked (object sender, System.EventArgs e)
	{
		DoTranslate();
	}
	
	private void DoTranslate()
	{
		
		if (m_WRTrans == null) m_WRTrans = new lib_wrtranslation();
		
		m_WRTrans.LanguageFromTo = enumLangFromTo.English_Italian;
		
		m_WRTrans.WordFrom = this.txtFrom.Text;
		
		m_WRTrans.TranslateAsynch();
		
		m_WRTrans.WaitDone();
				
		string[] result = m_WRTrans.WordTo;
		
		this.txtTo.Text = "";
		
		if (result == null) return;
		
		if (result.Length != 0)
		{
			for (int x = 0; x < result.Length; x++)
			{
				this.txtTo.Text += result[x] + " - ";
			}
			
			this.txtTo.Text = this.txtTo.Text.Substring(0,this.txtTo.Text.Length-3);
		
		}
	}

	protected virtual void OnTxtFromKeyPressEvent (object o, Gtk.KeyPressEventArgs args)
	{
		Console.WriteLine("in");
	}

	protected virtual void OnTxtFromKeyReleaseEvent (object o, Gtk.KeyReleaseEventArgs args)
	{
		Gdk.EventKey t = (Gdk.EventKey)args.Args.GetValue(0);
		
		int code = t.Key.value__;
		
		if (code == 65293 || code == 65421)
		{
			DoTranslate();
		}
		
	}

}