using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMES_Ziincheol.Classes
{

    #region Item
    public class ProductPlan
    {
        public int _Col0;
        public int Col0
        {
            get { return _Col0; }
            set { _Col0 = value; }
        }
        public int _Col1;
        public int Col1
        {
            get { return _Col1; }
            set { _Col1 = value; }
        }

        public int _Col2;
        public int Col2
        {
            get { return _Col2; }
            set { _Col2 = value; }
        }

        public int _Col3;
        public int Col3
        {
            get { return _Col3; }
            set { _Col3 = value; }
        }

        public int _Col4;
        public int Col4
        {
            get { return _Col4; }
            set { _Col4 = value; }
        }

        public int _Col5;
        public int Col5
        {
            get { return _Col5; }
            set { _Col5 = value; }
        }

        public int _Col6;
        public int Col6
        {
            get { return _Col6; }
            set { _Col6 = value; }
        }


        public string _T1;
        public string T1
        {
            get { return _T1; }
            set { _T1 = value; }
        }

        public ProductPlan(string t1, int col0, int col1, int col2,
            int col3, int col4, int col5, int col6)
        {
            this.T1 = t1;
            this.Col0 = col0;
            this.Col1 = col1;
            this.Col2 = col2;
            this.Col3 = col3;
            this.Col4 = col4;
            this.Col5 = col5;
            this.Col6 = col6;            
        }
    }
    #endregion
}
