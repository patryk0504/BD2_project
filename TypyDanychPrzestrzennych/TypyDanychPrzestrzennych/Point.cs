using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Text;
using System.Globalization;

//klasa reprezentujaca typ zlozony - Punkt

[Serializable]
[Microsoft.SqlServer.Server.SqlUserDefinedType(Format.Native)]
public struct Point : INullable
{

    private int _x;
    private int _y;
    private bool isNull;

    //metoda kodujaca obiekt klasy w postac ciagu znakowego
    public override string ToString()
    {
        if (this.IsNull)
            return "NULL";
        else
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(this._x);
            builder.Append(";");
            builder.Append(this._y);
            return builder.ToString();
        }
    }

    public bool IsNull
    {
        get
        {
            return isNull;
        }
    }

    public static Point Null
    {
        get
        {
            Point h = new Point();
            h.isNull = true;
            return h;
        }
    }

    //metoda parsujaca ciag znakow z polecenia SQL w celu utworzenia nowego obiektu
    [SqlMethod(OnNullCall = false)]
    public static Point Parse(SqlString s)
    {
        if (s.IsNull)
            return Null;
        Point u = new Point();
        string[] xy = s.Value.Split(";".ToCharArray());
        u.X = Int32.Parse(xy[0]);
        u.Y = Int32.Parse(xy[1]);
        return u;
    }

    //gettery oraz settery
    public int X
    {
        get
        {
            return this._x;
        }
        set
        {
            double temp = _x;
            _x = value;
        }
    }

    public int Y
    {
        get
        {
            return this._y;
        }
        set
        {
            double temp = _y;
            _y = value;
        }
    }

    //metoda zwracajaca dystans pomiedzy punktami
    public SqlDouble distance(Point p)
    {
        return Math.Sqrt(Math.Pow(this._x - p._x, 2) + Math.Pow(this._y - p._y, 2));
    }

}