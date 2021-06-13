using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;

namespace UnitTests
{
    [TestClass]
    public class UnitTestPoint
    {

      
        static string sqlconnection = "Data Source=MSSQLSERVER46;Initial Catalog=projektBD;Integrated Security=True";
        static SqlConnection conn = new SqlConnection(sqlconnection);
        
        
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            String sqlcommand = "create table TestPoint ( point dbo.Point);"
                              + "insert into TestPoint (point) values ('1;2');";
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(sqlcommand, conn);
                SqlDataReader datareader = command.ExecuteReader();
                datareader.Read();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { conn.Close(); }
        }


        [ClassCleanup()]
        public static void ClassCleanup()
        {
            
            String sqlcommand = "DROP TABLE TestPoint;";
            try
            {
               conn.Open();
               SqlCommand command = new SqlCommand(sqlcommand, conn);
               SqlDataReader datareader = command.ExecuteReader();
               datareader.Read();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { conn.Close(); }
        }

        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestPointGetX()
        {
            string query = "select point.X as testX from dbo.TestPoint;";
            int expected = 1;
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataReader datareader = command.ExecuteReader();
                while (datareader.Read())
                {
                    Assert.AreEqual(expected, datareader["testX"]);
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { conn.Close(); }
        }

        [TestMethod]
        public void TestPointGetY()
        {
            string query = "select point.Y as testY from dbo.TestPoint;";
            int expected = 2;
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataReader datareader = command.ExecuteReader();
                while (datareader.Read())
                {
                    Assert.AreEqual(expected, datareader["testY"]);
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { conn.Close(); }
        }

        [TestMethod]
        public void TestPointToString()
        {
            string query = "select point.ToString() as TestToString from dbo.TestPoint;";
            string expected = "1;2";
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataReader datareader = command.ExecuteReader();
                while (datareader.Read())
                {
                    Assert.AreEqual(expected, datareader["TestToString"]);
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { conn.Close(); }
        }

        [TestMethod]
        public void TestPointDistance()
        {
            
            string query = @"declare @onePoint as dbo.Point;
                            select top 1 @onePoint = point from dbo.TestPoint;
                            select point.distance(@onePoint) as TestDistance from dbo.TestPoint;";
            double expected = 0;
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataReader datareader = command.ExecuteReader();
                while (datareader.Read())
                {
                    Assert.AreEqual(expected, datareader["TestDistance"]);
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { conn.Close(); }
        }
        
        [TestMethod]
        public void TestPointTryToInsertDoublePoint()
        {
            string query = @"insert into TestPoint (point) values ('6.6;5');";
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                int status = command.ExecuteNonQuery();
                Assert.Fail();
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Test passed");
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
            finally { conn.Close(); }
        }
        
        [TestMethod]
        public void TestPointTryToInsertStringPoint()
        {
            string query = @"insert into TestPoint (point) values ('test;5');";
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                int status = command.ExecuteNonQuery();
                Assert.Fail();
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Test passed");
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
            finally { conn.Close(); }
        }
         
    }
}
