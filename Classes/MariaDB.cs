using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;//마리아DB
using System.Collections;
using System.Data;
using System.Configuration;
using System.Web;
using System.Data.SqlClient;// SQL 서버 사용하려면 필요.

namespace Detector
{
    public class MariaDB
    {
        #region 멤버변수
        public MySqlConnection connection;
        public SqlConnection conn;
        //private MySqlDataAdapter da;
        #endregion
        
        #region 생성자
        public MariaDB()
        {
        }
        #endregion
        
        #region Open()
        public bool Open()
        {
            bool bReturn = false;

            connection = new MySqlConnection(Global.DBConString);

            try
            {
                connection.Open();
                bReturn = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return bReturn;
        }
        #endregion

        #region Close()
        public void Close()
        {
            connection.Close();
        }
        #endregion

        #region Transaction
        public void RunTransaction(string myConnString)
        {
            MySqlConnection myConnection = new MySqlConnection(myConnString);
            myConnection.Open();

            MySqlCommand myCommand = myConnection.CreateCommand();
            MySqlTransaction myTrans;

            // Start a local transaction
            myTrans = myConnection.BeginTransaction();
            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            myCommand.Connection = myConnection;
            myCommand.Transaction = myTrans;

            try
            {
                myCommand.CommandText = "insert into Test (id, desc) VALUES (100, 'Description')";
                myCommand.ExecuteNonQuery();
                myCommand.CommandText = "insert into Test (id, desc) VALUES (101, 'Description')";
                myCommand.ExecuteNonQuery();
                myTrans.Commit();
                Console.WriteLine("Both records are written to database.");
            }
            catch (Exception e)
            {
                try
                {
                    myTrans.Rollback();
                }
                catch (SqlException ex)
                {
                    if (myTrans.Connection != null)
                    {
                        Console.WriteLine("An exception of type " + ex.GetType() +
                        " was encountered while attempting to roll back the transaction.");
                    }
                }

                Console.WriteLine("An exception of type " + e.GetType() +
                " was encountered while inserting the data.");
                Console.WriteLine("Neither record was written to database.");
            }
            finally
            {
                myConnection.Close();
            }
        }
        #endregion

        #region ExecQuery(string sql)
        public DataTable ExecQuery(string sql)
        {
            //
            // Query방법1
            // Query할 sql만 인자로 받아서 DB검색 후,
            // DataTable을 리턴하는 굉장히 간단한 방법 
            //
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            MySqlDataAdapter da;
            DataSet DS = new DataSet();

            try
            {
                conn.Open();

                da = new MySqlDataAdapter(sql, conn);
                da.Fill(DS, "Result");
            }
            catch (MySqlException ex)
            {
                // return the mysql error message
                // the caller can put it in a messagebox
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return ex.Message;
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return DS.Tables["Result"];
        }
        #endregion

        #region 공정이상 정보 읽어오기
        public ArrayList ChkShtErr_Select(string sAbnormalShtNo)
        {
            //
            // chkshterr 테이블 읽어오기
            //
            MySqlDataReader Reader;
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * from rmis.chkshterr WHERE 공정이상번호 = '" +
                                  sAbnormalShtNo + "'";

            ArrayList arlstData = new ArrayList();

            try
            {
                Reader = command.ExecuteReader();
                while (Reader.Read())
                {
                    arlstData.Add(Reader["품목코드"].ToString());   //[0]
                    arlstData.Add(Reader["부품명"].ToString());     //[1]
                    arlstData.Add(Reader["수량"].ToString());       //[2]
                    arlstData.Add(Reader["시리얼번호"].ToString());  //[3]
                    arlstData.Add(Reader["부적합사항"].ToString());  //[4]
                    arlstData.Add(Reader["조치요구사항"].ToString());//[5]
                    arlstData.Add(Reader["공정이상번호"].ToString());//[6]
                    arlstData.Add(Reader["제품코드"].ToString());    //[7]
                    arlstData.Add(Reader["수주번호"].ToString());    //[8]
                    arlstData.Add(Reader["발생일자시각"].ToString());//[9]
                    arlstData.Add(Reader["모델명"].ToString());//[10]
                    arlstData.Add(Reader["거래처"].ToString());//[11]
                    arlstData.Add(Reader["판매처"].ToString());//[12]
                    break;
                }
            }
            catch
            {
                MessageBox.Show("DB ChkshtErr_Select Error", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return arlstData;
        }
        #endregion

        #region ExecuteNonQuery
        int nReturn = 0;
        public int ExecuteNonQuery(string sql)
        {
            //
            // 미완성 함수
            //

            //bool bReturn = true;
            nReturn = 0;

            //if (Open())
            //{
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = sql;
            try
            {
                nReturn = command.ExecuteNonQuery();
            }
            catch
            {
                MessageBox.Show("DB Error", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //bReturn = false;
            }
            //}

            //Close();

            return nReturn;
        }
        #endregion

        #region ExecNonQurey_SP...Parameter 1
        public int ExecNonQurey_SP(string sp_name, ref string msg,
            string p_name1, object p_val1)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));                

                int nRowCnt = command.ExecuteNonQuery();

                msg = "OK";
                return nRowCnt;
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return -1;
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region ExecNonQurey_SP...Parameter 2
        public int ExecNonQurey_SP(string sp_name, ref string msg,
            string p_name1, object p_val1, string p_name2, object p_val2)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                
                int nRowCnt = command.ExecuteNonQuery();

                msg = "OK";
                return nRowCnt;
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return -1;
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region ExecNonQurey_SP...Parameter 3
        public int ExecNonQurey_SP(string sp_name, ref string msg,
            string p_name1, object p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));                
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));

                int nRowCnt = command.ExecuteNonQuery();

                msg = "OK";
                return nRowCnt;
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return -1;
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region ExecNonQurey_SP...Parameter 4
        public int ExecNonQurey_SP(string sp_name, ref string msg,
            string p_name1, object p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));

                int nRowCnt = command.ExecuteNonQuery();

                msg = "OK";
                return nRowCnt;
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return -1;
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region ExecNonQurey_SP...Parameter 5
        public int ExecNonQurey_SP(string sp_name, ref string msg,
            string p_name1, object p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4,
            string p_name5, object p_val5)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                
                int nRowCnt = command.ExecuteNonQuery();

                msg = "OK";
                return nRowCnt;
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return -1;
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region ExecNonQurey_SP...Parameter 6
        public int ExecNonQurey_SP(string sp_name, ref string msg,
            string p_name1, object p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4,
            string p_name5, object p_val5, string p_name6, object p_val6)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));                

                int nRowCnt = command.ExecuteNonQuery();

                msg = "OK";
                return nRowCnt;
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return -1;
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region ExecNonQurey_SP...Parameter 7
        public int ExecNonQurey_SP(string sp_name, ref string msg,
            string p_name1, object p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4,
            string p_name5, object p_val5, string p_name6, object p_val6,
            string p_name7, object p_val7)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));
                command.Parameters.Add(new MySqlParameter(p_name7, p_val7));

                int nRowCnt = command.ExecuteNonQuery();

                msg = "OK";
                return nRowCnt;
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return -1;
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region ExecNonQurey_SP...Parameter 8
        public int ExecNonQurey_SP(string sp_name, ref string msg,
            string p_name1, object p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4,
            string p_name5, object p_val5, string p_name6, object p_val6,
            string p_name7, object p_val7, string p_name8, object p_val8)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));
                command.Parameters.Add(new MySqlParameter(p_name7, p_val7));
                command.Parameters.Add(new MySqlParameter(p_name8, p_val8));

                int nRowCnt = command.ExecuteNonQuery();

                msg = "OK";
                return nRowCnt;
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return -1;
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region ExecNonQurey_SP...Parameter 9
        public int ExecNonQurey_SP(string sp_name, ref string msg,
             string p_name1, ref object p_val1, string p_name2, object p_val2,
             string p_name3, object p_val3, string p_name4, object p_val4,
             string p_name5, object p_val5, string p_name6, object p_val6,
             string p_name7, object p_val7, string p_name8, object p_val8,
             string p_name9, object p_val9)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters[p_name1].Direction = ParameterDirection.InputOutput;

                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));
                command.Parameters.Add(new MySqlParameter(p_name7, p_val7));
                command.Parameters.Add(new MySqlParameter(p_name8, p_val8));
                command.Parameters.Add(new MySqlParameter(p_name9, p_val9));

                int nRowCnt = command.ExecuteNonQuery();

                // Get values from the output params
                p_val1 = command.Parameters[p_name1].Value;
                //int PG = Convert.ToInt32(command.Parameters[p_name1].Value);
                
                msg = "OK";
                return nRowCnt;
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return -1;
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region ExecNonQurey_SP...Parameter 10
        public int ExecNonQurey_SP(string sp_name, ref string msg,
            string p_name1, object p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4,
            string p_name5, object p_val5, string p_name6, object p_val6,
            string p_name7, object p_val7, string p_name8, object p_val8,
            string p_name9, object p_val9, string p_name10, object p_val10)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);            

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));
                command.Parameters.Add(new MySqlParameter(p_name7, p_val7));
                command.Parameters.Add(new MySqlParameter(p_name8, p_val8));
                command.Parameters.Add(new MySqlParameter(p_name9, p_val9));
                command.Parameters.Add(new MySqlParameter(p_name10, p_val10));

                int nRowCnt = command.ExecuteNonQuery();

                msg = "OK";
                return nRowCnt;
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return -1;
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region ExecNonQurey_SP...Parameter 11
        public int ExecNonQurey_SP(string sp_name,
            string p_name1, object p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4,
            string p_name5, object p_val5, string p_name6, object p_val6,
            string p_name7, object p_val7, string p_name8, object p_val8,
            string p_name9, object p_val9, string p_name10, object p_val10,
            string p_name11, object p_val11)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            int nRowCnt = 0;

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));
                command.Parameters.Add(new MySqlParameter(p_name7, p_val7));
                command.Parameters.Add(new MySqlParameter(p_name8, p_val8));
                command.Parameters.Add(new MySqlParameter(p_name9, p_val9));
                command.Parameters.Add(new MySqlParameter(p_name10, p_val10));
                command.Parameters.Add(new MySqlParameter(p_name11, p_val11));

                nRowCnt = command.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(string.Format("{0}\n in {1}", ex.Message, sp_name), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return nRowCnt;
        }
        #endregion

        #region ExecNonQurey_SP...Parameter 12
        public int ExecNonQurey_SP(string sp_name,
            string p_name1, object p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4,
            string p_name5, object p_val5, string p_name6, object p_val6,
            string p_name7, object p_val7, string p_name8, object p_val8,
            string p_name9, object p_val9, string p_name10, object p_val10,
            string p_name11, object p_val11, string p_name12, object p_val12)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            int nRowCnt = 0;

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));
                command.Parameters.Add(new MySqlParameter(p_name7, p_val7));
                command.Parameters.Add(new MySqlParameter(p_name8, p_val8));
                command.Parameters.Add(new MySqlParameter(p_name9, p_val9));
                command.Parameters.Add(new MySqlParameter(p_name10, p_val10));
                command.Parameters.Add(new MySqlParameter(p_name11, p_val11));
                command.Parameters.Add(new MySqlParameter(p_name12, p_val12));

                nRowCnt = command.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(string.Format("{0}\n in {1}", ex.Message, sp_name), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return nRowCnt;
        }
        #endregion

        #region ExecNonQurey_SP...Parameter 13
        public int ExecNonQurey_SP(string sp_name, ref string msg,
            string p_name1, ref object p_val1, string p_name2, object p_val2, string p_name3, object p_val3,
            string p_name4, object p_val4, string p_name5, object p_val5, string p_name6, object p_val6,
            string p_name7, object p_val7, string p_name8, object p_val8, string p_name9, object p_val9,
            string p_name10, object p_val10, string p_name11, object p_val11, string p_name12, object p_val12,
            string p_name13, object p_val13)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters[p_name1].Direction = ParameterDirection.InputOutput;

                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));
                command.Parameters.Add(new MySqlParameter(p_name7, p_val7));
                command.Parameters.Add(new MySqlParameter(p_name8, p_val8));
                command.Parameters.Add(new MySqlParameter(p_name9, p_val9));
                command.Parameters.Add(new MySqlParameter(p_name10, p_val10));
                command.Parameters.Add(new MySqlParameter(p_name11, p_val11));
                command.Parameters.Add(new MySqlParameter(p_name12, p_val12));
                command.Parameters.Add(new MySqlParameter(p_name13, p_val13));

                int nRowCnt = command.ExecuteNonQuery();

                // Get values from the output params
                p_val1 = command.Parameters[p_name1].Value;
                //int PG = Convert.ToInt32(command.Parameters[p_name1].Value);

                msg = "OK";
                return nRowCnt;
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return -1;
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region ExecNonQurey_SP...Parameter 14
        public int ExecNonQurey_SP(string sp_name, ref string msg,
            string p_name1, ref object p_val1, string p_name2, object p_val2, string p_name3, object p_val3,
            string p_name4, object p_val4, string p_name5, object p_val5, string p_name6, object p_val6,
            string p_name7, object p_val7, string p_name8, object p_val8, string p_name9, object p_val9,
            string p_name10, object p_val10, string p_name11, object p_val11, string p_name12, object p_val12,
            string p_name13, object p_val13, string p_name14, object p_val14)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters[p_name1].Direction = ParameterDirection.InputOutput;

                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));
                command.Parameters.Add(new MySqlParameter(p_name7, p_val7));
                command.Parameters.Add(new MySqlParameter(p_name8, p_val8));
                command.Parameters.Add(new MySqlParameter(p_name9, p_val9));
                command.Parameters.Add(new MySqlParameter(p_name10, p_val10));
                command.Parameters.Add(new MySqlParameter(p_name11, p_val11));
                command.Parameters.Add(new MySqlParameter(p_name12, p_val12));
                command.Parameters.Add(new MySqlParameter(p_name13, p_val13));
                command.Parameters.Add(new MySqlParameter(p_name14, p_val14));

                int nRowCnt = command.ExecuteNonQuery();

                // Get values from the output params
                p_val1 = command.Parameters[p_name1].Value;
                //int PG = Convert.ToInt32(command.Parameters[p_name1].Value);

                msg = "OK";
                return nRowCnt;
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return -1;
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region ExecNonQurey_SP...Parameter 15
        public int ExecNonQurey_SP(string sp_name,
            string p_name1, string p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4,
            string p_name5, object p_val5, string p_name6, object p_val6,
            string p_name7, object p_val7, string p_name8, object p_val8,
            string p_name9, object p_val9, string p_name10, object p_val10,
            string p_name11, object p_val11, string p_name12, object p_val12,
            string p_name13, object p_val13, string p_name14, object p_val14,
            string p_name15, object p_val15)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            int nRowCnt = 0;

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));
                command.Parameters.Add(new MySqlParameter(p_name7, p_val7));
                command.Parameters.Add(new MySqlParameter(p_name8, p_val8));
                command.Parameters.Add(new MySqlParameter(p_name9, p_val9));
                command.Parameters.Add(new MySqlParameter(p_name10, p_val10));
                command.Parameters.Add(new MySqlParameter(p_name11, p_val11));
                command.Parameters.Add(new MySqlParameter(p_name12, p_val12));
                command.Parameters.Add(new MySqlParameter(p_name13, p_val13));
                command.Parameters.Add(new MySqlParameter(p_name14, p_val14));
                command.Parameters.Add(new MySqlParameter(p_name15, p_val15));

                nRowCnt = command.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(string.Format("{0}\n in {1}", ex.Message, sp_name), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return nRowCnt;
        }
        #endregion

        #region ExecNonQurey_SP...Parameter 16
        public int ExecNonQurey_SP(string sp_name,
            string p_name1, string p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4,
            string p_name5, object p_val5, string p_name6, object p_val6,
            string p_name7, object p_val7, string p_name8, object p_val8,
            string p_name9, object p_val9, string p_name10, object p_val10,
            string p_name11, object p_val11, string p_name12, object p_val12,
            string p_name13, object p_val13, string p_name14, object p_val14,
            string p_name15, object p_val15, string p_name16, object p_val16)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            int nRowCnt = 0;

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));
                command.Parameters.Add(new MySqlParameter(p_name7, p_val7));
                command.Parameters.Add(new MySqlParameter(p_name8, p_val8));
                command.Parameters.Add(new MySqlParameter(p_name9, p_val9));
                command.Parameters.Add(new MySqlParameter(p_name10, p_val10));
                command.Parameters.Add(new MySqlParameter(p_name11, p_val11));
                command.Parameters.Add(new MySqlParameter(p_name12, p_val12));
                command.Parameters.Add(new MySqlParameter(p_name13, p_val13));
                command.Parameters.Add(new MySqlParameter(p_name14, p_val14));
                command.Parameters.Add(new MySqlParameter(p_name15, p_val15));
                command.Parameters.Add(new MySqlParameter(p_name16, p_val16));

                nRowCnt = command.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(string.Format("{0}\n in {1}", ex.Message, sp_name), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return nRowCnt;
        }
        #endregion

        #region ExecNonQurey_SP...Parameter 17
        public int ExecNonQurey_SP(string sp_name,
            string p_name1, string p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4,
            string p_name5, object p_val5, string p_name6, object p_val6,
            string p_name7, object p_val7, string p_name8, object p_val8,
            string p_name9, object p_val9, string p_name10, object p_val10,
            string p_name11, object p_val11, string p_name12, object p_val12,
            string p_name13, object p_val13, string p_name14, object p_val14,
            string p_name15, object p_val15, string p_name16, object p_val16,
            string p_name17, object p_val17)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            int nRowCnt = 0;

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));
                command.Parameters.Add(new MySqlParameter(p_name7, p_val7));
                command.Parameters.Add(new MySqlParameter(p_name8, p_val8));
                command.Parameters.Add(new MySqlParameter(p_name9, p_val9));
                command.Parameters.Add(new MySqlParameter(p_name10, p_val10));
                command.Parameters.Add(new MySqlParameter(p_name11, p_val11));
                command.Parameters.Add(new MySqlParameter(p_name12, p_val12));
                command.Parameters.Add(new MySqlParameter(p_name13, p_val13));
                command.Parameters.Add(new MySqlParameter(p_name14, p_val14));
                command.Parameters.Add(new MySqlParameter(p_name15, p_val15));
                command.Parameters.Add(new MySqlParameter(p_name16, p_val16));
                command.Parameters.Add(new MySqlParameter(p_name17, p_val17));

                nRowCnt = command.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(string.Format("{0}\n in {1}", ex.Message, sp_name), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return nRowCnt;
        }
        #endregion

        #region ExecNonQurey_SP...Parameter 18
        public int ExecNonQurey_SP(string sp_name,
                                   string p_name1, string p_val1, string p_name2, object p_val2,
                                   string p_name3, object p_val3, string p_name4, object p_val4,
                                   string p_name5, object p_val5, string p_name6, object p_val6,
                                   string p_name7, object p_val7, string p_name8, object p_val8,
                                   string p_name9, object p_val9, string p_name10, object p_val10,
                                   string p_name11, object p_val11, string p_name12, object p_val12,
                                   string p_name13, object p_val13, string p_name14, object p_val14,
                                   string p_name15, object p_val15, string p_name16, object p_val16,
                                   string p_name17, object p_val17, string p_name18, object p_val18)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            int nRowCnt = 0;

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));
                command.Parameters.Add(new MySqlParameter(p_name7, p_val7));
                command.Parameters.Add(new MySqlParameter(p_name8, p_val8));
                command.Parameters.Add(new MySqlParameter(p_name9, p_val9));
                command.Parameters.Add(new MySqlParameter(p_name10, p_val10));
                command.Parameters.Add(new MySqlParameter(p_name11, p_val11));
                command.Parameters.Add(new MySqlParameter(p_name12, p_val12));
                command.Parameters.Add(new MySqlParameter(p_name13, p_val13));
                command.Parameters.Add(new MySqlParameter(p_name14, p_val14));
                command.Parameters.Add(new MySqlParameter(p_name15, p_val15));
                command.Parameters.Add(new MySqlParameter(p_name16, p_val16));
                command.Parameters.Add(new MySqlParameter(p_name17, p_val17));
                command.Parameters.Add(new MySqlParameter(p_name18, p_val18));

                nRowCnt = command.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(string.Format("{0}\n in {1}", ex.Message, sp_name), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return nRowCnt;
        }
        #endregion

        #region ExecNonQurey_SP...Parameter 19
        public int ExecNonQurey_SP(string sp_name,
            string p_name1, string p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4,
            string p_name5, object p_val5, string p_name6, object p_val6,
            string p_name7, object p_val7, string p_name8, object p_val8,
            string p_name9, object p_val9, string p_name10, object p_val10,
            string p_name11, object p_val11, string p_name12, object p_val12,
            string p_name13, object p_val13, string p_name14, object p_val14,
            string p_name15, object p_val15, string p_name16, object p_val16,
            string p_name17, object p_val17, string p_name18, object p_val18,
            string p_name19, object p_val19)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            int nRowCnt = 0;

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));
                command.Parameters.Add(new MySqlParameter(p_name7, p_val7));
                command.Parameters.Add(new MySqlParameter(p_name8, p_val8));
                command.Parameters.Add(new MySqlParameter(p_name9, p_val9));
                command.Parameters.Add(new MySqlParameter(p_name10, p_val10));
                command.Parameters.Add(new MySqlParameter(p_name11, p_val11));
                command.Parameters.Add(new MySqlParameter(p_name12, p_val12));
                command.Parameters.Add(new MySqlParameter(p_name13, p_val13));
                command.Parameters.Add(new MySqlParameter(p_name14, p_val14));
                command.Parameters.Add(new MySqlParameter(p_name15, p_val15));
                command.Parameters.Add(new MySqlParameter(p_name16, p_val16));
                command.Parameters.Add(new MySqlParameter(p_name17, p_val17));
                command.Parameters.Add(new MySqlParameter(p_name18, p_val18));
                command.Parameters.Add(new MySqlParameter(p_name19, p_val19));

                nRowCnt = command.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(string.Format("{0}\n in {1}", ex.Message, sp_name), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return nRowCnt;
        }
        #endregion

        #region ExecNonQurey_SP...Parameter 19
        public int ExecNonQurey_SP(string sp_name,
            string p_name1, string p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4,
            string p_name5, object p_val5, string p_name6, object p_val6,
            string p_name7, object p_val7, string p_name8, object p_val8,
            string p_name9, object p_val9, string p_name10, object p_val10,
            string p_name11, object p_val11, string p_name12, object p_val12,
            string p_name13, object p_val13, string p_name14, object p_val14,
            string p_name15, object p_val15, string p_name16, object p_val16,
            string p_name17, object p_val17, string p_name18, object p_val18,
            string p_name19, object p_val19, string p_name20, object p_val20,
            string p_name21, object p_val21, string p_name22, object p_val22,
            string p_name23, object p_val23, string p_name24, object p_val24,
            string p_name25, object p_val25, string p_name26, object p_val26,
            string p_name27, object p_val27, string p_name28, object p_val28,
            string p_name29, object p_val29, string p_name30, object p_val30,
            string p_name31, object p_val31)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            int nRowCnt = 0;

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));
                command.Parameters.Add(new MySqlParameter(p_name7, p_val7));
                command.Parameters.Add(new MySqlParameter(p_name8, p_val8));
                command.Parameters.Add(new MySqlParameter(p_name9, p_val9));
                command.Parameters.Add(new MySqlParameter(p_name10, p_val10));
                command.Parameters.Add(new MySqlParameter(p_name11, p_val11));
                command.Parameters.Add(new MySqlParameter(p_name12, p_val12));
                command.Parameters.Add(new MySqlParameter(p_name13, p_val13));
                command.Parameters.Add(new MySqlParameter(p_name14, p_val14));
                command.Parameters.Add(new MySqlParameter(p_name15, p_val15));
                command.Parameters.Add(new MySqlParameter(p_name16, p_val16));
                command.Parameters.Add(new MySqlParameter(p_name17, p_val17));
                command.Parameters.Add(new MySqlParameter(p_name18, p_val18));
                command.Parameters.Add(new MySqlParameter(p_name19, p_val19));
                command.Parameters.Add(new MySqlParameter(p_name20, p_val20));
                command.Parameters.Add(new MySqlParameter(p_name21, p_val21));
                command.Parameters.Add(new MySqlParameter(p_name22, p_val22));
                command.Parameters.Add(new MySqlParameter(p_name23, p_val23));
                command.Parameters.Add(new MySqlParameter(p_name24, p_val24));
                command.Parameters.Add(new MySqlParameter(p_name25, p_val25));
                command.Parameters.Add(new MySqlParameter(p_name26, p_val26));
                command.Parameters.Add(new MySqlParameter(p_name27, p_val27));
                command.Parameters.Add(new MySqlParameter(p_name28, p_val28));
                command.Parameters.Add(new MySqlParameter(p_name29, p_val29));
                command.Parameters.Add(new MySqlParameter(p_name30, p_val30));
                command.Parameters.Add(new MySqlParameter(p_name31, p_val31));

                nRowCnt = command.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(string.Format("{0}\n in {1}", ex.Message, sp_name), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return nRowCnt;
        }
        #endregion

        #region ExecNonQurey_SP...Parameter 30
        public int ExecNonQurey_SP(string sp_name,
            string p_name1, string p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4,
            string p_name5, object p_val5, string p_name6, object p_val6, 
            string p_name7, object p_val7, string p_name8, object p_val8,
            string p_name9, object p_val9, string p_name10, object p_val10,            
            string p_name11, object p_val11, string p_name12, object p_val12,
            string p_name13, object p_val13, string p_name14, object p_val14,
            string p_name15, object p_val15, string p_name16, object p_val16,
            string p_name17, object p_val17, string p_name18, object p_val18,
            string p_name19, object p_val19, string p_name20, object p_val20,            
            string p_name21, object p_val21, string p_name22, object p_val22,
            string p_name23, object p_val23, string p_name24, object p_val24,
            string p_name25, object p_val25, string p_name26, object p_val26,
            string p_name27, object p_val27, string p_name28, object p_val28,
            string p_name29, object p_val29, string p_name30, object p_val30)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            int nRowCnt = 0;

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));
                command.Parameters.Add(new MySqlParameter(p_name7, p_val7));
                command.Parameters.Add(new MySqlParameter(p_name8, p_val8));
                command.Parameters.Add(new MySqlParameter(p_name9, p_val9));
                command.Parameters.Add(new MySqlParameter(p_name10, p_val10));
                command.Parameters.Add(new MySqlParameter(p_name11, p_val11));
                command.Parameters.Add(new MySqlParameter(p_name12, p_val12));
                command.Parameters.Add(new MySqlParameter(p_name13, p_val13));
                command.Parameters.Add(new MySqlParameter(p_name14, p_val14));
                command.Parameters.Add(new MySqlParameter(p_name15, p_val15));
                command.Parameters.Add(new MySqlParameter(p_name16, p_val16));
                command.Parameters.Add(new MySqlParameter(p_name17, p_val17));
                command.Parameters.Add(new MySqlParameter(p_name18, p_val18));
                command.Parameters.Add(new MySqlParameter(p_name19, p_val19));
                command.Parameters.Add(new MySqlParameter(p_name20, p_val20));
                command.Parameters.Add(new MySqlParameter(p_name21, p_val21));
                command.Parameters.Add(new MySqlParameter(p_name22, p_val22));
                command.Parameters.Add(new MySqlParameter(p_name23, p_val23));
                command.Parameters.Add(new MySqlParameter(p_name24, p_val24));
                command.Parameters.Add(new MySqlParameter(p_name25, p_val25));
                command.Parameters.Add(new MySqlParameter(p_name26, p_val26));
                command.Parameters.Add(new MySqlParameter(p_name27, p_val27));
                command.Parameters.Add(new MySqlParameter(p_name28, p_val28));
                command.Parameters.Add(new MySqlParameter(p_name29, p_val29));
                command.Parameters.Add(new MySqlParameter(p_name30, p_val30));

                nRowCnt = command.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(string.Format("{0}\n in {1}", ex.Message, sp_name), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return nRowCnt;
        }
        #endregion

        #region GetDT_SP...Parameter None
        public DataTable GetDT_SP(string sp_name, ref string msg)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            MySqlDataAdapter adpt = new MySqlDataAdapter();
            DataSet DS = new DataSet();

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;

                adpt.SelectCommand = command;
                adpt.Fill(DS, "Result");

                msg = "OK";
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return null;
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return DS.Tables["Result"];
        }
        #endregion

        #region GetDT_SP...Parameter List
        public DataTable GetDT_SP(string sp_name, ref string msg,
            List<MySqlParameter> paraList)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            MySqlDataAdapter adpt = new MySqlDataAdapter();
            DataSet DS = new DataSet();

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;

                foreach (var item in paraList)
                {
                    command.Parameters.Add(item);
                }                

                adpt.SelectCommand = command;
                adpt.Fill(DS, "Result");

                msg = "OK";
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return null;
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return DS.Tables["Result"];
        }
        #endregion

        #region GetDT_SP...Parameter1
        public DataTable GetDT_SP(string sp_name, ref string msg, 
            string p_name1, object p_val1)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            MySqlDataAdapter adpt = new MySqlDataAdapter();
            DataSet DS = new DataSet();

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));

                adpt.SelectCommand = command;
                adpt.Fill(DS, "Result");

                msg = "OK";
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return null;
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return DS.Tables["Result"];
        }
        #endregion

        #region GetDT_SP...Parameter2
        public DataTable GetDT_SP(string sp_name, ref string msg,
            string p_name1, object p_val1, string p_name2, object p_val2)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            MySqlDataAdapter adpt = new MySqlDataAdapter();
            DataSet DS = new DataSet();

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));

                adpt.SelectCommand = command;
                adpt.Fill(DS, "Result");

                msg = "OK";
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return null;
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return DS.Tables["Result"];
        }
        #endregion

        #region GetDT_SP...Parameter3
        public DataTable GetDT_SP(string sp_name, ref string msg,
            string p_name1, object p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            MySqlDataAdapter adpt = new MySqlDataAdapter();
            DataSet DS = new DataSet();

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));                
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));

                adpt.SelectCommand = command;
                adpt.Fill(DS, "Result");

                msg = "OK";
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return null;
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return DS.Tables["Result"];
        }
        #endregion

        #region GetDT_SP...Parameter4
        public DataTable GetDT_SP(string sp_name, ref string msg,
            string p_name1, object p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            MySqlDataAdapter adpt = new MySqlDataAdapter();
            DataSet DS = new DataSet();

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));                
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));

                adpt.SelectCommand = command;
                adpt.Fill(DS, "Result");

                msg = "OK";
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return null;
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return DS.Tables["Result"];
        }
        public DataTable Trans_SP(string sp_name,
            string p_name1, object p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4)
        {
            //MySqlTransation 
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            MySqlDataAdapter adpt = new MySqlDataAdapter();
            DataSet DS = new DataSet();

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));

                adpt.SelectCommand = command;
                adpt.Fill(DS, "Result");
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(string.Format("{0}\n in {1}", ex.Message, sp_name), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return DS.Tables["Result"];
        }
        #endregion

        #region GetDT_SP...Parameter5
        public DataTable GetDT_SP(string sp_name, ref string msg,
            string p_name1, object p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4,
            string p_name5, object p_val5)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            MySqlDataAdapter adpt = new MySqlDataAdapter();
            DataSet DS = new DataSet();

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));

                adpt.SelectCommand = command;
                adpt.Fill(DS, "Result");

                msg = "OK";
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return DS.Tables["Result"];
        }
        #endregion

        #region GetDT_SP...Parameter6
        public DataTable GetDT_SP(string sp_name, ref string msg,
            string p_name1, object p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4,
            string p_name5, object p_val5, string p_name6, object p_val6)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            MySqlDataAdapter adpt = new MySqlDataAdapter();
            DataSet DS = new DataSet();

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));

                adpt.SelectCommand = command;
                adpt.Fill(DS, "Result");

                msg = "OK";
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return DS.Tables["Result"];
        }
        #endregion

        #region GetDT_SP...Parameter7
        public DataTable GetDT_SP(string sp_name, ref string msg,
            string p_name1, object p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4,
            string p_name5, object p_val5, string p_name6, object p_val6,
            string p_name7, object p_val7)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            MySqlDataAdapter adpt = new MySqlDataAdapter();
            DataSet DS = new DataSet();

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));
                command.Parameters.Add(new MySqlParameter(p_name7, p_val7));

                adpt.SelectCommand = command;
                adpt.Fill(DS, "Result");

                msg = "OK";
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return null;
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return DS.Tables["Result"];
        }
        #endregion

        #region GetDT_SP...Parameter8
        public DataTable GetDT_SP(string sp_name, ref string msg,
            string p_name1, object p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4,
            string p_name5, object p_val5, string p_name6, object p_val6,
            string p_name7, object p_val7, string p_name8, object p_val8)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            MySqlDataAdapter adpt = new MySqlDataAdapter();
            DataSet DS = new DataSet();

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));
                command.Parameters.Add(new MySqlParameter(p_name7, p_val7));
                command.Parameters.Add(new MySqlParameter(p_name8, p_val8));

                adpt.SelectCommand = command;
                adpt.Fill(DS, "Result");

                msg = "OK";
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return null;
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return DS.Tables["Result"];
        }
        #endregion

        #region GetDT_SP...Parameter9
        public DataTable GetDT_SP(string sp_name, ref string msg,
            string p_name1, object p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4,
            string p_name5, object p_val5, string p_name6, object p_val6,
            string p_name7, object p_val7, string p_name8, object p_val8,
            string p_name9, object p_val9)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            MySqlDataAdapter adpt = new MySqlDataAdapter();
            DataSet DS = new DataSet();

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));
                command.Parameters.Add(new MySqlParameter(p_name7, p_val7));
                command.Parameters.Add(new MySqlParameter(p_name8, p_val8));
                command.Parameters.Add(new MySqlParameter(p_name9, p_val9));

                adpt.SelectCommand = command;
                adpt.Fill(DS, "Result");

                msg = "OK";
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return null;
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return DS.Tables["Result"];
        }
        #endregion

        #region GetDT_SP...Parameter10
        public DataTable GetDT_SP(string sp_name, ref string msg,
            string p_name1, object p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4,
            string p_name5, object p_val5, string p_name6, object p_val6,
            string p_name7, object p_val7, string p_name8, object p_val8,
            string p_name9, object p_val9, string p_name10, object p_val10)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            MySqlDataAdapter adpt = new MySqlDataAdapter();
            DataSet DS = new DataSet();

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));
                command.Parameters.Add(new MySqlParameter(p_name7, p_val7));
                command.Parameters.Add(new MySqlParameter(p_name8, p_val8));
                command.Parameters.Add(new MySqlParameter(p_name9, p_val9));
                command.Parameters.Add(new MySqlParameter(p_name10, p_val10));

                adpt.SelectCommand = command;
                adpt.Fill(DS, "Result");

                msg = "OK";
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return null;
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return DS.Tables["Result"];
        }
        #endregion

        #region GetDT_SP...Parameter11
        public DataTable GetDT_SP(string sp_name,
            string p_name1, object p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4,
            string p_name5, object p_val5, string p_name6, object p_val6,
            string p_name7, object p_val7, string p_name8, object p_val8,
            string p_name9, object p_val9, string p_name10, object p_val10,
            string p_name11, object p_val11)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            MySqlDataAdapter adpt = new MySqlDataAdapter();
            DataSet DS = new DataSet();

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));
                command.Parameters.Add(new MySqlParameter(p_name7, p_val7));
                command.Parameters.Add(new MySqlParameter(p_name8, p_val8));
                command.Parameters.Add(new MySqlParameter(p_name9, p_val9));
                command.Parameters.Add(new MySqlParameter(p_name10, p_val10));
                command.Parameters.Add(new MySqlParameter(p_name11, p_val11));

                adpt.SelectCommand = command;
                adpt.Fill(DS, "Result");
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(string.Format("{0}\n in {1}", ex.Message, sp_name), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return DS.Tables["Result"];
        }
        #endregion

        #region GetDT_SP...Parameter12
        public DataTable GetDT_SP(string sp_name,
            string p_name1, object p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4,
            string p_name5, object p_val5, string p_name6, object p_val6,
            string p_name7, object p_val7, string p_name8, object p_val8,
            string p_name9, object p_val9, string p_name10, object p_val10,
            string p_name11, object p_val11, string p_name12, object p_val12)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            MySqlDataAdapter adpt = new MySqlDataAdapter();
            DataSet DS = new DataSet();

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));
                command.Parameters.Add(new MySqlParameter(p_name7, p_val7));
                command.Parameters.Add(new MySqlParameter(p_name8, p_val8));
                command.Parameters.Add(new MySqlParameter(p_name9, p_val9));
                command.Parameters.Add(new MySqlParameter(p_name10, p_val10));
                command.Parameters.Add(new MySqlParameter(p_name11, p_val11));
                command.Parameters.Add(new MySqlParameter(p_name12, p_val12));

                adpt.SelectCommand = command;
                adpt.Fill(DS, "Result");
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(string.Format("{0}\n in {1}", ex.Message, sp_name), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return DS.Tables["Result"];
        }
        #endregion

        #region GetDT_SP...Parameter13
        public DataTable GetDT_SP(string sp_name, ref string msg,
            string p_name1, object p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4,
            string p_name5, object p_val5, string p_name6, object p_val6,
            string p_name7, object p_val7, string p_name8, object p_val8,
            string p_name9, object p_val9, string p_name10, object p_val10,
            string p_name11, object p_val11, string p_name12, object p_val12,
            string p_name13, object p_val13)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            MySqlDataAdapter adpt = new MySqlDataAdapter();
            DataSet DS = new DataSet();

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));
                command.Parameters.Add(new MySqlParameter(p_name7, p_val7));
                command.Parameters.Add(new MySqlParameter(p_name8, p_val8));
                command.Parameters.Add(new MySqlParameter(p_name9, p_val9));
                command.Parameters.Add(new MySqlParameter(p_name10, p_val10));
                command.Parameters.Add(new MySqlParameter(p_name11, p_val11));
                command.Parameters.Add(new MySqlParameter(p_name12, p_val12));
                command.Parameters.Add(new MySqlParameter(p_name13, p_val13));

                adpt.SelectCommand = command;
                adpt.Fill(DS, "Result");

                msg = "OK";
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return null;
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return DS.Tables["Result"];
        }
        #endregion

        #region GetDT_SP...Parameter18
        public DataTable GetDT_SP(string sp_name, ref string msg,
            string p_name1, object p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4,
            string p_name5, object p_val5, string p_name6, object p_val6,
            string p_name7, object p_val7, string p_name8, object p_val8,
            string p_name9, object p_val9, string p_name10, object p_val10,
            string p_name11, object p_val11, string p_name12, object p_val12,
            string p_name13, object p_val13, string p_name14, object p_val14,
            string p_name15, object p_val15, string p_name16, object p_val16,
            string p_name17, object p_val17, string p_name18, object p_val18)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            MySqlDataAdapter adpt = new MySqlDataAdapter();
            DataSet DS = new DataSet();

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));
                command.Parameters.Add(new MySqlParameter(p_name7, p_val7));
                command.Parameters.Add(new MySqlParameter(p_name8, p_val8));
                command.Parameters.Add(new MySqlParameter(p_name9, p_val9));
                command.Parameters.Add(new MySqlParameter(p_name10, p_val10));
                command.Parameters.Add(new MySqlParameter(p_name11, p_val11));
                command.Parameters.Add(new MySqlParameter(p_name12, p_val12));
                command.Parameters.Add(new MySqlParameter(p_name13, p_val13));
                command.Parameters.Add(new MySqlParameter(p_name14, p_val14));
                command.Parameters.Add(new MySqlParameter(p_name15, p_val15));
                command.Parameters.Add(new MySqlParameter(p_name16, p_val16));
                command.Parameters.Add(new MySqlParameter(p_name17, p_val17));
                command.Parameters.Add(new MySqlParameter(p_name18, p_val18));

                adpt.SelectCommand = command;
                adpt.Fill(DS, "Result");

                msg = "OK";
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return null;
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return DS.Tables["Result"];
        }
        #endregion


        #region GetDT_SP...Parameter28
        public DataTable GetDT_SP(string sp_name, ref string msg,
            string p_name1, object p_val1, string p_name2, object p_val2,
            string p_name3, object p_val3, string p_name4, object p_val4,
            string p_name5, object p_val5, string p_name6, object p_val6,
            string p_name7, object p_val7, string p_name8, object p_val8,
            string p_name9, object p_val9, string p_name10, object p_val10,
            string p_name11, object p_val11, string p_name12, object p_val12,
            string p_name13, object p_val13, string p_name14, object p_val14,
            string p_name15, object p_val15, string p_name16, object p_val16,
            string p_name17, object p_val17, string p_name18, object p_val18,
            string p_name19, object p_val19, string p_name20, object p_val20,
            string p_name21, object p_val21, string p_name22, object p_val22,
            string p_name23, object p_val23, string p_name24, object p_val24,
            string p_name25, object p_val25, string p_name26, object p_val26,
            string p_name27, object p_val27, string p_name28, object p_val28)
        {
            MySqlConnection conn = new MySqlConnection(Global.DBConString);
            MySqlDataAdapter adpt = new MySqlDataAdapter();
            DataSet DS = new DataSet();

            try
            {
                conn.Open();

                MySqlCommand command = new MySqlCommand(sp_name, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter(p_name1, p_val1));
                command.Parameters.Add(new MySqlParameter(p_name2, p_val2));
                command.Parameters.Add(new MySqlParameter(p_name3, p_val3));
                command.Parameters.Add(new MySqlParameter(p_name4, p_val4));
                command.Parameters.Add(new MySqlParameter(p_name5, p_val5));
                command.Parameters.Add(new MySqlParameter(p_name6, p_val6));
                command.Parameters.Add(new MySqlParameter(p_name7, p_val7));
                command.Parameters.Add(new MySqlParameter(p_name8, p_val8));
                command.Parameters.Add(new MySqlParameter(p_name9, p_val9));
                command.Parameters.Add(new MySqlParameter(p_name10, p_val10));
                command.Parameters.Add(new MySqlParameter(p_name11, p_val11));
                command.Parameters.Add(new MySqlParameter(p_name12, p_val12));
                command.Parameters.Add(new MySqlParameter(p_name13, p_val13));
                command.Parameters.Add(new MySqlParameter(p_name14, p_val14));
                command.Parameters.Add(new MySqlParameter(p_name15, p_val15));
                command.Parameters.Add(new MySqlParameter(p_name16, p_val16));
                command.Parameters.Add(new MySqlParameter(p_name17, p_val17));
                command.Parameters.Add(new MySqlParameter(p_name18, p_val18));
                command.Parameters.Add(new MySqlParameter(p_name19, p_val19));
                command.Parameters.Add(new MySqlParameter(p_name20, p_val20));
                command.Parameters.Add(new MySqlParameter(p_name21, p_val21));
                command.Parameters.Add(new MySqlParameter(p_name22, p_val22));
                command.Parameters.Add(new MySqlParameter(p_name23, p_val23));
                command.Parameters.Add(new MySqlParameter(p_name24, p_val24));
                command.Parameters.Add(new MySqlParameter(p_name25, p_val25));
                command.Parameters.Add(new MySqlParameter(p_name26, p_val26));
                command.Parameters.Add(new MySqlParameter(p_name27, p_val27));
                command.Parameters.Add(new MySqlParameter(p_name28, p_val28));
                
                adpt.SelectCommand = command;
                adpt.Fill(DS, "Result");

                msg = "OK";
            }
            catch (MySqlException ex)
            {
                msg = ex.Message;
                return null;
            }
            finally
            {
                conn.Close();  // always close the connection
            }

            return DS.Tables["Result"];
        }
        #endregion


    }
}
