//written by André Betz 
//http://www.andrebetz.de

using System;
using System.Collections;
using System.Diagnostics;

namespace Sudoku
{
	/// <summary>
	/// Summary description for SudokuGame.
	/// </summary>
	public class SudokuGame
	{
		private int[] m_Field = new int[81];
		private ArrayList m_Mem = new ArrayList();
		private int m_Depth = 0;
		private static int ms_Cnt = 0;
		private int m_ClsNr = 0;

		public bool Set(int x, int y, int Nr)
		{
			int PosNr = Position(x,y);
			if(CheckField(x,y,Nr))
			{
				m_Field[PosNr] = Nr;
				return true;
			}
			return false;
		}

		public bool Set(int Pos, int Nr)
		{
			int x = -1;
			int y = -1;
			Position(Pos,ref x,ref y);
			return Set(x,y,Nr);
		}
		
		public int Get(int x, int y)
		{
			int PosNr = Position(x,y);
			if(PosNr<81)
			{
				return m_Field[PosNr];
			}
			return -1;
		}

		public int Get(int PosNr)
		{
			if(PosNr<81)
			{
				return m_Field[PosNr];
			}
			return -1;
		}

		private int Position(int x, int y)
		{
			return y*9+x;
		}

		private void Position(int Pos, ref int x, ref int y)
		{
			x = Pos%9;
			y = Pos/9;
		}

		private bool CheckField(int x, int y, int TestNr)
		{
			if(TestNr>0&&TestNr<10)
			{
				for(int i=0;i<9;i++)
				{
					int Nr = Get(i,y);
					if(Nr==TestNr)
					{
						return false;
					}
					Nr = Get(x,i);
					if(Nr==TestNr)
					{
						return false;
					}
				}
				for(int w=0;w<3;w++)
				{
					for(int z=0;z<3;z++)
					{
						int Nr = Get((x/3)*3+w,(y/3)*3+z);
						if(Nr==TestNr)
						{
							return false;
						}
					}
				}

				bool InsideNrs = false;
				int[] Nrs = GetPossibleNr(Position(x,y));
				for(int i=0;i<Nrs.Length;i++)
				{
					if(Nrs[i]==TestNr)
					{
						InsideNrs = true;
					}
				}
				if(!InsideNrs)
				{
					return false;
				}

				bool[] MemField = (bool[])m_Mem[TestNr-1];

				for(int i=0;i<9;i++)
				{
					int Pos = Position(i,y);
					MemField[Pos] = true;
					Pos = Position(x,i);
					MemField[Pos] = true;
				}
				for(int w=0;w<3;w++)
				{
					for(int z=0;z<3;z++)
					{
						int Pos = Position((x/3)*3+w,(y/3)*3+z);
						MemField[Pos] = true;
					}
				}
				for(int i=0;i<9;i++)
				{
					MemField = (bool[])m_Mem[i];
					int Pos = Position(x,y);
					MemField[Pos] = true;
				}
			}

			return true;
		}

		public void Delete(int x,int y)
		{
			int Nr = Get(x,y);
			if(Nr>0&&Nr<10)
			{
				int Pos = Position(x,y);
				m_Field[Pos] = 0;
			}

			int[] FieldTmp = new int[m_Field.Length];
			for(int i=0;i<m_Field.Length;i++)
			{
				FieldTmp[i] = m_Field[i];
			}
			for(int j=0;j<m_Mem.Count;j++)
			{
				bool[] MemField = (bool[])m_Mem[j];
				for(int i=0;i<MemField.Length;i++)
				{
					MemField[i] = false;
				}
			}
			m_Field = new int[FieldTmp.Length];
			for(int i=0;i<m_Field.Length;i++)
			{
				Set(i,FieldTmp[i]);
			}
		}

		private void Init()
		{
			for(int j=0;j<9;j++)
			{
				bool[] MemField = new bool[81];
				for(int i=0;i<MemField.Length;i++)
				{
					MemField[i] = false;
				}
				m_Mem.Add(MemField);
			}

			for(int i=0;i<m_Field.Length;i++)
			{
				m_Field[i] = 0;
			}
		}

		private bool Finished()
		{
			for(int i=0;i<m_Field.Length;i++)
			{
				if(m_Field[i]==0)
				{
					return false;
				}
			}
			return true;
		}

		private bool MovePossible()
		{
			for(int i=0;i<m_Field.Length;i++)
			{
				int[] Nrs = GetPossibleNr(i);
				if(m_Field[i]==0 && Nrs.Length>0)
				{
					return true;
				}
				else if(m_Field[i]==0 && Nrs.Length==0)
				{
					return false;
				}
				else if(m_Field[i]!=0 && Nrs.Length>0)
				{
					return false;
				}
			}
			return false;
		}

		public bool Solve()
		{
			while(SolveTree());
			return Finished();
		}

		private bool SolveTree()
		{
			while(SolveOne());
			
			if(MovePossible())
			{
				for(int fld=0;fld<m_Field.Length;fld++)
				{
					int[] posNrs = GetPossibleNr(fld);
					if(posNrs.Length>1)
					{
						for(int possib=0;possib<posNrs.Length;possib++)
						{
							Debug.WriteLine(ms_Cnt.ToString()+"\t"+m_Depth.ToString()+"\t"+Field2String());

							SudokuGame Try = new SudokuGame(this,m_Depth);
							int actTryNr = posNrs[possib];
							Try.Set(fld,actTryNr);
							if(Try.Solve())
							{
								Copy(Try);
								return true;
							}
						}
						return false;
					}
				}
			}
			return false;
		}

		private bool SolveOne()
		{
			bool bChanged = false;
			for(int i=0;i<m_Field.Length;i++)
			{
				int[] Nrs = GetPossibleNr(i);
				if(Nrs.Length==1)
				{
					Set(i,Nrs[0]);
					bChanged = true;
				}
			}
			return bChanged;
		}

		private int[] GetPossibleNr(int Pos)
		{
			ArrayList Nrs = new ArrayList();
			for(int i=0;i<m_Mem.Count;i++)
			{
				bool[] MemField = (bool[])m_Mem[i];
				if(MemField[Pos]==false)
				{
					Nrs.Add(i+1);
				}
			}
			return (int[])Nrs.ToArray(typeof(int));
		}

		public void Save()
		{
			WC.TextLoader txtld = new WC.TextLoader("sudoku_"+m_Depth.ToString()+".txt");
			txtld.Save(Field2String());
		}

		private string Field2String()
		{
			string Numbers = "";
			for(int i=0;i<m_Field.Length;i++)
			{
				Numbers += m_Field[i].ToString();
			}
			return Numbers;
		}

		public bool Load()
		{
			string Numbers = "";
			WC.TextLoader txtld = new WC.TextLoader("sudoku_"+m_Depth.ToString()+".txt");
			Numbers = txtld.Load();
			char[] arr = Numbers.ToCharArray();
			if(arr.Length>=m_Field.Length)
			{
				for(int i=0;i<m_Field.Length;i++)
				{
					try
					{
						string strNr = arr[i].ToString();
						int Nr = Convert.ToInt32(strNr);
						Set(i,Nr);
					}
					catch
					{
					}
				}
			}
			else
			{
				return false;
			}
			return true;
		}

		public SudokuGame()
		{
			Init();
		}

		private void Copy(SudokuGame sdkGm)
		{
			for(int y=0;y<9;y++)
			{
				for(int x=0;x<9;x++)
				{
					int Nr = sdkGm.Get(x,y);
					this.Set(x,y,Nr);
				}
			}
		}

		public SudokuGame(SudokuGame sdkGm,int Depth)
		{
			Init();
			Copy(sdkGm);
			m_Depth = Depth+1;
			ms_Cnt++;
			m_ClsNr = ms_Cnt;
		}
	}
}
