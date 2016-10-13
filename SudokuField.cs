//written by André Betz 
//http://www.andrebetz.de

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Sudoku
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class SudokuField : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox[] m_Fields = new TextBox[81];
		private System.Windows.Forms.Button SolveBtn;
		private SudokuGame m_Game = new SudokuGame();
		private bool m_Changer = true;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SudokuField()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			int XSize = 32;
			int YSize = 32;

			for(int i=0;i<m_Fields.Length;i++)
			{
				int XPoint = 10+XSize*(i%9);
				int YPoint = 10+YSize*(i/9);
				m_Fields[i] = new TextBox();
				m_Fields[i].Location = new System.Drawing.Point(XPoint, YPoint);
				m_Fields[i].Size = new System.Drawing.Size(XSize, YSize);
				m_Fields[i].TabIndex = i;
				m_Fields[i].TextChanged += new System.EventHandler(this.Field_TextChanged);
				this.Controls.Add(m_Fields[i]);
			}
			this.ClientSize = new System.Drawing.Size(20+XSize*9,40+YSize*9);
			this.SolveBtn.Location = new System.Drawing.Point(10, 10+YSize*9);

			if(m_Game.Load())
			{
				ShowField();
			}
		}

		private void ShowField()
		{
			m_Changer = false;
			for(int i=0;i<m_Fields.Length;i++)
			{
				int Nr = m_Game.Get(i);
				if(Nr>0)
				{
					m_Fields[i].Text = Nr.ToString();
				}
				else
				{
					m_Fields[i].Text = "";
				}
			}
			m_Changer = true;
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SolveBtn = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// SolveBtn
			// 
			this.SolveBtn.Location = new System.Drawing.Point(8, 8);
			this.SolveBtn.Name = "SolveBtn";
			this.SolveBtn.TabIndex = 0;
			this.SolveBtn.Text = "Solve";
			this.SolveBtn.Click += new System.EventHandler(this.SolveBtn_Click);
			// 
			// SudokuField
			// 
			this.AutoScale = false;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(178, 55);
			this.Controls.Add(this.SolveBtn);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SudokuField";
			this.Text = "Sudoku AndreBetz.de";
			this.Load += new System.EventHandler(this.SudokuField_Load);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new SudokuField());
		}

		private void SudokuField_Load(object sender, System.EventArgs e)
		{
		
		}

		private void Field_TextChanged(object sender, System.EventArgs e)
		{
			if(m_Changer)
			{
				TextBox tb = (TextBox)sender;
				string cont = tb.Text;
				int Pos = tb.TabIndex;
				int Nr = 0;
				if(cont=="")
				{
					m_Game.Delete(Pos%9,Pos/9);
					ShowField();
				}
				else
				{
					try
					{
						Nr = Convert.ToInt32(cont);
						if(Nr<1||Nr>9)
						{
							tb.Text = "";
						}
						else
						{
							if(!m_Game.Set(Pos%9,Pos/9,Nr))
							{
								tb.Text = "";
							}
						}
					}
					catch
					{
						tb.Text = "";
					}
				}
			}
		}

		private void SolveBtn_Click(object sender, System.EventArgs e)
		{
			m_Game.Save();
			m_Game.Solve();
			ShowField();
		}
	}
}
