// MyClass.cs created with MonoDevelop
// User: dario at 4:30 PM 3/28/2009
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Net;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace Badlydone.WRTranslator
{
	
	public enum enumLangFromTo
	{
		None,
		English_Italian,
		Italian_English,
		English_French,
		French_English,
		English_Spanish,
		Spanish_English,
		Espanol_Francais,
		Francais_Espanol,
		Espanol_Portugues,
		Portugues_Espanol
	};
	
	public class lib_wrtranslation
	{
		
		private bool m_InProgress = false;
		private enumLangFromTo m_TypeTranslation = enumLangFromTo.None;
		private string m_From = "";
		private string[] m_To;
		private string m_WebSite = "";
		private BackgroundWorker m_workback = null;
		
		public lib_wrtranslation()
		{
			// costructor
			m_WebSite = @"http://www.wordreference.com/";
			
			// asych work
			m_workback = new BackgroundWorker();
      m_workback.WorkerReportsProgress = true;
      m_workback.WorkerSupportsCancellation = true;
      m_workback.DoWork += new DoWorkEventHandler(m_workback_DoWork);
      m_workback.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_workback_RunWorkerCompleted);
			
		}
		
		private string getKeyTrans()
		{
			switch (m_TypeTranslation)
			{
			case enumLangFromTo.None:
				return "";
			case enumLangFromTo.English_Italian:
				return "enit";
			case enumLangFromTo.Italian_English:
				return "enit";
			case enumLangFromTo.English_French:
				return "enfr";
			case enumLangFromTo.French_English:
				return "fren";
			case enumLangFromTo.English_Spanish:
				return "enes";
			case enumLangFromTo.Spanish_English:
				return "esen";
			case enumLangFromTo.Espanol_Francais:
				return "esfr";
			case enumLangFromTo.Francais_Espanol:
				return "fres";
			case enumLangFromTo.Espanol_Portugues:
				return "espt";
			case enumLangFromTo.Portugues_Espanol:
				return "ptes";
			}
			return "";
		}
		
		public enumLangFromTo LanguageFromTo
		{
			get { return m_TypeTranslation; }
			set { m_TypeTranslation = value; }
		}

        public lib_return_translate ReturnedTranslate { get; set; }
		
		public string WordFrom
		{
			get { return m_From; }
			set { m_From = value; }
		}
		
		public string[] WordTo
		{
			get { return m_To; }
			set { m_To = value; }
		}
		
		public void Translate()
		{
			m_InProgress = true;
			
			DoTranslation();
		}
		
		public void WaitDone()
		{
#if !UNIX
			while (m_InProgress)
	            {
	                System.Windows.Forms.Application.DoEvents();
	            }
				
#else
	   		while (m_InProgress)
        	{
				while (GLib.MainContext.Iteration());
        	}  
#endif
		}
		
		public void TranslateAsynch()
		{
			if (m_workback.IsBusy) return;
			
			m_InProgress = true;
			
			m_workback.RunWorkerAsync();
			
		}
		
		void m_workback_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            m_InProgress = false;
        }

        void m_workback_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                DoTranslation();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                m_workback.CancelAsync();
                m_InProgress = false;
            }

        }
		
		private void DoTranslation()
		{

            ReturnedTranslate = new lib_return_translate();
			
			Console.WriteLine("Make HTTP request");
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(m_WebSite + this.getKeyTrans() + "/" + m_From);
			
			
			Console.WriteLine("Set the request");
			
			CookieContainer cc = new CookieContainer();
			
			req.Proxy = null;
			req.CookieContainer = cc;
			req.Method = "GET";
			req.Headers.Add("llang", this.getKeyTrans() + "i");
			req.Headers.Add("tranword", m_From);
			req.UserAgent = @"Mozilla/5.0 (X11; U; Linux i686; en-US; rv:1.9.0.7) Gecko/2009030422 Ubuntu/8.10 (intrepid) Firefox/3.0.7";
			req.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
				
			
			Console.WriteLine("Run HTTP request");
			HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
			
			
			Stream str =  resp.GetResponseStream();
			StreamReader strR = new StreamReader(str);
			string sPage = strR.ReadToEnd();


			resp.Close();
			
			string RegExp = @"<span class=trn>([a-z0-9]*)</span>";
			Regex Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
			MatchCollection matches = Engine.Matches(sPage);
			m_To = new string[matches.Count];
			int x = 0;
			foreach (Match words in matches)
			{
				m_To[x] = words.Groups[1].Value;
				
				Console.WriteLine(m_To[x]);
				
				x++;
			}

            ReturnedTranslate.ToAll = m_To;

            RegExp = @"<tr class='even'><td class='FrW2'>([a-z0-9\s]*)</td><td class='POS2'>([a-z]*)</td><td class='FrCN2'>([a-z0-9\s\(\)]*)</td><td class='ToW2'>([a-z0-9\s]*)<span class='POS2'>nf</span></td></tr><tr class='oddEx'><td colspan=6 class='FrEx2'>([A-Za-z0-9\s\.]*)</td></tr><tr class='evenEx'><td colspan=6 class='ToEx2'>([A-Za-z0-9\s\.]*)</td></tr>";
            Engine = new Regex(RegExp, RegexOptions.IgnoreCase);
            matches = Engine.Matches(sPage);

            foreach (Match words in matches)
            {

                ReturnedTranslate.From = words.Groups[1].Value;
                Console.WriteLine("From: " + words.Groups[1].Value);

                ReturnedTranslate.TypeWord = words.Groups[2].Value;
                Console.WriteLine("Type: " + words.Groups[2].Value);

                ReturnedTranslate.Description = words.Groups[3].Value;
                Console.WriteLine("Description: " + words.Groups[3].Value);

                ReturnedTranslate.To = words.Groups[4].Value;
                Console.WriteLine("To: " + words.Groups[4].Value);

                ReturnedTranslate.Phrase_1 = words.Groups[5].Value;
                Console.WriteLine("Phrase 1: " + words.Groups[5].Value);

                ReturnedTranslate.Phrase_2 = words.Groups[6].Value;
                Console.WriteLine("Phrase 2: " + words.Groups[6].Value);


            }

			Console.WriteLine("Done!");
			
			m_InProgress = false;
			
		}
		
	}

    public class lib_return_translate
    {

        public string From { get; set; }
        public string[] ToAll { get; set; }
        public string TypeWord { get; set; }
        public string Description { get; set; }
        public string To { get; set; }
        public string Phrase_1 { get; set; }
        public string Phrase_2 { get; set; }

    }

}
