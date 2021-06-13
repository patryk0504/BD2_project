using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Globalization;

//klasa reprezentujaca typ zlozony - Polygon

[Serializable]
[Microsoft.SqlServer.Server.SqlUserDefinedType(
    Format.UserDefined,
    MaxByteSize = 8000,
    IsByteOrdered = true
 )]
public struct Polygon : INullable, IBinarySerialize
{
    private List<Point> points;
    private bool isNull;

    //metoda kodujaca obiekt klasy w postac ciagu znakowego
    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        foreach (var point in points)
        {
            builder.Append("[");
            builder.Append(point);
            builder.Append("]");

        }
        return builder.ToString();
    }



    public bool IsNull
    {
        get
        {
            return isNull;
        }
    }

    public static Polygon Null
    {
        get
        {
            Polygon h = new Polygon();
            h.isNull = true;
            return h;
        }
    }

    //metoda prywatna zwracajaca punkt znajdujacy sie we wnetrzu wielokata
    private Point AveragePointInside()
    {
        int x = 0;
        int y = 0;
        foreach (var point in points)
        {
            x += point.X;
            y += point.Y;
        }
        Point result = new Point();
        result.X = x/points.Count;
        result.Y = y/points.Count;
        return result;
    }

    //metoda prywatna sortujaca liste punktow tworzacych wielokat
    private void SortAngular()
    {
        Point avgPoint = AveragePointInside();
        Comparison<Point> mycomp = (a, b) => {
            double a1 = Math.Atan2(a.Y - avgPoint.Y, a.X - avgPoint.X);
            double a2 = Math.Atan2(b.Y - avgPoint.Y, b.X - avgPoint.X);
            double angle1 = (a1* (180/Math.PI) + 360) % 360;
            double angle2 = (a2 * (180 / Math.PI) + 360) % 360;

            return (int)(angle2 - angle1);
        };
        points.Sort(mycomp);
    }

    //metoda obliczajaca pole wielokata
    public SqlDouble area()
    {
        SortAngular();
        int psum = 0;
        int nsum = 0;

        for (int i = 0; i < points.Count; i++)
        {
            int sindex = (i + 1) % points.Count;
            int prod = points[i].X * points[sindex].Y;
            psum += prod;
        }

        for (int i = 0; i < points.Count; i++)
        {
            int sindex = (i + 1) % points.Count;
            int prod = points[sindex].X * points[i].Y;
            nsum += prod;
        }
        
        return Math.Abs(0.5*(psum - nsum));
    }

    //metoda sprawdzajaca czy punkt znajduje sie wewnatrz wielokata - crossing number test
    public SqlBoolean IsPointInside(Point P)
    {
        int counter = 0;
        List<Point> V = new List<Point>();
        foreach (var point in points)
        {
            V.Add(point);
        }
        V.Add(points[0]);
        // loop through all edges of the polygon
        for (int i = 0; i < V.Count-1; i++)
        { 
            if (((V[i].Y <= P.Y) && (V[i + 1].Y > P.Y))
             || ((V[i].Y > P.Y) && (V[i + 1].Y <= P.Y)))
            { 
                double vt = (double)(P.Y - V[i].Y) / (V[i + 1].Y - V[i].Y);
                if (P.X < V[i].X + vt * (V[i + 1].X - V[i].X))
                    ++counter;
            }
        }
        if (counter == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //metoda walidujaca czy stworzony wielokat jest poprawny
    private bool CheckPolygon()
    {
        for (int k = 0; k < points.Count; k++)
        {
            int prevX = points[k].X;
            int prevY = points[k].Y;
            for (int i = k+1; i < points.Count; i++)
            {
                if (prevX == points[i].X && prevY == points[i].Y)
                {
                    return false;
                }
            }
        }
        return true;
    }

    //metoda parsujaca ciag znakow z polecenia SQL w celu utworzenia nowego obiektu
    public static Polygon Parse(SqlString s)
    {
        
        if (s.IsNull)
            return Null;
        Polygon u = new Polygon();
  
        string[] xy = s.Value.Split("/".ToCharArray());
        if (xy.Length < 3)
        {
            throw new ArgumentException("Invalid Polygon. You must provide >= 3 vertices.");
        }
        List<Point> p = new List<Point>();
        foreach (var point in xy)
        {
            string[] separate = point.Split(";".ToCharArray());
            Point temp = new Point();
            temp.X = Int32.Parse(separate[0]);
            temp.Y = Int32.Parse(separate[1]);
            p.Add(temp);

        }
        u.points = p;
        if (!u.CheckPolygon())
        {
            throw new ArgumentException("Invalid Polygon. Some vertices are equal.");
        }
        
        return u;
    }

    //metoda serializujaca obiekt
    public void Write(System.IO.BinaryWriter w)
    {
        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < points.Count-1; i++)
        {
            builder.Append(points[i].ToString());
            builder.Append("/");
        }

        builder.Append(points[points.Count-1]);

        int maxStringSize = 255;
        string paddedString;
        paddedString = builder.ToString().PadRight(maxStringSize, '\0');
        for (int i = 0; i < paddedString.Length; i++)
        {
            w.Write(paddedString[i]);
        }
    }

    //metoda deserializujaca obiekt
    public void Read(System.IO.BinaryReader r)
    {
        char[] chars;
        int maxStringSize = 255;
        int stringEnd;
        string stringValue;

        chars = r.ReadChars(maxStringSize);
        stringEnd = Array.IndexOf(chars, '\0');
        if (stringEnd == 0)
        {
            stringValue = null;
            return;
        }
        stringValue = new String(chars, 0, stringEnd);
        
        string[] xy = stringValue.Split("/".ToCharArray());
        List<Point> p = new List<Point>();
        
        foreach(var point in xy){
            string [] separate = point.Split(";".ToCharArray());
            Point temp = new Point();
            temp.X = Int32.Parse(separate[0]);
            temp.Y = Int32.Parse(separate[1]);
            p.Add(temp);  
        } 
        points = p;
    }
}


