using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;


namespace MainApp
{
    //Glowna klasa programu konsolowego
    class Program
    {

        static String sqlconnection = "Data Source=MSSQLSERVER46;Initial Catalog=projektBD;Integrated Security=True";

        
        static void Main(string[] args)
        {
            bool showMenu = true;
            while (showMenu)
            {
                showMenu = MainMenu();
            }
        }

        //metoda wyswietlajaca wszystkie punkty z bazy danych
        private static void getAllPoints()
        {
            String query = "select ID, point.ToString() as point from dbo.Points;";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataReader datareader = command.ExecuteReader();
                Console.WriteLine("\n##########################");
                while (datareader.Read())
                {
                    string i = datareader["ID"].ToString();
                    Console.WriteLine("ID ( " + i + " ) -> " + datareader["point"].ToString());
                     
                }
                Console.WriteLine("##########################\n");

            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { conn.Close(); }
        }

        //metoda wyswietlajaca wszystkie wielokaty z bazy danych
        private static void getAllPolygons()
        {
            String query = "select ID, polygon.ToString() as polygon from dbo.Polygons;";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataReader datareader = command.ExecuteReader();
                Console.WriteLine("\n##########################");

                while (datareader.Read())
                {
                    string i = datareader["ID"].ToString();
                    Console.WriteLine("ID ( " + i + " ) -> " + datareader["polygon"].ToString());
                }
                Console.WriteLine("##########################\n");

            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { conn.Close(); }
        }

        //metoda wprowadzajaca nowy punkt do bazy danych
        private static void InsertPoint(string value)
        {
            String query = "insert into dbo.Points (point) values (@value);";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {
                
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@value", SqlDbType.VarChar);
                command.Parameters["@value"].Value = value;


                Int32 rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine("RowsAffected: {0}", rowsAffected);
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { conn.Close(); }
        }

        //metoda wprowadzajca nowy wielokat do bazy danych (punkty podane manualnie)
        private static void InsertPolygonManually(string value)
        {
            String query = "insert into dbo.Polygons (polygon) values (@value);";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {

                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@value", SqlDbType.VarChar);
                command.Parameters["@value"].Value = value;


                Int32 rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine("RowsAffected: {0}", rowsAffected);
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { conn.Close(); }
        }

        //metoda wprowadzajaca nowy wielokat do bazy danych (punkty wybrane z tabeli Points)
        private static void InsertPolygonFromPointsTable(string value)
        {
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {

                conn.Open();
                SqlCommand command = new SqlCommand("dbo.InsertPolygonFromPointsTable", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@List",value));

                Int32 rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine("RowsAffected: 1");
            }
            catch (SqlException ex)
            {

                Console.WriteLine(ex.Message);
            }
            finally { conn.Close(); }
        }

        //metoda kasujaca punkt
        private static void DeletePoint(string pointID)
        {
            String query = "delete from dbo.Points where ID = @pointID;";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {

                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@pointID", SqlDbType.Int);
                command.Parameters["@pointID"].Value = int.Parse(pointID);
                Int32 rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine("RowsDeleted: {0}", rowsAffected);
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { conn.Close(); }
        }

        //metoda kasujaca wielokat
        private static void DeletePolygon (string polygonID)
        {
            String query = "delete from dbo.Polygons where ID = @polygonID;";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {

                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@polygonID", SqlDbType.Int);
                command.Parameters["@polygonID"].Value = int.Parse(polygonID);
                Int32 rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine("RowsDeleted: {0}", rowsAffected);
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { conn.Close(); }
        }

        //metoda wyswietlajaca zwrocone pole wielokata
        private static void getPolygonArea(string id)
        {
            String query = "select ID, polygon.area() as area from dbo.Polygons where ID = @ID;";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@ID", SqlDbType.Int);
                command.Parameters["@ID"].Value = int.Parse(id);

                SqlDataReader datareader = command.ExecuteReader();

                while (datareader.Read())
                {
                    string i = datareader["ID"].ToString();
                    Console.WriteLine("Area of polygon with ID ( " + i + " ) -> area = " + datareader["area"].ToString());
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { conn.Close(); }
        }

        //metoda wyswietlajaca wynik zapytania o zawieranie sie punktu w wielokacie
        private static void checkIfPointInsidePolygon(string idPolygon, string idPoint)
        {
            String query = @"declare @onePoint as dbo.Point;
                            select @onePoint = point from dbo.Points where ID = @idPoint;
                            select polygon.IsPointInside(@onePoint) as status from dbo.Polygons where ID = @idPolygon;";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@idPoint", SqlDbType.Int);
                command.Parameters.Add("@idPolygon", SqlDbType.Int);

                command.Parameters["@idPoint"].Value = int.Parse(idPoint);
                command.Parameters["@idPolygon"].Value = int.Parse(idPolygon);

                SqlDataReader datareader = command.ExecuteReader();

                while (datareader.Read())
                {
                    string isInsideString = datareader["status"].ToString();
                    bool isInside = bool.Parse(isInsideString);
                    if (isInside)
                    {
                        Console.WriteLine("Point (ID = " + idPoint + ") is inside Polygon (ID = " + idPolygon + ")\n");
                    }
                    else
                    {
                        Console.WriteLine("Point (ID = " + idPoint + ") is outside Polygon (ID = " + idPolygon + ")\n");

                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { conn.Close(); }
        }

        //metoda wyswietlajaca dystans pomiedzy dwoma punktami
        private static void distanceBetweenPoints(string firstPoint, string secondPoint)
        {
            String query = @"declare @onePoint as dbo.Point;
                            select @onePoint = point from dbo.Points where ID = @firstPoint;
                            select point.distance(@onePoint) as distance from dbo.Points where ID = @secondPoint;";
            SqlConnection conn = new SqlConnection(sqlconnection);
            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@firstPoint", SqlDbType.Int);
                command.Parameters.Add("@secondPoint", SqlDbType.Int);

                command.Parameters["@firstPoint"].Value = int.Parse(firstPoint);
                command.Parameters["@secondPoint"].Value = int.Parse(secondPoint);

                SqlDataReader datareader = command.ExecuteReader();

                while (datareader.Read())
                {
                    string distance = datareader["distance"].ToString();
                    Console.WriteLine("\nDistance between points ID(" + firstPoint + " - " + secondPoint + ") is = " + distance + "\n");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { conn.Close(); }
        }

        //glowne menu aplikacja
        private static bool MainMenu()
        {
            Console.Clear();
            Console.WriteLine("Choose an option:\n");
            Console.WriteLine("#### INSERT ####");
            Console.WriteLine("1) Insert Point");
            Console.WriteLine("2) Insert Polygon\n");
            Console.WriteLine("#### SELECT ####");
            Console.WriteLine("3) List Points");
            Console.WriteLine("4) List Polygons\n");
            Console.WriteLine("#### DELETE ####");
            Console.WriteLine("5) Delete Points");
            Console.WriteLine("6) Delete Polygons\n");
            Console.WriteLine("#### METHODS ####");
            Console.WriteLine("7) Polygon area");
            Console.WriteLine("8) Check if Point inside Polygon");
            Console.WriteLine("9) Distance between points\n");
            Console.WriteLine("0) Exit");
            Console.Write("\r\nSelect an option: ");

            switch (Console.ReadLine())
            {
                case "1"://insert point
                    Console.WriteLine("-> Enter coordinates in format: x;y [e.g. 1;2]");
                    String value = Console.ReadLine();
                    if (!String.IsNullOrEmpty(value))
                        InsertPoint(value);
                    Console.WriteLine("-> Press Enter to continue...");
                    Console.ReadLine();
                    return true;
                case "2"://insert polygon
                    Console.WriteLine("-> Do you want to use points from table (t) or enter points manually (m)?");
                    String flag = Console.ReadLine();
                    if (flag.Equals("t"))
                    {
                        getAllPoints();
                        Console.WriteLine("-> Select points ID's [e.g. 1,2,3]");
                        string listOfId = Console.ReadLine();
                        if (!String.IsNullOrEmpty(listOfId))
                            InsertPolygonFromPointsTable(listOfId);
                        Console.WriteLine("-> Press Enter to continue...");
                        Console.ReadLine();
                    }
                    else if (flag.Equals("m"))
                    {
                        Console.WriteLine("-> Enter coordinates in format: x1;y1/x2;y2\n [e.g. 1;2/2;4/6;6]");
                        string listOfPoints = Console.ReadLine();
                        InsertPolygonManually(listOfPoints);
                        Console.WriteLine("-> Press Enter to continue...");
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("Wrong parameter;");
                    }
                    return true;
                case "3"://list points
                    Console.WriteLine("All points:\n");
                    getAllPoints();
                    Console.WriteLine("-> Press Enter to continue...");
                    Console.ReadLine();
                    return true;

                case "4"://list polygons
                    Console.WriteLine("All polygons:\n");
                    getAllPolygons();
                    Console.WriteLine("-> Press Enter to continue...");
                    Console.ReadLine();
                    return true;
                case "5"://delete points
                    getAllPoints();
                    Console.WriteLine("\n-> Select point ID [e.g. 1]");
                    string deletePointID = Console.ReadLine();
                    if (deletePointID.All(char.IsDigit) && !String.IsNullOrEmpty(deletePointID))
                        DeletePoint(deletePointID);
                    Console.WriteLine("-> Press Enter to continue...");
                    Console.ReadLine();
                    return true;

                case "6"://delete polygon
                    getAllPolygons();
                    Console.WriteLine("\n-> Select polygon ID [e.g. 1]");
                    string deletePolygonID = Console.ReadLine();
                    if (deletePolygonID.All(char.IsDigit) && !String.IsNullOrEmpty(deletePolygonID))
                        DeletePolygon(deletePolygonID);
                    Console.WriteLine("-> Press Enter to continue...");
                    Console.ReadLine();
                    return true;
                case "7"://polygon area
                    getAllPolygons();
                    Console.WriteLine("-> Select polygon ID [e.g. 1]");
                    string idValue = Console.ReadLine();
                    if (idValue.All(char.IsDigit) && !String.IsNullOrEmpty(idValue))
                        getPolygonArea(idValue);
                    else
                    {
                        Console.WriteLine("Wrong parameter");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                        return true;
                    }


                    Console.WriteLine("-> Press Enter to continue...");
                    Console.ReadLine();
                    return true;
                case "8"://check if point inside polygon
                    getAllPolygons();
                    Console.WriteLine("\n-> Select polygon ID [e.g. 1]");
                    string polygonID = Console.ReadLine();
                    Console.WriteLine("\n");
                    getAllPoints();
                    Console.WriteLine("\n-> Select point ID [e.g. 1]");
                    string pointID = Console.ReadLine();
                    if (polygonID.All(char.IsDigit) && pointID.All(char.IsDigit) && !String.IsNullOrEmpty(pointID))
                        checkIfPointInsidePolygon(polygonID, pointID);
                    else
                    {
                        Console.WriteLine("Wrong parameters");
                        Console.WriteLine("-> Press Enter to continue...");
                        Console.ReadLine();
                        return true;
                    }
                    Console.WriteLine("-> Press Enter to continue...");
                    Console.ReadLine();
                    return true;

                case "9"://distance between points
                    getAllPoints();
                    Console.WriteLine("\n-> Select 1st point ID [e.g. 1]");
                    string firstPoint = Console.ReadLine();
                    Console.WriteLine("\n-> Select 2st point ID [e.g. 2]");
                    string secondPoint = Console.ReadLine();
                    if (firstPoint.All(char.IsDigit) && secondPoint.All(char.IsDigit) && !String.IsNullOrEmpty(firstPoint) && !String.IsNullOrEmpty(secondPoint))
                        distanceBetweenPoints(firstPoint, secondPoint);
                    else
                    {
                        Console.WriteLine("Wrong parameters");
                        Console.WriteLine("-> Press Enter to continue...");
                        Console.ReadLine();
                        return true;
                    }
                    Console.WriteLine("-> Press Enter to continue...");
                    Console.ReadLine();
                    return true;
                case "0":
                    return false;
                default:
                    return true;
            }
        }

    }
}
