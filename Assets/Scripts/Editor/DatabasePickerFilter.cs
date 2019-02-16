
using System.Collections.Generic;
using System.Linq;

public class DatabasePickerFilter<T> where T : Data
{
    public enum FilterOperator
    {
        Less,
        LessEqual,
        Equal,
        GreaterEqual,
        Greater,
        NotEqual
    }

    public List<T> FilterBool(List<T> _in, string _property, bool isTrue)
    {
        return _in.Where(x =>
                ( x.GetType().GetProperty(_property) != null && (bool)x.GetType().GetProperty(_property).GetValue(x, null) == isTrue ) ||
                ( x.GetType().GetField(_property) != null && (bool)x.GetType().GetField(_property).GetValue(x) == isTrue )
        ).ToList();
    }

    public List<T> FilterEnum(List<T> _in, string _property, System.Enum _enum)
    {
        return _in.Where(x =>
                (x.GetType().GetProperty(_property) != null && ((System.Enum)x.GetType().GetProperty(_property).GetValue(x, null)).ToString() == _enum.ToString()) ||
                (x.GetType().GetField(_property) != null && ((System.Enum)x.GetType().GetField(_property).GetValue(x)).ToString() == _enum.ToString())
        ).ToList();
    }

    public List<T> FilterInt(List<T> _in, string _property, int _value, FilterOperator _operator)
    {
        if (_operator == FilterOperator.Less)
        {
            return _in.Where(x =>
                    (x.GetType().GetProperty(_property) != null && (int)x.GetType().GetProperty(_property).GetValue(x, null) < _value) ||
                    (x.GetType().GetField(_property) != null && (int)x.GetType().GetField(_property).GetValue(x) < _value)
            ).ToList();
        }
        else if (_operator == FilterOperator.LessEqual)
        {
            return _in.Where(x =>
                    (x.GetType().GetProperty(_property) != null && (int)x.GetType().GetProperty(_property).GetValue(x, null) <= _value) ||
                    (x.GetType().GetField(_property) != null && (int)x.GetType().GetField(_property).GetValue(x) < _value)
            ).ToList();
        }
        else if (_operator == FilterOperator.Equal)
        {
            return _in.Where(x =>
                    (x.GetType().GetProperty(_property) != null && (int)x.GetType().GetProperty(_property).GetValue(x, null) == _value) ||
                    (x.GetType().GetField(_property) != null && (int)x.GetType().GetField(_property).GetValue(x) == _value)
            ).ToList();
        }
        else if (_operator == FilterOperator.GreaterEqual)
        {
            return _in.Where(x =>
                    (x.GetType().GetProperty(_property) != null && (int)x.GetType().GetProperty(_property).GetValue(x, null) >= _value) ||
                    (x.GetType().GetField(_property) != null && (int)x.GetType().GetField(_property).GetValue(x) >= _value)
            ).ToList();
        }
        else if (_operator == FilterOperator.Greater)
        {
            return _in.Where(x =>
                    (x.GetType().GetProperty(_property) != null && (int)x.GetType().GetProperty(_property).GetValue(x, null) > _value) ||
                    (x.GetType().GetField(_property) != null && (int)x.GetType().GetField(_property).GetValue(x) > _value)
            ).ToList();
        }
        else // NotEqual
        {
            return _in.Where(x =>
                    (x.GetType().GetProperty(_property) != null && (int)x.GetType().GetProperty(_property).GetValue(x, null) != _value) ||
                    (x.GetType().GetField(_property) != null && (int)x.GetType().GetField(_property).GetValue(x) != _value)
            ).ToList();
        }
    }

    public List<T> FilterFloat(List<T> _in, string _property, float _value, FilterOperator _operator)
    {
        if (_operator == FilterOperator.Less)
        {
            return _in.Where(x =>
                    (x.GetType().GetProperty(_property) != null && (float)x.GetType().GetProperty(_property).GetValue(x, null) < _value) ||
                    (x.GetType().GetField(_property) != null && (float)x.GetType().GetField(_property).GetValue(x) < _value)
            ).ToList();
        }
        else if (_operator == FilterOperator.LessEqual)
        {
            return _in.Where(x =>
                    (x.GetType().GetProperty(_property) != null && (float)x.GetType().GetProperty(_property).GetValue(x, null) <= _value) ||
                    (x.GetType().GetField(_property) != null && (float)x.GetType().GetField(_property).GetValue(x) < _value)
            ).ToList();
        }
        else if (_operator == FilterOperator.Equal)
        {
            return _in.Where(x =>
                    (x.GetType().GetProperty(_property) != null && (float)x.GetType().GetProperty(_property).GetValue(x, null) == _value) ||
                    (x.GetType().GetField(_property) != null && (float)x.GetType().GetField(_property).GetValue(x) == _value)
            ).ToList();
        }
        else if (_operator == FilterOperator.GreaterEqual)
        {
            return _in.Where(x =>
                    (x.GetType().GetProperty(_property) != null && (float)x.GetType().GetProperty(_property).GetValue(x, null) >= _value) ||
                    (x.GetType().GetField(_property) != null && (float)x.GetType().GetField(_property).GetValue(x) >= _value)
            ).ToList();
        }
        else if (_operator == FilterOperator.Greater)
        {
            return _in.Where(x =>
                    (x.GetType().GetProperty(_property) != null && (float)x.GetType().GetProperty(_property).GetValue(x, null) > _value) ||
                    (x.GetType().GetField(_property) != null && (float)x.GetType().GetField(_property).GetValue(x) > _value)
            ).ToList();
        }
        else // NotEqual
        {
            return _in.Where(x =>
                    (x.GetType().GetProperty(_property) != null && (float)x.GetType().GetProperty(_property).GetValue(x, null) != _value) ||
                    (x.GetType().GetField(_property) != null && (float)x.GetType().GetField(_property).GetValue(x) != _value)
            ).ToList();
        }
    }

    virtual public string[] GetProperties()
    {
        string[] fields = typeof(T).GetFields().Where(f => f.IsPublic).Select(x => x.Name).ToArray();
        string[] properties = typeof(T).GetProperties().Select(x => x.Name).ToArray();
        return fields.Concat( properties ).ToArray();
        //return new string[]{ };
    }
}

