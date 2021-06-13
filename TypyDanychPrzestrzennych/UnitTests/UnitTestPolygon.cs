using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;

namespace UnitTests
{
    [TestClass]
    public class UnitTestPolygon
    {
        static string sqlconnection = "Data Source=MSSQLSERVER46;Initial Catalog=projektBD;Integrated Security=True";
        static SqlConnection conn = new SqlConnection(sqlconnection);


        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            String sqlcommand = @"create table TestPolygon ( polygon dbo.Polygon);
                                insert into TestPolygon (polygon) values ('0;0/0;3/3;3/3;0');
                                create table TestPoint2 (point dbo.Point);
                                insert into TestPoint2 (point) values ('1;2');
                                insert into TestPoint2 (point) values ('5;5');";

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

            String sqlcommand = @"DROP TABLE TestPolygon;
                                  DROP TABLE TestPoint2;";
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
        public void TestPolygonToString()
        {
            string query = "select polygon.ToString() as TestToString from dbo.TestPolygon;";
            string expected = "[0;0][0;3][3;3][3;0]";
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
        public void TestPolygonArea()
        {
            string query = "select polygon.area() as TestArea from dbo.TestPolygon;";
            double expected = 9;
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataReader datareader = command.ExecuteReader();
                while (datareader.Read())
                {
                    Assert.AreEqual(expected, datareader["TestArea"]);
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { conn.Close(); }
        }

        [TestMethod]
        public void TestPolygonPointInsideTrue()
        {
            string query = @"declare @onePoint as dbo.Point;
                            select top 1 @onePoint = point from dbo.TestPoint2 where point.X = 1 and point.Y = 2;
                            select polygon.IsPointInside(@onePoint) as TestIsInside from dbo.TestPolygon;";
            bool expected = true;
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataReader datareader = command.ExecuteReader();
                while (datareader.Read())
                {
                    Assert.AreEqual(expected, datareader["TestIsInside"]);
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { conn.Close(); }
        }

        [TestMethod]
        public void TestPolygonPointInsideFalse()
        {
            string query = @"declare @onePoint as dbo.Point;
                            select top 1 @onePoint = point from dbo.TestPoint2 where point.X = 5 and point.Y = 5;
                            select polygon.IsPointInside(@onePoint) as TestIsInside from dbo.TestPolygon;";
            bool expected = false;
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataReader datareader = command.ExecuteReader();
                while (datareader.Read())
                {
                    Assert.AreEqual(expected, datareader["TestIsInside"]);
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { conn.Close(); }
        }

        [TestMethod]
        public void TestPolygonInsertWithLessThan3Vertices()
        {
            string query = @"insert into dbo.TestPolygon (polygon) values ('0;0/0;3');";
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
        public void TestPolygonInsertWithEqualVertices()
        {
            string query = @"insert into dbo.TestPolygon (polygon) values ('0;0/0;0/0;0');";
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
        public void TestPolygonInsertStringPoint()
        {
            string query = @"insert into dbo.TestPolygon (polygon) values ('test;0/0;3/5;5');";
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
        public void TestPolygonInsertDoublePoint()
        {
            string query = @"insert into dbo.TestPolygon (polygon) values ('0;0/0;3/1.1;5');";
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
        public void TestPolygonInsertTrue()
        {
            string query = @"insert into dbo.TestPolygon (polygon) values ('0;0/1;1/2;0');";
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                int status = command.ExecuteNonQuery();
                if (status == 1)
                {
                    query = @"delete from dbo.TestPolygon where polygon.ToString() = '[0;0][1;1][2;0]';";
                }

                Assert.AreEqual(1, status);

            }
            catch (SqlException ex)
            {
                
                Console.WriteLine(ex.Message);
            }
            finally { conn.Close(); }
        }

    }
}
